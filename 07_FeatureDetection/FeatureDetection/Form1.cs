using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace FeatureDetection
{
    /// <summary>
    /// 第 7 章 特徵檢測 — 四個工具的功能測試程式。
    ///
    /// 【資源釋放】本檔案嚴格遵守第 18／19 章：
    ///   · Mat 欄位覆蓋一律「暫存 → 換新 → 放舊」（P-009 的正解）。
    ///   · PictureBox.Image 換圖時，舊的那張 Bitmap 是「我們自己 new 的」，必須自己 Dispose；
    ///     PictureBox 本身不會幫你放。（反之，若是別人給的物件就不要碰——不要 Dispose 借來的東西。）
    ///   · 形態學 kernel 在建構子建立一次、存成欄位，FormClosed 才釋放，
    ///     絕不在每次按鈕事件裡反覆 new。
    ///   · 所有中間 Mat 一律 using，不寫鏈式呼叫吃掉中間物件。
    /// </summary>
    public partial class Form1 : Form
    {
        // ── 尚未外部化的參數 ────────────────────────────────────────
        // 這幾個值在本示範裡用 const，是因為 UI 上沒有給它們控制項。
        // 正式專案裡它們和 minArea／maxArea 一樣，都屬於「機台參數」，
        // 必須進 Recipe／INI（第 16 章），不可以留在程式碼裡。
        private const int BlurKernelSize = 5;        // 高斯核尺寸，必須是奇數
        private const int MorphKernelSize = 3;       // 形態學 Open 的結構元素尺寸
        private const double ScratchAspectRatio = 3.0;  // 長寬比 > 此值視為刮痕

        // ── 顏色（OpenCV 用 BGR 順序，不是 RGB）────────────────────
        private static readonly MCvScalar ColorRoi = new MCvScalar(0, 224, 0);      // 綠：ROI 框
        private static readonly MCvScalar ColorDefect = new MCvScalar(32, 32, 255); // 紅：缺陷外接矩形
        private static readonly MCvScalar ColorCentroid = new MCvScalar(0, 224, 255); // 黃：形心十字

        private static readonly Color VerdictOkBack = Color.FromArgb(223, 246, 221);
        private static readonly Color VerdictOkFore = Color.FromArgb(27, 94, 32);
        private static readonly Color VerdictNgBack = Color.FromArgb(253, 231, 233);
        private static readonly Color VerdictNgFore = Color.FromArgb(164, 38, 44);

        private const string Divider =
            "--------------------------------------------------------------------------------";

        // ── 欄位（生命週期與 Form 相同）─────────────────────────────

        /// <summary>目前的來源影像（單通道灰階）。由本 Form 擁有，FormClosed 時釋放。</summary>
        private Mat _srcMat;

        /// <summary>
        /// 形態學結構元素。第 18 章：尺寸固定的 kernel 要「建一次、存欄位、結束才 Dispose」，
        /// 不可以在每幀（這裡是每次按鈕）迴圈內 new 而不放。
        /// </summary>
        private Mat _morphKernel;

        public Form1()
        {
            InitializeComponent();

            _morphKernel = CvInvoke.GetStructuringElement(
                ElementShape.Rectangle,
                new Size(MorphKernelSize, MorphKernelSize),
                new Point(-1, -1));   // 錨點 (-1,-1) = 置中

            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowWelcome();
        }

        /// <summary>
        /// 收尾：釋放本 Form 自己配置的所有非受管資源。
        ///
        /// 第 20 章：行為寫在執行期程式碼，不動 Designer.cs。
        /// 注意 PictureBox 不會自動 Dispose 它的 Image，一定要自己放。
        /// </summary>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetPictureBoxImage(picMain, null);
            SetPictureBoxImage(picResult, null);

            if (_srcMat != null) { _srcMat.Dispose(); _srcMat = null; }
            if (_morphKernel != null) { _morphKernel.Dispose(); _morphKernel = null; }
        }

        // ═══════════════════════════════════════════════════════════
        //  來源影像
        // ═══════════════════════════════════════════════════════════

        private void btnGenTestImage_Click(object sender, EventArgs e)
        {
            try
            {
                SetSource(TestImageGenerator.Create());
                SetRoi(TestImageGenerator.DefaultRoi);
                RefreshMainView(null);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[產生測試圖]");
                sb.AppendLine(TestImageGenerator.Description);
                sb.AppendLine();
                sb.AppendLine("這張圖每個元素都對應一個教學點，請由右側依序執行 ① ～ ④，");
                sb.AppendLine("或直接按 ⑤ 一鍵執行教材 §5 的完整流程。");
                txtLog.Text = sb.ToString();

                SetVerdictIdle();
            }
            catch (Exception ex)
            {
                ReportError("產生測試圖", ex);
            }
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "載入影像（會自動轉為灰階）";
                    dlg.Filter = "影像檔|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有檔案|*.*";
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;

                    Mat loaded = CvInvoke.Imread(dlg.FileName, ImreadModes.Grayscale);
                    if (loaded == null || loaded.IsEmpty)
                    {
                        if (loaded != null) loaded.Dispose();
                        MessageBox.Show(this, "無法讀取這個檔案，請確認格式。", "載入影像",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    SetSource(loaded);

                    // 影像尺寸改變，把 ROI 重設為置中的一半大小，避免沿用舊座標對不到
                    SetRoi(new Rectangle(loaded.Width / 4, loaded.Height / 4,
                                         loaded.Width / 2, loaded.Height / 2));
                    RefreshMainView(null);

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("[載入影像] " + dlg.FileName);
                    sb.AppendLine("  尺寸: " + _srcMat.Width + " × " + _srcMat.Height + ", 8-bit 灰階");
                    sb.AppendLine();
                    sb.AppendLine("ROI 已重設為影像中央的一半範圍，請視需要調整後再執行檢測。");
                    txtLog.Text = sb.ToString();

                    SetVerdictIdle();
                }
            }
            catch (Exception ex)
            {
                ReportError("載入影像", ex);
            }
        }

        /// <summary>
        /// 換掉來源影像：暫存 → 換新 → 放舊。
        ///
        /// 第 18 章 P-009：直接寫 _srcMat = f(_srcMat) 這種自我覆蓋，
        /// 舊物件會被無聲丟棄（漏放），或在 f 回傳同一物件時變成 use-after-dispose。
        /// </summary>
        private void SetSource(Mat newSource)
        {
            Mat old = _srcMat;
            _srcMat = newSource;
            if (old != null) old.Dispose();

            // 來源換了，右側的處理結果就過期了，一併清掉
            SetPictureBoxImage(picResult, null);
            lblResultCap.Text = "處理結果";
            lblRoiInfo.Text = "—";
        }

        private bool EnsureSource()
        {
            if (_srcMat != null && !_srcMat.IsEmpty) return true;
            MessageBox.Show(this, "請先按「產生測試圖」或「載入影像」。", "尚未載入影像",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        // ═══════════════════════════════════════════════════════════
        //  ROI
        // ═══════════════════════════════════════════════════════════

        private Rectangle GetRoi()
        {
            Rectangle raw = new Rectangle(
                (int)numRoiX.Value, (int)numRoiY.Value,
                (int)numRoiW.Value, (int)numRoiH.Value);

            if (_srcMat == null || _srcMat.IsEmpty) return raw;

            // 夾回影像範圍內，並把修正後的值寫回 UI，讓使用者看得到實際生效的座標
            Rectangle clamped = FeatureOps.ClampRoi(raw, _srcMat.Width, _srcMat.Height);
            if (clamped != raw) SetRoi(clamped);
            return clamped;
        }

        private void SetRoi(Rectangle roi)
        {
            numRoiX.Value = roi.X;
            numRoiY.Value = roi.Y;
            numRoiW.Value = roi.Width;
            numRoiH.Value = roi.Height;
        }

        private void btnResetRoi_Click(object sender, EventArgs e)
        {
            if (_srcMat != null && !_srcMat.IsEmpty
                && _srcMat.Width == TestImageGenerator.Width
                && _srcMat.Height == TestImageGenerator.Height)
            {
                SetRoi(TestImageGenerator.DefaultRoi);
            }
            else if (_srcMat != null && !_srcMat.IsEmpty)
            {
                SetRoi(new Rectangle(_srcMat.Width / 4, _srcMat.Height / 4,
                                     _srcMat.Width / 2, _srcMat.Height / 2));
            }
            else
            {
                SetRoi(TestImageGenerator.DefaultRoi);
            }
            RefreshMainView(null);
        }

        private void btnExtractRoi_Click(object sender, EventArgs e)
        {
            if (!EnsureSource()) return;

            try
            {
                Rectangle roi = GetRoi();

                // ExtractRoi 回傳的 Mat 與 _srcMat 共享像素 buffer，
                // Dispose 它只釋放 header——這正是教材 §1 要示範的重點。
                using (Mat roiMat = FeatureOps.ExtractRoi(_srcMat, roi))
                {
                    SetPictureBoxImage(picResult, ToDisplayBitmap(roiMat));
                    lblResultCap.Text = "處理結果 — ROI 子影像 " + roi.Width + " × " + roi.Height;
                    lblRoiInfo.Text = roi.Width + " × " + roi.Height + " px";

                    long srcPixels = (long)_srcMat.Width * _srcMat.Height;
                    long roiPixels = (long)roi.Width * roi.Height;
                    double ratio = srcPixels == 0 ? 0 : (100.0 * roiPixels / srcPixels);

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("[① ROI 擷取]");
                    sb.AppendLine();
                    sb.AppendLine("  var roi    = new Rectangle(" + roi.X + ", " + roi.Y + ", "
                                  + roi.Width + ", " + roi.Height + ");");
                    sb.AppendLine("  var roiMat = new Mat(src, roi);      // header 共享，不 Clone");
                    sb.AppendLine();
                    sb.AppendLine(string.Format("  原圖尺寸       : {0} × {1}   ({2:N0} px)",
                                  _srcMat.Width, _srcMat.Height, srcPixels));
                    sb.AppendLine(string.Format("  ROI 子影像尺寸 : {0} × {1}   ({2:N0} px)  →  待處理量降為 {3:F1}%",
                                  roi.Width, roi.Height, roiPixels, ratio));
                    sb.AppendLine();
                    sb.AppendLine("  ⚠ roiMat 與 src 共享同一塊像素 buffer。");
                    sb.AppendLine("    roiMat.Dispose() 只釋放 header，像素資料屬於 src；");
                    sb.AppendLine("    要放掉資料請釋放 src，而且 src 必須活得比 roiMat 久。");
                    sb.AppendLine();
                    sb.AppendLine("  ⚠ P-001：本 ROI 為固定座標，隱含「產品每次放的位置都一樣」。");
                    sb.AppendLine("    產線若無入片對位機構，必須先用模板匹配（第 9 章）算出本次影像");
                    sb.AppendLine("    相對基準的偏移量，再動態修正所有後續 ROI 的座標。");
                    txtLog.Text = sb.ToString();
                }

                RefreshMainView(null);
                SetVerdictIdle();
            }
            catch (Exception ex)
            {
                ReportError("ROI 擷取", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  ② Blob 連通域分析
        // ═══════════════════════════════════════════════════════════

        private void btnBlob_Click(object sender, EventArgs e)
        {
            if (!EnsureSource()) return;

            try
            {
                Rectangle roi = GetRoi();
                int threshold = (int)numThreshold.Value;
                int minArea = (int)numMinArea.Value;
                int maxArea = (int)numMaxArea.Value;

                BlobResult result;
                using (Mat roiMat = FeatureOps.ExtractRoi(_srcMat, roi))
                using (Mat binary = FeatureOps.Preprocess(roiMat, BlurKernelSize, threshold, _morphKernel))
                {
                    result = FeatureOps.DetectBlobs(binary, minArea, maxArea);
                    OffsetToAbsolute(result, roi);

                    SetPictureBoxImage(picResult, ToDisplayBitmap(binary));
                    lblResultCap.Text = "處理結果 — 高斯 " + BlurKernelSize + "×" + BlurKernelSize
                                        + " → 二值化 " + threshold
                                        + " → Open " + MorphKernelSize + "×" + MorphKernelSize;
                }

                RefreshMainView(result);
                lblRoiInfo.Text = roi.Width + " × " + roi.Height + " px";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("[② Blob 連通域分析]  ROI({0},{1},{2},{3})",
                              roi.X, roi.Y, roi.Width, roi.Height));
                sb.AppendLine("  前處理（第 6 章標準順序）: GaussianBlur " + BlurKernelSize + "×" + BlurKernelSize
                              + "  →  Threshold " + threshold
                              + "  →  MorphOpen " + MorphKernelSize + "×" + MorphKernelSize);
                sb.AppendLine("  ConnectedComponentsWithStats  →  連通域總數 " + result.TotalCount + "（不含背景）");
                sb.AppendLine(string.Format("  面積過濾 [minArea={0}, maxArea={1}]  →  保留 {2}",
                              minArea, maxArea, result.Kept.Count));
                AppendBlobTable(sb, result, minArea, maxArea);
                txtLog.Text = sb.ToString();

                SetVerdict(result.Kept.Count == 0,
                           result.Kept.Count == 0 ? "OK" : "NG　" + result.Kept.Count + " 個缺陷");
            }
            catch (Exception ex)
            {
                ReportError("Blob 分析", ex);
            }
        }

        /// <summary>
        /// DetectBlobs 回傳的座標是「ROI 內的相對座標」，
        /// 換算成整張影像的絕對座標，才能畫回原圖、也才是工程師要看的數字。
        /// </summary>
        private static void OffsetToAbsolute(BlobResult result, Rectangle roi)
        {
            for (int i = 0; i < result.Kept.Count; i++)
            {
                BlobInfo b = result.Kept[i];
                b.BoundingBox = new Rectangle(b.BoundingBox.X + roi.X, b.BoundingBox.Y + roi.Y,
                                              b.BoundingBox.Width, b.BoundingBox.Height);
                b.Centroid = new PointF(b.Centroid.X + roi.X, b.Centroid.Y + roi.Y);
            }
        }

        private void AppendBlobTable(StringBuilder sb, BlobResult result, int minArea, int maxArea)
        {
            sb.AppendLine(Divider);
            sb.AppendLine("  #     面積    形心 (x, y)         外接矩形 (x, y, w, h)      長寬比   研判");
            sb.AppendLine(Divider);

            if (result.Kept.Count == 0)
            {
                sb.AppendLine("  （面積過濾後沒有任何 Blob 通過——這代表沒有缺陷）");
            }
            else
            {
                foreach (BlobInfo b in result.Kept)
                {
                    sb.AppendLine(string.Format(
                        "  {0,-3} {1,7:N0}   ({2,6:F1}, {3,6:F1})     ({4,4}, {5,4}, {6,4}, {7,4})     {8,6:F2}   {9}",
                        b.Index, b.Area, b.Centroid.X, b.Centroid.Y,
                        b.BoundingBox.X, b.BoundingBox.Y, b.BoundingBox.Width, b.BoundingBox.Height,
                        b.AspectRatio,
                        b.AspectRatio > ScratchAspectRatio ? "刮痕" : "異物"));
                }
            }

            sb.AppendLine(Divider);
            sb.AppendLine("  座標為整張影像的絕對座標（已加上 ROI 偏移）。");
            sb.AppendLine();
            sb.AppendLine("  已濾除：");
            sb.AppendLine(string.Format("    · {0,4} 個  <  minArea = {1}   （雜訊：光照不均、鏡頭灰塵、感光元件雜訊）",
                          result.RejectedTooSmall, minArea));
            if (result.RejectedTooLarge > 0)
            {
                sb.AppendLine(string.Format("    · {0,4} 個  >  maxArea = {1} （產品本體，最大者 {2:N0} px）",
                              result.RejectedTooLarge, maxArea, result.LargestRejectedArea));
            }
            else
            {
                sb.AppendLine(string.Format("    · {0,4} 個  >  maxArea = {1}", 0, maxArea));
            }
            sb.AppendLine();
            sb.AppendLine("  研判規則：長寬比 > " + ScratchAspectRatio.ToString("F1")
                          + " 視為刮痕（細長），否則視為異物（接近方形）。");
            sb.AppendLine("  ⚠ minArea／maxArea 一定要外部化為機台參數（第 16 章 Recipe），");
            sb.AppendLine("    不同產品、不同鏡頭倍率下的雜訊大小與缺陷尺寸都不同，寫死就換產品失準。");
        }

        // ═══════════════════════════════════════════════════════════
        //  ③ Canny 邊緣檢測
        // ═══════════════════════════════════════════════════════════

        private void btnCanny_Click(object sender, EventArgs e)
        {
            if (!EnsureSource()) return;

            try
            {
                Rectangle roi = GetRoi();
                int th1 = (int)numTh1.Value;
                int th2 = (int)numTh2.Value;

                int edgeCount;
                using (Mat roiMat = FeatureOps.ExtractRoi(_srcMat, roi))
                using (Mat edges = FeatureOps.DetectEdges(roiMat, th1, th2, BlurKernelSize))
                {
                    edgeCount = CvInvoke.CountNonZero(edges);
                    SetPictureBoxImage(picResult, ToDisplayBitmap(edges));
                    lblResultCap.Text = "處理結果 — Canny 邊緣 (" + th1 + " / " + th2 + ")";
                }

                long roiPixels = (long)roi.Width * roi.Height;
                double pct = roiPixels == 0 ? 0 : (100.0 * edgeCount / roiPixels);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("[③ Canny 邊緣檢測]  ROI({0},{1},{2},{3})",
                              roi.X, roi.Y, roi.Width, roi.Height));
                sb.AppendLine();
                sb.AppendLine("  CvInvoke.GaussianBlur(roi, blur, new Size(" + BlurKernelSize + ","
                              + BlurKernelSize + "), 0);   // Canny 前務必先去雜訊");
                sb.AppendLine("  CvInvoke.Canny(blur, edges, " + th1 + ", " + th2 + ");");
                sb.AppendLine();
                sb.AppendLine(string.Format("  邊緣像素數：{0:N0} px（佔 ROI 面積 {1:F2}%）", edgeCount, pct));
                sb.AppendLine();
                sb.AppendLine("  threshold1 = 低閾值，負責「延伸連接」已經確定的邊緣；");
                sb.AppendLine("  threshold2 = 高閾值，負責「起頭」——梯度超過它才算強邊緣。");
                sb.AppendLine("  經驗值 threshold2 約為 threshold1 的 2～3 倍；");
                sb.AppendLine("  調低 → 雜訊邊緣被一起留下；調高 → 細微邊緣開始斷裂。");
                sb.AppendLine();
                sb.AppendLine("  邊緣輸出的是「線」不是「區域」，適合量測輪廓位置／寬度（第 8 章尺寸量測），");
                sb.AppendLine("  不適合統計面積——要算「有幾個、各多大」請改用 ② Blob。");
                txtLog.Text = sb.ToString();

                RefreshMainView(null);
                SetVerdictIdle();
            }
            catch (Exception ex)
            {
                ReportError("Canny 邊緣檢測", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  ④ 像素計數
        // ═══════════════════════════════════════════════════════════

        private void btnCountPixels_Click(object sender, EventArgs e)
        {
            if (!EnsureSource()) return;

            try
            {
                Rectangle roi = GetRoi();
                int low = (int)numLow.Value;
                int high = (int)numHigh.Value;
                int maxAllowed = (int)numMaxCount.Value;

                if (low > high)
                {
                    MessageBox.Show(this, "亮度下限不可大於上限。", "像素計數",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int count;
                using (Mat roiMat = FeatureOps.ExtractRoi(_srcMat, roi))
                using (Mat mask = FeatureOps.BuildRangeMask(roiMat, low, high))
                {
                    count = CvInvoke.CountNonZero(mask);
                    SetPictureBoxImage(picResult, ToDisplayBitmap(mask));
                    lblResultCap.Text = "處理結果 — InRange 遮罩 [" + low + ", " + high + "]";
                }

                long roiPixels = (long)roi.Width * roi.Height;
                double pct = roiPixels == 0 ? 0 : (100.0 * count / roiPixels);
                bool isDefect = count > maxAllowed;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("[④ 像素計數]  ROI({0},{1},{2},{3})",
                              roi.X, roi.Y, roi.Width, roi.Height));
                sb.AppendLine();
                sb.AppendLine("  CvInvoke.InRange(roiGray, low(" + low + "), high(" + high + "), mask);");
                sb.AppendLine("  int count = CvInvoke.CountNonZero(mask);");
                sb.AppendLine("  // 等同教材的雙層 for 迴圈，但走向量化實作，快得多");
                sb.AppendLine();
                sb.AppendLine(string.Format("  ROI 總像素       : {0,9:N0} px", roiPixels));
                sb.AppendLine(string.Format("  落在 [{0}, {1}] 的 : {2,9:N0} px   ({3:F2}%)", low, high, count, pct));
                sb.AppendLine(string.Format("  允許上限         : {0,9:N0} px", maxAllowed));
                sb.AppendLine();
                sb.AppendLine(isDefect
                    ? string.Format("  {0:N0} > {1:N0}  →  NG（範圍內像素數超標）", count, maxAllowed)
                    : string.Format("  {0:N0} ≤ {1:N0}  →  OK", count, maxAllowed));
                sb.AppendLine();
                sb.AppendLine("  本工具不需二值化、不需 Blob 分析，直接對灰階計數，");
                sb.AppendLine("  計算量極低，是四個工具中最快的一個。");
                txtLog.Text = sb.ToString();

                RefreshMainView(null);
                SetVerdict(!isDefect, isDefect ? "NG　" + count.ToString("N0") + " px" : "OK");
            }
            catch (Exception ex)
            {
                ReportError("像素計數", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  ⑤ 完整流程（教材 §5）
        // ═══════════════════════════════════════════════════════════

        private void btnRunAll_Click(object sender, EventArgs e)
        {
            if (!EnsureSource()) return;

            try
            {
                Rectangle roi = GetRoi();
                int threshold = (int)numThreshold.Value;
                int minArea = (int)numMinArea.Value;
                int maxArea = (int)numMaxArea.Value;

                Stopwatch sw = new Stopwatch();
                double tRoi, tPre, tBlob;
                BlobResult result;

                sw.Restart();
                using (Mat roiMat = FeatureOps.ExtractRoi(_srcMat, roi))
                {
                    tRoi = sw.Elapsed.TotalMilliseconds;

                    sw.Restart();
                    using (Mat binary = FeatureOps.Preprocess(roiMat, BlurKernelSize, threshold, _morphKernel))
                    {
                        tPre = sw.Elapsed.TotalMilliseconds;

                        sw.Restart();
                        result = FeatureOps.DetectBlobs(binary, minArea, maxArea);
                        tBlob = sw.Elapsed.TotalMilliseconds;

                        OffsetToAbsolute(result, roi);
                        SetPictureBoxImage(picResult, ToDisplayBitmap(binary));
                        lblResultCap.Text = "處理結果 — 完整流程輸出";
                    }
                }

                RefreshMainView(result);
                lblRoiInfo.Text = roi.Width + " × " + roi.Height + " px";

                bool isOk = (result.Kept.Count == 0);
                int scratches = 0;
                foreach (BlobInfo b in result.Kept)
                {
                    if (b.AspectRatio > ScratchAspectRatio) scratches++;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[⑤ 完整流程]  教材 §5「典型異物檢測流程」");
                sb.AppendLine(Divider);
                sb.AppendLine(string.Format("  Step 1  取 ROI    : Rectangle({0}, {1}, {2}, {3}){4}... {5,6:F2} ms",
                              roi.X, roi.Y, roi.Width, roi.Height, "".PadRight(6), tRoi));
                sb.AppendLine(string.Format("  Step 2  前處理    : 高斯 {0}×{0} → 二值化 {1} → Open {2}×{2}   ... {3,6:F2} ms",
                              BlurKernelSize, threshold, MorphKernelSize, tPre));
                sb.AppendLine(string.Format("  Step 3  特徵檢測  : Blob + 面積過濾 [{0}, {1}]{2}... {3,6:F2} ms",
                              minArea, maxArea, "".PadRight(9), tBlob));
                sb.AppendLine(string.Format("  Step 4  判斷      : blobs.Count == 0 ?  OK : NG{0}... {1,6:F2} ms",
                              "".PadRight(11), 0.0));
                sb.AppendLine(Divider);
                sb.AppendLine(string.Format("  總耗時 {0:F2} ms   ｜   偵測到 {1} 個缺陷（{2} 異物 + {3} 刮痕）   →   {4}",
                              tRoi + tPre + tBlob, result.Kept.Count,
                              result.Kept.Count - scratches, scratches, isOk ? "OK" : "NG"));
                AppendBlobTable(sb, result, minArea, maxArea);
                txtLog.Text = sb.ToString();

                SetVerdict(isOk, isOk ? "OK" : "NG　" + result.Kept.Count + " 個缺陷");
            }
            catch (Exception ex)
            {
                ReportError("完整流程", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  顯示
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// 重畫左側主畫面：灰階原圖轉彩色，疊上綠色 ROI 框，
        /// 有 Blob 結果時再疊紅色外接矩形與黃色形心十字。
        /// </summary>
        private void RefreshMainView(BlobResult blobs)
        {
            if (_srcMat == null || _srcMat.IsEmpty) return;

            Rectangle roi = GetRoi();

            using (Mat display = new Mat())
            {
                CvInvoke.CvtColor(_srcMat, display, ColorConversion.Gray2Bgr);
                CvInvoke.Rectangle(display, roi, ColorRoi, 2, LineType.EightConnected, 0);

                if (blobs != null)
                {
                    foreach (BlobInfo b in blobs.Kept)
                    {
                        Rectangle box = Rectangle.Inflate(b.BoundingBox, 2, 2);
                        CvInvoke.Rectangle(display, box, ColorDefect, 2, LineType.EightConnected, 0);

                        int cx = (int)Math.Round(b.Centroid.X);
                        int cy = (int)Math.Round(b.Centroid.Y);
                        CvInvoke.Line(display, new Point(cx - 5, cy), new Point(cx + 5, cy),
                                      ColorCentroid, 2, LineType.EightConnected, 0);
                        CvInvoke.Line(display, new Point(cx, cy - 5), new Point(cx, cy + 5),
                                      ColorCentroid, 2, LineType.EightConnected, 0);

                        CvInvoke.PutText(display, "#" + b.Index, new Point(box.X, box.Y - 5),
                                         FontFace.HersheySimplex, 0.5, ColorDefect, 1,
                                         LineType.EightConnected, false);
                    }
                }

                SetPictureBoxImage(picMain, ToDisplayBitmap(display));
            }
        }

        /// <summary>
        /// 把 Mat 轉成一張「自己擁有像素資料」的 Bitmap 再交給 UI。
        ///
        /// ⚠ 絕對不可以直接把 mat.ToBitmap() 的結果餵給 PictureBox：
        ///
        ///   Emgu 的 ToBitmap() 對 3 通道 BGR 是「共用 Mat 的像素 buffer」而不是複製
        ///   （1 通道灰階因為要建調色盤才會複製——但那是版本相依的實作細節，不能賴）。
        ///   Mat 一旦離開 using 被 Dispose，PictureBox 手上那張 Bitmap 就指向已釋放的記憶體；
        ///   下一次重繪就是 AccessViolation，整個行程無聲消失，而且不會進 try/catch。
        ///
        ///   更陰險的是它不會「馬上」死：剛釋放的記憶體通常還沒被作業系統收回，
        ///   畫面看起來完全正常，要等到那塊記憶體被別的配置重用才炸——
        ///   崩潰點與元兇相隔很遠，正是第 18 章講的「崩潰點是受害者不是元兇」。
        ///
        ///   這就是第 18 章「共享底層 buffer 的物件不能拿來當獨立物件持有」的實例。
        /// </summary>
        private static Bitmap ToDisplayBitmap(Mat mat)
        {
            using (Bitmap shared = mat.ToBitmap())
            {
                Bitmap copy = new Bitmap(shared.Width, shared.Height, PixelFormat.Format24bppRgb);
                try
                {
                    using (Graphics g = Graphics.FromImage(copy))
                    {
                        g.DrawImageUnscaled(shared, 0, 0);
                    }
                    return copy;
                }
                catch
                {
                    copy.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// 換 PictureBox 的圖：暫存 → 換新 → 放舊。
        ///
        /// 這裡放掉舊 Bitmap 是正確的，因為每一張都是我們自己 new 出來（ToDisplayBitmap）再交給控制項的。
        /// 第 18 章的反向陷阱是「不要 Dispose 借來的物件」——若這張圖不是自己配置的就不能碰。
        /// PictureBox 本身不會自動釋放 Image，不自己放就是每按一次漏一張全尺寸點陣圖，
        /// 連續操作下會看到記憶體與 GDI handle 單調上升（P-011 假性 OOM 的來源）。
        /// </summary>
        private static void SetPictureBoxImage(PictureBox pic, Image newImage)
        {
            Image old = pic.Image;
            pic.Image = newImage;
            if (old != null) old.Dispose();
        }

        private void SetVerdict(bool isOk, string text)
        {
            lblVerdict.Text = text;
            lblVerdict.BackColor = isOk ? VerdictOkBack : VerdictNgBack;
            lblVerdict.ForeColor = isOk ? VerdictOkFore : VerdictNgFore;
        }

        private void SetVerdictIdle()
        {
            lblVerdict.Text = "待檢測";
            lblVerdict.BackColor = SystemColors.Control;
            lblVerdict.ForeColor = Color.Gray;
        }

        private void ShowWelcome()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("第 7 章 特徵檢測 — 功能測試程式");
            sb.AppendLine(Divider);
            sb.AppendLine();
            sb.AppendLine("  ① ROI       先框範圍，排除背景、加速計算。座標要動態修正（搭配定位）。");
            sb.AppendLine("  ② Blob      統計「有幾塊、各多大、在哪裡」。面積過濾是去雜訊最後一關。");
            sb.AppendLine("  ③ 邊緣      偵測輪廓位置。要量測尺寸、定位邊界用這個，不用 Blob。");
            sb.AppendLine("  ④ 像素計數  最簡單最快。「某種亮度的面積超標嗎」直接用這個。");
            sb.AppendLine();
            sb.AppendLine(Divider);
            sb.AppendLine();
            sb.AppendLine("請先按左上角的「產生測試圖」（不需準備影像檔），或「載入影像」讀入自己的圖。");
            txtLog.Text = sb.ToString();
        }

        /// <summary>
        /// 錯誤回報。
        ///
        /// 這裡用 MessageBox 是可以的——按鈕事件屬於「使用者觸發的低頻路徑」。
        /// 第 18 章禁止彈 UI 的是「高頻路徑的 catch」（每幀迴圈裡），
        /// 那種地方一律靜默寫檔，否則 MessageBox 自己吃 GDI，會正反饋堆疊成死亡螺旋。
        /// </summary>
        private void ReportError(string what, Exception ex)
        {
            txtLog.Text = "[" + what + "] 發生例外" + Environment.NewLine
                        + Environment.NewLine + ex.ToString();
            MessageBox.Show(this, what + " 失敗：" + ex.Message, "錯誤",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace SizeMeasurement
{
    /// <summary>
    /// 第 8 章 尺寸量測 — 標定／線段量測／點位量測／統計的功能測試程式。
    ///
    /// 【資源釋放】本檔案嚴格遵守第 18／19 章：
    ///   · Mat 欄位覆蓋一律「暫存 → 換新 → 放舊」（P-009 的正解）。
    ///   · PictureBox.Image 換圖時，舊的那張 Bitmap 是「我們自己 new 的」，必須自己 Dispose；
    ///     PictureBox 本身不會幫你放。
    ///   · 所有中間 Mat 一律 using，回傳 Mat 的方法在例外路徑也會釋放已配置的物件。
    ///   · CvInvoke.Moments 回傳的是非受管物件，也要 Dispose（見 MeasureOps.FindBrightCentroid）。
    /// </summary>
    public partial class Form1 : Form
    {
        // ── 來源影像的種類 ──────────────────────────────────────────
        // 標定要在「標定圖」上做、量測要在「量測圖」上做。
        // 程式不強制擋，但會在報告裡提醒——用錯圖標定是新手最常見的錯。
        private enum SourceKind { None, CalibrationTarget, MeasurementOk, MeasurementNg, External }

        // ── 主畫面目前要疊哪一種標記 ────────────────────────────────
        private enum OverlayMode { None, Calibration, Width, Distance, Stat }

        // ── 顏色（OpenCV 用 BGR 順序，不是 RGB）────────────────────
        private static readonly MCvScalar ColorScan = new MCvScalar(0, 224, 224);    // 黃：掃描線
        private static readonly MCvScalar ColorOk = new MCvScalar(0, 224, 0);        // 綠：判定 OK
        private static readonly MCvScalar ColorNg = new MCvScalar(32, 32, 255);      // 紅：判定 NG
        private static readonly MCvScalar ColorTolerance = new MCvScalar(255, 176, 0); // 青：公差帶上下限
        private static readonly MCvScalar ColorPoint = new MCvScalar(255, 96, 255);  // 洋紅：特徵點
        private static readonly MCvScalar ColorGrid = new MCvScalar(72, 72, 72);     // 剖面圖格線
        private static readonly MCvScalar ColorCurve = new MCvScalar(224, 224, 224); // 剖面圖曲線
        private static readonly MCvScalar ColorCanvas = new MCvScalar(28, 28, 28);   // 剖面圖底色

        private static readonly Color VerdictOkBack = Color.FromArgb(223, 246, 221);
        private static readonly Color VerdictOkFore = Color.FromArgb(27, 94, 32);
        private static readonly Color VerdictNgBack = Color.FromArgb(253, 231, 233);
        private static readonly Color VerdictNgFore = Color.FromArgb(164, 38, 44);
        private static readonly Color ScaleReadyFore = Color.FromArgb(27, 94, 32);
        private static readonly Color ScaleMissingFore = Color.FromArgb(179, 107, 0);

        private const string Divider =
            "--------------------------------------------------------------------------------";

        // 剖面圖畫布尺寸（教材 §6：量測結果要看得見，數字之外還要有圖）
        private const int ProfileWidth = 800;
        private const int ProfileHeight = 400;

        // 標定用的掃描線數與取樣帶寬（半寬，px）。
        // 標定是「一次定生死」的工作：這組值定完之後，之後每一件產品的 mm 都建立在它上面，
        // 所以不會只掃一條線就定案——多掃幾條取平均，把雜訊的影響壓到 1/√N。
        private const int CalibLineCount = 5;
        private const int CalibBandHalf = 50;

        // ── 欄位（生命週期與 Form 相同）─────────────────────────────

        /// <summary>目前的來源影像（單通道灰階）。由本 Form 擁有，FormClosed 時釋放。</summary>
        private Mat _srcMat;

        private SourceKind _sourceKind = SourceKind.None;

        /// <summary>
        /// 目前的比例尺。null 代表「還沒標定」——這時候所有量測都只有 px，沒有 mm。
        /// 正式專案這個值來自 Recipe／INI（第 16 章），不是每次開機重標。
        /// </summary>
        private Calibration _calibration;

        // ── 可視化狀態：最後一次量測的結果，供 RefreshMainView 重畫 ──
        private OverlayMode _overlay = OverlayMode.None;
        private EdgePairResult _lastRow;        // 水平量測（寬度／標定的 X 方向）
        private EdgePairResult _lastColumn;     // 垂直量測（標定的 Y 方向）
        private EdgePairResult[] _lastLines;    // 多線統計
        private PointF _hole1;
        private PointF _hole2;
        private bool _lastVerdictOk;
        private double _lastNominalMm;
        private double _lastToleranceMm;

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateScaleLabel();
            ShowWelcome();
        }

        /// <summary>
        /// 收尾：釋放本 Form 自己配置的所有非受管資源。
        /// 第 20 章：行為寫在執行期程式碼，不動 Designer.cs。
        /// </summary>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetPictureBoxImage(picMain, null);
            SetPictureBoxImage(picResult, null);

            if (_srcMat != null) { _srcMat.Dispose(); _srcMat = null; }
        }

        // ═══════════════════════════════════════════════════════════
        //  來源影像
        // ═══════════════════════════════════════════════════════════

        private void btnGenCalib_Click(object sender, EventArgs e)
        {
            try
            {
                SetSource(TestImageGenerator.CreateCalibrationTarget(), SourceKind.CalibrationTarget);
                RefreshMainView();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[產生標定圖]");
                sb.AppendLine(TestImageGenerator.CalibrationDescription);
                sb.AppendLine();
                sb.AppendLine("接著按右側 ① 的「執行標定」，把這片標定板的像素尺寸換算成 mm/px。");
                sb.AppendLine("沒有這一步，後面所有量測都只有 px——第 8 章的一切都建立在標定上。");
                txtLog.Text = sb.ToString();

                SetVerdictIdle();
            }
            catch (Exception ex)
            {
                ReportError("產生標定圖", ex);
            }
        }

        private void btnGenMeasureOk_Click(object sender, EventArgs e)
        {
            GenerateMeasurementImage(false);
        }

        private void btnGenMeasureNg_Click(object sender, EventArgs e)
        {
            GenerateMeasurementImage(true);
        }

        private void GenerateMeasurementImage(bool defective)
        {
            try
            {
                SetSource(TestImageGenerator.CreateMeasurementImage(defective),
                          defective ? SourceKind.MeasurementNg : SourceKind.MeasurementOk);
                numScanY.Value = TestImageGenerator.DefaultScanY;
                RefreshMainView();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(defective ? "[產生量測圖（不良品）]" : "[產生量測圖（良品）]");
                sb.AppendLine(TestImageGenerator.MeasurementDescription(defective));
                sb.AppendLine();
                if (_calibration == null)
                {
                    sb.AppendLine("⚠ 目前尚未標定，量出來的只會是像素。");
                    sb.AppendLine("  請先按「產生標定圖」→ ①「執行標定」，或直接按 ⑤ 一鍵執行完整流程。");
                }
                else
                {
                    sb.AppendLine("目前比例尺：" + _calibration.ToString());
                    sb.AppendLine("請依序執行右側 ② ③ ④，或按 ⑤ 一鍵執行完整流程。");
                }
                txtLog.Text = sb.ToString();

                SetVerdictIdle();
            }
            catch (Exception ex)
            {
                ReportError("產生量測圖", ex);
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

                    SetSource(loaded, SourceKind.External);
                    numScanY.Value = _srcMat.Height / 2;
                    RefreshMainView();

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("[載入影像] " + dlg.FileName);
                    sb.AppendLine("  尺寸: " + _srcMat.Width + " × " + _srcMat.Height + ", 8-bit 灰階");
                    sb.AppendLine();
                    sb.AppendLine("量測範圍已改成整張影像，掃描線 Y 預設在影像中央。");
                    sb.AppendLine("③ 點位量測的搜尋範圍是為內建測試圖設計的固定座標，外部影像請改用內建測試圖示範。");
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
        private void SetSource(Mat newSource, SourceKind kind)
        {
            Mat old = _srcMat;
            _srcMat = newSource;
            if (old != null) old.Dispose();

            _sourceKind = kind;

            // 來源換了，之前的量測結果就過期了，一併清掉
            _overlay = OverlayMode.None;
            _lastRow = null;
            _lastColumn = null;
            _lastLines = null;
            SetPictureBoxImage(picResult, null);
            lblResultCap.Text = "灰階剖面圖（Profile）";
            lblCalibResult.Text = "—";
            lblWidthResult.Text = "—";
            lblDistResult.Text = "—";
            lblStatResult.Text = "—";
        }

        private bool EnsureSource()
        {
            if (_srcMat != null && !_srcMat.IsEmpty) return true;
            MessageBox.Show(this, "請先產生標定圖／量測圖，或載入影像。", "尚未載入影像",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private bool EnsureCalibration()
        {
            if (_calibration != null) return true;
            MessageBox.Show(this,
                "還沒有比例尺，量出來的只有像素。\n\n"
                + "請先按「產生標定圖」，再執行 ①「執行標定」。",
                "尚未標定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        /// <summary>量測範圍：內建測試圖用預設 ROI，外部影像用整張圖。</summary>
        private Rectangle GetMeasureRoi()
        {
            if (_srcMat.Width == TestImageGenerator.Width && _srcMat.Height == TestImageGenerator.Height)
                return TestImageGenerator.MeasureRoi;
            return new Rectangle(0, 0, _srcMat.Width, _srcMat.Height);
        }

        /// <summary>邊緣閾值：UI 填 0 表示「自動取該掃描線的灰階中點」。</summary>
        private double GetEdgeThreshold()
        {
            return (double)numEdgeTh.Value;
        }

        // ═══════════════════════════════════════════════════════════
        //  ① 標定（教材 §1）
        // ═══════════════════════════════════════════════════════════

        private void btnCalibrate_Click(object sender, EventArgs e)
        {
            if (!EnsureSource()) return;

            try
            {
                StringBuilder sb = new StringBuilder();
                Stopwatch sw = Stopwatch.StartNew();
                bool ok = DoCalibrate(sb);
                sw.Stop();

                sb.AppendLine();
                sb.AppendLine("  耗時 " + sw.ElapsedMilliseconds + " ms");
                txtLog.Text = sb.ToString();

                if (ok) SetVerdictIdle();
                else SetVerdict(false, "標定失敗");
            }
            catch (Exception ex)
            {
                ReportError("執行標定", ex);
            }
        }

        /// <summary>
        /// 標定的核心：在標定板上量出「X 方向像素寬」與「Y 方向像素高」，各自除以已知尺寸。
        ///
        /// 取樣位置是影像中央的一條橫帶與一條豎帶（標定板置中，一定穿得過），
        /// 每個方向掃 CalibLineCount 條線取平均——單線會被雜訊帶著跑，
        /// 而比例尺一旦標偏，之後每一件產品的 mm 都跟著偏，錯誤沒有自我修正的機會。
        /// </summary>
        private bool DoCalibrate(StringBuilder sb)
        {
            double knownMm = (double)numKnownMm.Value;
            double threshold = GetEdgeThreshold();

            int midY = _srcMat.Height / 2;
            int midX = _srcMat.Width / 2;
            Rectangle rowBand = new Rectangle(0, midY - CalibBandHalf, _srcMat.Width, CalibBandHalf * 2);
            Rectangle colBand = new Rectangle(midX - CalibBandHalf, 0, CalibBandHalf * 2, _srcMat.Height);

            EdgePairResult[] rows = MeasureOps.MeasureMultipleRows(_srcMat, rowBand, CalibLineCount, threshold);
            EdgePairResult[] cols = MeasureOps.MeasureMultipleColumns(_srcMat, colBand, CalibLineCount, threshold);

            StatResult rowStat = MeasureOps.CalcStats(MeasureOps.LengthsOf(rows));
            StatResult colStat = MeasureOps.CalcStats(MeasureOps.LengthsOf(cols));

            EdgePairResult row = rows[CalibLineCount / 2];   // 中央那條，拿來畫圖與畫剖面
            EdgePairResult col = cols[CalibLineCount / 2];

            sb.AppendLine("[① 標定 Calibration]（教材 §1）");
            sb.AppendLine(Divider);
            sb.AppendLine("  標定板已知尺寸 : " + knownMm.ToString("F3") + " mm × " + knownMm.ToString("F3") + " mm");
            sb.AppendLine("  邊緣閾值       : " + (threshold > 0 ? threshold.ToString("F0") : "自動（該線灰階中點）"));
            sb.AppendLine("  取樣           : X、Y 各掃 " + CalibLineCount + " 條線取平均（單線會被雜訊帶著跑）");
            sb.AppendLine();

            if (_sourceKind != SourceKind.CalibrationTarget && _sourceKind != SourceKind.External)
            {
                sb.AppendLine("⚠ 目前顯示的是「量測圖」，不是標定圖。");
                sb.AppendLine("  在待測件上標定，等於拿產品當尺——量到的尺寸會被寫成比例尺，之後每一件都錯。");
                sb.AppendLine();
            }

            if (rowStat.Count == 0 || colStat.Count == 0 || !row.Found || !col.Found)
            {
                sb.AppendLine("✗ 標定失敗：掃描線找不到成對的邊緣。");
                sb.AppendLine("  掃描線必須「從背景開始、在背景結束」，中間只有標定板一個目標。");
                _overlay = OverlayMode.None;
                RefreshMainView();
                return false;
            }

            _calibration = Calibration.FromSquareTarget(knownMm, rowStat.Mean, colStat.Mean);

            sb.AppendLine("  X 方向（" + rowStat.Count + " 條水平線，中央第 " + row.ScanPos + " 列）");
            sb.AppendLine("    中央線左緣 " + row.FirstEdge.ToString("F3") + " px、右緣 " + row.SecondEdge.ToString("F3") + " px"
                        + "（灰階範圍 " + row.ProfileMin.ToString("F0") + " ~ " + row.ProfileMax.ToString("F0") + "）");
            sb.AppendLine("    寬度平均 = " + rowStat.Mean.ToString("F4") + " px"
                        + "（σ = " + rowStat.StdDev.ToString("F4") + " px、全距 " + rowStat.Range.ToString("F4") + " px）");
            sb.AppendLine("    scaleX = " + knownMm.ToString("F3") + " / " + rowStat.Mean.ToString("F4")
                        + " = " + _calibration.ScaleX.ToString("F6") + " mm/px"
                        + "（" + _calibration.ScaleXMicron.ToString("F3") + " μm/px）");
            sb.AppendLine();
            sb.AppendLine("  Y 方向（" + colStat.Count + " 條垂直線，中央第 " + col.ScanPos + " 行）");
            sb.AppendLine("    中央線上緣 " + col.FirstEdge.ToString("F3") + " px、下緣 " + col.SecondEdge.ToString("F3") + " px");
            sb.AppendLine("    高度平均 = " + colStat.Mean.ToString("F4") + " px"
                        + "（σ = " + colStat.StdDev.ToString("F4") + " px、全距 " + colStat.Range.ToString("F4") + " px）");
            sb.AppendLine("    scaleY = " + knownMm.ToString("F3") + " / " + colStat.Mean.ToString("F4")
                        + " = " + _calibration.ScaleY.ToString("F6") + " mm/px"
                        + "（" + _calibration.ScaleYMicron.ToString("F3") + " μm/px）");
            sb.AppendLine();
            sb.AppendLine("  X／Y 差異 : " + _calibration.AnisotropyPercent.ToString("F2") + " %");
            if (_calibration.AnisotropyPercent >= 0.5)
            {
                sb.AppendLine("    → 差這麼多就不能假設 scaleX = scaleY。");
                sb.AppendLine("      斜向距離若只用單一比例尺，會系統性錯掉同樣的百分比（見 ③）。");
            }
            else
            {
                sb.AppendLine("    → 差異很小，粗略量測可以取平均值；精密量測仍建議分開用。");
            }
            sb.AppendLine();
            sb.AppendLine("  ⓘ 正式專案：這兩個值屬於機台參數，要存進 Recipe／INI（第 16 章）；");
            sb.AppendLine("    換鏡頭、調焦、動到相機安裝位置之後，一律重新標定。");

            _lastRow = row;
            _lastColumn = col;
            _overlay = OverlayMode.Calibration;

            lblCalibResult.Text = _calibration.ScaleX.ToString("F6") + " / " + _calibration.ScaleY.ToString("F6");
            UpdateScaleLabel();
            RefreshMainView();
            ShowProfile(MeasureOps.ExtractRowProfile(_srcMat, row.ScanPos, 0, _srcMat.Width - 1), 0,
                        ResolveThreshold(row, threshold), row,
                        "Calibration target - row " + row.ScanPos);
            return true;
        }

        // ═══════════════════════════════════════════════════════════
        //  ② 線段量測：寬度（教材 §2／§4）
        // ═══════════════════════════════════════════════════════════

        private void btnMeasureWidth_Click(object sender, EventArgs e)
        {
            if (!EnsureSource()) return;

            try
            {
                StringBuilder sb = new StringBuilder();
                Stopwatch sw = Stopwatch.StartNew();
                bool ok = DoMeasureWidth(sb);
                sw.Stop();

                sb.AppendLine();
                sb.AppendLine("  耗時 " + sw.ElapsedMilliseconds + " ms");
                txtLog.Text = sb.ToString();

                if (!ok) SetVerdict(false, "量測失敗");
            }
            catch (Exception ex)
            {
                ReportError("量測寬度", ex);
            }
        }

        private bool DoMeasureWidth(StringBuilder sb)
        {
            Rectangle roi = GetMeasureRoi();
            int scanY = (int)numScanY.Value;
            double threshold = GetEdgeThreshold();
            double nominal = (double)numNominal.Value;
            double tolerance = (double)numTolerance.Value;

            double[] profile = MeasureOps.ExtractRowProfile(_srcMat, scanY, roi.Left, roi.Right - 1);
            EdgePairResult row = MeasureOps.MeasureAlongRow(_srcMat, scanY, roi.Left, roi.Right - 1, threshold);
            double usedTh = ResolveThreshold(row, threshold);

            sb.AppendLine("[② 線段量測 — 寬度]（教材 §2／§4）");
            sb.AppendLine(Divider);
            sb.AppendLine("  掃描範圍 : X " + roi.Left + " ~ " + (roi.Right - 1) + "，掃描線 Y = " + scanY);
            sb.AppendLine("  邊緣閾值 : " + usedTh.ToString("F1")
                        + (threshold > 0 ? "（手動）" : "（自動：該線灰階中點）"));
            sb.AppendLine("  灰階範圍 : " + row.ProfileMin.ToString("F0") + " ~ " + row.ProfileMax.ToString("F0"));
            sb.AppendLine();

            if (!row.Found)
            {
                sb.AppendLine("✗ 找不到成對的邊緣。");
                sb.AppendLine("  檢查：掃描線有沒有穿過待測物？兩端是不是都留了背景？閾值是不是落在明暗之間？");
                _overlay = OverlayMode.None;
                lblWidthResult.Text = "找不到邊緣";
                RefreshMainView();
                ShowProfile(profile, roi.Left, usedTh, row, "Scan row " + scanY);
                return false;
            }

            sb.AppendLine("  Step 1  抽出掃描線灰階（" + profile.Length + " 點）");
            sb.AppendLine("  Step 2  找邊緣（線性內插取亞像素）");
            sb.AppendLine("            左緣 = " + row.FirstEdge.ToString("F3") + " px");
            sb.AppendLine("            右緣 = " + row.SecondEdge.ToString("F3") + " px");
            sb.AppendLine("            寬度 = " + row.LengthPx.ToString("F3") + " px");

            _lastRow = row;
            _overlay = OverlayMode.Width;
            _lastNominalMm = nominal;
            _lastToleranceMm = tolerance;

            if (_calibration == null)
            {
                sb.AppendLine();
                sb.AppendLine("  Step 3  換算 mm — ⚠ 尚未標定，跳過。");
                sb.AppendLine("            沒有比例尺就沒有 mm，這個數字目前只能跟「像素」比。");
                lblWidthResult.Text = row.LengthPx.ToString("F3") + " px（未標定）";
                _lastVerdictOk = false;
                RefreshMainView();
                ShowProfile(profile, roi.Left, usedTh, row, "Scan row " + scanY);
                SetVerdictIdle();
                return true;
            }

            double widthMm = _calibration.ToMmX(row.LengthPx);
            bool isOk = MeasureOps.IsWithinTolerance(widthMm, nominal, tolerance);

            sb.AppendLine("  Step 3  換算 mm：" + row.LengthPx.ToString("F3") + " px × "
                        + _calibration.ScaleX.ToString("F6") + " mm/px = " + widthMm.ToString("F4") + " mm");
            sb.AppendLine("  Step 4  公差判定：" + nominal.ToString("F3") + " ± " + tolerance.ToString("F3") + " mm"
                        + "（" + (nominal - tolerance).ToString("F3") + " ~ " + (nominal + tolerance).ToString("F3") + "）");
            sb.AppendLine("            偏差 = " + ((widthMm - nominal) * 1000.0).ToString("F1") + " μm"
                        + "  →  " + (isOk ? "OK" : "NG"));
            sb.AppendLine();
            sb.AppendLine("  ⓘ 亞像素的價值：1 px = " + _calibration.ScaleXMicron.ToString("F1") + " μm。");
            sb.AppendLine("    只取整數像素的話，光量化誤差就佔掉公差帶 ±"
                        + (tolerance * 1000.0).ToString("F0") + " μm（全寬 "
                        + (tolerance * 2000.0).ToString("F0") + " μm）的 "
                        + (_calibration.ScaleXMicron / (tolerance * 2000.0) * 100.0).ToString("F0") + " %。");

            lblWidthResult.Text = widthMm.ToString("F4") + " mm";
            _lastVerdictOk = isOk;
            RefreshMainView();
            ShowProfile(profile, roi.Left, usedTh, row, "Scan row " + scanY);
            SetVerdict(isOk, isOk ? "OK  " + widthMm.ToString("F3") + " mm" : "NG  " + widthMm.ToString("F3") + " mm");
            return true;
        }

        // ═══════════════════════════════════════════════════════════
        //  ③ 點位量測：距離與角度（教材 §3）
        // ═══════════════════════════════════════════════════════════

        private void btnMeasureDist_Click(object sender, EventArgs e)
        {
            if (!EnsureSource()) return;
            if (!EnsureCalibration()) return;

            try
            {
                StringBuilder sb = new StringBuilder();
                Stopwatch sw = Stopwatch.StartNew();
                bool ok = DoMeasureDistance(sb);
                sw.Stop();

                sb.AppendLine();
                sb.AppendLine("  耗時 " + sw.ElapsedMilliseconds + " ms");
                txtLog.Text = sb.ToString();

                if (!ok) SetVerdict(false, "量測失敗");
            }
            catch (Exception ex)
            {
                ReportError("量測距離與角度", ex);
            }
        }

        private bool DoMeasureDistance(StringBuilder sb)
        {
            sb.AppendLine("[③ 點位量測 — 距離與角度]（教材 §3／§4）");
            sb.AppendLine(Divider);

            if (_srcMat.Width != TestImageGenerator.Width || _srcMat.Height != TestImageGenerator.Height)
            {
                sb.AppendLine("✗ 定位孔的搜尋範圍是為內建測試圖（800 × 600）寫死的固定座標。");
                sb.AppendLine("  外部影像請改用內建量測圖示範，或依第 9 章用模板匹配動態定位。");
                return false;
            }

            // 孔比背景亮，用固定閾值 128 把它挑出來即可（孔的灰階 210、背景 40）
            const double HoleThreshold = 128.0;
            PointF h1 = MeasureOps.FindBrightCentroid(_srcMat, TestImageGenerator.Hole1SearchArea, HoleThreshold);
            PointF h2 = MeasureOps.FindBrightCentroid(_srcMat, TestImageGenerator.Hole2SearchArea, HoleThreshold);

            if (h1.IsEmpty || h2.IsEmpty)
            {
                sb.AppendLine("✗ 搜尋範圍內找不到亮區，無法取形心。");
                return false;
            }

            double dxPx = h2.X - h1.X;
            double dyPx = h2.Y - h1.Y;
            double distRight = _calibration.DistanceMm(h1, h2);
            double distWrong = _calibration.DistanceMmWrongDemo(h1, h2);
            double angleRight = _calibration.AngleDeg(h1, h2);
            double anglePx = Calibration.AngleDegPxDemo(h1, h2);

            sb.AppendLine("  孔 1 形心 : (" + h1.X.ToString("F3") + ", " + h1.Y.ToString("F3") + ") px");
            sb.AppendLine("  孔 2 形心 : (" + h2.X.ToString("F3") + ", " + h2.Y.ToString("F3") + ") px");
            sb.AppendLine("  Δ         : dx = " + dxPx.ToString("F3") + " px、dy = " + dyPx.ToString("F3") + " px");
            sb.AppendLine();
            sb.AppendLine("  ── 距離 ─────────────────────────────────────────────");
            sb.AppendLine("  正解：dx、dy 各自換 mm 再開根");
            sb.AppendLine("        dxMm = " + dxPx.ToString("F3") + " × " + _calibration.ScaleX.ToString("F6")
                        + " = " + _calibration.ToMmX(dxPx).ToString("F4") + " mm");
            sb.AppendLine("        dyMm = " + dyPx.ToString("F3") + " × " + _calibration.ScaleY.ToString("F6")
                        + " = " + _calibration.ToMmY(dyPx).ToString("F4") + " mm");
            sb.AppendLine("        dist = √(dxMm² + dyMm²) = " + distRight.ToString("F4") + " mm");
            sb.AppendLine();
            sb.AppendLine("  ✗ 錯法：先對像素座標開根，最後才乘一個 scaleX（教材 §4 的測驗題）");
            sb.AppendLine("        dist = √(dxPx² + dyPx²) × scaleX = " + distWrong.ToString("F4") + " mm");
            sb.AppendLine();
            double diffUm = Math.Abs(distRight - distWrong) * 1000.0;
            double tolBandUm = (double)numTolerance.Value * 2000.0;   // 公差帶全寬（±tol 的兩倍）
            sb.AppendLine("  兩者差 " + diffUm.ToString("F1") + " μm"
                        + "（" + (Math.Abs(distRight - distWrong) / distRight * 100.0).ToString("F2") + " %）");
            sb.AppendLine("    → 這個錯誤不會拋例外、不會讓畫面怪怪的，只會讓每一件都偏同一個量。");
            if (tolBandUm > 0.0)
            {
                sb.AppendLine("      以目前的公差帶 ±" + ((double)numTolerance.Value * 1000.0).ToString("F0")
                            + " μm（全寬 " + tolBandUm.ToString("F0") + " μm）來看，這一項就佔掉 "
                            + (diffUm / tolBandUm * 100.0).ToString("F0") + " %。");
            }
            sb.AppendLine();
            sb.AppendLine("  ── 角度 ─────────────────────────────────────────────");
            sb.AppendLine("  正解（先換 mm）: atan2(dyMm, dxMm) = " + angleRight.ToString("F3") + "°");
            sb.AppendLine("  ✗ 直接用像素    : atan2(dyPx, dxPx) = " + anglePx.ToString("F3") + "°");
            sb.AppendLine("  兩者差 " + Math.Abs(angleRight - anglePx).ToString("F3") + "°");
            sb.AppendLine("    → X、Y 比例尺不同時，「影像裡看到的角度」不等於「實際角度」。");

            _hole1 = h1;
            _hole2 = h2;
            _overlay = OverlayMode.Distance;
            _lastVerdictOk = true;

            lblDistResult.Text = distRight.ToString("F4") + " mm";
            RefreshMainView();
            SetPictureBoxImage(picResult, null);
            lblResultCap.Text = "灰階剖面圖（Profile）— 點位量測不使用掃描線";
            SetVerdictIdle();
            return true;
        }

        // ═══════════════════════════════════════════════════════════
        //  ④ 統計量測（教材 §5）
        // ═══════════════════════════════════════════════════════════

        private void btnMeasureStat_Click(object sender, EventArgs e)
        {
            if (!EnsureSource()) return;

            try
            {
                StringBuilder sb = new StringBuilder();
                Stopwatch sw = Stopwatch.StartNew();
                bool ok = DoMeasureStat(sb);
                sw.Stop();

                sb.AppendLine();
                sb.AppendLine("  耗時 " + sw.ElapsedMilliseconds + " ms");
                txtLog.Text = sb.ToString();

                if (!ok) SetVerdict(false, "量測失敗");
            }
            catch (Exception ex)
            {
                ReportError("多線量測與統計", ex);
            }
        }

        private bool DoMeasureStat(StringBuilder sb)
        {
            Rectangle roi = GetMeasureRoi();
            int lineCount = (int)numLineCount.Value;
            double threshold = GetEdgeThreshold();
            double nominal = (double)numNominal.Value;
            double tolerance = (double)numTolerance.Value;
            double maxStd = (double)numMaxStd.Value;

            EdgePairResult[] lines = MeasureOps.MeasureMultipleRows(_srcMat, roi, lineCount, threshold);

            sb.AppendLine("[④ 統計量測]（教材 §5）");
            sb.AppendLine(Divider);
            sb.AppendLine("  掃描範圍 : " + roi.X + ", " + roi.Y + ", " + roi.Width + " × " + roi.Height
                        + "，掃描線 " + lineCount + " 條");
            sb.AppendLine();

            int found = 0;
            for (int i = 0; i < lines.Length; i++) if (lines[i].Found) found++;

            if (found == 0)
            {
                sb.AppendLine("✗ 沒有任何一條掃描線找到成對的邊緣。");
                _overlay = OverlayMode.None;
                lblStatResult.Text = "找不到邊緣";
                RefreshMainView();
                return false;
            }

            bool calibrated = (_calibration != null);
            string unit = calibrated ? "mm" : "px";

            double[] values = new double[found];
            int k = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].Found) continue;
                values[k++] = calibrated ? _calibration.ToMmX(lines[i].LengthPx) : lines[i].LengthPx;
            }

            StatResult stat = MeasureOps.CalcStats(values);

            sb.AppendLine("  #   掃描線 Y    左緣 px      右緣 px      寬度 px      寬度 " + unit);
            sb.AppendLine("  " + Divider.Substring(0, 74));
            k = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                EdgePairResult r = lines[i];
                if (!r.Found)
                {
                    sb.AppendLine("  " + (i + 1).ToString().PadLeft(2) + "  " + r.ScanPos.ToString().PadLeft(8)
                                + "    （找不到成對邊緣）");
                    continue;
                }
                sb.AppendLine("  " + (i + 1).ToString().PadLeft(2)
                            + "  " + r.ScanPos.ToString().PadLeft(8)
                            + "  " + r.FirstEdge.ToString("F3").PadLeft(11)
                            + "  " + r.SecondEdge.ToString("F3").PadLeft(11)
                            + "  " + r.LengthPx.ToString("F3").PadLeft(11)
                            + "  " + values[k++].ToString("F4").PadLeft(11));
            }
            sb.AppendLine();
            sb.AppendLine("  樣本數     : " + stat.Count + " / " + lineCount);
            sb.AppendLine("  平均值     : " + stat.Mean.ToString("F4") + " " + unit);
            sb.AppendLine("  標準差 σ   : " + stat.StdDev.ToString("F4") + " " + unit
                        + (calibrated ? "（" + (stat.StdDev * 1000.0).ToString("F2") + " μm）" : ""));
            sb.AppendLine("  最小 / 最大: " + stat.Min.ToString("F4") + " / " + stat.Max.ToString("F4") + " " + unit);
            sb.AppendLine("  全距       : " + stat.Range.ToString("F4") + " " + unit);
            sb.AppendLine();

            _lastLines = lines;
            _overlay = OverlayMode.Stat;
            _lastNominalMm = nominal;
            _lastToleranceMm = tolerance;

            // 剖面圖顯示中間那條掃描線，讓「10 條線的統計」也看得到其中一條的長相
            int midIndex = lines.Length / 2;
            EdgePairResult midLine = lines[midIndex];
            ShowProfile(MeasureOps.ExtractRowProfile(_srcMat, midLine.ScanPos, roi.Left, roi.Right - 1),
                        roi.Left, ResolveThreshold(midLine, threshold), midLine,
                        "Line " + (midIndex + 1) + " / " + lines.Length + " - row " + midLine.ScanPos);

            if (!calibrated)
            {
                sb.AppendLine("  ⚠ 尚未標定，以上都是像素值，無法做公差判定。");
                lblStatResult.Text = stat.Mean.ToString("F3") + " px（未標定）";
                _lastVerdictOk = false;
                RefreshMainView();
                SetVerdictIdle();
                return true;
            }

            bool meanOk = MeasureOps.IsWithinTolerance(stat.Mean, nominal, tolerance);
            bool stdOk = stat.StdDev <= maxStd;
            bool isOk = meanOk && stdOk;

            sb.AppendLine("  ── 判定（兩個條件都要成立）──────────────────────────");
            sb.AppendLine("  ① 平均值在公差內 : |" + stat.Mean.ToString("F4") + " − " + nominal.ToString("F3") + "| = "
                        + Math.Abs(stat.Mean - nominal).ToString("F4") + " ≦ " + tolerance.ToString("F3")
                        + "  →  " + (meanOk ? "OK" : "NG"));
            sb.AppendLine("  ② 標準差夠小     : " + stat.StdDev.ToString("F4") + " ≦ " + maxStd.ToString("F4")
                        + "  →  " + (stdOk ? "OK" : "NG"));
            sb.AppendLine("  綜合判定         : " + (isOk ? "OK" : "NG"));
            sb.AppendLine();
            if (meanOk && !stdOk)
            {
                sb.AppendLine("  ⓘ 這正是教材 §5 情境判斷題的狀況：平均值剛好過關，但數值抖動太大。");
                sb.AppendLine("    標準差過大代表這個量測本身不可信，只看平均值判 OK 是不妥當的。");
            }
            else
            {
                sb.AppendLine("  ⓘ 只看平均值是不夠的——平均值落在公差內可能只是運氣。");
                sb.AppendLine("    「平均值在公差內」AND「標準差夠小」兩個條件同時成立，這個 OK 才可信。");
            }

            lblStatResult.Text = stat.Mean.ToString("F4") + " ± " + stat.StdDev.ToString("F4");
            _lastVerdictOk = isOk;
            RefreshMainView();
            SetVerdict(isOk, (isOk ? "OK  " : "NG  ") + stat.Mean.ToString("F3") + " mm  σ="
                           + (stat.StdDev * 1000.0).ToString("F1") + " μm");
            return true;
        }

        // ═══════════════════════════════════════════════════════════
        //  ⑤ 完整流程
        // ═══════════════════════════════════════════════════════════

        private void btnRunAll_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                Stopwatch sw = new Stopwatch();

                sb.AppendLine("[⑤ 一鍵執行完整流程]");
                sb.AppendLine(Divider);
                sb.AppendLine("實務順序：先用標定板取得比例尺，再拿它去量產品。");
                sb.AppendLine();

                // Step 1：標定圖 → 標定
                sw.Restart();
                SetSource(TestImageGenerator.CreateCalibrationTarget(), SourceKind.CalibrationTarget);
                bool ok = DoCalibrate(sb);
                sw.Stop();
                sb.AppendLine("  耗時 " + sw.ElapsedMilliseconds + " ms");
                sb.AppendLine();

                if (!ok)
                {
                    txtLog.Text = sb.ToString();
                    SetVerdict(false, "流程中止：標定失敗");
                    return;
                }

                // Step 2：換上量測圖 → 單線寬度
                SetSource(TestImageGenerator.CreateMeasurementImage(false), SourceKind.MeasurementOk);
                numScanY.Value = TestImageGenerator.DefaultScanY;

                sw.Restart();
                DoMeasureWidth(sb);
                sw.Stop();
                sb.AppendLine("  耗時 " + sw.ElapsedMilliseconds + " ms");
                sb.AppendLine();

                // Step 3：點位量測
                sw.Restart();
                DoMeasureDistance(sb);
                sw.Stop();
                sb.AppendLine("  耗時 " + sw.ElapsedMilliseconds + " ms");
                sb.AppendLine();

                // Step 4：多線統計（這一步的判定當作最終判定）
                sw.Restart();
                DoMeasureStat(sb);
                sw.Stop();
                sb.AppendLine("  耗時 " + sw.ElapsedMilliseconds + " ms");
                sb.AppendLine();

                sb.AppendLine(Divider);
                sb.AppendLine("流程結束。最終判定以 ④ 的統計結果為準（平均值 AND 標準差都要過）。");
                sb.AppendLine("想看 NG 的樣子，按上方「產生量測圖（不良品）」再執行 ② 或 ④。");
                txtLog.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                ReportError("一鍵執行完整流程", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  可視化（教材 §6）
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// 重畫左側主畫面：灰階原圖轉彩色，再依目前的量測種類疊上標記。
        ///
        /// 教材 §6：量測結果不能只有數字，要在影像上標出來——
        /// 操作員才看得出「程式到底量到哪兩點」，除錯時才知道是演算法錯還是取像錯。
        /// </summary>
        private void RefreshMainView()
        {
            if (_srcMat == null || _srcMat.IsEmpty) return;

            using (Mat display = new Mat())
            {
                CvInvoke.CvtColor(_srcMat, display, ColorConversion.Gray2Bgr);

                switch (_overlay)
                {
                    case OverlayMode.Calibration: DrawCalibrationOverlay(display); break;
                    case OverlayMode.Width: DrawWidthOverlay(display); break;
                    case OverlayMode.Distance: DrawDistanceOverlay(display); break;
                    case OverlayMode.Stat: DrawStatOverlay(display); break;
                }

                SetPictureBoxImage(picMain, ToDisplayBitmap(display));
            }
        }

        private void DrawCalibrationOverlay(Mat display)
        {
            if (_lastRow == null || _lastColumn == null) return;

            int y = _lastRow.ScanPos;
            int x = _lastColumn.ScanPos;

            DrawMeasureLine(display, (int)Math.Round(_lastRow.FirstEdge), y,
                            (int)Math.Round(_lastRow.SecondEdge), y, ColorOk);
            DrawMeasureLine(display, x, (int)Math.Round(_lastColumn.FirstEdge),
                            x, (int)Math.Round(_lastColumn.SecondEdge), ColorOk);

            // PutText 不支援中文（會變成問號），影像上的標註一律用英數字
            PutLabel(display, _lastRow.LengthPx.ToString("F1") + " px",
                     new Point((int)_lastRow.FirstEdge + 12, y - 12), ColorOk);
            PutLabel(display, _lastColumn.LengthPx.ToString("F1") + " px",
                     new Point(x + 12, (int)_lastColumn.FirstEdge + 24), ColorOk);
        }

        private void DrawWidthOverlay(Mat display)
        {
            if (_lastRow == null || !_lastRow.Found) return;

            Rectangle roi = GetMeasureRoi();
            int y = _lastRow.ScanPos;
            MCvScalar color = _lastVerdictOk ? ColorOk : ColorNg;

            // 掃描線（黃色細線）：讓人看到「這條線掃過哪裡」
            CvInvoke.Line(display, new Point(roi.Left, y), new Point(roi.Right - 1, y),
                          ColorScan, 1, LineType.EightConnected, 0);

            int left = (int)Math.Round(_lastRow.FirstEdge);
            int right = (int)Math.Round(_lastRow.SecondEdge);
            DrawMeasureLine(display, left, y, right, y, color);

            // 公差帶：把 nominal ± tolerance 換算回像素，畫在量測線段的兩側
            if (_calibration != null && _lastToleranceMm > 0.0)
            {
                double centerPx = (_lastRow.FirstEdge + _lastRow.SecondEdge) / 2.0;
                double halfLo = _calibration.ToPxX(_lastNominalMm - _lastToleranceMm) / 2.0;
                double halfHi = _calibration.ToPxX(_lastNominalMm + _lastToleranceMm) / 2.0;

                DrawTolerimit(display, centerPx - halfLo, y);
                DrawTolerimit(display, centerPx + halfLo, y);
                DrawTolerimit(display, centerPx - halfHi, y);
                DrawTolerimit(display, centerPx + halfHi, y);
            }

            string text = (_calibration != null)
                ? _calibration.ToMmX(_lastRow.LengthPx).ToString("F3") + " mm"
                : _lastRow.LengthPx.ToString("F2") + " px";
            PutLabel(display, text + (_calibration != null ? (_lastVerdictOk ? "  OK" : "  NG") : ""),
                     new Point(left + 8, y - 14), color);
        }

        private void DrawDistanceOverlay(Mat display)
        {
            Point p1 = new Point((int)Math.Round(_hole1.X), (int)Math.Round(_hole1.Y));
            Point p2 = new Point((int)Math.Round(_hole2.X), (int)Math.Round(_hole2.Y));

            CvInvoke.Rectangle(display, TestImageGenerator.Hole1SearchArea, ColorScan, 1, LineType.EightConnected, 0);
            CvInvoke.Rectangle(display, TestImageGenerator.Hole2SearchArea, ColorScan, 1, LineType.EightConnected, 0);

            CvInvoke.Line(display, p1, p2, ColorPoint, 2, LineType.EightConnected, 0);
            DrawCross(display, p1, ColorPoint, 9);
            DrawCross(display, p2, ColorPoint, 9);

            if (_calibration != null)
            {
                // 標註放在兩個搜尋框的外側（上方與下方），避免壓到形心十字與框線
                int textX = Math.Min(p1.X, p2.X) + 12;
                int topY = Math.Min(TestImageGenerator.Hole1SearchArea.Top,
                                    TestImageGenerator.Hole2SearchArea.Top) - 8;
                int bottomY = Math.Max(TestImageGenerator.Hole1SearchArea.Bottom,
                                       TestImageGenerator.Hole2SearchArea.Bottom) + 22;

                PutLabel(display, _calibration.DistanceMm(_hole1, _hole2).ToString("F4") + " mm",
                         new Point(textX, topY), ColorPoint);
                PutLabel(display, _calibration.AngleDeg(_hole1, _hole2).ToString("F2") + " deg",
                         new Point(textX, bottomY), ColorPoint);
            }
        }

        private void DrawStatOverlay(Mat display)
        {
            if (_lastLines == null) return;

            MCvScalar color = _lastVerdictOk ? ColorOk : ColorNg;
            Rectangle roi = GetMeasureRoi();
            CvInvoke.Rectangle(display, roi, ColorScan, 1, LineType.EightConnected, 0);

            for (int i = 0; i < _lastLines.Length; i++)
            {
                EdgePairResult r = _lastLines[i];
                if (!r.Found) continue;

                int y = r.ScanPos;
                CvInvoke.Line(display,
                              new Point((int)Math.Round(r.FirstEdge), y),
                              new Point((int)Math.Round(r.SecondEdge), y),
                              color, 1, LineType.EightConnected, 0);
                DrawCross(display, new Point((int)Math.Round(r.FirstEdge), y), color, 4);
                DrawCross(display, new Point((int)Math.Round(r.SecondEdge), y), color, 4);
            }

            PutLabel(display, _lastLines.Length + " lines", new Point(roi.Left + 4, roi.Top - 8), ColorScan);
        }

        /// <summary>畫一段量測線：兩端十字 + 中間連線，這是教材 §6 的標準標記法。</summary>
        private static void DrawMeasureLine(Mat display, int x1, int y1, int x2, int y2, MCvScalar color)
        {
            CvInvoke.Line(display, new Point(x1, y1), new Point(x2, y2), color, 2, LineType.EightConnected, 0);
            DrawCross(display, new Point(x1, y1), color, 8);
            DrawCross(display, new Point(x2, y2), color, 8);
        }

        private static void DrawCross(Mat display, Point center, MCvScalar color, int half)
        {
            CvInvoke.Line(display, new Point(center.X - half, center.Y), new Point(center.X + half, center.Y),
                          color, 2, LineType.EightConnected, 0);
            CvInvoke.Line(display, new Point(center.X, center.Y - half), new Point(center.X, center.Y + half),
                          color, 2, LineType.EightConnected, 0);
        }

        /// <summary>公差帶的一條上下限標線。</summary>
        private static void DrawTolerimit(Mat display, double x, int y)
        {
            int xi = (int)Math.Round(x);
            CvInvoke.Line(display, new Point(xi, y - 16), new Point(xi, y + 16),
                          ColorTolerance, 1, LineType.EightConnected, 0);
        }

        /// <summary>
        /// 在影像上寫字。
        ///
        /// ⚠ CvInvoke.PutText 只支援 ASCII，中文一律變成問號——OpenCV 的內建字型沒有中文字模。
        ///   所以影像上的標註只放數字與英文，中文說明留在下方的量測報告與 UI 控制項。
        ///   真的要在影像上寫中文，得改用 GDI+（Graphics.DrawString）另外畫，
        ///   那時 Font／Brush／Graphics 都是 GDI 物件，一樣要 using 包好（第 18 章）。
        /// </summary>
        private static void PutLabel(Mat display, string text, Point pos, MCvScalar color)
        {
            CvInvoke.PutText(display, text, pos, FontFace.HersheySimplex, 0.55, color, 1,
                             LineType.EightConnected, false);
        }

        // ═══════════════════════════════════════════════════════════
        //  灰階剖面圖
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// 把掃描線的灰階值畫成折線圖，疊上閾值線與找到的邊緣位置。
        ///
        /// 這張圖是第 8 章最值得看的一張：邊緣不是「一條線」，
        /// 而是灰階從低爬到高的一段斜坡；閾值切在斜坡上的哪個高度，就決定了邊緣座標。
        /// 學員看過這張圖，才會理解為什麼閾值要定在明暗中點、為什麼內插能取到亞像素。
        /// </summary>
        private void ShowProfile(double[] profile, int offsetX, double threshold,
                                 EdgePairResult edges, string caption)
        {
            using (Mat chart = RenderProfile(profile, offsetX, threshold, edges, caption))
            {
                SetPictureBoxImage(picResult, ToDisplayBitmap(chart));
            }
            lblResultCap.Text = "灰階剖面圖（Profile）— " + caption;
        }

        /// <summary>繪製剖面圖。回傳的 Mat 由呼叫端負責 Dispose。</summary>
        private Mat RenderProfile(double[] profile, int offsetX, double threshold,
                                  EdgePairResult edges, string caption)
        {
            const int MarginL = 56;
            const int MarginR = 16;
            const int MarginT = 30;
            const int MarginB = 34;

            Mat canvas = new Mat(ProfileHeight, ProfileWidth, DepthType.Cv8U, 3);
            try
            {
                canvas.SetTo(ColorCanvas, null);

                int plotW = ProfileWidth - MarginL - MarginR;
                int plotH = ProfileHeight - MarginT - MarginB;
                Rectangle plot = new Rectangle(MarginL, MarginT, plotW, plotH);

                // 格線與 Y 軸刻度（灰階 0 ~ 255）
                for (int g = 0; g <= 255; g += 51)
                {
                    int y = MarginT + (int)Math.Round((255 - g) * (plotH - 1.0) / 255.0);
                    CvInvoke.Line(canvas, new Point(MarginL, y), new Point(MarginL + plotW - 1, y),
                                  ColorGrid, 1, LineType.EightConnected, 0);
                    CvInvoke.PutText(canvas, g.ToString(), new Point(8, y + 5),
                                     FontFace.HersheySimplex, 0.4, ColorGrid, 1, LineType.EightConnected, false);
                }
                CvInvoke.Rectangle(canvas, plot, ColorGrid, 1, LineType.EightConnected, 0);

                // 閾值線
                int thY = MarginT + (int)Math.Round((255 - threshold) * (plotH - 1.0) / 255.0);
                CvInvoke.Line(canvas, new Point(MarginL, thY), new Point(MarginL + plotW - 1, thY),
                              ColorScan, 1, LineType.EightConnected, 0);
                CvInvoke.PutText(canvas, "threshold " + threshold.ToString("F1"),
                                 new Point(MarginL + 6, thY - 6),
                                 FontFace.HersheySimplex, 0.42, ColorScan, 1, LineType.EightConnected, false);

                // 灰階曲線
                if (profile.Length >= 2)
                {
                    Point prev = ProfilePoint(profile, 0, plot);
                    for (int i = 1; i < profile.Length; i++)
                    {
                        Point cur = ProfilePoint(profile, i, plot);
                        CvInvoke.Line(canvas, prev, cur, ColorCurve, 1, LineType.EightConnected, 0);
                        prev = cur;
                    }
                }

                // 邊緣位置（垂直線 + 座標標註）
                if (edges != null && edges.Found)
                {
                    MCvScalar color = _lastVerdictOk ? ColorOk : ColorOk;   // 剖面圖只表示「找到了」，不表示判定
                    DrawProfileEdge(canvas, plot, profile.Length, edges.FirstEdge - offsetX, edges.FirstEdge, color);
                    DrawProfileEdge(canvas, plot, profile.Length, edges.SecondEdge - offsetX, edges.SecondEdge, color);
                }

                CvInvoke.PutText(canvas, caption + "   (" + profile.Length + " px)", new Point(MarginL, 20),
                                 FontFace.HersheySimplex, 0.5, ColorCurve, 1, LineType.EightConnected, false);
                CvInvoke.PutText(canvas, "X (px)", new Point(ProfileWidth / 2 - 24, ProfileHeight - 10),
                                 FontFace.HersheySimplex, 0.42, ColorGrid, 1, LineType.EightConnected, false);
                return canvas;
            }
            catch
            {
                canvas.Dispose();
                throw;
            }
        }

        private static Point ProfilePoint(double[] profile, int index, Rectangle plot)
        {
            int x = plot.X + (int)Math.Round(index * (plot.Width - 1.0) / (profile.Length - 1));
            int y = plot.Y + (int)Math.Round((255.0 - profile[index]) * (plot.Height - 1.0) / 255.0);
            return new Point(x, y);
        }

        private static void DrawProfileEdge(Mat canvas, Rectangle plot, int pointCount,
                                            double indexPos, double absolutePos, MCvScalar color)
        {
            if (pointCount < 2) return;

            int x = plot.X + (int)Math.Round(indexPos * (plot.Width - 1.0) / (pointCount - 1));
            CvInvoke.Line(canvas, new Point(x, plot.Y), new Point(x, plot.Y + plot.Height - 1),
                          color, 1, LineType.EightConnected, 0);
            CvInvoke.PutText(canvas, absolutePos.ToString("F2"), new Point(x - 26, plot.Y + plot.Height + 20),
                             FontFace.HersheySimplex, 0.42, color, 1, LineType.EightConnected, false);
        }

        /// <summary>把 UI 的閾值設定換算成「這次實際用到的閾值」，供報告顯示。</summary>
        private static double ResolveThreshold(EdgePairResult result, double uiThreshold)
        {
            if (uiThreshold > 0.0) return uiThreshold;
            if (result == null) return 128.0;
            return (result.ProfileMin + result.ProfileMax) / 2.0;
        }

        // ═══════════════════════════════════════════════════════════
        //  顯示工具
        // ═══════════════════════════════════════════════════════════

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
        /// PictureBox 本身不會自動釋放 Image，不自己放就是每按一次漏一張全尺寸點陣圖，
        /// 連續操作下會看到記憶體與 GDI handle 單調上升（P-011 假性 OOM 的來源）。
        /// </summary>
        private static void SetPictureBoxImage(PictureBox pic, Image newImage)
        {
            Image old = pic.Image;
            pic.Image = newImage;
            if (old != null) old.Dispose();
        }

        private void UpdateScaleLabel()
        {
            if (_calibration == null)
            {
                lblScaleValue.Text = "尚未標定 — 量出來的只有 px，沒有 mm";
                lblScaleValue.ForeColor = ScaleMissingFore;
                return;
            }

            lblScaleValue.Text = "X = " + _calibration.ScaleX.ToString("F6")
                               + "、Y = " + _calibration.ScaleY.ToString("F6") + " mm/px"
                               + "（X／Y 差 " + _calibration.AnisotropyPercent.ToString("F2") + " %）";
            lblScaleValue.ForeColor = ScaleReadyFore;
        }

        private void SetVerdict(bool isOk, string text)
        {
            lblVerdict.Text = text;
            lblVerdict.BackColor = isOk ? VerdictOkBack : VerdictNgBack;
            lblVerdict.ForeColor = isOk ? VerdictOkFore : VerdictNgFore;
        }

        private void SetVerdictIdle()
        {
            lblVerdict.Text = "待量測";
            lblVerdict.BackColor = SystemColors.Control;
            lblVerdict.ForeColor = Color.Gray;
        }

        private void ShowWelcome()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("第 8 章 尺寸量測 — 功能測試程式");
            sb.AppendLine(Divider);
            sb.AppendLine();
            sb.AppendLine("  ① 標定      拍已知尺寸的標定板，算出 mm/px。X、Y 要分開標。");
            sb.AppendLine("  ② 線段量測  掃描一條線的灰階，找兩個邊緣，相減得寬度（亞像素）。");
            sb.AppendLine("  ③ 點位量測  取特徵點座標，算兩點距離與夾角。dx、dy 要先各自換 mm。");
            sb.AppendLine("  ④ 統計量測  掃多條線取平均與標準差。標準差才是「量測穩不穩」的答案。");
            sb.AppendLine();
            sb.AppendLine(Divider);
            sb.AppendLine();
            sb.AppendLine("第 7 章的特徵檢測回答「有沒有問題」，第 8 章回答「差了多少」。");
            sb.AppendLine("差多少要有單位，單位來自標定——所以第一件事永遠是標定。");
            sb.AppendLine();
            sb.AppendLine("請先按左上角「產生標定圖」（不需準備影像檔），");
            sb.AppendLine("或直接按右下角 ⑤「一鍵執行完整流程」，把整條路跑一次給你看。");
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

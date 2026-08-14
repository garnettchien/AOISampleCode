using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace TemplateMatching
{
    /// <summary>
    /// 第 9 章 模板匹配與定位 — 功能測試程式。
    ///
    /// 【資源釋放】本檔案嚴格遵守第 18／19 章：
    ///   · Mat 欄位覆蓋一律「暫存 → 換新 → 放舊」。
    ///   · PictureBox.Image 換圖時，舊的那張是我們自己 new 的，必須自己 Dispose。
    ///   · 交給 PictureBox 的點陣圖一律用 ToDisplayBitmap() 複製一份獨立資料
    ///     （Emgu 的 ToBitmap() 對 3 通道 BGR 是共用 Mat 的 buffer，見該方法註解）。
    ///   · 所有中間 Mat 一律 using。
    /// </summary>
    public partial class Form1 : Form
    {
        // ── 顏色（OpenCV 用 BGR 順序）──────────────────────────────
        private static readonly MCvScalar ColorTemplate = new MCvScalar(0, 208, 255);  // 黃：模板框
        private static readonly MCvScalar ColorSearch = new MCvScalar(0, 224, 0);      // 綠：搜尋區
        private static readonly MCvScalar ColorReference = new MCvScalar(160, 160, 160); // 灰：基準位置
        private static readonly MCvScalar ColorFound = new MCvScalar(32, 32, 255);     // 紅：定位結果
        private static readonly MCvScalar ColorWrong = new MCvScalar(255, 96, 96);     // 藍：錯誤法結果
        private static readonly MCvScalar ColorCross = new MCvScalar(0, 224, 255);     // 黃：匹配中心十字

        private static readonly Color VerdictOkBack = Color.FromArgb(223, 246, 221);
        private static readonly Color VerdictOkFore = Color.FromArgb(27, 94, 32);
        private static readonly Color VerdictNgBack = Color.FromArgb(253, 231, 233);
        private static readonly Color VerdictNgFore = Color.FromArgb(164, 38, 44);
        private static readonly Color InfoOkFore = Color.FromArgb(27, 122, 27);
        private static readonly Color InfoBadFore = Color.FromArgb(196, 43, 28);

        private const string Divider =
            "--------------------------------------------------------------------------------";

        // ── 欄位（生命週期與 Form 相同）─────────────────────────────

        /// <summary>基準影像：偏移 (0,0) 的那張，教導模板永遠用它。</summary>
        private Mat _refMat;

        /// <summary>測試影像：套用目前偏移設定的那張，定位跑在它上面。</summary>
        private Mat _testMat;

        /// <summary>教導出來的模板。獨立擁有像素資料（CropTemplate 內有 Clone）。</summary>
        private Mat _template;

        /// <summary>使用者載入的原始影像；null 表示目前用合成測試圖。</summary>
        private Mat _loadedScene;

        /// <summary>教導時記下的模板左上角座標，算偏移量的基準。</summary>
        private Point _referencePos;

        /// <summary>InitializeComponent 期間 ValueChanged 會亂噴，用這個旗標擋掉。</summary>
        private bool _uiReady;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _uiReady = true;
            ShowWelcome();
            UpdateSearchInfo();
        }

        /// <summary>
        /// 收尾：釋放本 Form 自己配置的所有非受管資源。
        /// PictureBox 不會自動 Dispose 它的 Image，一定要自己放。
        /// </summary>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetPictureBoxImage(picRef, null);
            SetPictureBoxImage(picTest, null);
            SetPictureBoxImage(picTemplate, null);

            if (_refMat != null) { _refMat.Dispose(); _refMat = null; }
            if (_testMat != null) { _testMat.Dispose(); _testMat = null; }
            if (_template != null) { _template.Dispose(); _template = null; }
            if (_loadedScene != null) { _loadedScene.Dispose(); _loadedScene = null; }
        }

        // ═══════════════════════════════════════════════════════════
        //  來源影像
        // ═══════════════════════════════════════════════════════════

        private void btnGenTestImage_Click(object sender, EventArgs e)
        {
            try
            {
                SetLoadedScene(null);                       // 切回合成模式
                SetTemplate(null);                          // 換了來源，舊模板作廢
                SetPictureBoxImage(picTemplate, null);
                SetTemplateRect(TestImageGenerator.DefaultTemplateRect);
                numMarginX.Value = TestImageGenerator.DefaultMargin;
                numMarginY.Value = TestImageGenerator.DefaultMargin;

                RegenerateImages();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[產生測試影像]");
                sb.AppendLine(TestImageGenerator.Description);
                sb.AppendLine();
                sb.AppendLine(string.Format("  本次產品偏移 : dX = {0:+#;-#;0}, dY = {1:+#;-#;0}   （角度 {2:F1}°）",
                              (int)numOffsetX.Value, (int)numOffsetY.Value, numAngle.Value));
                sb.AppendLine();
                sb.AppendLine("  基準影像 = 偏移 (0, 0) 的那張，永遠用它教導模板；");
                sb.AppendLine("  測試影像 = 套用上面偏移量的那張，用來驗證定位有沒有跟著動。");
                sb.AppendLine();
                sb.AppendLine("請依序執行 ① 教導模板 → ② 確認搜尋區 → ③ 執行定位。");
                txtLog.Text = sb.ToString();

                SetVerdictIdle();
            }
            catch (Exception ex)
            {
                ReportError("產生測試影像", ex);
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

                    SetLoadedScene(loaded);
                    SetTemplate(null);
                    SetPictureBoxImage(picTemplate, null);

                    // 模板框預設放在影像中央，尺寸取短邊的 1/8
                    int side = Math.Max(16, Math.Min(loaded.Width, loaded.Height) / 8);
                    SetTemplateRect(new Rectangle((loaded.Width - side) / 2,
                                                  (loaded.Height - side) / 2, side, side));
                    numMarginX.Value = side;
                    numMarginY.Value = side;

                    RegenerateImages();

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("[載入影像] " + dlg.FileName);
                    sb.AppendLine("  尺寸: " + loaded.Width + " × " + loaded.Height + ", 8-bit 灰階");
                    sb.AppendLine();
                    sb.AppendLine("  上方的「產品偏移」會直接平移／旋轉這張影像來模擬產品位移，");
                    sb.AppendLine("  所以 ⑤ 偏移掃描驗收一樣可以用。");
                    sb.AppendLine();
                    sb.AppendLine("  模板框已預設在影像中央，請自行調整到有明確紋理的位置再教導。");
                    txtLog.Text = sb.ToString();

                    SetVerdictIdle();
                }
            }
            catch (Exception ex)
            {
                ReportError("載入影像", ex);
            }
        }

        /// <summary>依目前的偏移設定產生一張影像。合成模式與載入模式共用這個入口。</summary>
        private Mat MakeImage(int dx, int dy, double angleDeg)
        {
            if (_loadedScene != null)
                return TestImageGenerator.Transform(_loadedScene, dx, dy, angleDeg, 0);

            return TestImageGenerator.Create(dx, dy, angleDeg);
        }

        /// <summary>重新產生基準影像與測試影像。</summary>
        private void RegenerateImages()
        {
            SetRefMat(MakeImage(0, 0, 0));
            SetTestMat(MakeImage((int)numOffsetX.Value, (int)numOffsetY.Value, (double)numAngle.Value));
            UpdateSearchInfo();
            RefreshViews(null, null);
        }

        // ── 欄位覆蓋：一律「暫存 → 換新 → 放舊」（第 18 章 P-009）──

        private void SetRefMat(Mat m) { Mat old = _refMat; _refMat = m; if (old != null) old.Dispose(); }
        private void SetTestMat(Mat m) { Mat old = _testMat; _testMat = m; if (old != null) old.Dispose(); }
        private void SetTemplate(Mat m) { Mat old = _template; _template = m; if (old != null) old.Dispose(); }
        private void SetLoadedScene(Mat m) { Mat old = _loadedScene; _loadedScene = m; if (old != null) old.Dispose(); }

        private bool EnsureSource()
        {
            if (_refMat != null && !_refMat.IsEmpty) return true;
            MessageBox.Show(this, "請先按「產生測試影像」或「載入影像」。", "尚未載入影像",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private bool EnsureTemplate()
        {
            if (!EnsureSource()) return false;
            if (_template != null && !_template.IsEmpty) return true;
            MessageBox.Show(this, "請先按 ①「教導模板」。", "尚未教導模板",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        // ═══════════════════════════════════════════════════════════
        //  參數變更
        // ═══════════════════════════════════════════════════════════

        /// <summary>模板框或 margin 改變 → 重算搜尋區資訊並重畫（不必重跑定位）。</summary>
        private void OnRectParamChanged(object sender, EventArgs e)
        {
            if (!_uiReady) return;
            UpdateSearchInfo();
            RefreshViews(null, null);
        }

        /// <summary>偏移量或角度改變 → 重新產生測試影像。</summary>
        private void OnTestImageParamChanged(object sender, EventArgs e)
        {
            if (!_uiReady || _refMat == null) return;
            try
            {
                SetTestMat(MakeImage((int)numOffsetX.Value, (int)numOffsetY.Value, (double)numAngle.Value));
                RefreshViews(null, null);
                SetVerdictIdle();
            }
            catch (Exception ex)
            {
                ReportError("更新測試影像", ex);
            }
        }

        private void btnResetOffset_Click(object sender, EventArgs e)
        {
            numOffsetX.Value = 0;
            numOffsetY.Value = 0;
            numAngle.Value = 0;
        }

        private void chkMultiAngle_CheckedChanged(object sender, EventArgs e)
        {
            bool on = chkMultiAngle.Checked;
            lblAngleRange.Enabled = on;
            numAngleRange.Enabled = on;
            lblAngleStep.Enabled = on;
            numAngleStep.Enabled = on;
        }

        private Rectangle GetTemplateRect()
        {
            return new Rectangle((int)numTmplX.Value, (int)numTmplY.Value,
                                 (int)numTmplW.Value, (int)numTmplH.Value);
        }

        private void SetTemplateRect(Rectangle r)
        {
            numTmplX.Value = r.X; numTmplY.Value = r.Y;
            numTmplW.Value = r.Width; numTmplH.Value = r.Height;
        }

        private Rectangle GetSearchRect()
        {
            int w = (_refMat != null) ? _refMat.Width : TestImageGenerator.Width;
            int h = (_refMat != null) ? _refMat.Height : TestImageGenerator.Height;
            return TemplateOps.BuildSearchRect(GetTemplateRect(),
                                               (int)numMarginX.Value, (int)numMarginY.Value, w, h);
        }

        /// <summary>
        /// 即時顯示結果圖尺寸與可容忍的偏移範圍。
        ///
        /// 教材 §3：這個檢查在執行 MatchTemplate 之前就能做，是擋下 P-006 的第一道關卡。
        /// 把它做成「跟著參數即時更新」，使用者調 margin 時馬上看得到後果。
        /// </summary>
        private void UpdateSearchInfo()
        {
            Rectangle tr = GetTemplateRect();
            Rectangle sr = GetSearchRect();
            Size rm = TemplateOps.CalcResultMapSize(sr, tr);
            bool ok = (rm.Width > 1 && rm.Height > 1);

            // 容忍偏移量 = 模板框到搜尋區各邊的實際距離（靠近影像邊界時會不對稱，取最小值才安全）
            int left = tr.X - sr.X, right = sr.Right - tr.Right;
            int top = tr.Y - sr.Y, bottom = sr.Bottom - tr.Bottom;
            int tolX = Math.Min(left, right), tolY = Math.Min(top, bottom);

            lblResultMapInfo.ForeColor = ok ? InfoOkFore : InfoBadFore;
            lblResultMapInfo.Text = ok
                ? string.Format("結果圖 {0} × {1}　可容忍偏移 ±{2}", rm.Width, rm.Height, Math.Min(tolX, tolY))
                : string.Format("結果圖 {0} × {1}　定位空間不足，位置會鎖死",
                                Math.Max(0, rm.Width), Math.Max(0, rm.Height));

            lblSearchRectInfo.Text = string.Format("搜尋區 ({0}, {1}, {2}, {3})", sr.X, sr.Y, sr.Width, sr.Height);
        }

        // ═══════════════════════════════════════════════════════════
        //  ① 教導模板
        // ═══════════════════════════════════════════════════════════

        private void btnTeach_Click(object sender, EventArgs e)
        {
            if (!EnsureSource()) return;

            try
            {
                Rectangle tr = TemplateOps.ClampRect(GetTemplateRect(), _refMat.Width, _refMat.Height);
                SetTemplateRect(tr);

                SetTemplate(TemplateOps.CropTemplate(_refMat, tr));
                _referencePos = new Point(tr.X, tr.Y);
                SetPictureBoxImage(picTemplate, ToDisplayBitmap(_template));

                UpdateSearchInfo();
                RefreshViews(null, null);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[① 教導模板]");
                sb.AppendLine();
                sb.AppendLine(string.Format("  var templateRect = new Rectangle({0}, {1}, {2}, {3});",
                              tr.X, tr.Y, tr.Width, tr.Height));
                sb.AppendLine("  _template = new Mat(referenceImage, templateRect).Clone();   // 要獨立資料，故 Clone");
                sb.AppendLine();
                sb.AppendLine(string.Format("  模板尺寸 : {0} × {1}", tr.Width, tr.Height));
                sb.AppendLine(string.Format("  基準位置 : ({0}, {1})      ← 記下來，之後算偏移量要用", tr.X, tr.Y));
                sb.AppendLine();
                sb.AppendLine("  教導（Teaching）的完整流程：");
                sb.AppendLine("    拍一張良品影像當基準 → 在影像上框選目標區域當模板");
                sb.AppendLine("    → 存成檔案（PNG／BMP）→ 同時記錄框的座標作為「基準位置」");
                sb.AppendLine();
                sb.AppendLine("  ⚠ 模板框要選「高對比、有獨特紋理」的區域。");
                if (_loadedScene == null)
                {
                    Rectangle flat = TestImageGenerator.FlatAreaRect;
                    sb.AppendLine(string.Format("    試著把模板框拉到平坦區 ({0}, {1}, {2}, {3})，你會看到分數到處都很高、",
                                  flat.X, flat.Y, flat.Width, flat.Height));
                    sb.AppendLine("    定位結果亂跳——因為那裡到處都長得一樣，沒有可辨識的特徵。");
                }
                sb.AppendLine();
                sb.AppendLine("  註：這裡用 Clone() 是刻意的。new Mat(src, rect) 只共享 header，");
                sb.AppendLine("      來源影像一換掉模板就失效。模板要活得比來源久，所以必須複製一份。");
                sb.AppendLine("      這與第 7 章 ROI 的「刻意共享」正好相反，差別在於誰要活得比較久。");
                txtLog.Text = sb.ToString();

                SetVerdictIdle();
            }
            catch (Exception ex)
            {
                ReportError("教導模板", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  ③ 執行定位
        // ═══════════════════════════════════════════════════════════

        private void btnLocate_Click(object sender, EventArgs e)
        {
            if (!EnsureTemplate()) return;

            try
            {
                Rectangle tr = GetTemplateRect();
                Rectangle sr = GetSearchRect();
                double threshold = (double)numThreshold.Value;

                Stopwatch sw = Stopwatch.StartNew();
                MatchResult r = chkMultiAngle.Checked
                    ? TemplateOps.MatchMultiAngle(_testMat, _template, sr, _referencePos, threshold,
                                                  (double)numAngleRange.Value, (double)numAngleStep.Value,
                                                  TestImageGenerator.BackgroundGray)
                    : TemplateOps.Match(_testMat, _template, sr, _referencePos, threshold);
                double ms = sw.Elapsed.TotalMilliseconds;

                RefreshViews(r, null);

                int setDx = (int)numOffsetX.Value, setDy = (int)numOffsetY.Value;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[③ 執行定位]  TemplateMatchingType.CcoeffNormed");
                sb.AppendLine();
                sb.AppendLine("  CvInvoke.MatchTemplate(searchROI, template, resultMap, CcoeffNormed);");
                sb.AppendLine("  CvInvoke.MinMaxLoc(resultMap, ref minVal, ref maxVal, ref minLoc, ref maxLoc);");
                sb.AppendLine(Divider);
                sb.AppendLine(string.Format("  模板框            : ({0}, {1}, {2}, {3})", tr.X, tr.Y, tr.Width, tr.Height));
                sb.AppendLine(string.Format("  搜尋區            : ({0}, {1}, {2}, {3})", sr.X, sr.Y, sr.Width, sr.Height));
                sb.AppendLine(string.Format("  結果圖尺寸        : {0} × {1}", r.ResultMapSize.Width, r.ResultMapSize.Height));
                sb.AppendLine(string.Format("  最高分 maxVal     : {0:F4}        （門檻 {1:F2}）", r.Score, threshold));
                sb.AppendLine(string.Format("  maxLoc（搜尋區內）: ({0}, {1})", r.MaxLoc.X, r.MaxLoc.Y));
                if (chkMultiAngle.Checked)
                {
                    sb.AppendLine(string.Format("  最佳角度          : {0:F1}°   （試了 {1} 個角度）", r.AngleDeg, r.AnglesTried));
                }
                sb.AppendLine(string.Format("  耗時              : {0:F2} ms", ms));
                sb.AppendLine();
                sb.AppendLine("  換算回原圖座標：");
                sb.AppendLine(string.Format("    matchPos.X = searchRect.X + maxLoc.X = {0} + {1} = {2}", sr.X, r.MaxLoc.X, r.Location.X));
                sb.AppendLine(string.Format("    matchPos.Y = searchRect.Y + maxLoc.Y = {0} + {1} = {2}", sr.Y, r.MaxLoc.Y, r.Location.Y));
                sb.AppendLine();
                sb.AppendLine(string.Format("  基準位置          : ({0}, {1})", _referencePos.X, _referencePos.Y));
                sb.AppendLine(string.Format("  偵測位置          : ({0}, {1})", r.Location.X, r.Location.Y));
                sb.AppendLine(string.Format("  偏移量            : ({0:+#;-#;0}, {1:+#;-#;0})", r.Offset.X, r.Offset.Y));
                sb.AppendLine(string.Format("  設定的產品偏移    : ({0:+#;-#;0}, {1:+#;-#;0})   {2}",
                              setDx, setDy,
                              (r.Found && r.Offset.X == setDx && r.Offset.Y == setDy) ? "← 完全一致 ✔" : ""));
                sb.AppendLine(Divider);
                sb.AppendLine(r.Found
                    ? string.Format("  {0:F4} ≥ {1:F2}  →  定位成功", r.Score, threshold)
                    : string.Format("  {0:F4} < {1:F2}  →  定位失敗（NotFound）", r.Score, threshold));
                sb.AppendLine();
                if (!r.Found)
                {
                    sb.AppendLine(string.Format("  ⚠ 注意 MinMaxLoc 還是給了一個座標 ({0}, {1})。它永遠會給。", r.Location.X, r.Location.Y));
                    sb.AppendLine("    沒有分數門檻的話，程式會拿這個假座標去修正所有後續 ROI，");
                    sb.AppendLine("    每一項檢測都會在錯誤的位置上進行——而且完全不會有任何錯誤訊息。");
                    sb.AppendLine();
                    sb.AppendLine("  【正確的處置】");
                    sb.AppendLine("    1. 加大 margin，讓搜尋區涵蓋產品實際可能的偏移範圍");
                    sb.AppendLine("    2. 而不是調低門檻硬吞——那只是把「找不到」偽裝成「找到了」");
                    sb.AppendLine("    3. 定位失敗時應回報 AlignFail 並中止本次檢測，不要繼續往下跑");
                }
                else
                {
                    sb.AppendLine("  ⚠ MinMaxLoc 永遠會回一個「最高分」，即使影像中根本沒有目標物，");
                    sb.AppendLine("    所以一定要設分數門檻，否則會拿到毫無意義的座標還以為定位成功。");
                    sb.AppendLine();
                    sb.AppendLine("  【定位結果的用途】把偏移量套到所有後續 ROI（教材 §5 Step 3）：");
                    sb.AppendLine(string.Format("      foreach (var roi in allRois) roi.Offset({0}, {1});", r.Offset.X, r.Offset.Y));
                    sb.AppendLine("  這正是第 7 章 P-001「固定座標 ROI 會漂移」的解法。");
                }
                txtLog.Text = sb.ToString();

                SetVerdict(r.Found,
                    r.Found ? string.Format("定位成功　{0:F3}", r.Score)
                            : string.Format("定位失敗　{0:F3}", r.Score),
                    r.Found ? string.Format("匹配座標　({0}, {1})\r\n偏移量　　({2:+#;-#;0}, {3:+#;-#;0})", r.Location.X, r.Location.Y, r.Offset.X, r.Offset.Y)
                            : string.Format("匹配座標　({0}, {1})  ← 不可信\r\n偏移量　　—", r.Location.X, r.Location.Y));
            }
            catch (Exception ex)
            {
                ReportError("執行定位", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  ④ P-006 對照實驗
        // ═══════════════════════════════════════════════════════════

        private void btnCompare_Click(object sender, EventArgs e)
        {
            if (!EnsureTemplate()) return;

            try
            {
                Rectangle tr = GetTemplateRect();
                double threshold = (double)numThreshold.Value;

                // ❌ 錯誤：搜尋區直接用模板那一塊（教材 P-006 的原始寫法）
                MatchResult wrong = TemplateOps.Match(_testMat, _template, tr, _referencePos, threshold);

                // ✔ 正確：搜尋區由模板向四周外擴 margin
                Rectangle sr = GetSearchRect();
                MatchResult right = TemplateOps.Match(_testMat, _template, sr, _referencePos, threshold);

                RefreshViews(right, wrong);

                int setDx = (int)numOffsetX.Value, setDy = (int)numOffsetY.Value;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("[④ P-006 對照實驗]  產品偏移 dX = {0:+#;-#;0}, dY = {1:+#;-#;0}", setDx, setDy));
                sb.AppendLine();
                sb.AppendLine("  同一張測試影像、同一個模板，只差在「搜尋區怎麼取」。");
                bool wrongOffsetOk = (wrong.Offset.X == setDx && wrong.Offset.Y == setDy);
                bool rightOffsetOk = (right.Offset.X == setDx && right.Offset.Y == setDy);

                sb.AppendLine(Divider);
                sb.AppendLine(CompareRow("", "錯誤法", "正確法", null));
                sb.AppendLine(CompareRow("", "搜尋區 = 模板", "搜尋區 = 模板 + margin", null));
                sb.AppendLine(Divider);
                sb.AppendLine(CompareRow("搜尋區", Fmt(wrong.SearchRect), Fmt(right.SearchRect), null));
                sb.AppendLine(CompareRow("結果圖尺寸",
                              wrong.ResultMapSize.Width + "x" + wrong.ResultMapSize.Height,
                              right.ResultMapSize.Width + "x" + right.ResultMapSize.Height, null));
                sb.AppendLine(CompareRow("maxLoc",
                              "(" + wrong.MaxLoc.X + ", " + wrong.MaxLoc.Y + ")",
                              "(" + right.MaxLoc.X + ", " + right.MaxLoc.Y + ")", null));
                sb.AppendLine(CompareRow("換算後座標",
                              "(" + wrong.Location.X + ", " + wrong.Location.Y + ")",
                              "(" + right.Location.X + ", " + right.Location.Y + ")", null));
                sb.AppendLine(CompareRow("偏移量",
                              string.Format("({0:+#;-#;0}, {1:+#;-#;0})", wrong.Offset.X, wrong.Offset.Y),
                              string.Format("({0:+#;-#;0}, {1:+#;-#;0})", right.Offset.X, right.Offset.Y),
                              // ✔／✘ 在等寬字型下不是整數欄寬，只能放行尾
                              (wrongOffsetOk ? "✔" : "✘") + " ／ " + (rightOffsetOk ? "✔" : "✘")));
                sb.AppendLine(CompareRow("最高分 maxVal",
                              wrong.Score.ToString("F4"), right.Score.ToString("F4"), null));
                sb.AppendLine(CompareRow("判定",
                              wrong.Found ? "PASS" : "FAIL (低於門檻)",
                              right.Found ? "PASS" : "FAIL (低於門檻)", null));
                sb.AppendLine(Divider);
                sb.AppendLine();
                sb.AppendLine("  錯誤法的 maxLoc 不是「算出來是 (0,0)」，而是「只有 (0,0) 可以回」——");
                sb.AppendLine("  結果圖只有 1 個像素，MinMaxLoc 別無選擇。");
                sb.AppendLine();
                if (setDx == 0 && setDy == 0)
                {
                    sb.AppendLine("  ★ 目前偏移是 (0, 0)，兩種方法的結果完全一樣、分數都是滿分。");
                    sb.AppendLine("    這就是 happy-path 會全綠的原因——把偏移調成非 0 再按一次，差異就出來了。");
                }
                else
                {
                    sb.AppendLine("  這次因為產品移開了，錯誤法的分數掉下來才被門檻擋住。但如果門檻設鬆一點、");
                    sb.AppendLine("  或產品只偏移 2~3 px 讓分數還有 0.8，錯誤法就會回報「定位成功、偏移 (0,0)」——");
                    sb.AppendLine("  程式不當、不報錯、測試全綠，而位置從頭到尾沒動過。");
                    sb.AppendLine("  這才是 P-006 真正可怕的地方。");
                }
                sb.AppendLine();
                sb.AppendLine("  來源：TSMC F8 水霧（2026-06，噴頭定位）");
                txtLog.Text = sb.ToString();

                SetVerdict(right.Found,
                    right.Found ? string.Format("正確法成功　{0:F3}", right.Score) : "兩種方法都失敗",
                    string.Format("正確法　({0}, {1})  偏移 ({2:+#;-#;0}, {3:+#;-#;0})\r\n錯誤法　({4}, {5})  偏移 ({6:+#;-#;0}, {7:+#;-#;0})",
                                  right.Location.X, right.Location.Y, right.Offset.X, right.Offset.Y,
                                  wrong.Location.X, wrong.Location.Y, wrong.Offset.X, wrong.Offset.Y));
            }
            catch (Exception ex)
            {
                ReportError("P-006 對照實驗", ex);
            }
        }

        private static string Fmt(Rectangle r)
        {
            return "(" + r.X + "," + r.Y + "," + r.Width + "," + r.Height + ")";
        }

        /// <summary>
        /// 依「顯示寬度」補空白，用來排等寬字型下的表格。
        ///
        /// string.Format 的 {0,20} 是按「字元數」補空白，但等寬字型下一個漢字佔 2 欄，
        /// 中文標籤字數不同時欄位就會歪掉。表格是這支程式的教學重點，必須自己算寬度。
        ///
        /// （實測 Consolas 9.5pt：漢字 = 2 欄；→ ± ≥ 都是 1 欄；★ 是 2 欄；
        ///   但 ✔ ✘ 約 1.5 欄不是整數，所以那兩個符號一律只放行尾，不放在對齊欄位裡。）
        /// </summary>
        private static string PadDisplay(string s, int width, bool alignRight)
        {
            int pad = Math.Max(0, width - DisplayWidth(s));
            return alignRight ? new string(' ', pad) + s : s + new string(' ', pad);
        }

        private static int DisplayWidth(string s)
        {
            int w = 0;
            for (int i = 0; i < s.Length; i++) w += IsWideChar(s[i]) ? 2 : 1;
            return w;
        }

        private static bool IsWideChar(char c)
        {
            return (c >= 0x1100 && c <= 0x115F)      // 韓文字母
                || (c >= 0x2E80 && c <= 0xA4CF)      // CJK 部首 ～ 彝族（含漢字、注音、假名）
                || (c >= 0xAC00 && c <= 0xD7A3)      // 韓文音節
                || (c >= 0xF900 && c <= 0xFAFF)      // CJK 相容漢字
                || (c >= 0xFE30 && c <= 0xFE6F)      // CJK 相容形式
                || (c >= 0xFF00 && c <= 0xFF60)      // 全形 ASCII
                || (c >= 0xFFE0 && c <= 0xFFE6)      // 全形符號
                || c == 0x2605 || c == 0x2606;       // ★☆
        }

        /// <summary>對照表的一列：中文標籤 + 兩個 ASCII 數值欄。</summary>
        private static string CompareRow(string label, string wrongVal, string rightVal, string note)
        {
            return "  " + PadDisplay(label, 16, false)
                        + PadDisplay(wrongVal, 22, true)
                        + PadDisplay(rightVal, 26, true)
                        + (string.IsNullOrEmpty(note) ? "" : "   " + note);
        }

        // ═══════════════════════════════════════════════════════════
        //  ⑤ 偏移掃描驗收
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// 教材黃金法則的自動化版本。
        ///
        /// 「編譯過 + happy-path 過」不代表行為正確。驗收定位功能的正確方法，
        /// 是拿「產品真的有偏移」的影像跑一遍，確認回傳座標真的跟著移動。
        /// 這裡把偏移量從 −range 掃到 +range，兩種取搜尋區的方法各跑一次並列出來。
        /// </summary>
        private void btnSweep_Click(object sender, EventArgs e)
        {
            if (!EnsureTemplate()) return;

            try
            {
                int range = (int)numSweepRange.Value;
                int step = (int)numSweepStep.Value;
                double threshold = (double)numThreshold.Value;
                double angle = (double)numAngle.Value;
                Rectangle tr = GetTemplateRect();
                Rectangle sr = GetSearchRect();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("[⑤ 偏移掃描驗收]  掃描範圍 ±{0}，步距 {1}", range, step));
                sb.AppendLine();
                sb.AppendLine("  教材黃金法則：「編譯過 + happy-path 過」≠「行為符合用途」。");
                sb.AppendLine("  正確的驗收方式是拿有偏移的影像跑一遍，確認座標真的跟著移動。");
                sb.AppendLine();
                sb.AppendLine(string.Format("  模板框 {0}　搜尋區 {1}　容忍偏移 X ±{2}　Y ±{3}",
                              Fmt(tr), Fmt(sr), tr.X - sr.X, tr.Y - sr.Y));
                sb.AppendLine(Divider);
                // 表頭要與下方資料列的欄位右緣對齊（col 40 / col 71），所以同樣走顯示寬度計算
                sb.AppendLine("  " + PadDisplay("實際偏移", 15, false)
                                   + PadDisplay("正確法（外擴 margin）", 23, true)
                                   + PadDisplay("錯誤法（搜尋區 = 模板）", 31, true));
                sb.AppendLine("  " + PadDisplay("dX", 3, true) + "  " + PadDisplay("dY", 4, true)
                                   + "      " + PadDisplay("偵測dX", 6, true) + "  " + PadDisplay("偵測dY", 6, true)
                                   + "   " + PadDisplay("分數", 6, true)
                                   + "        " + PadDisplay("偵測dX", 6, true) + "  " + PadDisplay("偵測dY", 6, true)
                                   + "   " + PadDisplay("分數", 6, true));
                sb.AppendLine(Divider);

                int rightPass = 0, wrongPass = 0, total = 0;
                Cursor old = this.Cursor;
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    for (int dx = -range; dx <= range; dx += step)
                    {
                        int dy = dx / 2;      // 讓兩軸都動，才驗得到 Y 方向
                        total++;

                        using (Mat img = MakeImage(dx, dy, angle))
                        {
                            MatchResult right = TemplateOps.Match(img, _template, sr, _referencePos, threshold);
                            MatchResult wrong = TemplateOps.Match(img, _template, tr, _referencePos, threshold);

                            bool rOk = right.Found && right.Offset.X == dx && right.Offset.Y == dy;
                            bool wOk = wrong.Found && wrong.Offset.X == dx && wrong.Offset.Y == dy;
                            if (rOk) rightPass++;
                            if (wOk) wrongPass++;

                            string note = "";
                            if (Math.Abs(dx) > (tr.X - sr.X) || Math.Abs(dy) > (tr.Y - sr.Y))
                                note = "  ← 超出 margin";
                            else if (dx == 0 && dy == 0)
                                note = "  ← happy-path";

                            sb.AppendLine(string.Format("  {0,3}  {1,4}      {2,6}  {3,6}   {4,6:F3}        {5,6}  {6,6}   {7,6:F3}{8}",
                                dx, dy,
                                right.Found ? right.Offset.X.ToString() : "——",
                                right.Found ? right.Offset.Y.ToString() : "——",
                                right.Score,
                                wrong.Found ? wrong.Offset.X.ToString() : "——",
                                wrong.Found ? wrong.Offset.Y.ToString() : "——",
                                wrong.Score,
                                note));
                        }
                    }
                }
                finally
                {
                    this.Cursor = old;
                }

                sb.AppendLine(Divider);
                sb.AppendLine(string.Format("  正確法 : {0} / {1} 通過（偵測偏移與實際偏移完全一致才算通過）", rightPass, total));
                sb.AppendLine(string.Format("  錯誤法 : {0} / {1} 通過", wrongPass, total));
                sb.AppendLine();
                sb.AppendLine("  ★ 看 dX = 0 那一列：兩種方法都回 (0, 0)、分數都是滿分，完全一樣。");
                sb.AppendLine("    這就是「用影像自身裁切當模板、再搜尋同一張影像」的單元測試會全綠的原因——");
                sb.AppendLine("    那一列剛好是錯誤法唯一會答對的情況，happy-path 把 bug 遮得死死的。");
                sb.AppendLine();
                sb.AppendLine("  ★ 標示「超出 margin」的列是「正確的失敗」：");
                sb.AppendLine("    margin 要涵蓋產品實際可能的偏移範圍，不夠就加大 margin，");
                sb.AppendLine("    而不是調低門檻硬吞。");
                txtLog.Text = sb.ToString();

                // 掃描會改動測試影像，跑完把畫面還原成目前的偏移設定
                SetTestMat(MakeImage((int)numOffsetX.Value, (int)numOffsetY.Value, angle));
                RefreshViews(null, null);

                SetVerdict(rightPass > wrongPass,
                    string.Format("掃描完成　正確法 {0}/{1}", rightPass, total),
                    string.Format("正確法　{0} / {1} 通過\r\n錯誤法　{2} / {3} 通過", rightPass, total, wrongPass, total));
            }
            catch (Exception ex)
            {
                ReportError("偏移掃描驗收", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  顯示
        // ═══════════════════════════════════════════════════════════

        private void RefreshViews(MatchResult right, MatchResult wrong)
        {
            if (_refMat == null || _refMat.IsEmpty) return;

            Rectangle tr = GetTemplateRect();
            Rectangle sr = GetSearchRect();

            // 左：基準影像 + 黃模板框 + 綠搜尋區
            using (Mat view = new Mat())
            {
                CvInvoke.CvtColor(_refMat, view, ColorConversion.Gray2Bgr);
                CvInvoke.Rectangle(view, sr, ColorSearch, 2, LineType.EightConnected, 0);
                CvInvoke.Rectangle(view, tr, ColorTemplate, 2, LineType.EightConnected, 0);
                PutLabel(view, "TEMPLATE", new Point(tr.X, tr.Y - 6), ColorTemplate);
                SetPictureBoxImage(picRef, ToDisplayBitmap(view));
            }

            // 右：測試影像 + 綠搜尋區 + 灰虛線基準位置 + 定位結果
            if (_testMat != null && !_testMat.IsEmpty)
            {
                using (Mat view = new Mat())
                {
                    CvInvoke.CvtColor(_testMat, view, ColorConversion.Gray2Bgr);
                    CvInvoke.Rectangle(view, sr, ColorSearch, 2, LineType.EightConnected, 0);
                    DrawDashedRect(view, new Rectangle(_referencePos.X, _referencePos.Y, tr.Width, tr.Height),
                                   ColorReference, 1, 6);

                    if (wrong != null)
                    {
                        Rectangle wr = new Rectangle(wrong.Location.X, wrong.Location.Y, tr.Width, tr.Height);
                        CvInvoke.Rectangle(view, wr, ColorWrong, 2, LineType.EightConnected, 0);
                        PutLabel(view, "WRONG", new Point(wr.X, wr.Bottom + 16), ColorWrong);
                    }

                    if (right != null)
                    {
                        Rectangle fr = new Rectangle(right.Location.X, right.Location.Y, tr.Width, tr.Height);
                        MCvScalar c = right.Found ? ColorFound : ColorWrong;
                        CvInvoke.Rectangle(view, fr, c, 2, LineType.EightConnected, 0);
                        PutLabel(view, right.Found ? "FOUND" : "LOW SCORE", new Point(fr.X, fr.Y - 6), c);

                        if (right.Found)
                        {
                            int cx = fr.X + fr.Width / 2, cy = fr.Y + fr.Height / 2;
                            CvInvoke.Line(view, new Point(cx - 9, cy), new Point(cx + 9, cy), ColorCross, 2, LineType.EightConnected, 0);
                            CvInvoke.Line(view, new Point(cx, cy - 9), new Point(cx, cy + 9), ColorCross, 2, LineType.EightConnected, 0);
                        }
                    }

                    SetPictureBoxImage(picTest, ToDisplayBitmap(view));
                }

                lblTestCap.Text = string.Format("測試影像（偏移 {0:+#;-#;0}, {1:+#;-#;0}　角度 {2:F1}°）　灰虛線 = 基準位置　紅 = 定位結果",
                                                (int)numOffsetX.Value, (int)numOffsetY.Value, numAngle.Value);
            }
        }

        /// <summary>OpenCV 沒有虛線矩形，自己畫。用來表示「基準位置」以區別實際定位結果。</summary>
        private static void DrawDashedRect(Mat img, Rectangle r, MCvScalar color, int thickness, int dash)
        {
            for (int x = r.Left; x < r.Right; x += dash * 2)
            {
                int x2 = Math.Min(x + dash, r.Right);
                CvInvoke.Line(img, new Point(x, r.Top), new Point(x2, r.Top), color, thickness, LineType.EightConnected, 0);
                CvInvoke.Line(img, new Point(x, r.Bottom), new Point(x2, r.Bottom), color, thickness, LineType.EightConnected, 0);
            }
            for (int y = r.Top; y < r.Bottom; y += dash * 2)
            {
                int y2 = Math.Min(y + dash, r.Bottom);
                CvInvoke.Line(img, new Point(r.Left, y), new Point(r.Left, y2), color, thickness, LineType.EightConnected, 0);
                CvInvoke.Line(img, new Point(r.Right, y), new Point(r.Right, y2), color, thickness, LineType.EightConnected, 0);
            }
        }

        private static void PutLabel(Mat img, string text, Point at, MCvScalar color)
        {
            CvInvoke.PutText(img, text, at, FontFace.HersheySimplex, 0.45, color, 1, LineType.EightConnected, false);
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
        ///   畫面看起來完全正常，要等那塊記憶體被別的配置重用才炸——
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
        /// 每一張都是我們自己 new 出來（ToDisplayBitmap）再交給控制項的，所以要自己放。
        /// PictureBox 本身不會釋放 Image，不放就是每按一次漏一張全尺寸點陣圖。
        /// </summary>
        private static void SetPictureBoxImage(PictureBox pic, Image newImage)
        {
            Image old = pic.Image;
            pic.Image = newImage;
            if (old != null) old.Dispose();
        }

        private void SetVerdict(bool isOk, string text, string detail)
        {
            lblVerdict.Text = text;
            lblVerdict.BackColor = isOk ? VerdictOkBack : VerdictNgBack;
            lblVerdict.ForeColor = isOk ? VerdictOkFore : VerdictNgFore;
            lblMatchDetail.Text = detail;
        }

        private void SetVerdictIdle()
        {
            lblVerdict.Text = "待定位";
            lblVerdict.BackColor = SystemColors.Control;
            lblVerdict.ForeColor = Color.Gray;
            lblMatchDetail.Text = "匹配座標　—\r\n偏移量　　—";
        }

        private void ShowWelcome()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("第 9 章 模板匹配與定位 — 功能測試程式");
            sb.AppendLine(Divider);
            sb.AppendLine();
            sb.AppendLine("  「比對」問的是：這裡有沒有我要的圖案？   關注 → 最高分是否過門檻");
            sb.AppendLine("  「定位」問的是：這個圖案在哪裡？         關注 → 最高分的位置");
            sb.AppendLine();
            sb.AppendLine("  做定位時，搜尋區必須明顯大於模板。兩者一樣大時結果圖退化成 1×1，");
            sb.AppendLine("  MinMaxLoc 永遠只能回 (0,0)，位置看起來「定到了」其實根本沒動過。");
            sb.AppendLine("  這就是踩雷 P-006。");
            sb.AppendLine();
            sb.AppendLine(Divider);
            sb.AppendLine();
            sb.AppendLine("  ① 教導模板        從基準影像裁出模板，記下基準位置");
            sb.AppendLine("  ② 搜尋區設定      調 margin，即時看到結果圖尺寸夠不夠");
            sb.AppendLine("  ③ 執行定位        算出偏移量，這個值要套到所有後續 ROI");
            sb.AppendLine("  ④ P-006 對照      同一張圖，兩種搜尋區取法並列給你看");
            sb.AppendLine("  ⑤ 偏移掃描驗收    黃金法則的自動化版本 ← 本章重點");
            sb.AppendLine();
            sb.AppendLine(Divider);
            sb.AppendLine();
            sb.AppendLine("請先按左上角的「產生測試影像」（不需準備影像檔），或「載入影像」讀入自己的圖。");
            txtLog.Text = sb.ToString();
        }

        /// <summary>
        /// 錯誤回報。按鈕事件屬於使用者觸發的低頻路徑，用 MessageBox 沒問題。
        /// 第 18 章禁止彈 UI 的是「高頻路徑的 catch」（每幀迴圈裡），那種地方一律靜默寫檔。
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

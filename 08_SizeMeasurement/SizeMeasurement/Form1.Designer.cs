namespace SizeMeasurement
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        // 版面沿用第 7 章的骨架，ClientSize = 1184 × 761。
        //
        // 停靠順序提醒（第 20 章）：Controls.Add 的索引愈大愈先停靠，
        // 所以 Fill 的控制項要「最先 Add」（索引 0、最後停靠、吃剩餘空間）。
        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnGenCalib = new System.Windows.Forms.Button();
            this.btnGenMeasureOk = new System.Windows.Forms.Button();
            this.btnGenMeasureNg = new System.Windows.Forms.Button();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.lblSeparator = new System.Windows.Forms.Label();
            this.lblScaleCap = new System.Windows.Forms.Label();
            this.lblScaleValue = new System.Windows.Forms.Label();

            this.pnlRight = new System.Windows.Forms.Panel();
            this.grpCalib = new System.Windows.Forms.GroupBox();
            this.lblKnownMm = new System.Windows.Forms.Label();
            this.numKnownMm = new System.Windows.Forms.NumericUpDown();
            this.btnCalibrate = new System.Windows.Forms.Button();
            this.lblCalibResult = new System.Windows.Forms.Label();
            this.grpLine = new System.Windows.Forms.GroupBox();
            this.lblScanY = new System.Windows.Forms.Label();
            this.numScanY = new System.Windows.Forms.NumericUpDown();
            this.lblEdgeTh = new System.Windows.Forms.Label();
            this.numEdgeTh = new System.Windows.Forms.NumericUpDown();
            this.btnMeasureWidth = new System.Windows.Forms.Button();
            this.lblWidthResult = new System.Windows.Forms.Label();
            this.grpPoint = new System.Windows.Forms.GroupBox();
            this.lblPointHint = new System.Windows.Forms.Label();
            this.btnMeasureDist = new System.Windows.Forms.Button();
            this.lblDistResult = new System.Windows.Forms.Label();
            this.grpStat = new System.Windows.Forms.GroupBox();
            this.lblLineCount = new System.Windows.Forms.Label();
            this.numLineCount = new System.Windows.Forms.NumericUpDown();
            this.lblNominal = new System.Windows.Forms.Label();
            this.numNominal = new System.Windows.Forms.NumericUpDown();
            this.lblTolerance = new System.Windows.Forms.Label();
            this.numTolerance = new System.Windows.Forms.NumericUpDown();
            this.lblMaxStd = new System.Windows.Forms.Label();
            this.numMaxStd = new System.Windows.Forms.NumericUpDown();
            this.btnMeasureStat = new System.Windows.Forms.Button();
            this.lblStatResult = new System.Windows.Forms.Label();
            this.grpFlow = new System.Windows.Forms.GroupBox();
            this.lblFlowDesc = new System.Windows.Forms.Label();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.lblVerdict = new System.Windows.Forms.Label();

            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlImages = new System.Windows.Forms.Panel();
            this.lblMainCap = new System.Windows.Forms.Label();
            this.picMain = new System.Windows.Forms.PictureBox();
            this.lblResultCap = new System.Windows.Forms.Label();
            this.picResult = new System.Windows.Forms.PictureBox();
            this.pnlLog = new System.Windows.Forms.Panel();
            this.lblLogCap = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();

            this.pnlTop.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.grpCalib.SuspendLayout();
            this.grpLine.SuspendLayout();
            this.grpPoint.SuspendLayout();
            this.grpStat.SuspendLayout();
            this.grpFlow.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlImages.SuspendLayout();
            this.pnlLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKnownMm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScanY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEdgeTh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLineCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNominal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxStd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picResult)).BeginInit();
            this.SuspendLayout();

            // ═══════════════ pnlTop（Dock = Top, H = 46）═══════════════
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1184, 46);
            this.pnlTop.TabIndex = 0;

            this.btnGenCalib.Location = new System.Drawing.Point(12, 9);
            this.btnGenCalib.Name = "btnGenCalib";
            this.btnGenCalib.Size = new System.Drawing.Size(112, 28);
            this.btnGenCalib.TabIndex = 0;
            this.btnGenCalib.Text = "產生標定圖";
            this.btnGenCalib.UseVisualStyleBackColor = true;
            this.btnGenCalib.Click += new System.EventHandler(this.btnGenCalib_Click);

            this.btnGenMeasureOk.Location = new System.Drawing.Point(130, 9);
            this.btnGenMeasureOk.Name = "btnGenMeasureOk";
            this.btnGenMeasureOk.Size = new System.Drawing.Size(140, 28);
            this.btnGenMeasureOk.TabIndex = 1;
            this.btnGenMeasureOk.Text = "產生量測圖（良品）";
            this.btnGenMeasureOk.UseVisualStyleBackColor = true;
            this.btnGenMeasureOk.Click += new System.EventHandler(this.btnGenMeasureOk_Click);

            this.btnGenMeasureNg.Location = new System.Drawing.Point(276, 9);
            this.btnGenMeasureNg.Name = "btnGenMeasureNg";
            this.btnGenMeasureNg.Size = new System.Drawing.Size(150, 28);
            this.btnGenMeasureNg.TabIndex = 2;
            this.btnGenMeasureNg.Text = "產生量測圖（不良品）";
            this.btnGenMeasureNg.UseVisualStyleBackColor = true;
            this.btnGenMeasureNg.Click += new System.EventHandler(this.btnGenMeasureNg_Click);

            this.btnLoadImage.Location = new System.Drawing.Point(432, 9);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(92, 28);
            this.btnLoadImage.TabIndex = 3;
            this.btnLoadImage.Text = "載入影像";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);

            this.lblSeparator.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblSeparator.Location = new System.Drawing.Point(536, 11);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new System.Drawing.Size(1, 24);
            this.lblSeparator.TabIndex = 4;

            this.lblScaleCap.AutoSize = true;
            this.lblScaleCap.Location = new System.Drawing.Point(550, 17);
            this.lblScaleCap.Name = "lblScaleCap";
            this.lblScaleCap.Size = new System.Drawing.Size(76, 15);
            this.lblScaleCap.TabIndex = 5;
            this.lblScaleCap.Text = "目前比例尺：";

            this.lblScaleValue.AutoSize = true;
            this.lblScaleValue.ForeColor = System.Drawing.Color.FromArgb(179, 107, 0);
            this.lblScaleValue.Location = new System.Drawing.Point(636, 17);
            this.lblScaleValue.Name = "lblScaleValue";
            this.lblScaleValue.Size = new System.Drawing.Size(400, 15);
            this.lblScaleValue.TabIndex = 6;
            // 沒有標定就沒有 mm 這個單位——這是第 8 章的第一件事，所以做成常駐提醒
            this.lblScaleValue.Text = "尚未標定 — 量出來的只有 px，沒有 mm";

            this.pnlTop.Controls.Add(this.lblScaleValue);
            this.pnlTop.Controls.Add(this.lblScaleCap);
            this.pnlTop.Controls.Add(this.lblSeparator);
            this.pnlTop.Controls.Add(this.btnLoadImage);
            this.pnlTop.Controls.Add(this.btnGenMeasureNg);
            this.pnlTop.Controls.Add(this.btnGenMeasureOk);
            this.pnlTop.Controls.Add(this.btnGenCalib);

            // ═══════════════ pnlRight（Dock = Right, W = 296）═══════════════
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(888, 46);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(296, 715);
            this.pnlRight.TabIndex = 1;

            // ── ① 標定 ────────────────────────────────────────────
            this.grpCalib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCalib.Location = new System.Drawing.Point(10, 10);
            this.grpCalib.Name = "grpCalib";
            this.grpCalib.Size = new System.Drawing.Size(276, 108);
            this.grpCalib.TabIndex = 0;
            this.grpCalib.TabStop = false;
            this.grpCalib.Text = "① 標定 Calibration（教材 §1）";

            this.lblKnownMm.AutoSize = true;
            this.lblKnownMm.Location = new System.Drawing.Point(14, 28);
            this.lblKnownMm.Name = "lblKnownMm";
            this.lblKnownMm.Size = new System.Drawing.Size(128, 15);
            this.lblKnownMm.TabIndex = 0;
            this.lblKnownMm.Text = "標定板已知尺寸 mm";

            this.numKnownMm.DecimalPlaces = 3;
            this.numKnownMm.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            this.numKnownMm.Location = new System.Drawing.Point(184, 24);
            this.numKnownMm.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numKnownMm.Minimum = new decimal(new int[] { 1, 0, 0, 196608 });
            this.numKnownMm.Name = "numKnownMm";
            this.numKnownMm.Size = new System.Drawing.Size(78, 25);
            this.numKnownMm.TabIndex = 1;
            this.numKnownMm.Value = new decimal(new int[] { 10000, 0, 0, 196608 });

            this.btnCalibrate.Location = new System.Drawing.Point(14, 56);
            this.btnCalibrate.Name = "btnCalibrate";
            this.btnCalibrate.Size = new System.Drawing.Size(120, 28);
            this.btnCalibrate.TabIndex = 2;
            this.btnCalibrate.Text = "執行標定";
            this.btnCalibrate.UseVisualStyleBackColor = true;
            this.btnCalibrate.Click += new System.EventHandler(this.btnCalibrate_Click);

            this.lblCalibResult.AutoSize = true;
            this.lblCalibResult.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblCalibResult.Location = new System.Drawing.Point(142, 62);
            this.lblCalibResult.Name = "lblCalibResult";
            this.lblCalibResult.Size = new System.Drawing.Size(15, 15);
            this.lblCalibResult.TabIndex = 3;
            this.lblCalibResult.Text = "—";

            this.grpCalib.Controls.Add(this.lblCalibResult);
            this.grpCalib.Controls.Add(this.btnCalibrate);
            this.grpCalib.Controls.Add(this.numKnownMm);
            this.grpCalib.Controls.Add(this.lblKnownMm);

            // ── ② 線段量測 ────────────────────────────────────────
            this.grpLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLine.Location = new System.Drawing.Point(10, 126);
            this.grpLine.Name = "grpLine";
            this.grpLine.Size = new System.Drawing.Size(276, 128);
            this.grpLine.TabIndex = 1;
            this.grpLine.TabStop = false;
            this.grpLine.Text = "② 線段量測 — 寬度（§2／§4）";

            this.lblScanY.AutoSize = true;
            this.lblScanY.Location = new System.Drawing.Point(14, 28);
            this.lblScanY.Name = "lblScanY";
            this.lblScanY.Size = new System.Drawing.Size(62, 15);
            this.lblScanY.TabIndex = 0;
            this.lblScanY.Text = "掃描線 Y";

            this.numScanY.Location = new System.Drawing.Point(184, 24);
            this.numScanY.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.numScanY.Name = "numScanY";
            this.numScanY.Size = new System.Drawing.Size(78, 25);
            this.numScanY.TabIndex = 1;
            this.numScanY.Value = new decimal(new int[] { 210, 0, 0, 0 });

            this.lblEdgeTh.AutoSize = true;
            this.lblEdgeTh.Location = new System.Drawing.Point(14, 57);
            this.lblEdgeTh.Name = "lblEdgeTh";
            this.lblEdgeTh.Size = new System.Drawing.Size(160, 15);
            this.lblEdgeTh.TabIndex = 2;
            this.lblEdgeTh.Text = "邊緣閾值（0 = 自動取中點）";

            this.numEdgeTh.Location = new System.Drawing.Point(184, 53);
            this.numEdgeTh.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            this.numEdgeTh.Name = "numEdgeTh";
            this.numEdgeTh.Size = new System.Drawing.Size(78, 25);
            this.numEdgeTh.TabIndex = 3;
            this.numEdgeTh.Value = new decimal(new int[] { 0, 0, 0, 0 });

            this.btnMeasureWidth.Location = new System.Drawing.Point(14, 85);
            this.btnMeasureWidth.Name = "btnMeasureWidth";
            this.btnMeasureWidth.Size = new System.Drawing.Size(120, 28);
            this.btnMeasureWidth.TabIndex = 4;
            this.btnMeasureWidth.Text = "量測寬度";
            this.btnMeasureWidth.UseVisualStyleBackColor = true;
            this.btnMeasureWidth.Click += new System.EventHandler(this.btnMeasureWidth_Click);

            this.lblWidthResult.AutoSize = true;
            this.lblWidthResult.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblWidthResult.Location = new System.Drawing.Point(142, 91);
            this.lblWidthResult.Name = "lblWidthResult";
            this.lblWidthResult.Size = new System.Drawing.Size(15, 15);
            this.lblWidthResult.TabIndex = 5;
            this.lblWidthResult.Text = "—";

            this.grpLine.Controls.Add(this.lblWidthResult);
            this.grpLine.Controls.Add(this.btnMeasureWidth);
            this.grpLine.Controls.Add(this.numEdgeTh);
            this.grpLine.Controls.Add(this.lblEdgeTh);
            this.grpLine.Controls.Add(this.numScanY);
            this.grpLine.Controls.Add(this.lblScanY);

            // ── ③ 點位量測 ────────────────────────────────────────
            this.grpPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPoint.Location = new System.Drawing.Point(10, 262);
            this.grpPoint.Name = "grpPoint";
            this.grpPoint.Size = new System.Drawing.Size(276, 92);
            this.grpPoint.TabIndex = 2;
            this.grpPoint.TabStop = false;
            this.grpPoint.Text = "③ 點位量測 — 距離／角度（§3）";

            this.lblPointHint.AutoSize = true;
            this.lblPointHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblPointHint.Location = new System.Drawing.Point(14, 26);
            this.lblPointHint.Name = "lblPointHint";
            this.lblPointHint.Size = new System.Drawing.Size(220, 15);
            this.lblPointHint.TabIndex = 0;
            this.lblPointHint.Text = "取兩個定位孔的形心，算孔距與夾角";

            this.btnMeasureDist.Location = new System.Drawing.Point(14, 48);
            this.btnMeasureDist.Name = "btnMeasureDist";
            this.btnMeasureDist.Size = new System.Drawing.Size(148, 28);
            this.btnMeasureDist.TabIndex = 1;
            this.btnMeasureDist.Text = "量測距離與角度";
            this.btnMeasureDist.UseVisualStyleBackColor = true;
            this.btnMeasureDist.Click += new System.EventHandler(this.btnMeasureDist_Click);

            this.lblDistResult.AutoSize = true;
            this.lblDistResult.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblDistResult.Location = new System.Drawing.Point(170, 54);
            this.lblDistResult.Name = "lblDistResult";
            this.lblDistResult.Size = new System.Drawing.Size(15, 15);
            this.lblDistResult.TabIndex = 2;
            this.lblDistResult.Text = "—";

            this.grpPoint.Controls.Add(this.lblDistResult);
            this.grpPoint.Controls.Add(this.btnMeasureDist);
            this.grpPoint.Controls.Add(this.lblPointHint);

            // ── ④ 統計量測 ────────────────────────────────────────
            this.grpStat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpStat.Location = new System.Drawing.Point(10, 362);
            this.grpStat.Name = "grpStat";
            this.grpStat.Size = new System.Drawing.Size(276, 186);
            this.grpStat.TabIndex = 3;
            this.grpStat.TabStop = false;
            this.grpStat.Text = "④ 統計量測（§5）";

            this.lblLineCount.AutoSize = true;
            this.lblLineCount.Location = new System.Drawing.Point(14, 26);
            this.lblLineCount.Name = "lblLineCount";
            this.lblLineCount.Size = new System.Drawing.Size(78, 15);
            this.lblLineCount.TabIndex = 0;
            this.lblLineCount.Text = "掃描線數 N";

            this.numLineCount.Location = new System.Drawing.Point(184, 22);
            this.numLineCount.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numLineCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numLineCount.Name = "numLineCount";
            this.numLineCount.Size = new System.Drawing.Size(78, 25);
            this.numLineCount.TabIndex = 1;
            this.numLineCount.Value = new decimal(new int[] { 10, 0, 0, 0 });

            this.lblNominal.AutoSize = true;
            this.lblNominal.Location = new System.Drawing.Point(14, 55);
            this.lblNominal.Name = "lblNominal";
            this.lblNominal.Size = new System.Drawing.Size(90, 15);
            this.lblNominal.TabIndex = 2;
            this.lblNominal.Text = "標稱值 mm";

            this.numNominal.DecimalPlaces = 3;
            this.numNominal.Increment = new decimal(new int[] { 10, 0, 0, 196608 });
            this.numNominal.Location = new System.Drawing.Point(184, 51);
            this.numNominal.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numNominal.Name = "numNominal";
            this.numNominal.Size = new System.Drawing.Size(78, 25);
            this.numNominal.TabIndex = 3;
            this.numNominal.Value = new decimal(new int[] { 4000, 0, 0, 196608 });

            this.lblTolerance.AutoSize = true;
            this.lblTolerance.Location = new System.Drawing.Point(14, 84);
            this.lblTolerance.Name = "lblTolerance";
            this.lblTolerance.Size = new System.Drawing.Size(90, 15);
            this.lblTolerance.TabIndex = 4;
            this.lblTolerance.Text = "公差 ± mm";

            this.numTolerance.DecimalPlaces = 3;
            this.numTolerance.Increment = new decimal(new int[] { 10, 0, 0, 196608 });
            this.numTolerance.Location = new System.Drawing.Point(184, 80);
            this.numTolerance.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numTolerance.Name = "numTolerance";
            this.numTolerance.Size = new System.Drawing.Size(78, 25);
            this.numTolerance.TabIndex = 5;
            this.numTolerance.Value = new decimal(new int[] { 50, 0, 0, 196608 });

            this.lblMaxStd.AutoSize = true;
            this.lblMaxStd.Location = new System.Drawing.Point(14, 113);
            this.lblMaxStd.Name = "lblMaxStd";
            this.lblMaxStd.Size = new System.Drawing.Size(130, 15);
            this.lblMaxStd.TabIndex = 6;
            this.lblMaxStd.Text = "標準差上限 mm";

            this.numMaxStd.DecimalPlaces = 4;
            this.numMaxStd.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
            this.numMaxStd.Location = new System.Drawing.Point(184, 109);
            this.numMaxStd.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numMaxStd.Name = "numMaxStd";
            this.numMaxStd.Size = new System.Drawing.Size(78, 25);
            this.numMaxStd.TabIndex = 7;
            this.numMaxStd.Value = new decimal(new int[] { 100, 0, 0, 262144 });

            this.btnMeasureStat.Location = new System.Drawing.Point(14, 141);
            this.btnMeasureStat.Name = "btnMeasureStat";
            this.btnMeasureStat.Size = new System.Drawing.Size(148, 28);
            this.btnMeasureStat.TabIndex = 8;
            this.btnMeasureStat.Text = "多線量測 ＋ 統計";
            this.btnMeasureStat.UseVisualStyleBackColor = true;
            this.btnMeasureStat.Click += new System.EventHandler(this.btnMeasureStat_Click);

            this.lblStatResult.AutoSize = true;
            this.lblStatResult.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStatResult.Location = new System.Drawing.Point(170, 147);
            this.lblStatResult.Name = "lblStatResult";
            this.lblStatResult.Size = new System.Drawing.Size(15, 15);
            this.lblStatResult.TabIndex = 9;
            this.lblStatResult.Text = "—";

            this.grpStat.Controls.Add(this.lblStatResult);
            this.grpStat.Controls.Add(this.btnMeasureStat);
            this.grpStat.Controls.Add(this.numMaxStd);
            this.grpStat.Controls.Add(this.lblMaxStd);
            this.grpStat.Controls.Add(this.numTolerance);
            this.grpStat.Controls.Add(this.lblTolerance);
            this.grpStat.Controls.Add(this.numNominal);
            this.grpStat.Controls.Add(this.lblNominal);
            this.grpStat.Controls.Add(this.numLineCount);
            this.grpStat.Controls.Add(this.lblLineCount);

            // ── ⑤ 完整流程 ────────────────────────────────────────
            this.grpFlow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFlow.Location = new System.Drawing.Point(10, 556);
            this.grpFlow.Name = "grpFlow";
            this.grpFlow.Size = new System.Drawing.Size(276, 80);
            this.grpFlow.TabIndex = 4;
            this.grpFlow.TabStop = false;
            this.grpFlow.Text = "⑤ 完整流程";

            this.lblFlowDesc.AutoSize = true;
            this.lblFlowDesc.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblFlowDesc.Location = new System.Drawing.Point(14, 26);
            this.lblFlowDesc.Name = "lblFlowDesc";
            this.lblFlowDesc.Size = new System.Drawing.Size(240, 15);
            this.lblFlowDesc.TabIndex = 0;
            this.lblFlowDesc.Text = "標定 → 寬度 → 孔距 → 多線統計 → 判定";

            this.btnRunAll.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRunAll.Location = new System.Drawing.Point(14, 42);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(248, 30);
            this.btnRunAll.TabIndex = 1;
            this.btnRunAll.Text = "▶ 一鍵執行完整流程";
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.btnRunAll_Click);

            this.grpFlow.Controls.Add(this.btnRunAll);
            this.grpFlow.Controls.Add(this.lblFlowDesc);

            // ── 判定燈 ────────────────────────────────────────────
            this.lblVerdict.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVerdict.BackColor = System.Drawing.SystemColors.Control;
            this.lblVerdict.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVerdict.Font = new System.Drawing.Font("Microsoft JhengHei UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblVerdict.ForeColor = System.Drawing.Color.Gray;
            this.lblVerdict.Location = new System.Drawing.Point(10, 658);
            this.lblVerdict.Name = "lblVerdict";
            this.lblVerdict.Size = new System.Drawing.Size(276, 38);
            this.lblVerdict.TabIndex = 5;
            this.lblVerdict.Text = "待量測";
            this.lblVerdict.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.pnlRight.Controls.Add(this.lblVerdict);
            this.pnlRight.Controls.Add(this.grpFlow);
            this.pnlRight.Controls.Add(this.grpStat);
            this.pnlRight.Controls.Add(this.grpPoint);
            this.pnlRight.Controls.Add(this.grpLine);
            this.pnlRight.Controls.Add(this.grpCalib);

            // ═══════════════ pnlMain（Dock = Fill）═══════════════
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 46);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(888, 715);
            this.pnlMain.TabIndex = 2;

            // ── 影像區（Dock = Fill）──────────────────────────────
            this.pnlImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImages.Location = new System.Drawing.Point(0, 0);
            this.pnlImages.Name = "pnlImages";
            this.pnlImages.Size = new System.Drawing.Size(888, 361);
            this.pnlImages.TabIndex = 0;

            this.lblMainCap.AutoSize = true;
            this.lblMainCap.Location = new System.Drawing.Point(10, 8);
            this.lblMainCap.Name = "lblMainCap";
            this.lblMainCap.Size = new System.Drawing.Size(160, 15);
            this.lblMainCap.TabIndex = 0;
            this.lblMainCap.Text = "影像 ＋ 量測標記";

            this.picMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.picMain.BackColor = System.Drawing.Color.Black;
            this.picMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picMain.Location = new System.Drawing.Point(8, 26);
            this.picMain.Name = "picMain";
            this.picMain.Size = new System.Drawing.Size(428, 325);
            this.picMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMain.TabIndex = 1;
            this.picMain.TabStop = false;

            this.lblResultCap.AutoSize = true;
            this.lblResultCap.Location = new System.Drawing.Point(450, 8);
            this.lblResultCap.Name = "lblResultCap";
            this.lblResultCap.Size = new System.Drawing.Size(200, 15);
            this.lblResultCap.TabIndex = 2;
            this.lblResultCap.Text = "灰階剖面圖（Profile）";

            this.picResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.picResult.BackColor = System.Drawing.Color.Black;
            this.picResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picResult.Location = new System.Drawing.Point(448, 26);
            this.picResult.Name = "picResult";
            this.picResult.Size = new System.Drawing.Size(428, 325);
            this.picResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picResult.TabIndex = 3;
            this.picResult.TabStop = false;

            this.pnlImages.Controls.Add(this.picResult);
            this.pnlImages.Controls.Add(this.lblResultCap);
            this.pnlImages.Controls.Add(this.picMain);
            this.pnlImages.Controls.Add(this.lblMainCap);

            // ── 結果文字區（Dock = Bottom, H = 354）───────────────
            this.pnlLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLog.Location = new System.Drawing.Point(0, 361);
            this.pnlLog.Name = "pnlLog";
            this.pnlLog.Size = new System.Drawing.Size(888, 354);
            this.pnlLog.TabIndex = 1;

            this.lblLogCap.AutoSize = true;
            this.lblLogCap.Location = new System.Drawing.Point(10, 0);
            this.lblLogCap.Name = "lblLogCap";
            this.lblLogCap.Size = new System.Drawing.Size(60, 15);
            this.lblLogCap.TabIndex = 0;
            this.lblLogCap.Text = "量測報告";

            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.txtLog.Location = new System.Drawing.Point(8, 18);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(868, 326);
            this.txtLog.TabIndex = 1;
            this.txtLog.WordWrap = false;

            this.pnlLog.Controls.Add(this.txtLog);
            this.pnlLog.Controls.Add(this.lblLogCap);

            // Fill 要先 Add（索引小 = 後停靠 = 吃剩餘空間）
            this.pnlMain.Controls.Add(this.pnlImages);
            this.pnlMain.Controls.Add(this.pnlLog);

            // ═══════════════ Form1 ═══════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(920, 640);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "第 8 章 尺寸量測 — SizeMeasurement";

            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.grpCalib.ResumeLayout(false);
            this.grpCalib.PerformLayout();
            this.grpLine.ResumeLayout(false);
            this.grpLine.PerformLayout();
            this.grpPoint.ResumeLayout(false);
            this.grpPoint.PerformLayout();
            this.grpStat.ResumeLayout(false);
            this.grpStat.PerformLayout();
            this.grpFlow.ResumeLayout(false);
            this.grpFlow.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlImages.ResumeLayout(false);
            this.pnlImages.PerformLayout();
            this.pnlLog.ResumeLayout(false);
            this.pnlLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numKnownMm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScanY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEdgeTh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLineCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNominal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxStd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picResult)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnGenCalib;
        private System.Windows.Forms.Button btnGenMeasureOk;
        private System.Windows.Forms.Button btnGenMeasureNg;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Label lblSeparator;
        private System.Windows.Forms.Label lblScaleCap;
        private System.Windows.Forms.Label lblScaleValue;

        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.GroupBox grpCalib;
        private System.Windows.Forms.Label lblKnownMm;
        private System.Windows.Forms.NumericUpDown numKnownMm;
        private System.Windows.Forms.Button btnCalibrate;
        private System.Windows.Forms.Label lblCalibResult;
        private System.Windows.Forms.GroupBox grpLine;
        private System.Windows.Forms.Label lblScanY;
        private System.Windows.Forms.NumericUpDown numScanY;
        private System.Windows.Forms.Label lblEdgeTh;
        private System.Windows.Forms.NumericUpDown numEdgeTh;
        private System.Windows.Forms.Button btnMeasureWidth;
        private System.Windows.Forms.Label lblWidthResult;
        private System.Windows.Forms.GroupBox grpPoint;
        private System.Windows.Forms.Label lblPointHint;
        private System.Windows.Forms.Button btnMeasureDist;
        private System.Windows.Forms.Label lblDistResult;
        private System.Windows.Forms.GroupBox grpStat;
        private System.Windows.Forms.Label lblLineCount;
        private System.Windows.Forms.NumericUpDown numLineCount;
        private System.Windows.Forms.Label lblNominal;
        private System.Windows.Forms.NumericUpDown numNominal;
        private System.Windows.Forms.Label lblTolerance;
        private System.Windows.Forms.NumericUpDown numTolerance;
        private System.Windows.Forms.Label lblMaxStd;
        private System.Windows.Forms.NumericUpDown numMaxStd;
        private System.Windows.Forms.Button btnMeasureStat;
        private System.Windows.Forms.Label lblStatResult;
        private System.Windows.Forms.GroupBox grpFlow;
        private System.Windows.Forms.Label lblFlowDesc;
        private System.Windows.Forms.Button btnRunAll;
        private System.Windows.Forms.Label lblVerdict;

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlImages;
        private System.Windows.Forms.Label lblMainCap;
        private System.Windows.Forms.PictureBox picMain;
        private System.Windows.Forms.Label lblResultCap;
        private System.Windows.Forms.PictureBox picResult;
        private System.Windows.Forms.Panel pnlLog;
        private System.Windows.Forms.Label lblLogCap;
        private System.Windows.Forms.TextBox txtLog;
    }
}

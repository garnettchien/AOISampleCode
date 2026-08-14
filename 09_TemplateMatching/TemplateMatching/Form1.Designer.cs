namespace TemplateMatching
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

        // 版面座標對應 mockup\09_模板匹配_UI_mockup.html，ClientSize = 1184 × 761。
        //
        // 停靠順序提醒（第 20 章）：Controls.Add 的索引愈大愈先停靠，
        // 所以 Fill 的控制項要「最先 Add」（索引 0、最後停靠、吃剩餘空間）。
        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnGenTestImage = new System.Windows.Forms.Button();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.lblSeparator = new System.Windows.Forms.Label();
            this.lblOffsetCap = new System.Windows.Forms.Label();
            this.lblDx = new System.Windows.Forms.Label();
            this.numOffsetX = new System.Windows.Forms.NumericUpDown();
            this.lblDy = new System.Windows.Forms.Label();
            this.numOffsetY = new System.Windows.Forms.NumericUpDown();
            this.lblAngleCap = new System.Windows.Forms.Label();
            this.numAngle = new System.Windows.Forms.NumericUpDown();
            this.btnResetOffset = new System.Windows.Forms.Button();
            this.lblPitfall = new System.Windows.Forms.Label();

            this.pnlRight = new System.Windows.Forms.Panel();
            this.grpTeach = new System.Windows.Forms.GroupBox();
            this.lblTmplCap = new System.Windows.Forms.Label();
            this.lblTx = new System.Windows.Forms.Label();
            this.numTmplX = new System.Windows.Forms.NumericUpDown();
            this.lblTy = new System.Windows.Forms.Label();
            this.numTmplY = new System.Windows.Forms.NumericUpDown();
            this.lblTw = new System.Windows.Forms.Label();
            this.numTmplW = new System.Windows.Forms.NumericUpDown();
            this.lblTh = new System.Windows.Forms.Label();
            this.numTmplH = new System.Windows.Forms.NumericUpDown();
            this.picTemplate = new System.Windows.Forms.PictureBox();
            this.btnTeach = new System.Windows.Forms.Button();

            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.lblMarginCap = new System.Windows.Forms.Label();
            this.lblMx = new System.Windows.Forms.Label();
            this.numMarginX = new System.Windows.Forms.NumericUpDown();
            this.lblMy = new System.Windows.Forms.Label();
            this.numMarginY = new System.Windows.Forms.NumericUpDown();
            this.lblResultMapInfo = new System.Windows.Forms.Label();
            this.lblSearchRectInfo = new System.Windows.Forms.Label();

            this.grpLocate = new System.Windows.Forms.GroupBox();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.numThreshold = new System.Windows.Forms.NumericUpDown();
            this.chkMultiAngle = new System.Windows.Forms.CheckBox();
            this.lblAngleRange = new System.Windows.Forms.Label();
            this.numAngleRange = new System.Windows.Forms.NumericUpDown();
            this.lblAngleStep = new System.Windows.Forms.Label();
            this.numAngleStep = new System.Windows.Forms.NumericUpDown();
            this.btnLocate = new System.Windows.Forms.Button();

            this.grpCompare = new System.Windows.Forms.GroupBox();
            this.lblCompareDesc = new System.Windows.Forms.Label();
            this.btnCompare = new System.Windows.Forms.Button();

            this.grpSweep = new System.Windows.Forms.GroupBox();
            this.lblSweepCap = new System.Windows.Forms.Label();
            this.numSweepRange = new System.Windows.Forms.NumericUpDown();
            this.lblSweepStep = new System.Windows.Forms.Label();
            this.numSweepStep = new System.Windows.Forms.NumericUpDown();
            this.lblSweepDesc = new System.Windows.Forms.Label();
            this.btnSweep = new System.Windows.Forms.Button();

            this.lblMatchDetail = new System.Windows.Forms.Label();
            this.lblVerdict = new System.Windows.Forms.Label();

            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlImages = new System.Windows.Forms.Panel();
            this.lblRefCap = new System.Windows.Forms.Label();
            this.picRef = new System.Windows.Forms.PictureBox();
            this.lblTestCap = new System.Windows.Forms.Label();
            this.picTest = new System.Windows.Forms.PictureBox();
            this.pnlLog = new System.Windows.Forms.Panel();
            this.lblLogCap = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();

            this.pnlTop.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.grpTeach.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.grpLocate.SuspendLayout();
            this.grpCompare.SuspendLayout();
            this.grpSweep.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlImages.SuspendLayout();
            this.pnlLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOffsetX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOffsetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTmplX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTmplY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTmplW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTmplH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAngleRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAngleStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSweepRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSweepStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRef)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTest)).BeginInit();
            this.SuspendLayout();

            // ═══════════════ pnlTop（Dock = Top, H = 46）═══════════════
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1184, 46);
            this.pnlTop.TabIndex = 0;

            this.btnGenTestImage.Location = new System.Drawing.Point(12, 9);
            this.btnGenTestImage.Name = "btnGenTestImage";
            this.btnGenTestImage.Size = new System.Drawing.Size(104, 28);
            this.btnGenTestImage.TabIndex = 0;
            this.btnGenTestImage.Text = "產生測試影像";
            this.btnGenTestImage.UseVisualStyleBackColor = true;
            this.btnGenTestImage.Click += new System.EventHandler(this.btnGenTestImage_Click);

            this.btnLoadImage.Location = new System.Drawing.Point(122, 9);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(92, 28);
            this.btnLoadImage.TabIndex = 1;
            this.btnLoadImage.Text = "載入影像";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);

            this.lblSeparator.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblSeparator.Location = new System.Drawing.Point(224, 11);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new System.Drawing.Size(1, 24);
            this.lblSeparator.TabIndex = 2;

            this.lblOffsetCap.AutoSize = true;
            this.lblOffsetCap.Location = new System.Drawing.Point(238, 17);
            this.lblOffsetCap.Name = "lblOffsetCap";
            this.lblOffsetCap.Size = new System.Drawing.Size(66, 15);
            this.lblOffsetCap.TabIndex = 3;
            this.lblOffsetCap.Text = "產品偏移：";

            this.lblDx.AutoSize = true;
            this.lblDx.Location = new System.Drawing.Point(306, 17);
            this.lblDx.Name = "lblDx";
            this.lblDx.Size = new System.Drawing.Size(20, 15);
            this.lblDx.TabIndex = 4;
            this.lblDx.Text = "dX";

            this.numOffsetX.Location = new System.Drawing.Point(326, 12);
            this.numOffsetX.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            this.numOffsetX.Minimum = new decimal(new int[] { 200, 0, 0, -2147483648 });
            this.numOffsetX.Name = "numOffsetX";
            this.numOffsetX.Size = new System.Drawing.Size(58, 25);
            this.numOffsetX.TabIndex = 5;
            this.numOffsetX.Value = new decimal(new int[] { 20, 0, 0, 0 });
            this.numOffsetX.ValueChanged += new System.EventHandler(this.OnTestImageParamChanged);

            this.lblDy.AutoSize = true;
            this.lblDy.Location = new System.Drawing.Point(394, 17);
            this.lblDy.Name = "lblDy";
            this.lblDy.Size = new System.Drawing.Size(20, 15);
            this.lblDy.TabIndex = 6;
            this.lblDy.Text = "dY";

            this.numOffsetY.Location = new System.Drawing.Point(414, 12);
            this.numOffsetY.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            this.numOffsetY.Minimum = new decimal(new int[] { 200, 0, 0, -2147483648 });
            this.numOffsetY.Name = "numOffsetY";
            this.numOffsetY.Size = new System.Drawing.Size(58, 25);
            this.numOffsetY.TabIndex = 7;
            this.numOffsetY.Value = new decimal(new int[] { 10, 0, 0, 0 });
            this.numOffsetY.ValueChanged += new System.EventHandler(this.OnTestImageParamChanged);

            this.lblAngleCap.AutoSize = true;
            this.lblAngleCap.Location = new System.Drawing.Point(482, 17);
            this.lblAngleCap.Name = "lblAngleCap";
            this.lblAngleCap.Size = new System.Drawing.Size(30, 15);
            this.lblAngleCap.TabIndex = 8;
            this.lblAngleCap.Text = "角度";

            this.numAngle.DecimalPlaces = 1;
            this.numAngle.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            this.numAngle.Location = new System.Drawing.Point(512, 12);
            this.numAngle.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            this.numAngle.Minimum = new decimal(new int[] { 30, 0, 0, -2147483648 });
            this.numAngle.Name = "numAngle";
            this.numAngle.Size = new System.Drawing.Size(58, 25);
            this.numAngle.TabIndex = 9;
            this.numAngle.ValueChanged += new System.EventHandler(this.OnTestImageParamChanged);

            this.btnResetOffset.Location = new System.Drawing.Point(580, 9);
            this.btnResetOffset.Name = "btnResetOffset";
            this.btnResetOffset.Size = new System.Drawing.Size(72, 28);
            this.btnResetOffset.TabIndex = 10;
            this.btnResetOffset.Text = "歸零";
            this.btnResetOffset.UseVisualStyleBackColor = true;
            this.btnResetOffset.Click += new System.EventHandler(this.btnResetOffset_Click);

            this.lblPitfall.AutoSize = true;
            this.lblPitfall.ForeColor = System.Drawing.Color.FromArgb(179, 107, 0);
            this.lblPitfall.Location = new System.Drawing.Point(668, 17);
            this.lblPitfall.Name = "lblPitfall";
            this.lblPitfall.Size = new System.Drawing.Size(340, 15);
            this.lblPitfall.TabIndex = 11;
            this.lblPitfall.Text = "⚠ P-006：搜尋區 ≈ 模板 → MinMaxLoc 永遠回 (0,0)";

            this.pnlTop.Controls.Add(this.lblPitfall);
            this.pnlTop.Controls.Add(this.btnResetOffset);
            this.pnlTop.Controls.Add(this.numAngle);
            this.pnlTop.Controls.Add(this.lblAngleCap);
            this.pnlTop.Controls.Add(this.numOffsetY);
            this.pnlTop.Controls.Add(this.lblDy);
            this.pnlTop.Controls.Add(this.numOffsetX);
            this.pnlTop.Controls.Add(this.lblDx);
            this.pnlTop.Controls.Add(this.lblOffsetCap);
            this.pnlTop.Controls.Add(this.lblSeparator);
            this.pnlTop.Controls.Add(this.btnLoadImage);
            this.pnlTop.Controls.Add(this.btnGenTestImage);

            // ═══════════════ pnlRight（Dock = Right, W = 296）═══════════════
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(888, 46);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(296, 715);
            this.pnlRight.TabIndex = 1;

            // ── ① 教導模板 ────────────────────────────────────────
            this.grpTeach.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTeach.Location = new System.Drawing.Point(10, 10);
            this.grpTeach.Name = "grpTeach";
            this.grpTeach.Size = new System.Drawing.Size(276, 118);
            this.grpTeach.TabIndex = 0;
            this.grpTeach.TabStop = false;
            this.grpTeach.Text = "① 教導模板";

            this.lblTmplCap.AutoSize = true;
            this.lblTmplCap.Location = new System.Drawing.Point(14, 24);
            this.lblTmplCap.Name = "lblTmplCap";
            this.lblTmplCap.Size = new System.Drawing.Size(45, 15);
            this.lblTmplCap.TabIndex = 0;
            this.lblTmplCap.Text = "模板框";

            this.lblTx.AutoSize = true;
            this.lblTx.Location = new System.Drawing.Point(62, 24);
            this.lblTx.Name = "lblTx";
            this.lblTx.Size = new System.Drawing.Size(14, 15);
            this.lblTx.TabIndex = 1;
            this.lblTx.Text = "X";

            this.numTmplX.Location = new System.Drawing.Point(76, 20);
            this.numTmplX.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.numTmplX.Name = "numTmplX";
            this.numTmplX.Size = new System.Drawing.Size(56, 25);
            this.numTmplX.TabIndex = 2;
            this.numTmplX.Value = new decimal(new int[] { 156, 0, 0, 0 });
            this.numTmplX.ValueChanged += new System.EventHandler(this.OnRectParamChanged);

            this.lblTy.AutoSize = true;
            this.lblTy.Location = new System.Drawing.Point(140, 24);
            this.lblTy.Name = "lblTy";
            this.lblTy.Size = new System.Drawing.Size(14, 15);
            this.lblTy.TabIndex = 3;
            this.lblTy.Text = "Y";

            this.numTmplY.Location = new System.Drawing.Point(154, 20);
            this.numTmplY.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.numTmplY.Name = "numTmplY";
            this.numTmplY.Size = new System.Drawing.Size(56, 25);
            this.numTmplY.TabIndex = 4;
            this.numTmplY.Value = new decimal(new int[] { 126, 0, 0, 0 });
            this.numTmplY.ValueChanged += new System.EventHandler(this.OnRectParamChanged);

            this.lblTw.AutoSize = true;
            this.lblTw.Location = new System.Drawing.Point(62, 50);
            this.lblTw.Name = "lblTw";
            this.lblTw.Size = new System.Drawing.Size(16, 15);
            this.lblTw.TabIndex = 5;
            this.lblTw.Text = "W";

            this.numTmplW.Location = new System.Drawing.Point(76, 46);
            this.numTmplW.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.numTmplW.Minimum = new decimal(new int[] { 4, 0, 0, 0 });
            this.numTmplW.Name = "numTmplW";
            this.numTmplW.Size = new System.Drawing.Size(56, 25);
            this.numTmplW.TabIndex = 6;
            this.numTmplW.Value = new decimal(new int[] { 48, 0, 0, 0 });
            this.numTmplW.ValueChanged += new System.EventHandler(this.OnRectParamChanged);

            this.lblTh.AutoSize = true;
            this.lblTh.Location = new System.Drawing.Point(140, 50);
            this.lblTh.Name = "lblTh";
            this.lblTh.Size = new System.Drawing.Size(14, 15);
            this.lblTh.TabIndex = 7;
            this.lblTh.Text = "H";

            this.numTmplH.Location = new System.Drawing.Point(154, 46);
            this.numTmplH.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.numTmplH.Minimum = new decimal(new int[] { 4, 0, 0, 0 });
            this.numTmplH.Name = "numTmplH";
            this.numTmplH.Size = new System.Drawing.Size(56, 25);
            this.numTmplH.TabIndex = 8;
            this.numTmplH.Value = new decimal(new int[] { 48, 0, 0, 0 });
            this.numTmplH.ValueChanged += new System.EventHandler(this.OnRectParamChanged);

            this.picTemplate.BackColor = System.Drawing.Color.Black;
            this.picTemplate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTemplate.Location = new System.Drawing.Point(216, 20);
            this.picTemplate.Name = "picTemplate";
            this.picTemplate.Size = new System.Drawing.Size(48, 48);
            this.picTemplate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picTemplate.TabIndex = 9;
            this.picTemplate.TabStop = false;

            this.btnTeach.Location = new System.Drawing.Point(14, 78);
            this.btnTeach.Name = "btnTeach";
            this.btnTeach.Size = new System.Drawing.Size(250, 28);
            this.btnTeach.TabIndex = 10;
            this.btnTeach.Text = "教導模板（從基準影像裁切）";
            this.btnTeach.UseVisualStyleBackColor = true;
            this.btnTeach.Click += new System.EventHandler(this.btnTeach_Click);

            this.grpTeach.Controls.Add(this.btnTeach);
            this.grpTeach.Controls.Add(this.picTemplate);
            this.grpTeach.Controls.Add(this.numTmplH);
            this.grpTeach.Controls.Add(this.lblTh);
            this.grpTeach.Controls.Add(this.numTmplW);
            this.grpTeach.Controls.Add(this.lblTw);
            this.grpTeach.Controls.Add(this.numTmplY);
            this.grpTeach.Controls.Add(this.lblTy);
            this.grpTeach.Controls.Add(this.numTmplX);
            this.grpTeach.Controls.Add(this.lblTx);
            this.grpTeach.Controls.Add(this.lblTmplCap);

            // ── ② 搜尋區設定 ──────────────────────────────────────
            this.grpSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSearch.Location = new System.Drawing.Point(10, 136);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(276, 96);
            this.grpSearch.TabIndex = 1;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "② 搜尋區設定";

            this.lblMarginCap.AutoSize = true;
            this.lblMarginCap.Location = new System.Drawing.Point(14, 24);
            this.lblMarginCap.Name = "lblMarginCap";
            this.lblMarginCap.Size = new System.Drawing.Size(78, 15);
            this.lblMarginCap.TabIndex = 0;
            this.lblMarginCap.Text = "外擴 margin";

            this.lblMx.AutoSize = true;
            this.lblMx.Location = new System.Drawing.Point(102, 24);
            this.lblMx.Name = "lblMx";
            this.lblMx.Size = new System.Drawing.Size(14, 15);
            this.lblMx.TabIndex = 1;
            this.lblMx.Text = "X";

            this.numMarginX.Location = new System.Drawing.Point(116, 20);
            this.numMarginX.Maximum = new decimal(new int[] { 400, 0, 0, 0 });
            this.numMarginX.Name = "numMarginX";
            this.numMarginX.Size = new System.Drawing.Size(56, 25);
            this.numMarginX.TabIndex = 2;
            this.numMarginX.Value = new decimal(new int[] { 48, 0, 0, 0 });
            this.numMarginX.ValueChanged += new System.EventHandler(this.OnRectParamChanged);

            this.lblMy.AutoSize = true;
            this.lblMy.Location = new System.Drawing.Point(180, 24);
            this.lblMy.Name = "lblMy";
            this.lblMy.Size = new System.Drawing.Size(14, 15);
            this.lblMy.TabIndex = 3;
            this.lblMy.Text = "Y";

            this.numMarginY.Location = new System.Drawing.Point(194, 20);
            this.numMarginY.Maximum = new decimal(new int[] { 400, 0, 0, 0 });
            this.numMarginY.Name = "numMarginY";
            this.numMarginY.Size = new System.Drawing.Size(56, 25);
            this.numMarginY.TabIndex = 4;
            this.numMarginY.Value = new decimal(new int[] { 48, 0, 0, 0 });
            this.numMarginY.ValueChanged += new System.EventHandler(this.OnRectParamChanged);

            this.lblResultMapInfo.AutoSize = true;
            this.lblResultMapInfo.Location = new System.Drawing.Point(14, 52);
            this.lblResultMapInfo.Name = "lblResultMapInfo";
            this.lblResultMapInfo.Size = new System.Drawing.Size(200, 15);
            this.lblResultMapInfo.TabIndex = 5;
            this.lblResultMapInfo.Text = "—";

            this.lblSearchRectInfo.AutoSize = true;
            this.lblSearchRectInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblSearchRectInfo.Location = new System.Drawing.Point(14, 72);
            this.lblSearchRectInfo.Name = "lblSearchRectInfo";
            this.lblSearchRectInfo.Size = new System.Drawing.Size(180, 15);
            this.lblSearchRectInfo.TabIndex = 6;
            this.lblSearchRectInfo.Text = "—";

            this.grpSearch.Controls.Add(this.lblSearchRectInfo);
            this.grpSearch.Controls.Add(this.lblResultMapInfo);
            this.grpSearch.Controls.Add(this.numMarginY);
            this.grpSearch.Controls.Add(this.lblMy);
            this.grpSearch.Controls.Add(this.numMarginX);
            this.grpSearch.Controls.Add(this.lblMx);
            this.grpSearch.Controls.Add(this.lblMarginCap);

            // ── ③ 執行定位 ────────────────────────────────────────
            this.grpLocate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLocate.Location = new System.Drawing.Point(10, 240);
            this.grpLocate.Name = "grpLocate";
            this.grpLocate.Size = new System.Drawing.Size(276, 144);
            this.grpLocate.TabIndex = 2;
            this.grpLocate.TabStop = false;
            this.grpLocate.Text = "③ 執行定位";

            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(14, 26);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(150, 15);
            this.lblThreshold.TabIndex = 0;
            this.lblThreshold.Text = "分數門檻 ScoreThreshold";

            this.numThreshold.DecimalPlaces = 2;
            this.numThreshold.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            this.numThreshold.Location = new System.Drawing.Point(184, 22);
            this.numThreshold.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numThreshold.Name = "numThreshold";
            this.numThreshold.Size = new System.Drawing.Size(78, 25);
            this.numThreshold.TabIndex = 1;
            this.numThreshold.Value = new decimal(new int[] { 75, 0, 0, 131072 });

            this.chkMultiAngle.AutoSize = true;
            this.chkMultiAngle.Location = new System.Drawing.Point(14, 54);
            this.chkMultiAngle.Name = "chkMultiAngle";
            this.chkMultiAngle.Size = new System.Drawing.Size(150, 19);
            this.chkMultiAngle.TabIndex = 2;
            this.chkMultiAngle.Text = "啟用多角度搜尋（§4）";
            this.chkMultiAngle.UseVisualStyleBackColor = true;
            this.chkMultiAngle.CheckedChanged += new System.EventHandler(this.chkMultiAngle_CheckedChanged);

            this.lblAngleRange.AutoSize = true;
            this.lblAngleRange.Enabled = false;
            this.lblAngleRange.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAngleRange.Location = new System.Drawing.Point(32, 82);
            this.lblAngleRange.Name = "lblAngleRange";
            this.lblAngleRange.Size = new System.Drawing.Size(48, 15);
            this.lblAngleRange.TabIndex = 3;
            this.lblAngleRange.Text = "範圍 ±";

            this.numAngleRange.Enabled = false;
            this.numAngleRange.Location = new System.Drawing.Point(88, 78);
            this.numAngleRange.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            this.numAngleRange.Name = "numAngleRange";
            this.numAngleRange.Size = new System.Drawing.Size(50, 25);
            this.numAngleRange.TabIndex = 4;
            this.numAngleRange.Value = new decimal(new int[] { 10, 0, 0, 0 });

            this.lblAngleStep.AutoSize = true;
            this.lblAngleStep.Enabled = false;
            this.lblAngleStep.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAngleStep.Location = new System.Drawing.Point(146, 82);
            this.lblAngleStep.Name = "lblAngleStep";
            this.lblAngleStep.Size = new System.Drawing.Size(30, 15);
            this.lblAngleStep.TabIndex = 5;
            this.lblAngleStep.Text = "步距";

            this.numAngleStep.DecimalPlaces = 1;
            this.numAngleStep.Enabled = false;
            this.numAngleStep.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            this.numAngleStep.Location = new System.Drawing.Point(190, 78);
            this.numAngleStep.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            this.numAngleStep.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            this.numAngleStep.Name = "numAngleStep";
            this.numAngleStep.Size = new System.Drawing.Size(50, 25);
            this.numAngleStep.TabIndex = 6;
            this.numAngleStep.Value = new decimal(new int[] { 20, 0, 0, 65536 });

            this.btnLocate.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLocate.Location = new System.Drawing.Point(14, 106);
            this.btnLocate.Name = "btnLocate";
            this.btnLocate.Size = new System.Drawing.Size(250, 30);
            this.btnLocate.TabIndex = 7;
            this.btnLocate.Text = "▶ 執行定位";
            this.btnLocate.UseVisualStyleBackColor = true;
            this.btnLocate.Click += new System.EventHandler(this.btnLocate_Click);

            this.grpLocate.Controls.Add(this.btnLocate);
            this.grpLocate.Controls.Add(this.numAngleStep);
            this.grpLocate.Controls.Add(this.lblAngleStep);
            this.grpLocate.Controls.Add(this.numAngleRange);
            this.grpLocate.Controls.Add(this.lblAngleRange);
            this.grpLocate.Controls.Add(this.chkMultiAngle);
            this.grpLocate.Controls.Add(this.numThreshold);
            this.grpLocate.Controls.Add(this.lblThreshold);

            // ── ④ P-006 對照實驗 ──────────────────────────────────
            this.grpCompare.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCompare.Location = new System.Drawing.Point(10, 392);
            this.grpCompare.Name = "grpCompare";
            this.grpCompare.Size = new System.Drawing.Size(276, 88);
            this.grpCompare.TabIndex = 3;
            this.grpCompare.TabStop = false;
            this.grpCompare.Text = "④ P-006 對照實驗";

            this.lblCompareDesc.AutoSize = true;
            this.lblCompareDesc.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblCompareDesc.Location = new System.Drawing.Point(14, 24);
            this.lblCompareDesc.Name = "lblCompareDesc";
            this.lblCompareDesc.Size = new System.Drawing.Size(240, 15);
            this.lblCompareDesc.TabIndex = 0;
            this.lblCompareDesc.Text = "同時跑「搜尋區 = 模板」與「外擴 margin」";

            this.btnCompare.Location = new System.Drawing.Point(14, 48);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(250, 30);
            this.btnCompare.TabIndex = 1;
            this.btnCompare.Text = "▶ 執行對照實驗";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);

            this.grpCompare.Controls.Add(this.btnCompare);
            this.grpCompare.Controls.Add(this.lblCompareDesc);

            // ── ⑤ 偏移掃描驗收 ────────────────────────────────────
            this.grpSweep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSweep.Location = new System.Drawing.Point(10, 488);
            this.grpSweep.Name = "grpSweep";
            this.grpSweep.Size = new System.Drawing.Size(276, 112);
            this.grpSweep.TabIndex = 4;
            this.grpSweep.TabStop = false;
            this.grpSweep.Text = "⑤ 偏移掃描驗收";

            this.lblSweepCap.AutoSize = true;
            this.lblSweepCap.Location = new System.Drawing.Point(14, 26);
            this.lblSweepCap.Name = "lblSweepCap";
            this.lblSweepCap.Size = new System.Drawing.Size(80, 15);
            this.lblSweepCap.TabIndex = 0;
            this.lblSweepCap.Text = "掃描範圍 ±";

            this.numSweepRange.Location = new System.Drawing.Point(100, 22);
            this.numSweepRange.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            this.numSweepRange.Name = "numSweepRange";
            this.numSweepRange.Size = new System.Drawing.Size(52, 25);
            this.numSweepRange.TabIndex = 1;
            this.numSweepRange.Value = new decimal(new int[] { 60, 0, 0, 0 });

            this.lblSweepStep.AutoSize = true;
            this.lblSweepStep.Location = new System.Drawing.Point(162, 26);
            this.lblSweepStep.Name = "lblSweepStep";
            this.lblSweepStep.Size = new System.Drawing.Size(30, 15);
            this.lblSweepStep.TabIndex = 2;
            this.lblSweepStep.Text = "步距";

            this.numSweepStep.Location = new System.Drawing.Point(206, 22);
            this.numSweepStep.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numSweepStep.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numSweepStep.Name = "numSweepStep";
            this.numSweepStep.Size = new System.Drawing.Size(52, 25);
            this.numSweepStep.TabIndex = 3;
            this.numSweepStep.Value = new decimal(new int[] { 20, 0, 0, 0 });

            this.lblSweepDesc.AutoSize = true;
            this.lblSweepDesc.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblSweepDesc.Location = new System.Drawing.Point(14, 52);
            this.lblSweepDesc.Name = "lblSweepDesc";
            this.lblSweepDesc.Size = new System.Drawing.Size(230, 15);
            this.lblSweepDesc.TabIndex = 4;
            this.lblSweepDesc.Text = "自動改變偏移，驗證座標真的跟著動";

            this.btnSweep.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSweep.Location = new System.Drawing.Point(14, 72);
            this.btnSweep.Name = "btnSweep";
            this.btnSweep.Size = new System.Drawing.Size(250, 30);
            this.btnSweep.TabIndex = 5;
            this.btnSweep.Text = "▶ 偏移掃描驗收";
            this.btnSweep.UseVisualStyleBackColor = true;
            this.btnSweep.Click += new System.EventHandler(this.btnSweep_Click);

            this.grpSweep.Controls.Add(this.btnSweep);
            this.grpSweep.Controls.Add(this.lblSweepDesc);
            this.grpSweep.Controls.Add(this.numSweepStep);
            this.grpSweep.Controls.Add(this.lblSweepStep);
            this.grpSweep.Controls.Add(this.numSweepRange);
            this.grpSweep.Controls.Add(this.lblSweepCap);

            // ── 結果摘要 + 判定燈 ─────────────────────────────────
            this.lblMatchDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMatchDetail.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblMatchDetail.Location = new System.Drawing.Point(24, 614);
            this.lblMatchDetail.Name = "lblMatchDetail";
            this.lblMatchDetail.Size = new System.Drawing.Size(250, 40);
            this.lblMatchDetail.TabIndex = 5;
            this.lblMatchDetail.Text = "匹配座標　—\r\n偏移量　　—";

            this.lblVerdict.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVerdict.BackColor = System.Drawing.SystemColors.Control;
            this.lblVerdict.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVerdict.Font = new System.Drawing.Font("Microsoft JhengHei UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblVerdict.ForeColor = System.Drawing.Color.Gray;
            this.lblVerdict.Location = new System.Drawing.Point(10, 658);
            this.lblVerdict.Name = "lblVerdict";
            this.lblVerdict.Size = new System.Drawing.Size(276, 44);
            this.lblVerdict.TabIndex = 6;
            this.lblVerdict.Text = "待定位";
            this.lblVerdict.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.pnlRight.Controls.Add(this.lblVerdict);
            this.pnlRight.Controls.Add(this.lblMatchDetail);
            this.pnlRight.Controls.Add(this.grpSweep);
            this.pnlRight.Controls.Add(this.grpCompare);
            this.pnlRight.Controls.Add(this.grpLocate);
            this.pnlRight.Controls.Add(this.grpSearch);
            this.pnlRight.Controls.Add(this.grpTeach);

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

            this.lblRefCap.AutoSize = true;
            this.lblRefCap.Location = new System.Drawing.Point(10, 8);
            this.lblRefCap.Name = "lblRefCap";
            this.lblRefCap.Size = new System.Drawing.Size(300, 15);
            this.lblRefCap.TabIndex = 0;
            this.lblRefCap.Text = "基準影像（教導用）　黃框 = 模板　綠框 = 搜尋區";

            this.picRef.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.picRef.BackColor = System.Drawing.Color.Black;
            this.picRef.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picRef.Location = new System.Drawing.Point(8, 26);
            this.picRef.Name = "picRef";
            this.picRef.Size = new System.Drawing.Size(428, 325);
            this.picRef.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picRef.TabIndex = 1;
            this.picRef.TabStop = false;

            this.lblTestCap.AutoSize = true;
            this.lblTestCap.Location = new System.Drawing.Point(450, 8);
            this.lblTestCap.Name = "lblTestCap";
            this.lblTestCap.Size = new System.Drawing.Size(300, 15);
            this.lblTestCap.TabIndex = 2;
            this.lblTestCap.Text = "測試影像";

            this.picTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.picTest.BackColor = System.Drawing.Color.Black;
            this.picTest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTest.Location = new System.Drawing.Point(448, 26);
            this.picTest.Name = "picTest";
            this.picTest.Size = new System.Drawing.Size(428, 325);
            this.picTest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picTest.TabIndex = 3;
            this.picTest.TabStop = false;

            this.pnlImages.Controls.Add(this.picTest);
            this.pnlImages.Controls.Add(this.lblTestCap);
            this.pnlImages.Controls.Add(this.picRef);
            this.pnlImages.Controls.Add(this.lblRefCap);

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
            this.lblLogCap.Text = "執行結果";

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
            this.Text = "第 9 章 模板匹配與定位 — TemplateMatching";

            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.grpTeach.ResumeLayout(false);
            this.grpTeach.PerformLayout();
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.grpLocate.ResumeLayout(false);
            this.grpLocate.PerformLayout();
            this.grpCompare.ResumeLayout(false);
            this.grpCompare.PerformLayout();
            this.grpSweep.ResumeLayout(false);
            this.grpSweep.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlImages.ResumeLayout(false);
            this.pnlImages.PerformLayout();
            this.pnlLog.ResumeLayout(false);
            this.pnlLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOffsetX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOffsetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTmplX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTmplY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTmplW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTmplH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAngleRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAngleStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSweepRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSweepStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picRef)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTest)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnGenTestImage;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Label lblSeparator;
        private System.Windows.Forms.Label lblOffsetCap;
        private System.Windows.Forms.Label lblDx;
        private System.Windows.Forms.NumericUpDown numOffsetX;
        private System.Windows.Forms.Label lblDy;
        private System.Windows.Forms.NumericUpDown numOffsetY;
        private System.Windows.Forms.Label lblAngleCap;
        private System.Windows.Forms.NumericUpDown numAngle;
        private System.Windows.Forms.Button btnResetOffset;
        private System.Windows.Forms.Label lblPitfall;

        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.GroupBox grpTeach;
        private System.Windows.Forms.Label lblTmplCap;
        private System.Windows.Forms.Label lblTx;
        private System.Windows.Forms.NumericUpDown numTmplX;
        private System.Windows.Forms.Label lblTy;
        private System.Windows.Forms.NumericUpDown numTmplY;
        private System.Windows.Forms.Label lblTw;
        private System.Windows.Forms.NumericUpDown numTmplW;
        private System.Windows.Forms.Label lblTh;
        private System.Windows.Forms.NumericUpDown numTmplH;
        private System.Windows.Forms.PictureBox picTemplate;
        private System.Windows.Forms.Button btnTeach;

        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Label lblMarginCap;
        private System.Windows.Forms.Label lblMx;
        private System.Windows.Forms.NumericUpDown numMarginX;
        private System.Windows.Forms.Label lblMy;
        private System.Windows.Forms.NumericUpDown numMarginY;
        private System.Windows.Forms.Label lblResultMapInfo;
        private System.Windows.Forms.Label lblSearchRectInfo;

        private System.Windows.Forms.GroupBox grpLocate;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.NumericUpDown numThreshold;
        private System.Windows.Forms.CheckBox chkMultiAngle;
        private System.Windows.Forms.Label lblAngleRange;
        private System.Windows.Forms.NumericUpDown numAngleRange;
        private System.Windows.Forms.Label lblAngleStep;
        private System.Windows.Forms.NumericUpDown numAngleStep;
        private System.Windows.Forms.Button btnLocate;

        private System.Windows.Forms.GroupBox grpCompare;
        private System.Windows.Forms.Label lblCompareDesc;
        private System.Windows.Forms.Button btnCompare;

        private System.Windows.Forms.GroupBox grpSweep;
        private System.Windows.Forms.Label lblSweepCap;
        private System.Windows.Forms.NumericUpDown numSweepRange;
        private System.Windows.Forms.Label lblSweepStep;
        private System.Windows.Forms.NumericUpDown numSweepStep;
        private System.Windows.Forms.Label lblSweepDesc;
        private System.Windows.Forms.Button btnSweep;

        private System.Windows.Forms.Label lblMatchDetail;
        private System.Windows.Forms.Label lblVerdict;

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlImages;
        private System.Windows.Forms.Label lblRefCap;
        private System.Windows.Forms.PictureBox picRef;
        private System.Windows.Forms.Label lblTestCap;
        private System.Windows.Forms.PictureBox picTest;
        private System.Windows.Forms.Panel pnlLog;
        private System.Windows.Forms.Label lblLogCap;
        private System.Windows.Forms.TextBox txtLog;
    }
}

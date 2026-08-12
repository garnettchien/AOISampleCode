namespace FeatureDetection
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

        // 版面座標對應 mockup\07_特徵檢測_UI_mockup.html，ClientSize = 1184 × 761。
        //
        // 停靠順序提醒（第 20 章）：Controls.Add 的索引愈大愈先停靠，
        // 所以 Fill 的控制項要「最先 Add」（索引 0、最後停靠、吃剩餘空間）。
        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.btnGenTestImage = new System.Windows.Forms.Button();
            this.lblSeparator = new System.Windows.Forms.Label();
            this.lblRoiCap = new System.Windows.Forms.Label();
            this.lblRoiX = new System.Windows.Forms.Label();
            this.numRoiX = new System.Windows.Forms.NumericUpDown();
            this.lblRoiY = new System.Windows.Forms.Label();
            this.numRoiY = new System.Windows.Forms.NumericUpDown();
            this.lblRoiW = new System.Windows.Forms.Label();
            this.numRoiW = new System.Windows.Forms.NumericUpDown();
            this.lblRoiH = new System.Windows.Forms.Label();
            this.numRoiH = new System.Windows.Forms.NumericUpDown();
            this.btnResetRoi = new System.Windows.Forms.Button();
            this.lblPitfall = new System.Windows.Forms.Label();

            this.pnlRight = new System.Windows.Forms.Panel();
            this.grpRoi = new System.Windows.Forms.GroupBox();
            this.btnExtractRoi = new System.Windows.Forms.Button();
            this.lblRoiInfo = new System.Windows.Forms.Label();
            this.grpBlob = new System.Windows.Forms.GroupBox();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.numThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblMinArea = new System.Windows.Forms.Label();
            this.numMinArea = new System.Windows.Forms.NumericUpDown();
            this.lblMaxArea = new System.Windows.Forms.Label();
            this.numMaxArea = new System.Windows.Forms.NumericUpDown();
            this.btnBlob = new System.Windows.Forms.Button();
            this.grpEdge = new System.Windows.Forms.GroupBox();
            this.lblTh1 = new System.Windows.Forms.Label();
            this.numTh1 = new System.Windows.Forms.NumericUpDown();
            this.lblTh2 = new System.Windows.Forms.Label();
            this.numTh2 = new System.Windows.Forms.NumericUpDown();
            this.lblEdgeHint = new System.Windows.Forms.Label();
            this.btnCanny = new System.Windows.Forms.Button();
            this.grpPixel = new System.Windows.Forms.GroupBox();
            this.lblLow = new System.Windows.Forms.Label();
            this.numLow = new System.Windows.Forms.NumericUpDown();
            this.lblHigh = new System.Windows.Forms.Label();
            this.numHigh = new System.Windows.Forms.NumericUpDown();
            this.lblMaxCount = new System.Windows.Forms.Label();
            this.numMaxCount = new System.Windows.Forms.NumericUpDown();
            this.btnCountPixels = new System.Windows.Forms.Button();
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
            this.grpRoi.SuspendLayout();
            this.grpBlob.SuspendLayout();
            this.grpEdge.SuspendLayout();
            this.grpPixel.SuspendLayout();
            this.grpFlow.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlImages.SuspendLayout();
            this.pnlLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTh1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTh2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHigh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picResult)).BeginInit();
            this.SuspendLayout();

            // ═══════════════ pnlTop（Dock = Top, H = 46）═══════════════
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1184, 46);
            this.pnlTop.TabIndex = 0;

            this.btnLoadImage.Location = new System.Drawing.Point(12, 9);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(92, 28);
            this.btnLoadImage.TabIndex = 0;
            this.btnLoadImage.Text = "載入影像";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);

            this.btnGenTestImage.Location = new System.Drawing.Point(110, 9);
            this.btnGenTestImage.Name = "btnGenTestImage";
            this.btnGenTestImage.Size = new System.Drawing.Size(104, 28);
            this.btnGenTestImage.TabIndex = 1;
            this.btnGenTestImage.Text = "產生測試圖";
            this.btnGenTestImage.UseVisualStyleBackColor = true;
            this.btnGenTestImage.Click += new System.EventHandler(this.btnGenTestImage_Click);

            this.lblSeparator.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblSeparator.Location = new System.Drawing.Point(224, 11);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new System.Drawing.Size(1, 24);
            this.lblSeparator.TabIndex = 2;

            this.lblRoiCap.AutoSize = true;
            this.lblRoiCap.Location = new System.Drawing.Point(238, 17);
            this.lblRoiCap.Name = "lblRoiCap";
            this.lblRoiCap.Size = new System.Drawing.Size(62, 15);
            this.lblRoiCap.TabIndex = 3;
            this.lblRoiCap.Text = "ROI 座標：";

            this.lblRoiX.AutoSize = true;
            this.lblRoiX.Location = new System.Drawing.Point(306, 17);
            this.lblRoiX.Name = "lblRoiX";
            this.lblRoiX.Size = new System.Drawing.Size(14, 15);
            this.lblRoiX.TabIndex = 4;
            this.lblRoiX.Text = "X";

            this.numRoiX.Location = new System.Drawing.Point(320, 12);
            this.numRoiX.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.numRoiX.Name = "numRoiX";
            this.numRoiX.Size = new System.Drawing.Size(58, 25);
            this.numRoiX.TabIndex = 5;
            this.numRoiX.Value = new decimal(new int[] { 80, 0, 0, 0 });

            this.lblRoiY.AutoSize = true;
            this.lblRoiY.Location = new System.Drawing.Point(388, 17);
            this.lblRoiY.Name = "lblRoiY";
            this.lblRoiY.Size = new System.Drawing.Size(14, 15);
            this.lblRoiY.TabIndex = 6;
            this.lblRoiY.Text = "Y";

            this.numRoiY.Location = new System.Drawing.Point(402, 12);
            this.numRoiY.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.numRoiY.Name = "numRoiY";
            this.numRoiY.Size = new System.Drawing.Size(58, 25);
            this.numRoiY.TabIndex = 7;
            this.numRoiY.Value = new decimal(new int[] { 60, 0, 0, 0 });

            this.lblRoiW.AutoSize = true;
            this.lblRoiW.Location = new System.Drawing.Point(470, 17);
            this.lblRoiW.Name = "lblRoiW";
            this.lblRoiW.Size = new System.Drawing.Size(16, 15);
            this.lblRoiW.TabIndex = 8;
            this.lblRoiW.Text = "W";

            this.numRoiW.Location = new System.Drawing.Point(486, 12);
            this.numRoiW.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.numRoiW.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numRoiW.Name = "numRoiW";
            this.numRoiW.Size = new System.Drawing.Size(58, 25);
            this.numRoiW.TabIndex = 9;
            this.numRoiW.Value = new decimal(new int[] { 400, 0, 0, 0 });

            this.lblRoiH.AutoSize = true;
            this.lblRoiH.Location = new System.Drawing.Point(554, 17);
            this.lblRoiH.Name = "lblRoiH";
            this.lblRoiH.Size = new System.Drawing.Size(14, 15);
            this.lblRoiH.TabIndex = 10;
            this.lblRoiH.Text = "H";

            this.numRoiH.Location = new System.Drawing.Point(570, 12);
            this.numRoiH.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            this.numRoiH.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numRoiH.Name = "numRoiH";
            this.numRoiH.Size = new System.Drawing.Size(58, 25);
            this.numRoiH.TabIndex = 11;
            this.numRoiH.Value = new decimal(new int[] { 300, 0, 0, 0 });

            this.btnResetRoi.Location = new System.Drawing.Point(638, 9);
            this.btnResetRoi.Name = "btnResetRoi";
            this.btnResetRoi.Size = new System.Drawing.Size(84, 28);
            this.btnResetRoi.TabIndex = 12;
            this.btnResetRoi.Text = "重設 ROI";
            this.btnResetRoi.UseVisualStyleBackColor = true;
            this.btnResetRoi.Click += new System.EventHandler(this.btnResetRoi_Click);

            this.lblPitfall.AutoSize = true;
            this.lblPitfall.ForeColor = System.Drawing.Color.FromArgb(179, 107, 0);
            this.lblPitfall.Location = new System.Drawing.Point(742, 17);
            this.lblPitfall.Name = "lblPitfall";
            this.lblPitfall.Size = new System.Drawing.Size(400, 15);
            this.lblPitfall.TabIndex = 13;
            // 文字長度受限於面板寬度（AutoSize 展開後不可超過 ClientSize.Width = 1184）。
            // 完整的 P-001 說明放在 ① ROI 的執行結果裡，這裡只留常駐提醒。
            this.lblPitfall.Text = "⚠ P-001：固定座標 ROI 會漂移 → 須搭第 9 章定位修正";

            this.pnlTop.Controls.Add(this.lblPitfall);
            this.pnlTop.Controls.Add(this.btnResetRoi);
            this.pnlTop.Controls.Add(this.numRoiH);
            this.pnlTop.Controls.Add(this.lblRoiH);
            this.pnlTop.Controls.Add(this.numRoiW);
            this.pnlTop.Controls.Add(this.lblRoiW);
            this.pnlTop.Controls.Add(this.numRoiY);
            this.pnlTop.Controls.Add(this.lblRoiY);
            this.pnlTop.Controls.Add(this.numRoiX);
            this.pnlTop.Controls.Add(this.lblRoiX);
            this.pnlTop.Controls.Add(this.lblRoiCap);
            this.pnlTop.Controls.Add(this.lblSeparator);
            this.pnlTop.Controls.Add(this.btnGenTestImage);
            this.pnlTop.Controls.Add(this.btnLoadImage);

            // ═══════════════ pnlRight（Dock = Right, W = 296）═══════════════
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(888, 46);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(296, 715);
            this.pnlRight.TabIndex = 1;

            // ── ① ROI ─────────────────────────────────────────────
            this.grpRoi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpRoi.Location = new System.Drawing.Point(10, 10);
            this.grpRoi.Name = "grpRoi";
            this.grpRoi.Size = new System.Drawing.Size(276, 72);
            this.grpRoi.TabIndex = 0;
            this.grpRoi.TabStop = false;
            this.grpRoi.Text = "① ROI 感興趣區域";

            this.btnExtractRoi.Location = new System.Drawing.Point(12, 26);
            this.btnExtractRoi.Name = "btnExtractRoi";
            this.btnExtractRoi.Size = new System.Drawing.Size(120, 28);
            this.btnExtractRoi.TabIndex = 0;
            this.btnExtractRoi.Text = "取出 ROI 子影像";
            this.btnExtractRoi.UseVisualStyleBackColor = true;
            this.btnExtractRoi.Click += new System.EventHandler(this.btnExtractRoi_Click);

            this.lblRoiInfo.AutoSize = true;
            this.lblRoiInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblRoiInfo.Location = new System.Drawing.Point(142, 33);
            this.lblRoiInfo.Name = "lblRoiInfo";
            this.lblRoiInfo.Size = new System.Drawing.Size(15, 15);
            this.lblRoiInfo.TabIndex = 1;
            this.lblRoiInfo.Text = "—";

            this.grpRoi.Controls.Add(this.lblRoiInfo);
            this.grpRoi.Controls.Add(this.btnExtractRoi);

            // ── ② Blob ────────────────────────────────────────────
            this.grpBlob.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBlob.Location = new System.Drawing.Point(10, 94);
            this.grpBlob.Name = "grpBlob";
            this.grpBlob.Size = new System.Drawing.Size(276, 152);
            this.grpBlob.TabIndex = 1;
            this.grpBlob.TabStop = false;
            this.grpBlob.Text = "② Blob 連通域分析";

            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(14, 30);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(70, 15);
            this.lblThreshold.TabIndex = 0;
            this.lblThreshold.Text = "二值化閾值";

            this.numThreshold.Location = new System.Drawing.Point(184, 25);
            this.numThreshold.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            this.numThreshold.Name = "numThreshold";
            this.numThreshold.Size = new System.Drawing.Size(78, 25);
            this.numThreshold.TabIndex = 1;
            this.numThreshold.Value = new decimal(new int[] { 128, 0, 0, 0 });

            this.lblMinArea.AutoSize = true;
            this.lblMinArea.Location = new System.Drawing.Point(14, 59);
            this.lblMinArea.Name = "lblMinArea";
            this.lblMinArea.Size = new System.Drawing.Size(110, 15);
            this.lblMinArea.TabIndex = 2;
            this.lblMinArea.Text = "最小面積 minArea";

            this.numMinArea.Location = new System.Drawing.Point(184, 54);
            this.numMinArea.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numMinArea.Name = "numMinArea";
            this.numMinArea.Size = new System.Drawing.Size(78, 25);
            this.numMinArea.TabIndex = 3;
            // 預設 30 而非教材舉例的 20：本測試圖實測後，雜訊(21px)與真實缺陷(37px)的
            // 分界線就落在這裡。閾值要量過自己的產品才能定，不是照抄書上的數字。
            this.numMinArea.Value = new decimal(new int[] { 30, 0, 0, 0 });

            this.lblMaxArea.AutoSize = true;
            this.lblMaxArea.Location = new System.Drawing.Point(14, 88);
            this.lblMaxArea.Name = "lblMaxArea";
            this.lblMaxArea.Size = new System.Drawing.Size(113, 15);
            this.lblMaxArea.TabIndex = 4;
            this.lblMaxArea.Text = "最大面積 maxArea";

            this.numMaxArea.Location = new System.Drawing.Point(184, 83);
            this.numMaxArea.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numMaxArea.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numMaxArea.Name = "numMaxArea";
            this.numMaxArea.Size = new System.Drawing.Size(78, 25);
            this.numMaxArea.TabIndex = 5;
            this.numMaxArea.Value = new decimal(new int[] { 5000, 0, 0, 0 });

            this.btnBlob.Location = new System.Drawing.Point(14, 112);
            this.btnBlob.Name = "btnBlob";
            this.btnBlob.Size = new System.Drawing.Size(248, 30);
            this.btnBlob.TabIndex = 6;
            this.btnBlob.Text = "執行 Blob 分析";
            this.btnBlob.UseVisualStyleBackColor = true;
            this.btnBlob.Click += new System.EventHandler(this.btnBlob_Click);

            this.grpBlob.Controls.Add(this.btnBlob);
            this.grpBlob.Controls.Add(this.numMaxArea);
            this.grpBlob.Controls.Add(this.lblMaxArea);
            this.grpBlob.Controls.Add(this.numMinArea);
            this.grpBlob.Controls.Add(this.lblMinArea);
            this.grpBlob.Controls.Add(this.numThreshold);
            this.grpBlob.Controls.Add(this.lblThreshold);

            // ── ③ Canny ───────────────────────────────────────────
            this.grpEdge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpEdge.Location = new System.Drawing.Point(10, 258);
            this.grpEdge.Name = "grpEdge";
            this.grpEdge.Size = new System.Drawing.Size(276, 142);
            this.grpEdge.TabIndex = 2;
            this.grpEdge.TabStop = false;
            this.grpEdge.Text = "③ Canny 邊緣檢測";

            this.lblTh1.AutoSize = true;
            this.lblTh1.Location = new System.Drawing.Point(14, 30);
            this.lblTh1.Name = "lblTh1";
            this.lblTh1.Size = new System.Drawing.Size(107, 15);
            this.lblTh1.TabIndex = 0;
            this.lblTh1.Text = "低閾值 Threshold1";

            this.numTh1.Location = new System.Drawing.Point(184, 25);
            this.numTh1.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numTh1.Name = "numTh1";
            this.numTh1.Size = new System.Drawing.Size(78, 25);
            this.numTh1.TabIndex = 1;
            this.numTh1.Value = new decimal(new int[] { 50, 0, 0, 0 });

            this.lblTh2.AutoSize = true;
            this.lblTh2.Location = new System.Drawing.Point(14, 59);
            this.lblTh2.Name = "lblTh2";
            this.lblTh2.Size = new System.Drawing.Size(107, 15);
            this.lblTh2.TabIndex = 2;
            this.lblTh2.Text = "高閾值 Threshold2";

            this.numTh2.Location = new System.Drawing.Point(184, 54);
            this.numTh2.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numTh2.Name = "numTh2";
            this.numTh2.Size = new System.Drawing.Size(78, 25);
            this.numTh2.TabIndex = 3;
            this.numTh2.Value = new decimal(new int[] { 150, 0, 0, 0 });

            this.lblEdgeHint.AutoSize = true;
            this.lblEdgeHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblEdgeHint.Location = new System.Drawing.Point(14, 86);
            this.lblEdgeHint.Name = "lblEdgeHint";
            this.lblEdgeHint.Size = new System.Drawing.Size(180, 15);
            this.lblEdgeHint.TabIndex = 4;
            this.lblEdgeHint.Text = "經驗值：Th2 約為 Th1 的 2～3 倍";

            this.btnCanny.Location = new System.Drawing.Point(14, 102);
            this.btnCanny.Name = "btnCanny";
            this.btnCanny.Size = new System.Drawing.Size(248, 30);
            this.btnCanny.TabIndex = 5;
            this.btnCanny.Text = "執行 Canny 邊緣檢測";
            this.btnCanny.UseVisualStyleBackColor = true;
            this.btnCanny.Click += new System.EventHandler(this.btnCanny_Click);

            this.grpEdge.Controls.Add(this.btnCanny);
            this.grpEdge.Controls.Add(this.lblEdgeHint);
            this.grpEdge.Controls.Add(this.numTh2);
            this.grpEdge.Controls.Add(this.lblTh2);
            this.grpEdge.Controls.Add(this.numTh1);
            this.grpEdge.Controls.Add(this.lblTh1);

            // ── ④ 像素計數 ────────────────────────────────────────
            this.grpPixel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPixel.Location = new System.Drawing.Point(10, 412);
            this.grpPixel.Name = "grpPixel";
            this.grpPixel.Size = new System.Drawing.Size(276, 152);
            this.grpPixel.TabIndex = 3;
            this.grpPixel.TabStop = false;
            this.grpPixel.Text = "④ 像素計數";

            this.lblLow.AutoSize = true;
            this.lblLow.Location = new System.Drawing.Point(14, 30);
            this.lblLow.Name = "lblLow";
            this.lblLow.Size = new System.Drawing.Size(85, 15);
            this.lblLow.TabIndex = 0;
            this.lblLow.Text = "亮度下限 Low";

            this.numLow.Location = new System.Drawing.Point(184, 25);
            this.numLow.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            this.numLow.Name = "numLow";
            this.numLow.Size = new System.Drawing.Size(78, 25);
            this.numLow.TabIndex = 1;
            this.numLow.Value = new decimal(new int[] { 0, 0, 0, 0 });

            this.lblHigh.AutoSize = true;
            this.lblHigh.Location = new System.Drawing.Point(14, 59);
            this.lblHigh.Name = "lblHigh";
            this.lblHigh.Size = new System.Drawing.Size(88, 15);
            this.lblHigh.TabIndex = 2;
            this.lblHigh.Text = "亮度上限 High";

            this.numHigh.Location = new System.Drawing.Point(184, 54);
            this.numHigh.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            this.numHigh.Name = "numHigh";
            this.numHigh.Size = new System.Drawing.Size(78, 25);
            this.numHigh.TabIndex = 3;
            this.numHigh.Value = new decimal(new int[] { 60, 0, 0, 0 });

            this.lblMaxCount.AutoSize = true;
            this.lblMaxCount.Location = new System.Drawing.Point(14, 88);
            this.lblMaxCount.Name = "lblMaxCount";
            this.lblMaxCount.Size = new System.Drawing.Size(93, 15);
            this.lblMaxCount.TabIndex = 4;
            this.lblMaxCount.Text = "允許上限 (px)";

            this.numMaxCount.Location = new System.Drawing.Point(184, 83);
            this.numMaxCount.Maximum = new decimal(new int[] { 9999999, 0, 0, 0 });
            this.numMaxCount.Name = "numMaxCount";
            this.numMaxCount.Size = new System.Drawing.Size(78, 25);
            this.numMaxCount.TabIndex = 5;
            this.numMaxCount.Value = new decimal(new int[] { 500, 0, 0, 0 });

            this.btnCountPixels.Location = new System.Drawing.Point(14, 112);
            this.btnCountPixels.Name = "btnCountPixels";
            this.btnCountPixels.Size = new System.Drawing.Size(248, 30);
            this.btnCountPixels.TabIndex = 6;
            this.btnCountPixels.Text = "執行像素計數";
            this.btnCountPixels.UseVisualStyleBackColor = true;
            this.btnCountPixels.Click += new System.EventHandler(this.btnCountPixels_Click);

            this.grpPixel.Controls.Add(this.btnCountPixels);
            this.grpPixel.Controls.Add(this.numMaxCount);
            this.grpPixel.Controls.Add(this.lblMaxCount);
            this.grpPixel.Controls.Add(this.numHigh);
            this.grpPixel.Controls.Add(this.lblHigh);
            this.grpPixel.Controls.Add(this.numLow);
            this.grpPixel.Controls.Add(this.lblLow);

            // ── ⑤ 完整流程 ────────────────────────────────────────
            this.grpFlow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFlow.Location = new System.Drawing.Point(10, 576);
            this.grpFlow.Name = "grpFlow";
            this.grpFlow.Size = new System.Drawing.Size(276, 80);
            this.grpFlow.TabIndex = 4;
            this.grpFlow.TabStop = false;
            this.grpFlow.Text = "⑤ 完整流程（教材 §5）";

            this.lblFlowDesc.AutoSize = true;
            this.lblFlowDesc.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblFlowDesc.Location = new System.Drawing.Point(14, 26);
            this.lblFlowDesc.Name = "lblFlowDesc";
            this.lblFlowDesc.Size = new System.Drawing.Size(240, 15);
            this.lblFlowDesc.TabIndex = 0;
            this.lblFlowDesc.Text = "ROI → 高斯 → 二值化 → Open → Blob → 判定";

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
            this.lblVerdict.Location = new System.Drawing.Point(10, 666);
            this.lblVerdict.Name = "lblVerdict";
            this.lblVerdict.Size = new System.Drawing.Size(276, 38);
            this.lblVerdict.TabIndex = 5;
            this.lblVerdict.Text = "待檢測";
            this.lblVerdict.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.pnlRight.Controls.Add(this.lblVerdict);
            this.pnlRight.Controls.Add(this.grpFlow);
            this.pnlRight.Controls.Add(this.grpPixel);
            this.pnlRight.Controls.Add(this.grpEdge);
            this.pnlRight.Controls.Add(this.grpBlob);
            this.pnlRight.Controls.Add(this.grpRoi);

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
            this.lblMainCap.Size = new System.Drawing.Size(190, 15);
            this.lblMainCap.TabIndex = 0;
            this.lblMainCap.Text = "原始影像 ＋ ROI／檢測標記";

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
            this.lblResultCap.Size = new System.Drawing.Size(60, 15);
            this.lblResultCap.TabIndex = 2;
            this.lblResultCap.Text = "處理結果";

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
            this.lblLogCap.Text = "檢測結果";

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
            this.Text = "第 7 章 特徵檢測 — FeatureDetection";

            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.grpRoi.ResumeLayout(false);
            this.grpRoi.PerformLayout();
            this.grpBlob.ResumeLayout(false);
            this.grpBlob.PerformLayout();
            this.grpEdge.ResumeLayout(false);
            this.grpEdge.PerformLayout();
            this.grpPixel.ResumeLayout(false);
            this.grpPixel.PerformLayout();
            this.grpFlow.ResumeLayout(false);
            this.grpFlow.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlImages.ResumeLayout(false);
            this.pnlImages.PerformLayout();
            this.pnlLog.ResumeLayout(false);
            this.pnlLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTh1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTh2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHigh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picResult)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Button btnGenTestImage;
        private System.Windows.Forms.Label lblSeparator;
        private System.Windows.Forms.Label lblRoiCap;
        private System.Windows.Forms.Label lblRoiX;
        private System.Windows.Forms.NumericUpDown numRoiX;
        private System.Windows.Forms.Label lblRoiY;
        private System.Windows.Forms.NumericUpDown numRoiY;
        private System.Windows.Forms.Label lblRoiW;
        private System.Windows.Forms.NumericUpDown numRoiW;
        private System.Windows.Forms.Label lblRoiH;
        private System.Windows.Forms.NumericUpDown numRoiH;
        private System.Windows.Forms.Button btnResetRoi;
        private System.Windows.Forms.Label lblPitfall;

        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.GroupBox grpRoi;
        private System.Windows.Forms.Button btnExtractRoi;
        private System.Windows.Forms.Label lblRoiInfo;
        private System.Windows.Forms.GroupBox grpBlob;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.NumericUpDown numThreshold;
        private System.Windows.Forms.Label lblMinArea;
        private System.Windows.Forms.NumericUpDown numMinArea;
        private System.Windows.Forms.Label lblMaxArea;
        private System.Windows.Forms.NumericUpDown numMaxArea;
        private System.Windows.Forms.Button btnBlob;
        private System.Windows.Forms.GroupBox grpEdge;
        private System.Windows.Forms.Label lblTh1;
        private System.Windows.Forms.NumericUpDown numTh1;
        private System.Windows.Forms.Label lblTh2;
        private System.Windows.Forms.NumericUpDown numTh2;
        private System.Windows.Forms.Label lblEdgeHint;
        private System.Windows.Forms.Button btnCanny;
        private System.Windows.Forms.GroupBox grpPixel;
        private System.Windows.Forms.Label lblLow;
        private System.Windows.Forms.NumericUpDown numLow;
        private System.Windows.Forms.Label lblHigh;
        private System.Windows.Forms.NumericUpDown numHigh;
        private System.Windows.Forms.Label lblMaxCount;
        private System.Windows.Forms.NumericUpDown numMaxCount;
        private System.Windows.Forms.Button btnCountPixels;
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

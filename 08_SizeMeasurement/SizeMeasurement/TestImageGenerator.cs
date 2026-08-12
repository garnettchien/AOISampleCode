using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace SizeMeasurement
{
    /// <summary>
    /// 產生合成測試圖，讓程式不需要準備影像檔就能跑。
    ///
    /// 本章需要「兩種圖」，這正好對應實務上的操作順序：
    ///   ① 標定圖：一片已知尺寸的標定板 → 用它算出 mm/px
    ///   ② 量測圖：真正要量的產品     → 套用①的比例尺換算成 mm
    /// 沒有①就沒有 mm 這個單位，②量出來的永遠只是像素——這是第 8 章的核心。
    ///
    /// 影像取 800 × 600（比第 7 章的 640 × 480 大），是為了放得下 500 px 的標定板，
    /// 好讓標定結果剛好對上教材 §1 的例子：10.000 mm / 500 px = 0.020 mm/px。
    /// </summary>
    public static class TestImageGenerator
    {
        public const int Width = 800;
        public const int Height = 600;

        // ── 灰階值 ──────────────────────────────────────────────────
        private const byte BackgroundGray = 40;    // 暗背景（背光式打光的典型畫面）
        private const byte TargetGray = 220;       // 標定板
        private const byte ComponentGray = 210;    // 待測元件與定位孔

        // ── 成像模擬 ────────────────────────────────────────────────
        // 順序刻意照物理現實：先「鏡頭模糊」再「感測器雜訊」。
        //   · 模糊發生在光學端（MTF、離焦），會讓邊緣變成有寬度的灰階過渡帶
        //     —— 沒有這個過渡帶，亞像素內插就無事可做。
        //   · 雜訊發生在感測端，加在模糊之後
        //     —— 它讓每條掃描線量到的值略有不同，§5 的標準差才有東西可算。
        private const int BlurKernelSize = 3;
        private const double NoiseSigma = 3.0;
        private const int NoiseSeed = 20260812;    // 固定種子：每次產生的圖完全相同，方便對照講解

        // ═══════════════════════════════════════════════════════════
        //  ① 標定圖
        // ═══════════════════════════════════════════════════════════

        /// <summary>標定板的已知實際尺寸（mm）。正方形，10.000 mm × 10.000 mm。</summary>
        public const double TargetKnownMm = 10.000;

        /// <summary>
        /// 標定板在影像中的像素尺寸：500 × 490。
        ///
        /// ⚠ Y 方向刻意比 X 少 10 px（差 2%）。這不是畫錯，是要示範教材 §1 的重點：
        ///   鏡頭畸變、感光元件的微小非方形、安裝傾斜，都會讓 X、Y 的 mm/px 不同。
        ///   量出來會是 scaleX = 0.020000、scaleY = 0.020408 mm/px。
        ///   若偷懶假設兩者相同，後面斜向距離就會系統性錯 2%。
        /// </summary>
        public const int TargetWidthPx = 500;
        public const int TargetHeightPx = 490;

        private static readonly Point TargetTopLeft =
            new Point((Width - TargetWidthPx) / 2, (Height - TargetHeightPx) / 2);   // (150, 55)

        /// <summary>標定板的預期位置（供 UI 畫搜尋範圍、決定掃描線位置）。</summary>
        public static Rectangle TargetArea
        {
            get { return new Rectangle(TargetTopLeft.X, TargetTopLeft.Y, TargetWidthPx, TargetHeightPx); }
        }

        /// <summary>
        /// 產生標定圖：暗背景上一個亮的正方形標定板。
        /// 呼叫端負責 Dispose 回傳的 Mat。
        /// </summary>
        public static Mat CreateCalibrationTarget()
        {
            Mat img = NewBackground();
            try
            {
                FillRect(img, TargetArea, TargetGray);
                ApplyLensBlur(img);
                AddSensorNoise(img);
                return img;
            }
            catch
            {
                // 例外路徑也要放掉已配置的 Mat（第 18 章：例外路徑漏放）
                img.Dispose();
                throw;
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  ② 量測圖
        // ═══════════════════════════════════════════════════════════

        /// <summary>待測元件的標稱寬度（mm）與公差（mm）。正式專案這兩個值來自工程圖，要進 Recipe。</summary>
        public const double ComponentNominalMm = 4.000;
        public const double ComponentToleranceMm = 0.050;

        /// <summary>良品的元件寬度：200 px × 0.020 mm/px = 4.000 mm，正中標稱值。</summary>
        public const int ComponentWidthOkPx = 200;

        /// <summary>不良品的元件寬度：206 px = 4.120 mm，超出 4.000 ± 0.050 的上限。</summary>
        public const int ComponentWidthNgPx = 206;

        public const int ComponentHeightPx = 120;
        private static readonly Point ComponentTopLeft = new Point(120, 150);

        /// <summary>
        /// 兩個定位孔的圓心：刻意相距 dx = 150 px、dy = 60 px。
        ///
        /// 這組數字是本章最重要的教學數字，因為 X、Y 比例尺不同：
        ///   正解（各自換 mm 再開根）：√((150×0.020000)² + (60×0.020408)²) = 3.2403 mm
        ///   錯法（先開根再乘 scaleX）：√(150² + 60²) × 0.020000        = 3.2311 mm
        /// 差 9.2 μm——公差 ±50 μm（帶寬 100 μm）的話，光這個錯誤就佔掉近一成公差帶。
        /// </summary>
        private static readonly Point Hole1Center = new Point(450, 200);
        private static readonly Point Hole2Center = new Point(600, 260);
        private const int HoleRadius = 18;
        private const int HoleSearchHalfSize = 40;

        /// <summary>孔 1 的搜尋範圍。形心法要求範圍內只有一個目標，所以框小一點。</summary>
        public static Rectangle Hole1SearchArea { get { return SearchAreaOf(Hole1Center); } }

        /// <summary>孔 2 的搜尋範圍。</summary>
        public static Rectangle Hole2SearchArea { get { return SearchAreaOf(Hole2Center); } }

        /// <summary>
        /// 量測 ROI：包住元件左右邊緣的掃描範圍。
        /// X 從 80 到 359，兩側都留了背景——掃描線一定要「從背景開始、在背景結束」，
        /// 否則找不到成對的邊緣。
        /// </summary>
        public static Rectangle MeasureRoi
        {
            get { return new Rectangle(80, 160, 280, 100); }
        }

        /// <summary>預設掃描線 Y 座標，落在元件的垂直中央。</summary>
        public const int DefaultScanY = 210;

        /// <summary>
        /// 產生量測圖：暗背景上一個亮的待測元件 + 兩個定位孔。
        /// 呼叫端負責 Dispose 回傳的 Mat。
        /// </summary>
        /// <param name="defective">true 產生寬度超出公差的不良品（206 px）。</param>
        public static Mat CreateMeasurementImage(bool defective)
        {
            Mat img = NewBackground();
            try
            {
                int w = defective ? ComponentWidthNgPx : ComponentWidthOkPx;
                FillRect(img, new Rectangle(ComponentTopLeft.X, ComponentTopLeft.Y, w, ComponentHeightPx),
                         ComponentGray);

                MCvScalar hole = new MCvScalar(ComponentGray);
                CvInvoke.Circle(img, Hole1Center, HoleRadius, hole, -1, LineType.EightConnected, 0);
                CvInvoke.Circle(img, Hole2Center, HoleRadius, hole, -1, LineType.EightConnected, 0);

                ApplyLensBlur(img);
                AddSensorNoise(img);
                return img;
            }
            catch
            {
                img.Dispose();
                throw;
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  UI 顯示用的說明文字
        // ═══════════════════════════════════════════════════════════

        public static string CalibrationDescription
        {
            get
            {
                return
                    "  尺寸        : " + Width + " × " + Height + ", 8-bit 灰階" + Environment.NewLine +
                    "  背景        : 灰階 " + BackgroundGray + Environment.NewLine +
                    "  標定板      : " + TargetWidthPx + " × " + TargetHeightPx + " px，灰階 " + TargetGray +
                                     "，已知實際尺寸 " + TargetKnownMm.ToString("F3") + " mm × " +
                                     TargetKnownMm.ToString("F3") + " mm" + Environment.NewLine +
                    "                → scaleX = " + (TargetKnownMm / TargetWidthPx).ToString("F6") + " mm/px" +
                                     "、scaleY = " + (TargetKnownMm / TargetHeightPx).ToString("F6") + " mm/px" + Environment.NewLine +
                    "  ⚠ Y 方向刻意比 X 少 10 px（差 2%），模擬鏡頭畸變／感光元件非方形／安裝傾斜。" + Environment.NewLine +
                    "    這就是教材 §1「X、Y 必須分開標定」的實證。" + Environment.NewLine +
                    "  成像模擬    : 高斯模糊 " + BlurKernelSize + "×" + BlurKernelSize +
                                     "（鏡頭 MTF，讓邊緣有灰階過渡帶）+ 高斯雜訊 σ=" + NoiseSigma.ToString("F1") +
                                     "（感測器）";
            }
        }

        public static string MeasurementDescription(bool defective)
        {
            int w = defective ? ComponentWidthNgPx : ComponentWidthOkPx;
            return
                "  尺寸        : " + Width + " × " + Height + ", 8-bit 灰階" + Environment.NewLine +
                "  背景        : 灰階 " + BackgroundGray + Environment.NewLine +
                "  待測元件    : " + w + " × " + ComponentHeightPx + " px，灰階 " + ComponentGray +
                                 "（" + (defective ? "不良品" : "良品") + "）" + Environment.NewLine +
                "                → 用 0.020 mm/px 換算 = " + (w * 0.020).ToString("F3") + " mm" +
                                 "，標稱 " + ComponentNominalMm.ToString("F3") +
                                 " ± " + ComponentToleranceMm.ToString("F3") + " mm" + Environment.NewLine +
                "  定位孔 ×2   : 半徑 " + HoleRadius + " px，圓心相距 dx = 150 px、dy = 60 px" + Environment.NewLine +
                "                → 正解 3.2402 mm；若誤用單一比例尺開根會算成 3.2310 mm（差 9.2 μm）" + Environment.NewLine +
                "  成像模擬    : 高斯模糊 " + BlurKernelSize + "×" + BlurKernelSize +
                                 " + 高斯雜訊 σ=" + NoiseSigma.ToString("F1") +
                                 "（雜訊讓每條掃描線的結果略有不同，§5 的標準差才有東西可算）";
        }

        // ═══════════════════════════════════════════════════════════
        //  內部工具
        // ═══════════════════════════════════════════════════════════

        /// <summary>配置一張填滿背景灰階的新影像。呼叫端負責 Dispose。</summary>
        private static Mat NewBackground()
        {
            Mat img = new Mat(Height, Width, DepthType.Cv8U, 1);
            try
            {
                img.SetTo(new MCvScalar(BackgroundGray), null);
                return img;
            }
            catch
            {
                img.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 畫一個填滿的矩形，並補償 OpenCV 的閉區間 off-by-one。
        ///
        /// ⚠ CvInvoke.Rectangle 的填滿模式是「閉區間」：
        ///   傳 Rectangle(120, 150, 200, 120) 實際會畫出 201 × 121 px，不是 200 × 120。
        ///   本範例要求量出來剛好 200.0 px 才對得上教材數字，所以寬高各減 1。
        ///   （第 7 章 README 也記錄過這個坑——它在做面積、尺寸判定時一定會咬人。）
        /// </summary>
        private static void FillRect(Mat img, Rectangle rect, byte gray)
        {
            Rectangle closed = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            CvInvoke.Rectangle(img, closed, new MCvScalar(gray), -1, LineType.EightConnected, 0);
        }

        /// <summary>模擬鏡頭成像：高斯模糊，讓理想的階梯邊緣變成有寬度的灰階過渡帶。</summary>
        private static void ApplyLensBlur(Mat img)
        {
            using (Mat blurred = new Mat())
            {
                CvInvoke.GaussianBlur(img, blurred, new Size(BlurKernelSize, BlurKernelSize),
                                      0, 0, BorderType.Default);
                blurred.CopyTo(img, null);
            }
        }

        /// <summary>
        /// 模擬感測器雜訊：對每個像素加一個常態分佈的擾動。
        ///
        /// 用 System.Random + Box-Muller 而不是 CvInvoke.Randn，是為了「固定種子」——
        /// 每次產生的圖必須完全相同，講解時的數字才對得起來。
        /// </summary>
        private static void AddSensorNoise(Mat img)
        {
            int count = Width * Height;
            byte[] buffer = new byte[count];

            // 新配置的 Mat 必定連續（IsContinuous），可以整塊搬進搬出
            Marshal.Copy(img.DataPointer, buffer, 0, count);

            Random rnd = new Random(NoiseSeed);
            for (int i = 0; i < count; i++)
            {
                // Box-Muller：把兩個均勻亂數轉成一個常態分佈的亂數
                double u1 = 1.0 - rnd.NextDouble();   // (0, 1]，避開 log(0)
                double u2 = rnd.NextDouble();
                double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

                int v = (int)Math.Round(buffer[i] + normal * NoiseSigma);
                if (v < 0) v = 0;
                else if (v > 255) v = 255;
                buffer[i] = (byte)v;
            }

            Marshal.Copy(buffer, 0, img.DataPointer, count);
        }

        private static Rectangle SearchAreaOf(Point center)
        {
            return new Rectangle(center.X - HoleSearchHalfSize, center.Y - HoleSearchHalfSize,
                                 HoleSearchHalfSize * 2, HoleSearchHalfSize * 2);
        }
    }
}

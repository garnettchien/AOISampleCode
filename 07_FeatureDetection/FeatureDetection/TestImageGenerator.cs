using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace FeatureDetection
{
    /// <summary>
    /// 產生合成測試圖，讓程式不需要準備影像檔就能跑。
    ///
    /// 這張圖是「刻意設計」的：每一個元素都對應第 7 章的一個教學點，
    /// 讓四個工具的效果都看得出來。詳見 Description 屬性。
    /// </summary>
    public static class TestImageGenerator
    {
        public const int Width = 640;
        public const int Height = 480;

        // 灰階值設定
        private const byte BackgroundGray = 90;    // 背景
        private const byte DefectGray = 235;       // 白色缺陷（高於二值化閾值 128）
        private const byte ContaminationGray = 15; // 暗色汙染（低於像素計數上限 60）

        // 白點半徑與位置。
        // 高斯模糊會讓白點脹大，二值化後的實測面積比理論值 πr² 大一截：
        //     r=2 → 21 px    r=3 → 37 px    r=6 → 137 px    r=10 → 357 px    r=15 → 777 px
        //
        // 這組數字就是預設 minArea = 30 的由來：雜訊與真實缺陷的分界線落在 21 與 37 之間。
        // 教材舉例用的 minArea = 20 是「示意值」，實務上一定要像這樣量過自己的產品才能定，
        // 不同鏡頭倍率下這條線會整組平移。
        //
        // （不用 r=1：它前處理後只剩 5 px 的十字形，會被 Open 3×3 整顆侵蝕掉，
        //   連連通域都不存在，反而看不到「面積過濾」這一關的作用。）
        private static readonly int[] DotRadius = { 2, 3, 6, 10, 15 };
        private static readonly Point[] DotCenter =
        {
            new Point(130, 110), new Point(200, 110), new Point(280, 115),
            new Point(370, 120), new Point(440, 130)
        };

        /// <summary>
        /// 白色大矩形，模擬產品本體：高斯後實測約 11,400 px，會被 maxArea=5000 濾掉。
        ///
        /// ⚠ OpenCV 的填滿矩形是「閉區間」——Rectangle(110,180,120,90) 實際畫出 121×91，
        ///   不是 120×90。這個 off-by-one 在做面積判定時會咬人，要記得。
        /// </summary>
        private static readonly Rectangle BigRect = new Rectangle(110, 180, 120, 90);

        /// <summary>近水平刮痕：外接矩形約 153×17，長寬比約 9，用來示範「細長 = 刮痕」。</summary>
        private static readonly Point ScratchFrom = new Point(270, 301);
        private static readonly Point ScratchTo = new Point(420, 313);
        private const int ScratchThickness = 3;

        /// <summary>暗色汙染區：實際 61×41 = 2,501 px（同樣是閉區間），給 ④ 像素計數判 NG 用。</summary>
        private static readonly Rectangle DarkPatch = new Rectangle(300, 200, 60, 40);

        private const int NoiseCount = 200;
        private const int NoiseSize = 2;   // 2×2 白點：能撐過高斯，但會被 Open 或 minArea 清掉

        /// <summary>預設 ROI，四個工具都在這個範圍內作用。</summary>
        public static Rectangle DefaultRoi
        {
            get { return new Rectangle(80, 60, 400, 300); }
        }

        /// <summary>給 UI 顯示用的測試圖說明。</summary>
        public static string Description
        {
            get
            {
                return
                    "  尺寸        : " + Width + " × " + Height + ", 8-bit 灰階" + Environment.NewLine +
                    "  背景        : 灰階 " + BackgroundGray + "（含輕微水平梯度，模擬光照不均）" + Environment.NewLine +
                    "  白點 ×5     : 半徑 2, 3, 6, 10, 15 px，灰階 " + DefectGray + Environment.NewLine +
                    "                前處理後實測面積 21 / 37 / 137 / 357 / 777 px" + Environment.NewLine +
                    "                → 半徑 2 那顆（21 px）會被 minArea=30 濾掉，示範「面積過濾去雜訊」" + Environment.NewLine +
                    "  白色大矩形  : 121 × 91，前處理後約 11,400 px" + Environment.NewLine +
                    "                → 大於 maxArea=5000，被當成產品本體排除" + Environment.NewLine +
                    "  刮痕        : 近水平線 150 × 3，外接矩形約 155 × 19" + Environment.NewLine +
                    "                → 長寬比約 8，示範「細長 = 刮痕、接近方形 = 異物」" + Environment.NewLine +
                    "  暗色汙染    : 61 × 41 = 2,501 px，灰階 " + ContaminationGray +
                        "  → 供 ④ 像素計數判 NG" + Environment.NewLine +
                    "  鹽雜訊      : " + NoiseCount + " 個 " + NoiseSize + "×" + NoiseSize + " 白點" + Environment.NewLine +
                    "                → 高斯 + Open 後全數清除，連通域總數只剩 7（5 白點 + 矩形 + 刮痕）";
            }
        }

        /// <summary>
        /// 產生測試圖。回傳的 Mat 由呼叫端負責 Dispose（第 19 章：誰接手、誰釋放）。
        /// </summary>
        public static Mat Create()
        {
            Mat img = new Mat(Height, Width, DepthType.Cv8U, 1);
            try
            {
                // ── 背景：帶輕微水平梯度，模擬光照不均 ──────────────────
                byte[] buffer = new byte[Width * Height];
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int v = BackgroundGray + (x * 12 / Width) - 6;
                        buffer[y * Width + x] = (byte)v;
                    }
                }

                // 鹽雜訊：固定亂數種子，讓每次產生的圖完全一樣，方便對照講解
                Random rnd = new Random(20260811);
                for (int i = 0; i < NoiseCount; i++)
                {
                    int nx = rnd.Next(0, Width - NoiseSize);
                    int ny = rnd.Next(0, Height - NoiseSize);
                    for (int dy = 0; dy < NoiseSize; dy++)
                    {
                        for (int dx = 0; dx < NoiseSize; dx++)
                        {
                            buffer[(ny + dy) * Width + (nx + dx)] = DefectGray;
                        }
                    }
                }

                // 新配置的 Mat 必定連續，可以整塊 Marshal.Copy 進去
                Marshal.Copy(buffer, 0, img.DataPointer, buffer.Length);

                // ── 缺陷：用 OpenCV 繪圖 API 疊上去 ────────────────────
                MCvScalar defect = new MCvScalar(DefectGray);
                MCvScalar dark = new MCvScalar(ContaminationGray);

                // 暗色汙染（先畫，讓後面的白色元素蓋在上面）
                CvInvoke.Rectangle(img, DarkPatch, dark, -1, LineType.EightConnected, 0);

                // 白色大矩形（產品本體）
                CvInvoke.Rectangle(img, BigRect, defect, -1, LineType.EightConnected, 0);

                // 五個白點（異物）
                for (int i = 0; i < DotRadius.Length; i++)
                {
                    CvInvoke.Circle(img, DotCenter[i], DotRadius[i], defect, -1, LineType.EightConnected, 0);
                }

                // 近水平刮痕
                CvInvoke.Line(img, ScratchFrom, ScratchTo, defect, ScratchThickness, LineType.EightConnected, 0);

                return img;
            }
            catch
            {
                // 例外路徑也要放掉已配置的 Mat（第 18 章：例外路徑漏放）
                img.Dispose();
                throw;
            }
        }
    }
}

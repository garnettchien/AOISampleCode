using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace TemplateMatching
{
    /// <summary>
    /// 產生合成測試影像，讓程式不需要準備影像檔就能跑。
    ///
    /// 這支產生器的關鍵能力是「產品位置可控」——
    /// 教材黃金法則說得很清楚：驗收定位功能，要用「產品真的有偏移」的影像去跑，
    /// 拿影像自身裁切當模板再搜尋同一張圖（偏移永遠是 0）只是 happy-path，
    /// 那個測試會全綠，卻完全遮蓋 P-006。
    /// </summary>
    public static class TestImageGenerator
    {
        public const int Width = 640;
        public const int Height = 480;

        // ── 灰階值 ────────────────────────────────────────────────
        public const byte BackgroundGray = 40;    // 背景（產品外）
        private const byte BodyGray = 120;        // 產品本體
        private const byte FlatGray = 140;        // 平坦無紋理區
        private const byte TraceGray = 90;        // 走線
        private const byte PadGray = 224;         // 焊墊
        private const byte MarkGray = 240;        // 十字定位標記

        // ── 幾何（標稱位置，偏移量 0 時的座標）────────────────────
        private static readonly Rectangle Body = new Rectangle(120, 90, 400, 300);
        private static readonly Rectangle FlatArea = new Rectangle(300, 150, 150, 120);

        /// <summary>十字定位標記：模板的目標。高對比、有明確紋理。</summary>
        private const int CrossCenterX = 180;
        private const int CrossCenterY = 150;
        private const int CrossArm = 20;          // 半臂長
        private const int CrossThick = 12;

        private const int NoiseCount = 160;
        private const int NoiseSize = 2;
        private const int NoiseBrighten = 45;

        /// <summary>
        /// 預設模板框：緊貼十字標記（40×40）再留 4 px 邊，共 48×48。
        /// 教材新手補充：模板要選高對比、有獨特紋理的區域。
        /// </summary>
        public static Rectangle DefaultTemplateRect
        {
            get { return new Rectangle(156, 126, 48, 48); }
        }

        /// <summary>預設外擴 margin = 模板寬高，使搜尋區約為模板的 3×3。</summary>
        public static int DefaultMargin
        {
            get { return DefaultTemplateRect.Width; }
        }

        /// <summary>平坦無紋理區的座標，供 UI 提示使用者做「壞模板」實驗。</summary>
        public static Rectangle FlatAreaRect
        {
            get { return FlatArea; }
        }

        public static string Description
        {
            get
            {
                return
                    "  尺寸      : " + Width + " × " + Height + ", 8-bit 灰階" + Environment.NewLine +
                    "  背景      : 灰階 " + BackgroundGray + "（含輕微梯度與雜訊，模擬真實取像）" + Environment.NewLine +
                    "  產品本體  : " + Body.Width + " × " + Body.Height + "，灰階 " + BodyGray +
                        "，標稱位置 (" + Body.X + ", " + Body.Y + ")" + Environment.NewLine +
                    "  十字標記  : " + (CrossArm * 2) + " × " + (CrossArm * 2) + "，灰階 " + MarkGray +
                        "   ← 定位目標（高對比、有紋理）" + Environment.NewLine +
                    "  焊墊 ×6   : 30 × 20，灰階 " + PadGray + "   ← 讓影像其他處也有紋理" + Environment.NewLine +
                    "  走線 ×5   : 120 × 4，灰階 " + TraceGray + Environment.NewLine +
                    "  平坦區    : " + FlatArea.Width + " × " + FlatArea.Height + "，灰階 " + FlatGray +
                        " ← 把模板框拉到 (" + FlatArea.X + ", " + FlatArea.Y + ", " +
                        FlatArea.Width + ", " + FlatArea.Height + ") 看分數會怎樣" + Environment.NewLine +
                    "  雜訊      : " + NoiseCount + " 個 " + NoiseSize + "×" + NoiseSize +
                        " 亮點，固定在感測器座標（不隨產品移動）";
            }
        }

        /// <summary>
        /// 產生測試影像。回傳的 Mat 由呼叫端負責 Dispose。
        /// </summary>
        /// <param name="offsetX">產品水平偏移量（px）。</param>
        /// <param name="offsetY">產品垂直偏移量（px）。</param>
        /// <param name="angleDeg">產品旋轉角度（度，逆時針）。0 時走整數平移，保證像素精確。</param>
        public static Mat Create(int offsetX, int offsetY, double angleDeg)
        {
            Mat result = null;
            Mat scene = BuildSceneAtNominal();
            try
            {
                if (Math.Abs(angleDeg) < 1e-9)
                {
                    // 純整數平移：直接搬像素，不經插值，偏移量 0 時可得到與基準完全相同的影像。
                    // （用 WarpAffine 做整數平移雖然結果也一樣，但走插值路徑沒必要。）
                    result = ShiftInteger(scene, offsetX, offsetY);
                }
                else
                {
                    result = WarpRotateTranslate(scene, offsetX, offsetY, angleDeg);
                }

                // 梯度與雜訊在「產品變形之後」才疊上去：
                // 雜訊屬於感測器，不會跟著產品跑。這也讓偏移 0 的那張圖與基準完全一致。
                ApplySensorEffects(result);
                return result;
            }
            catch
            {
                if (result != null) result.Dispose();
                throw;
            }
            finally
            {
                scene.Dispose();
            }
        }

        /// <summary>基準影像 = 偏移 (0,0)、角度 0 的那張，永遠用它教導模板。</summary>
        public static Mat CreateReference()
        {
            return Create(0, 0, 0);
        }

        /// <summary>
        /// 把任意影像平移／旋轉，供「載入自己的影像」時模擬產品偏移用。
        /// 回傳的 Mat 由呼叫端負責 Dispose。
        /// </summary>
        public static Mat Transform(Mat scene, int offsetX, int offsetY, double angleDeg, byte borderGray)
        {
            if (scene == null) throw new ArgumentNullException("scene");

            if (Math.Abs(angleDeg) < 1e-9)
                return ShiftIntegerAny(scene, offsetX, offsetY, borderGray);

            return WarpAny(scene, offsetX, offsetY, angleDeg, borderGray);
        }

        // ─────────────────────────────────────────────────────────────

        /// <summary>畫出標稱位置的場景（純淨版，還沒有梯度與雜訊）。</summary>
        private static Mat BuildSceneAtNominal()
        {
            Mat img = new Mat(Height, Width, DepthType.Cv8U, 1);
            try
            {
                img.SetTo(new MCvScalar(BackgroundGray));

                // 產品本體
                CvInvoke.Rectangle(img, Body, new MCvScalar(BodyGray), -1, LineType.EightConnected, 0);

                // 平坦無紋理區（故意留的「壞模板」示範區）
                CvInvoke.Rectangle(img, FlatArea, new MCvScalar(FlatGray), -1, LineType.EightConnected, 0);

                // 走線
                for (int i = 0; i < 5; i++)
                {
                    CvInvoke.Rectangle(img, new Rectangle(150, 200 + i * 22, 120, 4),
                                       new MCvScalar(TraceGray), -1, LineType.EightConnected, 0);
                }

                // 焊墊列
                for (int j = 0; j < 6; j++)
                {
                    CvInvoke.Rectangle(img, new Rectangle(160 + j * 60, 340, 30, 20),
                                       new MCvScalar(PadGray), -1, LineType.EightConnected, 0);
                }

                // 十字定位標記
                CvInvoke.Rectangle(img,
                    new Rectangle(CrossCenterX - CrossArm, CrossCenterY - CrossThick / 2,
                                  CrossArm * 2, CrossThick),
                    new MCvScalar(MarkGray), -1, LineType.EightConnected, 0);
                CvInvoke.Rectangle(img,
                    new Rectangle(CrossCenterX - CrossThick / 2, CrossCenterY - CrossArm,
                                  CrossThick, CrossArm * 2),
                    new MCvScalar(MarkGray), -1, LineType.EightConnected, 0);

                return img;
            }
            catch
            {
                img.Dispose();
                throw;
            }
        }

        private static Mat ShiftInteger(Mat scene, int dx, int dy)
        {
            return ShiftIntegerAny(scene, dx, dy, BackgroundGray);
        }

        private static Mat WarpRotateTranslate(Mat scene, int dx, int dy, double angleDeg)
        {
            return WarpAny(scene, dx, dy, angleDeg, BackgroundGray);
        }

        /// <summary>整數平移：背景填滿後，把重疊區域整塊搬過去。不經插值，像素精確。</summary>
        private static Mat ShiftIntegerAny(Mat scene, int dx, int dy, byte borderGray)
        {
            int w = scene.Width, h = scene.Height;
            Mat dst = new Mat(h, w, DepthType.Cv8U, 1);
            try
            {
                dst.SetTo(new MCvScalar(borderGray));

                int copyW = w - Math.Abs(dx);
                int copyH = h - Math.Abs(dy);
                if (copyW > 0 && copyH > 0)
                {
                    Rectangle srcRect = new Rectangle(Math.Max(0, -dx), Math.Max(0, -dy), copyW, copyH);
                    Rectangle dstRect = new Rectangle(Math.Max(0, dx), Math.Max(0, dy), copyW, copyH);
                    using (Mat srcView = new Mat(scene, srcRect))
                    using (Mat dstView = new Mat(dst, dstRect))
                    {
                        srcView.CopyTo(dstView);
                    }
                }
                return dst;
            }
            catch
            {
                dst.Dispose();
                throw;
            }
        }

        /// <summary>旋轉（繞影像中心）＋平移，一次 WarpAffine 完成。</summary>
        private static Mat WarpAny(Mat scene, int dx, int dy, double angleDeg, byte borderGray)
        {
            int w = scene.Width, h = scene.Height;
            Mat dst = new Mat();
            try
            {
                using (Mat m = new Mat())
                {
                    CvInvoke.GetRotationMatrix2D(new PointF(w / 2f, h / 2f), angleDeg, 1.0, m);

                    // 在旋轉矩陣的平移項上直接加偏移量，省一次 WarpAffine
                    using (Matrix<double> mm = new Matrix<double>(m.Rows, m.Cols))
                    {
                        m.CopyTo(mm);
                        mm.Data[0, 2] += dx;
                        mm.Data[1, 2] += dy;

                        CvInvoke.WarpAffine(scene, dst, mm, new Size(w, h),
                                            Inter.Linear, Warp.Default, BorderType.Constant,
                                            new MCvScalar(borderGray));
                    }
                }
                return dst;
            }
            catch
            {
                dst.Dispose();
                throw;
            }
        }

        /// <summary>疊上光照梯度與雜訊。這兩者屬於「感測器」，不隨產品移動。</summary>
        private static void ApplySensorEffects(Mat img)
        {
            byte[] buffer = new byte[Width * Height];
            Marshal.Copy(img.DataPointer, buffer, 0, buffer.Length);

            // 輕微水平梯度，模擬光照不均
            for (int y = 0; y < Height; y++)
            {
                int row = y * Width;
                for (int x = 0; x < Width; x++)
                {
                    int v = buffer[row + x] + (x * 10 / Width) - 5;
                    buffer[row + x] = ClampByte(v);
                }
            }

            // 固定種子亂數，讓每次產生的影像完全一樣，方便對照講解
            Random rnd = new Random(20260812);
            for (int i = 0; i < NoiseCount; i++)
            {
                int nx = rnd.Next(0, Width - NoiseSize);
                int ny = rnd.Next(0, Height - NoiseSize);
                for (int oy = 0; oy < NoiseSize; oy++)
                {
                    int row = (ny + oy) * Width;
                    for (int ox = 0; ox < NoiseSize; ox++)
                    {
                        buffer[row + nx + ox] = ClampByte(buffer[row + nx + ox] + NoiseBrighten);
                    }
                }
            }

            Marshal.Copy(buffer, 0, img.DataPointer, buffer.Length);
        }

        private static byte ClampByte(int v)
        {
            if (v < 0) return 0;
            if (v > 255) return 255;
            return (byte)v;
        }
    }
}

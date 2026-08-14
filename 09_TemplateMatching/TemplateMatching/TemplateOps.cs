using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace TemplateMatching
{
    /// <summary>
    /// 一次定位的完整結果。
    /// </summary>
    public class MatchResult
    {
        /// <summary>分數是否達到門檻。false 代表「沒找到」，此時 Location 不可信。</summary>
        public bool Found;

        /// <summary>最高分（TM_CCOEFF_NORMED，範圍 −1 ～ 1）。</summary>
        public double Score;

        /// <summary>MinMaxLoc 回傳的位置，座標系是「搜尋區內部」。</summary>
        public Point MaxLoc;

        /// <summary>換算回整張影像後的模板左上角座標。</summary>
        public Point Location;

        /// <summary>相對基準位置的偏移量，就是要套到後續所有 ROI 的那個值。</summary>
        public Point Offset;

        /// <summary>本次實際使用的搜尋區。</summary>
        public Rectangle SearchRect;

        /// <summary>結果圖尺寸 =（搜尋區 − 模板 + 1）。退化成 1×1 就是 P-006。</summary>
        public Size ResultMapSize;

        /// <summary>多角度搜尋時得分最高的角度；單角度固定為 0。</summary>
        public double AngleDeg;

        /// <summary>多角度搜尋實際試了幾個角度；單角度為 1。</summary>
        public int AnglesTried;
    }

    /// <summary>
    /// 第 9 章的模板匹配演算法。
    ///
    /// 本類別刻意「不碰任何 UI」——只吃 Mat、吐數值，方便單獨閱讀與測試。
    ///
    /// 【記憶體所有權約定】（第 19 章配對釋放鐵律）
    ///   · 回傳 Mat 的方法（CropTemplate / RotateTemplate），回傳值由呼叫端負責 Dispose。
    /// </summary>
    public static class TemplateOps
    {
        /// <summary>教材 §4 建議的分數門檻範圍。</summary>
        public const double DefaultScoreThreshold = 0.75;

        // ─────────────────────────────────────────────────────────────
        //  教導：從基準影像裁出模板
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 從基準影像裁出模板，回傳「獨立擁有像素資料」的 Mat。
        ///
        /// ⚠ 這裡一定要 Clone()：new Mat(src, rect) 只是共享 header，像素資料仍屬於 src。
        ///   模板要活得比來源影像久（來源會一直換成新的測試影像），
        ///   不複製的話，來源一被釋放模板就成了懸空指標。
        ///   ——這與第 7 章 ROI 的「刻意共享」正好相反，差別在於「誰要活得比較久」。
        /// </summary>
        public static Mat CropTemplate(Mat source, Rectangle templateRect)
        {
            if (source == null) throw new ArgumentNullException("source");

            Rectangle r = ClampRect(templateRect, source.Width, source.Height);
            using (Mat view = new Mat(source, r))
            {
                return view.Clone();
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  搜尋區：P-006 的核心
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 由模板框向四周外擴 margin 產生搜尋區，並夾在影像範圍內。
        ///
        /// 教材 §2／P-006：做「定位」時搜尋區必須明顯大於模板，
        /// 否則結果圖退化、MinMaxLoc 永遠回 (0,0)。
        /// margin 取模板寬高時，搜尋區約為模板的 3×3，可容忍的偏移量 = margin。
        ///
        /// 註：教材 P-006 的示意程式用
        ///       Math.Min(imgW, templateRect.Width + marginX * 2)
        ///     來夾寬度，那只夾了「寬度」沒有考慮起點 X，
        ///     當模板靠近影像右緣時 X + Width 仍可能超出影像。
        ///     這裡改成夾「右下角座標」再回推寬高，才是完整的邊界處理。
        /// </summary>
        public static Rectangle BuildSearchRect(Rectangle templateRect, int marginX, int marginY,
                                                int imageWidth, int imageHeight)
        {
            if (marginX < 0) marginX = 0;
            if (marginY < 0) marginY = 0;

            int left = Math.Max(0, templateRect.X - marginX);
            int top = Math.Max(0, templateRect.Y - marginY);
            int right = Math.Min(imageWidth, templateRect.Right + marginX);
            int bottom = Math.Min(imageHeight, templateRect.Bottom + marginY);

            return new Rectangle(left, top, Math.Max(1, right - left), Math.Max(1, bottom - top));
        }

        /// <summary>
        /// 結果圖尺寸 =（搜尋區 − 模板 + 1）。
        /// 這個值在執行 MatchTemplate 之前就能算出來，是驗證參數的第一道關卡。
        /// </summary>
        public static Size CalcResultMapSize(Rectangle searchRect, Rectangle templateRect)
        {
            return new Size(searchRect.Width - templateRect.Width + 1,
                            searchRect.Height - templateRect.Height + 1);
        }

        /// <summary>
        /// 定位空間是否足夠。結果圖任一邊 ≤ 1 就代表模板動不了，位置會鎖死。
        /// </summary>
        public static bool HasEnoughSearchSpace(Rectangle searchRect, Rectangle templateRect)
        {
            Size s = CalcResultMapSize(searchRect, templateRect);
            return s.Width > 1 && s.Height > 1;
        }

        // ─────────────────────────────────────────────────────────────
        //  定位
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 在搜尋區內尋找模板，回傳最佳匹配位置與分數。
        ///
        /// 刻意「不」擋下搜尋空間不足的情況——因為這支程式要示範 P-006 發生時的實際行為。
        /// 正式專案請在呼叫前用 HasEnoughSearchSpace() 驗證並丟例外，避免靜默失效。
        /// </summary>
        /// <param name="source">整張影像（單通道灰階）。</param>
        /// <param name="template">模板影像。</param>
        /// <param name="searchRect">搜尋區，座標系為整張影像。</param>
        /// <param name="referencePos">教導時記下的基準位置（模板左上角），用來算偏移量。</param>
        /// <param name="scoreThreshold">分數門檻，低於此值視為沒找到。</param>
        public static MatchResult Match(Mat source, Mat template, Rectangle searchRect,
                                        Point referencePos, double scoreThreshold)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (template == null) throw new ArgumentNullException("template");

            MatchResult result = new MatchResult();
            result.SearchRect = ClampRect(searchRect, source.Width, source.Height);
            result.ResultMapSize = new Size(result.SearchRect.Width - template.Width + 1,
                                            result.SearchRect.Height - template.Height + 1);
            result.AngleDeg = 0;
            result.AnglesTried = 1;

            if (result.ResultMapSize.Width < 1 || result.ResultMapSize.Height < 1)
            {
                // 搜尋區比模板還小，MatchTemplate 會直接丟例外，先擋下來
                result.Found = false;
                result.Score = 0;
                result.Location = referencePos;
                result.Offset = Point.Empty;
                return result;
            }

            using (Mat searchArea = new Mat(source, result.SearchRect))
            using (Mat resultMap = new Mat())
            {
                CvInvoke.MatchTemplate(searchArea, template, resultMap,
                                       TemplateMatchingType.CcoeffNormed);

                double minVal = 0, maxVal = 0;
                Point minLoc = Point.Empty, maxLoc = Point.Empty;
                CvInvoke.MinMaxLoc(resultMap, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                result.Score = maxVal;
                result.MaxLoc = maxLoc;

                // 換算回整張影像的座標：加上搜尋區左上角的偏移
                result.Location = new Point(result.SearchRect.X + maxLoc.X,
                                            result.SearchRect.Y + maxLoc.Y);
                result.Offset = new Point(result.Location.X - referencePos.X,
                                          result.Location.Y - referencePos.Y);

                // MinMaxLoc 永遠會回一個最高分位置，即使影像裡根本沒有目標物。
                // 沒有這道門檻，程式會拿著一個毫無意義的座標去修正所有後續 ROI。
                result.Found = (maxVal >= scoreThreshold);
            }

            return result;
        }

        // ─────────────────────────────────────────────────────────────
        //  多角度匹配（教材 §4）
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 把模板轉一個角度。回傳的 Mat 由呼叫端負責 Dispose。
        /// 用原尺寸輸出，邊角以 borderValue 填滿（模板通常只轉幾度，影響很小）。
        /// </summary>
        public static Mat RotateTemplate(Mat template, double angleDeg, double borderValue)
        {
            if (template == null) throw new ArgumentNullException("template");

            Mat rotated = new Mat();
            try
            {
                using (Mat m = new Mat())
                {
                    PointF center = new PointF((template.Width - 1) / 2f, (template.Height - 1) / 2f);
                    CvInvoke.GetRotationMatrix2D(center, angleDeg, 1.0, m);
                    CvInvoke.WarpAffine(template, rotated, m,
                                        new Size(template.Width, template.Height),
                                        Inter.Linear, Warp.Default, BorderType.Constant,
                                        new MCvScalar(borderValue));
                }
                return rotated;
            }
            catch
            {
                rotated.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 多角度搜尋：在 ±angleRange 內以 angleStep 逐角度各做一次匹配，取分數最高者。
        ///
        /// 教材 §4：角度數 × 搜尋區大小 決定運算時間。
        /// 角度步距減半 → 角度數加倍 → 時間加倍；搜尋區邊長加倍 → 面積四倍。
        /// 所以「先縮小搜尋區、再做多角度」，順序不能反。
        /// </summary>
        public static MatchResult MatchMultiAngle(Mat source, Mat template, Rectangle searchRect,
                                                  Point referencePos, double scoreThreshold,
                                                  double angleRange, double angleStep,
                                                  double borderValue)
        {
            if (angleStep <= 0) angleStep = 1;
            if (angleRange < 0) angleRange = 0;

            MatchResult best = null;
            int tried = 0;

            for (double a = -angleRange; a <= angleRange + 1e-9; a += angleStep)
            {
                tried++;
                MatchResult r;

                if (Math.Abs(a) < 1e-9)
                {
                    // 0 度不必轉，省一次 WarpAffine
                    r = Match(source, template, searchRect, referencePos, scoreThreshold);
                }
                else
                {
                    using (Mat rot = RotateTemplate(template, a, borderValue))
                    {
                        r = Match(source, rot, searchRect, referencePos, scoreThreshold);
                    }
                }

                r.AngleDeg = a;
                if (best == null || r.Score > best.Score) best = r;
            }

            if (best != null) best.AnglesTried = tried;
            return best;
        }

        // ─────────────────────────────────────────────────────────────
        //  共用
        // ─────────────────────────────────────────────────────────────

        /// <summary>把矩形夾回影像範圍內，避免使用者輸入的座標讓 OpenCV 丟例外。</summary>
        public static Rectangle ClampRect(Rectangle r, int imageWidth, int imageHeight)
        {
            int x = Math.Max(0, Math.Min(r.X, imageWidth - 1));
            int y = Math.Max(0, Math.Min(r.Y, imageHeight - 1));
            int w = Math.Max(1, Math.Min(r.Width, imageWidth - x));
            int h = Math.Max(1, Math.Min(r.Height, imageHeight - y));
            return new Rectangle(x, y, w, h);
        }
    }
}

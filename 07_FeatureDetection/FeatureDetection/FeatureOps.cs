using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace FeatureDetection
{
    /// <summary>
    /// 單一 Blob（連通域）的特徵資料。對應教材 §2「常用特徵」表格。
    /// </summary>
    public class BlobInfo
    {
        /// <summary>過濾後的顯示編號（從 1 開始）。</summary>
        public int Index;

        /// <summary>面積：連通域的像素總數。</summary>
        public int Area;

        /// <summary>形心：連通域的幾何中心座標。</summary>
        public PointF Centroid;

        /// <summary>外接矩形：包住連通域的最小矩形。</summary>
        public Rectangle BoundingBox;

        /// <summary>
        /// 長寬比 = 外接矩形長邊 / 短邊，恆 &gt;= 1。
        /// 教材：細長 = 刮痕、接近方形 = 異物。
        /// 取長邊/短邊而非固定寬/高，是為了讓水平刮痕與垂直刮痕得到相同的判斷結果。
        /// </summary>
        public double AspectRatio
        {
            get
            {
                double w = BoundingBox.Width;
                double h = BoundingBox.Height;
                if (w <= 0 || h <= 0) return 0;
                return (w >= h) ? (w / h) : (h / w);
            }
        }
    }

    /// <summary>
    /// Blob 分析的完整結果，含被過濾掉的統計數字（讓 UI 能說明「為什麼少了幾個」）。
    /// </summary>
    public class BlobResult
    {
        /// <summary>面積過濾前的連通域總數（不含背景）。</summary>
        public int TotalCount;

        /// <summary>通過面積過濾、視為缺陷的 Blob。</summary>
        public List<BlobInfo> Kept = new List<BlobInfo>();

        /// <summary>因為 Area &lt; minArea 被濾除的數量（視為雜訊）。</summary>
        public int RejectedTooSmall;

        /// <summary>因為 Area &gt; maxArea 被濾除的數量（視為產品本體）。</summary>
        public int RejectedTooLarge;

        /// <summary>被當成產品本體濾掉的最大面積，供 UI 說明用。</summary>
        public int LargestRejectedArea;
    }

    /// <summary>
    /// 第 7 章的四個特徵檢測工具。
    ///
    /// 本類別刻意「不碰任何 UI」——所有方法只吃 Mat、吐 Mat 或數值，
    /// 方便單獨閱讀、單獨測試，也方便日後搬進正式專案的演算法層。
    ///
    /// 【記憶體所有權約定】（第 19 章配對釋放鐵律）
    ///   · 回傳 Mat 的方法，其回傳值一律由「呼叫端」負責 Dispose。
    ///   · 唯一例外是 ExtractRoi()，見該方法的說明。
    /// </summary>
    public static class FeatureOps
    {
        // ─────────────────────────────────────────────────────────────
        // ① ROI：感興趣區域（教材 §1）
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 在影像上取 ROI 子區域。
        ///
        /// ⚠ 回傳的 Mat 與 src「共享同一塊像素 buffer」，不是複製品：
        ///     · 改 roiMat 的像素 = 改 src 的像素。
        ///     · roiMat.Dispose() 只釋放 header，像素資料仍屬於 src。
        ///     · 因此 src 必須活得比 roiMat 久，否則 roiMat 會變成懸空指標。
        /// 若真的需要一份獨立資料，請改用 ExtractRoi(...).Clone()（並記得多一次 Dispose）。
        /// </summary>
        public static Mat ExtractRoi(Mat src, Rectangle roi)
        {
            if (src == null) throw new ArgumentNullException("src");
            return new Mat(src, ClampRoi(roi, src.Width, src.Height));
        }

        /// <summary>
        /// 把 ROI 夾回影像範圍內，避免使用者輸入的座標超出邊界導致 OpenCV 例外。
        /// 實務上 ROI 座標來自設定檔或定位結果，一定要做這層防呆。
        /// </summary>
        public static Rectangle ClampRoi(Rectangle roi, int imgWidth, int imgHeight)
        {
            int x = Math.Max(0, Math.Min(roi.X, imgWidth - 1));
            int y = Math.Max(0, Math.Min(roi.Y, imgHeight - 1));
            int w = Math.Max(1, Math.Min(roi.Width, imgWidth - x));
            int h = Math.Max(1, Math.Min(roi.Height, imgHeight - y));
            return new Rectangle(x, y, w, h);
        }

        // ─────────────────────────────────────────────────────────────
        // 前處理（第 6 章標準流程順序，Blob 分析的前置作業）
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 高斯模糊 → 二值化 → 形態學 Open。
        ///
        /// 順序不可顛倒（第 6 章硬規則）：
        ///   · 濾波一定在二值化「之前」——放到 0/1 影像上會退化成形態學、失去去雜訊作用。
        ///   · Open/Close 這類清雜訊的形態學在二值化「之後」。
        /// </summary>
        /// <param name="gray">單通道灰階來源。</param>
        /// <param name="blurKernelSize">高斯核尺寸，必須是奇數。</param>
        /// <param name="threshold">二值化閾值。</param>
        /// <param name="morphKernel">形態學結構元素，由呼叫端建立並持有（見 Form1 的欄位）。</param>
        /// <returns>二值影像。呼叫端負責 Dispose。</returns>
        public static Mat Preprocess(Mat gray, int blurKernelSize, int threshold, Mat morphKernel)
        {
            if (gray == null) throw new ArgumentNullException("gray");

            // 高斯核必須是奇數，這裡直接防呆修正而不是丟例外
            int k = blurKernelSize;
            if (k < 1) k = 1;
            if (k % 2 == 0) k++;

            Mat binary = new Mat();
            try
            {
                using (Mat blurred = new Mat())
                {
                    CvInvoke.GaussianBlur(gray, blurred, new Size(k, k), 0);
                    CvInvoke.Threshold(blurred, binary, threshold, 255, ThresholdType.Binary);
                }

                if (morphKernel != null)
                {
                    // Open = 先侵蝕再膨脹，把細碎的鹽雜訊清掉、保留主體形狀
                    using (Mat opened = new Mat())
                    {
                        CvInvoke.MorphologyEx(binary, opened, MorphOp.Open, morphKernel,
                                              new Point(-1, -1), 1, BorderType.Default,
                                              new Emgu.CV.Structure.MCvScalar());
                        // 暫存 → 換新 → 放舊（第 18 章：覆蓋前先 Dispose 舊值）
                        Mat old = binary;
                        binary = opened.Clone();
                        old.Dispose();
                    }
                }
                return binary;
            }
            catch
            {
                // 例外路徑也要放掉已配置的資源，否則就是第 18 章講的「例外路徑漏放」
                binary.Dispose();
                throw;
            }
        }

        // ─────────────────────────────────────────────────────────────
        // ② Blob 連通域分析（教材 §2）
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 對二值影像做連通域分析，並依面積過濾。
        ///
        /// 教材 §2「面積過濾：去雜訊的最後防線」：
        ///   · Area &lt; minArea → 雜訊（光照不均、鏡頭灰塵、感光元件雜訊產生的零碎白點）
        ///   · Area &gt; maxArea → 產品本體，不是缺陷
        /// minArea / maxArea 必須由外部傳入，絕不可寫死成魔術數字。
        /// </summary>
        public static BlobResult DetectBlobs(Mat binary, int minArea, int maxArea)
        {
            if (binary == null) throw new ArgumentNullException("binary");

            BlobResult result = new BlobResult();

            using (Mat labels = new Mat())
            using (Mat stats = new Mat())
            using (Mat centroids = new Mat())
            {
                // 回傳值包含背景（label 0），實際連通域數 = n - 1
                int n = CvInvoke.ConnectedComponentsWithStats(
                            binary, labels, stats, centroids,
                            LineType.EightConnected, DepthType.Cv32S);

                result.TotalCount = Math.Max(0, n - 1);
                if (n <= 1) return result;

                // stats    : n × 5  CV_32S（Left, Top, Width, Height, Area）
                // centroids: n × 2  CV_64F（x, y）
                using (Matrix<int> statsData = new Matrix<int>(stats.Rows, stats.Cols))
                using (Matrix<double> centroidData = new Matrix<double>(centroids.Rows, centroids.Cols))
                {
                    stats.CopyTo(statsData);
                    centroids.CopyTo(centroidData);

                    for (int i = 1; i < n; i++)   // 從 1 開始：label 0 是背景
                    {
                        int area = statsData.Data[i, (int)ConnectedComponentsTypes.Area];

                        if (area < minArea)
                        {
                            result.RejectedTooSmall++;
                            continue;
                        }
                        if (area > maxArea)
                        {
                            result.RejectedTooLarge++;
                            if (area > result.LargestRejectedArea) result.LargestRejectedArea = area;
                            continue;
                        }

                        BlobInfo blob = new BlobInfo();
                        blob.Index = result.Kept.Count + 1;
                        blob.Area = area;
                        blob.BoundingBox = new Rectangle(
                            statsData.Data[i, (int)ConnectedComponentsTypes.Left],
                            statsData.Data[i, (int)ConnectedComponentsTypes.Top],
                            statsData.Data[i, (int)ConnectedComponentsTypes.Width],
                            statsData.Data[i, (int)ConnectedComponentsTypes.Height]);
                        blob.Centroid = new PointF(
                            (float)centroidData.Data[i, 0],
                            (float)centroidData.Data[i, 1]);

                        result.Kept.Add(blob);
                    }
                }
            }

            return result;
        }

        // ─────────────────────────────────────────────────────────────
        // ③ 邊緣檢測（教材 §3）
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Canny 邊緣檢測。
        ///
        /// 流程：高斯模糊去雜訊 → Sobel 計算梯度 → 非極大值抑制（讓邊緣變細）
        ///       → 雙門檻（高閾值找強邊緣、低閾值延伸連接）。
        /// 前三步 OpenCV 內建，但「高斯去雜訊」要自己先做——
        /// 不先模糊，雜訊會被當成邊緣放大（第 6 章：先高斯再 Sobel）。
        ///
        /// 經驗值：threshold2 約為 threshold1 的 2～3 倍。
        /// </summary>
        /// <returns>邊緣影像（單通道，邊緣處為 255）。呼叫端負責 Dispose。</returns>
        public static Mat DetectEdges(Mat gray, int threshold1, int threshold2, int blurKernelSize)
        {
            if (gray == null) throw new ArgumentNullException("gray");

            int k = blurKernelSize;
            if (k < 1) k = 1;
            if (k % 2 == 0) k++;

            Mat edges = new Mat();
            try
            {
                using (Mat blurred = new Mat())
                {
                    CvInvoke.GaussianBlur(gray, blurred, new Size(k, k), 0);
                    CvInvoke.Canny(blurred, edges, threshold1, threshold2);
                }
                return edges;
            }
            catch
            {
                edges.Dispose();
                throw;
            }
        }

        // ─────────────────────────────────────────────────────────────
        // ④ 像素計數（教材 §4）
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 建立「亮度落在 [low, high] 之間」的遮罩。
        ///
        /// 這等同教材 §4 的雙層 for 迴圈：
        ///     for (y...) for (x...) if (v &gt;= low &amp;&amp; v &lt;= high) count++;
        /// 但 InRange 走的是 OpenCV 的向量化實作，快非常多，
        /// 而且順便得到一張遮罩可以顯示出來看，不必只拿到一個數字。
        /// </summary>
        /// <returns>遮罩影像（符合範圍處為 255）。呼叫端負責 Dispose。</returns>
        public static Mat BuildRangeMask(Mat gray, int low, int high)
        {
            if (gray == null) throw new ArgumentNullException("gray");

            Mat mask = new Mat();
            try
            {
                using (ScalarArray lower = new ScalarArray(low))
                using (ScalarArray upper = new ScalarArray(high))
                {
                    CvInvoke.InRange(gray, lower, upper, mask);
                }
                return mask;
            }
            catch
            {
                mask.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 統計亮度落在 [low, high] 之間的像素數量。
        /// 不需要二值化、不需要 Blob 分析，直接對灰階計數，是四個工具中最快的一個。
        /// </summary>
        public static int CountPixelsInRange(Mat gray, int low, int high)
        {
            using (Mat mask = BuildRangeMask(gray, low, high))
            {
                return CvInvoke.CountNonZero(mask);
            }
        }
    }
}

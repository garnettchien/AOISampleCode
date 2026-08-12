using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace SizeMeasurement
{
    /// <summary>
    /// 單條掃描線的邊緣配對結果（教材 §2）。
    /// 一條掃描線上找到「一對」邊緣（前緣 + 後緣），兩者相減就是像素長度。
    /// </summary>
    public class EdgePairResult
    {
        /// <summary>是否成功找到成對的邊緣。找不到時其餘欄位不具意義。</summary>
        public bool Found;

        /// <summary>掃描線位置：水平掃描時是 Y 座標，垂直掃描時是 X 座標。</summary>
        public int ScanPos;

        /// <summary>前緣（亮度由暗轉亮）的絕對座標，亞像素。</summary>
        public double FirstEdge;

        /// <summary>後緣（亮度由亮轉暗）的絕對座標，亞像素。</summary>
        public double SecondEdge;

        /// <summary>這條掃描線上的灰階最小值，供 UI 說明「閾值該定在哪」。</summary>
        public double ProfileMin;

        /// <summary>這條掃描線上的灰階最大值。</summary>
        public double ProfileMax;

        /// <summary>兩個邊緣之間的像素距離。找不到邊緣時回 0。</summary>
        public double LengthPx
        {
            get { return Found ? (SecondEdge - FirstEdge) : 0.0; }
        }
    }

    /// <summary>
    /// 多次量測的統計結果（教材 §5）。
    /// 單次量測會被雜訊影響，精密場合一律「連取多次再看統計」。
    /// </summary>
    public class StatResult
    {
        /// <summary>成功納入統計的樣本數。</summary>
        public int Count;

        /// <summary>平均值——代表性尺寸。</summary>
        public double Mean;

        /// <summary>
        /// 標準差 σ——量測的離散程度，也就是「這個量測穩不穩」。
        /// 本範例用「樣本標準差」（除以 n−1），見 CalcStats 的說明。
        /// </summary>
        public double StdDev;

        public double Min;
        public double Max;

        /// <summary>全距 = 最大 − 最小。樣本數少時比標準差更直觀。</summary>
        public double Range { get { return Max - Min; } }
    }

    /// <summary>
    /// 第 8 章的量測工具：掃描線抽取、亞像素邊緣、寬度／點位量測、統計。
    ///
    /// 本類別刻意「不碰任何 UI」，也不碰 Calibration——
    /// 所有方法只吃 Mat、吐「像素」單位的數值。px 到 mm 的換算一律交給 Calibration，
    /// 這條界線讓「量測」與「標定」可以各自單獨閱讀、單獨測試。
    ///
    /// 【記憶體所有權約定】（第 19 章配對釋放鐵律）
    ///   本類別的方法都不回傳 Mat，內部配置的中間物件一律當場 using 掉，
    ///   呼叫端沒有任何釋放義務。
    /// </summary>
    public static class MeasureOps
    {
        /// <summary>找不到邊緣時的回傳值。座標不可能是負的，用 −1 當哨兵值。</summary>
        public const double EdgeNotFound = -1.0;

        // ─────────────────────────────────────────────────────────────
        // §2 掃描線：把一條線上的灰階值抽出來
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 沿水平方向抽出一條掃描線的灰階值（教材 §2 Step 1）。
        ///
        /// ⚠ 位移一定要用 Step，不可以用 Width：
        ///     OpenCV 每一列的開頭會做記憶體對齊，Step（一列佔幾個位元組）通常「大於等於」Width。
        ///     影像寬度剛好是 4 的倍數時兩者相等，用 Width 也「看起來正常」；
        ///     換一張寬度不是 4 倍數的圖，就會逐列偏移，profile 整條歪掉而且不會拋任何例外。
        ///     這種錯最難查——所以一律用 Step。
        /// </summary>
        /// <param name="gray">8-bit 單通道灰階影像。</param>
        /// <param name="y">掃描線的 Y 座標。</param>
        /// <param name="x0">起點 X（含）。</param>
        /// <param name="x1">終點 X（含）。</param>
        /// <returns>長度 x1−x0+1 的灰階陣列。</returns>
        public static double[] ExtractRowProfile(Mat gray, int y, int x0, int x1)
        {
            EnsureGray8(gray);

            y = Clamp(y, 0, gray.Height - 1);
            x0 = Clamp(x0, 0, gray.Width - 1);
            x1 = Clamp(x1, 0, gray.Width - 1);
            if (x1 < x0) { int t = x0; x0 = x1; x1 = t; }

            int count = x1 - x0 + 1;
            byte[] raw = new byte[count];

            // 一列是連續的，可以整段 Marshal.Copy 出來
            IntPtr rowStart = IntPtr.Add(gray.DataPointer, y * gray.Step + x0);
            Marshal.Copy(rowStart, raw, 0, count);

            double[] profile = new double[count];
            for (int i = 0; i < count; i++) profile[i] = raw[i];
            return profile;
        }

        /// <summary>
        /// 沿垂直方向抽出一條掃描線的灰階值。
        /// 垂直方向的像素在記憶體裡不連續（每次要跳一個 Step），所以逐點讀。
        /// </summary>
        public static double[] ExtractColumnProfile(Mat gray, int x, int y0, int y1)
        {
            EnsureGray8(gray);

            x = Clamp(x, 0, gray.Width - 1);
            y0 = Clamp(y0, 0, gray.Height - 1);
            y1 = Clamp(y1, 0, gray.Height - 1);
            if (y1 < y0) { int t = y0; y0 = y1; y1 = t; }

            int count = y1 - y0 + 1;
            double[] profile = new double[count];

            IntPtr basePtr = gray.DataPointer;
            int step = gray.Step;
            for (int i = 0; i < count; i++)
            {
                profile[i] = Marshal.ReadByte(basePtr, (y0 + i) * step + x);
            }
            return profile;
        }

        // ─────────────────────────────────────────────────────────────
        // §2 邊緣搜尋：亮度轉折點，取到亞像素
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 找第一個「由暗轉亮」的邊緣（前緣），從頭往後掃。
        ///
        /// 【為什麼要做亞像素】
        ///   只回傳整數索引，量測解析度就被鎖死在 1 px。以本範例 20 μm/px 為例，
        ///   1 px 就是 20 μm——公差 ±50 μm 的件，光量測本身的量化誤差就吃掉 4 成。
        ///   真實邊緣有灰階過渡（鏡頭 MTF、離焦、抗鋸齒都會造成），
        ///   在跨過閾值的那兩點之間做線性內插，解析度可以推進到 0.1 px 以下。
        ///
        /// 內插公式：pos = i + (threshold − p[i]) / (p[i+1] − p[i])
        /// 分母恆為正（因為 p[i] 小於 threshold 且 p[i+1] 大於等於 threshold），不會除以零。
        /// </summary>
        /// <returns>相對於 profile 起點的亞像素位置；找不到回 EdgeNotFound。</returns>
        public static double FindRisingEdge(double[] profile, double threshold)
        {
            if (profile == null) throw new ArgumentNullException("profile");

            for (int i = 0; i + 1 < profile.Length; i++)
            {
                if (profile[i] < threshold && profile[i + 1] >= threshold)
                {
                    return i + (threshold - profile[i]) / (profile[i + 1] - profile[i]);
                }
            }
            return EdgeNotFound;
        }

        /// <summary>
        /// 找最後一個「由亮轉暗」的邊緣（後緣），從尾端往前掃。
        ///
        /// 從尾端往前找，配上前緣的從頭往後找，取到的是「最外側的一對邊緣」。
        /// 若兩者都從頭掃，元件內部只要有一顆亮點或一道刮痕，
        /// 就會提早配對到內部的假邊緣，量出來的寬度整個變短。
        /// </summary>
        public static double FindFallingEdge(double[] profile, double threshold)
        {
            if (profile == null) throw new ArgumentNullException("profile");

            for (int i = profile.Length - 2; i >= 0; i--)
            {
                if (profile[i] >= threshold && profile[i + 1] < threshold)
                {
                    return i + (profile[i] - threshold) / (profile[i] - profile[i + 1]);
                }
            }
            return EdgeNotFound;
        }

        /// <summary>
        /// 建議閾值 = （這條線的最大值 + 最小值）／2。
        ///
        /// 取中點的理由：邊緣的灰階過渡大致對稱，中點落在過渡帶的正中央，
        /// 對「亮度整體漂移」最不敏感。實務上光源衰減、產品批次不同都會讓絕對灰階跑掉，
        /// 寫死一個 128 遲早會出事——閾值要嘛跟著影像算，要嘛進 Recipe 並定期校驗。
        /// </summary>
        public static double SuggestThreshold(double[] profile)
        {
            if (profile == null || profile.Length == 0) return 128.0;

            double min = profile[0], max = profile[0];
            for (int i = 1; i < profile.Length; i++)
            {
                if (profile[i] < min) min = profile[i];
                if (profile[i] > max) max = profile[i];
            }
            return (min + max) / 2.0;
        }

        // ─────────────────────────────────────────────────────────────
        // §2／§4 線段量測：一條掃描線量一個尺寸
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 沿水平掃描線量測寬度（教材 §2 的典型流程）。
        /// 回傳的邊緣座標是「影像絕對座標」，可以直接拿去畫圖。
        /// </summary>
        /// <param name="threshold">邊緣閾值；傳 0 或負值表示自動用該線的灰階中點。</param>
        public static EdgePairResult MeasureAlongRow(Mat gray, int y, int x0, int x1, double threshold)
        {
            double[] profile = ExtractRowProfile(gray, y, x0, x1);
            int left = Math.Min(x0, x1);
            return MeasurePair(profile, left, y, threshold);
        }

        /// <summary>沿垂直掃描線量測高度。標定板要量 Y 方向尺寸時用這個。</summary>
        public static EdgePairResult MeasureAlongColumn(Mat gray, int x, int y0, int y1, double threshold)
        {
            double[] profile = ExtractColumnProfile(gray, x, y0, y1);
            int top = Math.Min(y0, y1);
            return MeasurePair(profile, top, x, threshold);
        }

        /// <summary>
        /// 在 ROI 內均勻掃 lineCount 條水平線，各量一次寬度（教材 §5 的前半段）。
        /// 掃描線位置取每一等分的「中間」，避免落在 ROI 的上下邊界上。
        /// </summary>
        public static EdgePairResult[] MeasureMultipleRows(Mat gray, Rectangle roi, int lineCount, double threshold)
        {
            EnsureGray8(gray);
            if (lineCount < 1) throw new ArgumentOutOfRangeException("lineCount", "掃描線數必須至少 1 條。");

            Rectangle r = ClampRoi(roi, gray.Width, gray.Height);
            EdgePairResult[] results = new EdgePairResult[lineCount];

            for (int i = 0; i < lineCount; i++)
            {
                int y = r.Y + (int)Math.Round((i + 0.5) * r.Height / lineCount);
                y = Clamp(y, r.Y, r.Y + r.Height - 1);
                results[i] = MeasureAlongRow(gray, y, r.X, r.X + r.Width - 1, threshold);
            }
            return results;
        }

        /// <summary>
        /// 在 ROI 內均勻掃 lineCount 條「垂直」線，各量一次高度。
        /// 與 MeasureMultipleRows 對稱，標定 Y 方向比例尺時用這個。
        /// </summary>
        public static EdgePairResult[] MeasureMultipleColumns(Mat gray, Rectangle roi, int lineCount, double threshold)
        {
            EnsureGray8(gray);
            if (lineCount < 1) throw new ArgumentOutOfRangeException("lineCount", "掃描線數必須至少 1 條。");

            Rectangle r = ClampRoi(roi, gray.Width, gray.Height);
            EdgePairResult[] results = new EdgePairResult[lineCount];

            for (int i = 0; i < lineCount; i++)
            {
                int x = r.X + (int)Math.Round((i + 0.5) * r.Width / lineCount);
                x = Clamp(x, r.X, r.X + r.Width - 1);
                results[i] = MeasureAlongColumn(gray, x, r.Y, r.Y + r.Height - 1, threshold);
            }
            return results;
        }

        /// <summary>把多線量測的結果整理成長度陣列，只收成功的那幾條，方便丟給 CalcStats。</summary>
        public static double[] LengthsOf(EdgePairResult[] results)
        {
            if (results == null) throw new ArgumentNullException("results");

            int found = 0;
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] != null && results[i].Found) found++;
            }

            double[] lengths = new double[found];
            int k = 0;
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] != null && results[i].Found) lengths[k++] = results[i].LengthPx;
            }
            return lengths;
        }

        /// <summary>從一條 profile 找出成對邊緣，並換算回絕對座標。</summary>
        private static EdgePairResult MeasurePair(double[] profile, int offset, int scanPos, double threshold)
        {
            double th = (threshold > 0.0) ? threshold : SuggestThreshold(profile);

            EdgePairResult result = new EdgePairResult();
            result.ScanPos = scanPos;
            result.ProfileMin = double.MaxValue;
            result.ProfileMax = double.MinValue;
            for (int i = 0; i < profile.Length; i++)
            {
                if (profile[i] < result.ProfileMin) result.ProfileMin = profile[i];
                if (profile[i] > result.ProfileMax) result.ProfileMax = profile[i];
            }

            double first = FindRisingEdge(profile, th);
            double second = FindFallingEdge(profile, th);

            // 兩個邊緣都要找到，而且順序要對，才算量測成功
            if (first == EdgeNotFound || second == EdgeNotFound || second <= first)
            {
                result.Found = false;
                return result;
            }

            result.Found = true;
            result.FirstEdge = offset + first;
            result.SecondEdge = offset + second;
            return result;
        }

        // ─────────────────────────────────────────────────────────────
        // §3 點位量測：找特徵點的精確座標
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 求 ROI 內「亮區」的形心（亞像素），用來當定位孔／圓形特徵的中心。教材 §3。
        ///
        /// 做法：二值化之後算一階動差，形心 =（M10/M00, M01/M00）。
        /// 形心天生就是亞像素的——它是所有前景像素座標的平均，不受單一像素量化影響，
        /// 這是點位量測比「找單一邊緣點」穩定的原因。
        ///
        /// ⚠ ROI 內只能有「一個」目標。二值化後若有兩塊亮區，
        ///   算出來的是兩塊合起來的重心，落在兩者之間的空白處——數字很漂亮但完全沒有意義。
        ///   所以呼叫端要給夠小的搜尋範圍，或先用第 7 章的連通域分析把目標挑出來。
        /// </summary>
        /// <returns>影像絕對座標的形心；ROI 內沒有任何前景像素時回 PointF.Empty。</returns>
        public static PointF FindBrightCentroid(Mat gray, Rectangle roi, double threshold)
        {
            EnsureGray8(gray);

            Rectangle r = ClampRoi(roi, gray.Width, gray.Height);

            // sub 與 gray 共享像素 buffer（第 7 章 ExtractRoi 講過），只在本方法內用完即棄
            using (Mat sub = new Mat(gray, r))
            using (Mat bin = new Mat())
            {
                CvInvoke.Threshold(sub, bin, threshold, 255.0, ThresholdType.Binary);

                // ⚠ Emgu 的 Moments 是「非受管資源的包裝物件」，不是單純的結構——
                //   忘了 Dispose 就是每量一次漏一點，這正是第 19 章的配對釋放鐵律。
                using (Moments m = CvInvoke.Moments(bin, true))
                {
                    if (m.M00 <= 0.0) return PointF.Empty;

                    return new PointF(
                        (float)(r.X + m.M10 / m.M00),
                        (float)(r.Y + m.M01 / m.M00));
                }
            }
        }

        // ─────────────────────────────────────────────────────────────
        // §5 統計
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 計算平均值與標準差（教材 §5）。
        ///
        /// 【為什麼用 n−1 而不是 n】
        ///   除以 n 是「母體標準差」，適用於手上這批就是全部的情況；
        ///   量測拿到的是「從無限多次可能量測中抽出的樣本」，要用樣本標準差（除以 n−1），
        ///   否則會系統性低估離散程度——也就是把量測看得比實際更穩定，
        ///   這在製程能力評估上是往危險方向偏的錯。
        ///   樣本數只有 1 時無從估計離散度，標準差定義為 0。
        /// </summary>
        public static StatResult CalcStats(double[] values)
        {
            if (values == null) throw new ArgumentNullException("values");

            StatResult stat = new StatResult();
            stat.Count = values.Length;
            if (values.Length == 0) return stat;

            double sum = 0.0;
            double min = values[0], max = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
                if (values[i] < min) min = values[i];
                if (values[i] > max) max = values[i];
            }

            stat.Mean = sum / values.Length;
            stat.Min = min;
            stat.Max = max;

            if (values.Length < 2)
            {
                stat.StdDev = 0.0;
                return stat;
            }

            double sumSq = 0.0;
            for (int i = 0; i < values.Length; i++)
            {
                double d = values[i] - stat.Mean;
                sumSq += d * d;
            }
            stat.StdDev = Math.Sqrt(sumSq / (values.Length - 1));
            return stat;
        }

        // ─────────────────────────────────────────────────────────────
        // §4 公差判定
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 判斷量測值是否落在公差帶內：nominal − tolerance ≦ value ≦ nominal + tolerance。
        /// 注意這只回答「這一次量到的數字合不合格」，不回答「這個量測可不可信」——
        /// 後者要看標準差（教材 §5 的情境判斷題）。
        /// </summary>
        public static bool IsWithinTolerance(double value, double nominal, double tolerance)
        {
            return value >= nominal - tolerance && value <= nominal + tolerance;
        }

        // ─────────────────────────────────────────────────────────────
        // 工具
        // ─────────────────────────────────────────────────────────────

        /// <summary>把 ROI 夾回影像範圍內，避免座標越界導致 OpenCV 例外。</summary>
        public static Rectangle ClampRoi(Rectangle roi, int imgWidth, int imgHeight)
        {
            int x = Math.Max(0, Math.Min(roi.X, imgWidth - 1));
            int y = Math.Max(0, Math.Min(roi.Y, imgHeight - 1));
            int w = Math.Max(1, Math.Min(roi.Width, imgWidth - x));
            int h = Math.Max(1, Math.Min(roi.Height, imgHeight - y));
            return new Rectangle(x, y, w, h);
        }

        private static void EnsureGray8(Mat gray)
        {
            if (gray == null) throw new ArgumentNullException("gray");
            if (gray.IsEmpty) throw new ArgumentException("影像是空的。", "gray");
            if (gray.NumberOfChannels != 1 || gray.Depth != DepthType.Cv8U)
                throw new ArgumentException("量測只接受 8-bit 單通道灰階影像。", "gray");
        }

        private static int Clamp(int value, int lo, int hi)
        {
            if (value < lo) return lo;
            if (value > hi) return hi;
            return value;
        }
    }
}

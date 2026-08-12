using System;
using System.Drawing;

namespace SizeMeasurement
{
    /// <summary>
    /// 像素比例尺（Scale）——把「像素距離」換算成「實際尺寸」的唯一橋梁。對應教材 §1。
    ///
    /// 【X、Y 為什麼要分開存】
    ///   理論上正方形像素的相機，X、Y 的 mm/px 應該相同。但鏡頭畸變、感光元件的微小非方形、
    ///   安裝傾斜，都會讓兩者略有不同。公差 50 μm 以內的精密量測一定要分開標定；
    ///   只有公差 0.5 mm 以上的粗略量測，才可以假設兩者相同取平均值。
    ///
    /// 【正式專案怎麼做】
    ///   標定值屬於「機台參數」，必須存進 Recipe／INI（第 16 章）。
    ///   換鏡頭、調焦、動到相機安裝位置之後，一律重新標定。
    ///   本範例為了教學，每次按「執行標定」重算一次，不做持久化——這是刻意的簡化。
    ///
    /// 本類別為不可變（immutable）：標定結果一旦建立就不會被改到，
    /// 要換標定值就換一個新物件，避免「量到一半 Scale 被誰改掉」這種難查的錯。
    /// </summary>
    public class Calibration
    {
        private readonly double _scaleX;
        private readonly double _scaleY;

        /// <param name="scaleXMmPerPx">X 方向比例尺，單位 mm/px。</param>
        /// <param name="scaleYMmPerPx">Y 方向比例尺，單位 mm/px。</param>
        public Calibration(double scaleXMmPerPx, double scaleYMmPerPx)
        {
            if (scaleXMmPerPx <= 0.0 || scaleYMmPerPx <= 0.0)
                throw new ArgumentOutOfRangeException("scaleXMmPerPx", "比例尺必須大於 0。");

            _scaleX = scaleXMmPerPx;
            _scaleY = scaleYMmPerPx;
        }

        /// <summary>X 方向比例尺（mm/px）。</summary>
        public double ScaleX { get { return _scaleX; } }

        /// <summary>Y 方向比例尺（mm/px）。</summary>
        public double ScaleY { get { return _scaleY; } }

        /// <summary>X 方向比例尺（μm/px）。現場講規格時常用微米，順手提供。</summary>
        public double ScaleXMicron { get { return _scaleX * 1000.0; } }

        /// <summary>Y 方向比例尺（μm/px）。</summary>
        public double ScaleYMicron { get { return _scaleY * 1000.0; } }

        /// <summary>
        /// X、Y 比例尺的相對差異（%）。
        /// 這個數字就是「能不能假設 scaleX = scaleY」的判斷依據：
        /// 差幾個百分比，斜向距離就會錯幾個百分比。
        /// </summary>
        public double AnisotropyPercent
        {
            get { return Math.Abs(_scaleY - _scaleX) / _scaleX * 100.0; }
        }

        /// <summary>
        /// 由「正方形標定板」的量測結果建立比例尺。教材 §1 的標準流程：
        ///   拍一片已知尺寸的標定板 → 量出它的像素尺寸 → 相除。
        /// 例：已知 10.000 mm、量到 500 px → 10.000 / 500 = 0.020 mm/px = 20 μm/px。
        /// </summary>
        /// <param name="knownMm">標定板的已知邊長（mm）。</param>
        /// <param name="measuredWidthPx">影像中量到的寬度（px，亞像素）。</param>
        /// <param name="measuredHeightPx">影像中量到的高度（px，亞像素）。</param>
        public static Calibration FromSquareTarget(double knownMm, double measuredWidthPx, double measuredHeightPx)
        {
            if (knownMm <= 0.0)
                throw new ArgumentOutOfRangeException("knownMm", "標定板已知尺寸必須大於 0。");
            if (measuredWidthPx <= 0.0 || measuredHeightPx <= 0.0)
                throw new ArgumentOutOfRangeException("measuredWidthPx", "量到的像素尺寸必須大於 0（標定失敗時不可以建立比例尺）。");

            // X、Y 分開除——這一行就是「分開標定」的全部內容
            return new Calibration(knownMm / measuredWidthPx, knownMm / measuredHeightPx);
        }

        /// <summary>X 方向：像素 → mm。</summary>
        public double ToMmX(double px) { return px * _scaleX; }

        /// <summary>Y 方向：像素 → mm。</summary>
        public double ToMmY(double px) { return px * _scaleY; }

        /// <summary>X 方向：mm → 像素。畫公差帶、把規格換算成像素範圍時要用。</summary>
        public double ToPxX(double mm) { return mm / _scaleX; }

        /// <summary>Y 方向：mm → 像素。</summary>
        public double ToPxY(double mm) { return mm / _scaleY; }

        /// <summary>
        /// 兩點間的實際距離（mm）。教材 §3／§4 的正解。
        ///
        /// 【順序不可以顛倒】dx、dy 必須「各自」換算成 mm 之後才做平方和開根：
        ///     dxMm = dxPx × scaleX
        ///     dyMm = dyPx × scaleY
        ///     dist = √(dxMm² + dyMm²)
        /// 若先對像素座標開根、最後才乘一個 scaleX，等於把 Y 方向也用 X 的比例尺算，
        /// 在 scaleX ≠ scaleY 時結果就是錯的（錯多少見 DistanceMmWrongDemo）。
        /// </summary>
        public double DistanceMm(PointF p1, PointF p2)
        {
            double dxMm = (p2.X - p1.X) * _scaleX;
            double dyMm = (p2.Y - p1.Y) * _scaleY;
            return Math.Sqrt(dxMm * dxMm + dyMm * dyMm);
        }

        /// <summary>
        /// ⚠ 這是「刻意寫錯」的版本，只用來在報告裡跟正解並排對照，正式專案絕對不可以使用。
        ///
        /// 錯法就是教材 §4 自我檢核「找出問題」那一題：
        ///     先對「像素座標」開根，最後才乘一個 scaleX。
        /// 這等於假設 scaleY = scaleX。本範例的測試圖 X、Y 差 2%，
        /// 量出來的斜向距離就會差約 9 μm——公差 ±50 μm（帶寬 100 μm）的話，
        /// 這一個錯誤就佔掉近一成公差帶，而且畫面上完全看不出異狀。
        /// </summary>
        public double DistanceMmWrongDemo(PointF p1, PointF p2)
        {
            double dxPx = p2.X - p1.X;
            double dyPx = p2.Y - p1.Y;
            return Math.Sqrt(dxPx * dxPx + dyPx * dyPx) * _scaleX;   // ← 錯誤就在這一行
        }

        /// <summary>
        /// 兩點連線與水平方向的夾角（度，逆時針為正、影像 Y 軸向下所以直接算會是順時針為正）。
        ///
        /// 角度同樣要先換算成 mm 再算 atan2：X、Y 比例尺不同時，
        /// 直接用像素座標算出來的角度也是錯的（見 AngleDegPxDemo）。
        /// </summary>
        public double AngleDeg(PointF p1, PointF p2)
        {
            double dxMm = (p2.X - p1.X) * _scaleX;
            double dyMm = (p2.Y - p1.Y) * _scaleY;
            return Math.Atan2(dyMm, dxMm) * 180.0 / Math.PI;
        }

        /// <summary>
        /// ⚠ 同樣是「刻意寫錯」的對照版本：直接拿像素座標算 atan2。
        /// scaleX ≠ scaleY 時，影像裡看到的角度並不等於實際角度。
        /// </summary>
        public static double AngleDegPxDemo(PointF p1, PointF p2)
        {
            return Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) * 180.0 / Math.PI;
        }

        public override string ToString()
        {
            return "scaleX = " + _scaleX.ToString("F6") + " mm/px（" + ScaleXMicron.ToString("F3") + " μm/px）"
                 + "，scaleY = " + _scaleY.ToString("F6") + " mm/px（" + ScaleYMicron.ToString("F3") + " μm/px）";
        }
    }
}

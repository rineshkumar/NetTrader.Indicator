using System;

namespace NetTrader.Indicator
{
    public class MACDHistogramData
    {
        public DateTime DataDate { get; set; }
        public double? EmaLineDifference { get; set; }
        public string DataDirection { get; set; }
    }
}

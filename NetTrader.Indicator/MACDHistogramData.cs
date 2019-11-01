using System;

namespace NetTrader.Indicator
{
    public class MACDHistogramData
    {
        internal string isConvergingOrDiverging;

        public DateTime DataDate { get; set; }
        public double? EmaLineDifference { get; set; }
        public string EmaLineDistance { get; set; }

        public bool isDiffereneAmountDecreasing { get; internal set; }

    }
}

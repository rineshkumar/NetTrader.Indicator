using System;

namespace NetTrader.Indicator
{
    public class MACDHistogramData
    {
        public MomentumDirection isConvergingOrDiverging;

        public DateTime DataDate { get; set; }
        public double? EmaLineDifference { get; set; }
        public string EmaLineDistance { get; set; }

        public bool isDiffereneAmountDecreasing { get; internal set; }
        public double? changeInDivergenceMomentum { get; internal set; }
        public double ClosingValue { get; internal set; }

    }
}

using System.Collections.Generic;

namespace NetTrader.Indicator
{
    public class MACDSerie : IIndicatorSerie
    {
        public List<double?> MACDLine { get; set; }
        public List<MACDHistogramData> MACDHistogram { get; set; }
        public List<double?> Signal { get; set; }

        public MACDSerie()
        {
            this.MACDLine = new List<double?>();
            this.MACDHistogram = new List<MACDHistogramData>();
            this.Signal = new List<double?>();
        }
    }
}

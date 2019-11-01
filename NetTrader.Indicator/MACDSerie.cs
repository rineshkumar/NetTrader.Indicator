using System.Collections.Generic;

namespace NetTrader.Indicator
{
    public class MACDSerie : IIndicatorSerie
    {
        public List<double?> MACDLine { get; set; }
        public List<MACDHistogramData> MACDHistogramDataList { get; set; }
        public List<double?> Signal { get; set; }

        public MACDSerie()
        {
            this.MACDLine = new List<double?>();
            this.MACDHistogramDataList = new List<MACDHistogramData>();
            this.Signal = new List<double?>();
        }
    }
}

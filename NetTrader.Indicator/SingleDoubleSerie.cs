using System;
using System.Collections.Generic;

namespace NetTrader.Indicator
{
    public class SingleDoubleSerie : IIndicatorSerie
    {
        public List<double?> Values { get; set; }

        public SingleDoubleSerie()
        {
            Values = new List<double?>();
        }
    }
    public class SingleDoubleSeriesDataPointV2
    {
        public DateTime date { get; set; }
        public double? data { get; set; }
        public List<SignalTypes> SmaIndicatorSignals { get; set; }
    }
    public class SingleDoubleSerieV2 : IIndicatorSerie
    {
        public List<SingleDoubleSeriesDataPointV2> Values { get; set; }

        public SingleDoubleSerieV2()
        {
            Values = new List<SingleDoubleSeriesDataPointV2>();
        }
    }
}

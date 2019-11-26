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
        public List<BuySellSignal> SmaIndicatorSignals { get; set; }

        internal void findSignals(SingleDoubleSeriesDataPointV2 longTermSingleDoubleSeriesData, List<BuySellSignal> signals, double closingValue)
        {
            if (this.data < longTermSingleDoubleSeriesData.data)
            {
                //signals.Add(BuySellSignal.StmaLessThanLtma);
            }
            if (closingValue < longTermSingleDoubleSeriesData.data)
            {
                signals.Add(BuySellSignal.ClosingPriceLessThanLtma);
                if (longTermSingleDoubleSeriesData.data - closingValue > .5)
                {
                    signals.Add(BuySellSignal.StrongDifferenceClosingPriceLtma);
                }
            }

        }
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

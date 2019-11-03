using System;

namespace NetTrader.Indicator
{
    public class RsiDataPoint
    {

        public double? RSI { get; set; }
        public double? RS { get; set; }
        public DateTime Date { get; internal set; }
    }
}
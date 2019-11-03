using System.Collections.Generic;

namespace NetTrader.Indicator
{
    public class RSISerie : IIndicatorSerie
    {

        public List<RsiDataPoint> rsiDataPoints { get; set; }

        public RSISerie()
        {
            this.rsiDataPoints = new List<RsiDataPoint>();
        }
    }
}

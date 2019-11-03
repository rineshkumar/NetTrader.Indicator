using System.Collections.Generic;
using System.Linq;

namespace NetTrader.Indicator
{
    public class MACD : IndicatorCalculatorBase<MACDSerie>
    {
        protected override List<Ohlc> OhlcList { get; set; }
        protected int Fast = 12;
        protected int Slow = 26;
        protected int Signal = 9;
        protected bool Percent = false;

        public MACD()
        {

        }

        public MACD(bool percent)
        {
            this.Percent = percent;
        }

        public MACD(int fast, int slow, int signal)
        {
            this.Fast = fast;
            this.Slow = slow;
            this.Signal = signal;
        }

        public MACD(int fast, int slow, int signal, bool percent)
        {
            this.Fast = fast;
            this.Slow = slow;
            this.Signal = signal;
            this.Percent = percent;
        }

        public override MACDSerie Calculate()
        {
            MACDSerie macdSerie = new MACDSerie();

            EMA ema = new EMA(Fast, false);
            ema.Load(OhlcList);
            List<double?> fastEmaValues = ema.Calculate().Values;

            ema = new EMA(Slow, false);
            ema.Load(OhlcList);
            List<double?> slowEmaValues = ema.Calculate().Values;

            for (int i = 0; i < OhlcList.Count; i++)
            {
                // MACD Line
                if (fastEmaValues[i].HasValue && slowEmaValues[i].HasValue)
                {
                    if (!Percent)
                    {
                        macdSerie.MACDLine.Add(fastEmaValues[i].Value - slowEmaValues[i].Value);
                    }
                    else
                    {
                        // macd <- 100 * ( mavg.fast / mavg.slow - 1 )
                        macdSerie.MACDLine.Add(100 * ((fastEmaValues[i].Value / slowEmaValues[i].Value) - 1));
                    }
                    OhlcList[i].Close = macdSerie.MACDLine[i].Value;
                }
                else
                {
                    macdSerie.MACDLine.Add(null);
                    OhlcList[i].Close = 0.0;
                }
            }

            int zeroCount = macdSerie.MACDLine.Where(x => x == null).Count();
            ema = new EMA(Signal, false);
            ema.Load(OhlcList.Skip(zeroCount).ToList());
            List<double?> signalEmaValues = ema.Calculate().Values;
            for (int i = 0; i < zeroCount; i++)
            {
                signalEmaValues.Insert(0, null);
            }

            // Fill Signal and MACD Histogram lists
            for (int i = 0; i < signalEmaValues.Count; i++)
            {
                macdSerie.Signal.Add(signalEmaValues[i]);

                //macdSerie.MACDHistogram.Add(macdSerie.MACDLine[i] - macdSerie.Signal[i]);
                macdSerie.MACDHistogramDataList.Add(new MACDHistogramData() { DataDate = OhlcList[i].Date, EmaLineDifference = macdSerie.MACDLine[i] - macdSerie.Signal[i] });

            }
            /*
             * 1. Difference between MACD and signal line - For plotting histogram
             * 2. Distance between difference - To find the peak-through- slant pattens 
             * 3. For decreasing pattern we need to find the point when the decrease intensity reduces
             */
            for (int i = 1; i < macdSerie.MACDHistogramDataList.Count; i++)
            {
                if (
                    macdSerie.MACDHistogramDataList.ElementAt(i).EmaLineDifference != null &&
                    macdSerie.MACDHistogramDataList.ElementAt(i - 1).EmaLineDifference != null)
                {
                    SetConvergenceDivergence(macdSerie.MACDHistogramDataList.ElementAt(i - 1), macdSerie.MACDHistogramDataList.ElementAt(i));
                    SetChangeInMomentum(macdSerie.MACDHistogramDataList.ElementAt(i - 1), macdSerie.MACDHistogramDataList.ElementAt(i));
                    /* For ith Element 
                     * (i-1) - (1) < (i-2)-(i-1) 
                     */
                    macdSerie.MACDHistogramDataList.ElementAt(i).isDiffereneAmountDecreasing =
                        (
                        macdSerie.MACDHistogramDataList.ElementAt(i - 1).EmaLineDifference -
                        macdSerie.MACDHistogramDataList.ElementAt(i).EmaLineDifference) <
                        (
                        macdSerie.MACDHistogramDataList.ElementAt(i - 2).EmaLineDifference -
                        macdSerie.MACDHistogramDataList.ElementAt(i - 1).EmaLineDifference
                        ) ? true : false;

                }
            }

            return macdSerie;
        }

        private void SetChangeInMomentum(MACDHistogramData previousElement, MACDHistogramData currentElement)
        {
            currentElement.changeInDivergenceMomentum = previousElement.EmaLineDifference - currentElement.EmaLineDifference;

        }

        private void SetConvergenceDivergence(MACDHistogramData previousElement, MACDHistogramData currentElement)
        {
            var currentDataPoint = currentElement.EmaLineDifference;
            var previousDataPoint = previousElement.EmaLineDifference;
            if (previousDataPoint > 0)
            {
                if (currentDataPoint > 0)
                {
                    if (currentDataPoint > previousDataPoint)
                    {
                        currentElement.isConvergingOrDiverging = "PositiveDiverging";
                    }
                    else if (currentDataPoint < previousDataPoint)
                    {
                        currentElement.isConvergingOrDiverging = "PositiveConverging";
                    }
                }
                else if (currentDataPoint == 0)
                {
                    currentElement.isConvergingOrDiverging = "PositiveConverging";
                }
                else if (currentDataPoint < 0)
                {
                    currentElement.isConvergingOrDiverging = "NegativeDiverging";
                }

            }
            else if (previousDataPoint == 0)
            {

                if (currentDataPoint > 0)
                {
                    currentElement.isConvergingOrDiverging = "PositiveDiverging";

                }
                else if (currentDataPoint == 0)
                {
                    currentElement.isConvergingOrDiverging = "ZeroDivergence";
                }
                else if (currentDataPoint < 0)
                {
                    currentElement.isConvergingOrDiverging = "NegativeDiverging";
                }

            }
            else if (previousDataPoint < 0)
            {

                if (currentDataPoint > 0)
                {
                    currentElement.isConvergingOrDiverging = "PositiveDiverging";

                }
                else if (currentDataPoint == 0)
                {
                    currentElement.isConvergingOrDiverging = "NegativeConverging";
                }
                else if (currentDataPoint < 0)
                {
                    if (currentDataPoint > previousDataPoint)
                    {
                        currentElement.isConvergingOrDiverging = "NegativeConverging";
                    }
                    else if (currentDataPoint < previousDataPoint)
                    {
                        currentElement.isConvergingOrDiverging = "NegativeDiverging";
                    }
                }

            }


        }
    }
}

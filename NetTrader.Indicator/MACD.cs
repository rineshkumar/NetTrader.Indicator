using System;
using System.Collections.Generic;
using System.Linq;

namespace NetTrader.Indicator
{
    public enum MarketSentiment
    {
        Bearish,
        Bullish,
        Unknown
    }
    public enum SignalTypes
    {
        BuyWithUpperLimitSet = 2
    }
    public enum MomentumDirection
    {
        PositiveConvergence,
        PositiveDivergence,
        NegativeConvergence,
        NegativeDivergence,
        NoChange,
        PositiveToNegativeDivergence,
        NegativeToPositiveDivergence
    }
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
                macdSerie.MACDHistogramDataList.Add(new MACDHistogramData() { DataDate = OhlcList[i].Date, EmaLineDifference = macdSerie.MACDLine[i] - macdSerie.Signal[i], ClosingValue = OhlcList[i].AdjClose });

            }
            /*
             * 1. Difference between MACD and signal line - For plotting histogram
             * 2. Distance between difference - To find the peak-through- slant pattens 
             * 3. For decreasing pattern we need to find the point when the decrease intensity reduces
             */
            for (int i = 1; i < macdSerie.MACDHistogramDataList.Count; i++)
            {
                MACDHistogramData currentElement = macdSerie.MACDHistogramDataList.ElementAt(i);
                MACDHistogramData previousElement = macdSerie.MACDHistogramDataList.ElementAt(i - 1);
                if (
                    currentElement.EmaLineDifference != null &&
                    previousElement.EmaLineDifference != null)
                {
                    SetConvergenceDivergence(previousElement, currentElement);
                    SetChangeInMomentum(previousElement, currentElement);
                    /* For ith Element 
                     * (i-1) - (1) < (i-2)-(i-1) 
                     */
                    //macdSerie.MACDHistogramDataList.ElementAt(i).isDiffereneAmountDecreasing =
                    //    (
                    //    macdSerie.MACDHistogramDataList.ElementAt(i - 1).EmaLineDifference -
                    //    macdSerie.MACDHistogramDataList.ElementAt(i).EmaLineDifference) <
                    //    (
                    //    macdSerie.MACDHistogramDataList.ElementAt(i - 2).EmaLineDifference -
                    //    macdSerie.MACDHistogramDataList.ElementAt(i - 1).EmaLineDifference
                    //    ) ? true : false;
                    if (currentElement.changeInDivergenceMomentum != null && previousElement.changeInDivergenceMomentum != null)
                    {
                        currentElement.isDiffereneAmountDecreasing =

                        currentElement.changeInDivergenceMomentum.Value <
                        previousElement.changeInDivergenceMomentum.Value;
                    }
                    SetSignalType(macdSerie, i);

                }
            }

            return macdSerie;
        }

        private static void SetSignalType(MACDSerie macdSerie, int i)
        {

            if (GetMarketSentiment(macdSerie, i) == MarketSentiment.Bearish
                && IsBullishTrendContinuing(macdSerie, i) && IsTrendEnding(macdSerie, i) /*&& macdSerie.MACDHistogramDataList.ElementAt(i).EmaLineDifference < -.3*/)
            {
                macdSerie.MACDHistogramDataList.ElementAt(i).ActionSignal = SignalTypes.BuyWithUpperLimitSet;
            }
            //else if (IsBullishTrendStarting(macdSerie.MACDHistogramDataList.ElementAt(i)))
            //{
            //    macdSerie.MACDHistogramDataList.ElementAt(i).ActionSignal = SignalTypes.BuyWithUpperLimitSet;
            //}
        }

        private static bool IsBullishTrendStarting(MACDHistogramData currentElement)
        {
            return currentElement.isConvergingOrDiverging == MomentumDirection.PositiveToNegativeDivergence;
        }

        private static bool IsBullishTrendContinuing(MACDSerie macdSerie, int i)
        {
            return macdSerie.MACDHistogramDataList.ElementAt(i).isConvergingOrDiverging == MomentumDirection.NegativeDivergence;
        }

        private static bool IsTrendEnding(MACDSerie macdSerie, int i)
        {
            return macdSerie.MACDHistogramDataList.ElementAt(i).isDiffereneAmountDecreasing;
        }

        private static MarketSentiment GetMarketSentiment(MACDSerie macdSerie, int i)
        {
            MarketSentiment marketSentiment = MarketSentiment.Unknown;
            switch (macdSerie.MACDHistogramDataList.ElementAt(i).isConvergingOrDiverging)
            {
                case MomentumDirection.NegativeDivergence:
                case MomentumDirection.NegativeConvergence:
                    marketSentiment = MarketSentiment.Bearish;
                    break;

                case MomentumDirection.PositiveDivergence:
                case MomentumDirection.PositiveConvergence:
                case MomentumDirection.NegativeToPositiveDivergence:
                    marketSentiment = MarketSentiment.Bullish;
                    break;
            }
            return marketSentiment;
        }

        private void SetChangeInMomentum(MACDHistogramData previousElement, MACDHistogramData currentElement)
        {
            currentElement.changeInDivergenceMomentum = Math.Abs(previousElement.EmaLineDifference.Value - currentElement.EmaLineDifference.Value);

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
                        currentElement.isConvergingOrDiverging = MomentumDirection.PositiveDivergence;
                    }
                    else if (currentDataPoint < previousDataPoint)
                    {
                        currentElement.isConvergingOrDiverging = MomentumDirection.PositiveConvergence;
                    }
                }
                else if (currentDataPoint == 0)
                {
                    currentElement.isConvergingOrDiverging = MomentumDirection.PositiveConvergence; ;
                }
                else if (currentDataPoint < 0)
                {
                    currentElement.isConvergingOrDiverging = MomentumDirection.PositiveToNegativeDivergence;
                }

            }
            else if (previousDataPoint == 0)
            {

                if (currentDataPoint > 0)
                {
                    currentElement.isConvergingOrDiverging = MomentumDirection.PositiveDivergence;

                }
                else if (currentDataPoint == 0)
                {
                    currentElement.isConvergingOrDiverging = MomentumDirection.NoChange;
                }
                else if (currentDataPoint < 0)
                {
                    currentElement.isConvergingOrDiverging = MomentumDirection.NegativeDivergence;
                }

            }
            else if (previousDataPoint < 0)
            {

                if (currentDataPoint > 0)
                {
                    currentElement.isConvergingOrDiverging = MomentumDirection.NegativeToPositiveDivergence;

                }
                else if (currentDataPoint == 0)
                {
                    currentElement.isConvergingOrDiverging = MomentumDirection.NegativeConvergence;
                }
                else if (currentDataPoint < 0)
                {
                    if (currentDataPoint > previousDataPoint)
                    {
                        currentElement.isConvergingOrDiverging = MomentumDirection.NegativeConvergence; ;
                    }
                    else if (currentDataPoint < previousDataPoint)
                    {
                        currentElement.isConvergingOrDiverging = MomentumDirection.NegativeDivergence;
                    }
                }

            }


        }
    }
}

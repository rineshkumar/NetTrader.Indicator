﻿namespace NetTrader.Indicator
{
    public enum BuySellSignal
    {
        MacdBuyWithUpperLimitSet = 2,
        StmaLessThanLtma = 3,
        ClosingPriceLessThanLtma = 4,
        MacdNegativeBelowThree = 5,
        StrongDifferenceClosingPriceLtma = 6
    }
}

namespace SoftBook.Application
{
    /// <summary>
    /// Controls whether calculated add price is written to tab_party_master.rate_diff.
    /// When false (default): calculation is virtual only; stored rate_diff is used for sales orders.
    /// Set to true when ready to auto-persist calculated add price to the database.
    /// </summary>
    public static class PartyRateAdjustmentOptions
    {
        public const bool AutoPersistRateDiff = false;
    }
}

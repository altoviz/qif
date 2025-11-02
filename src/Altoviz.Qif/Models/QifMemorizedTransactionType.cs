namespace Altoviz.Qif.Models
{

    /// <summary>
    /// Represents the transaction type for memorized transactions
    /// </summary>
    public enum QifMemorizedTransactionType
    {
        /// <summary>
        /// Check transaction
        /// </summary>
        Check,

        /// <summary>
        /// Deposit transaction
        /// </summary>
        Deposit,

        /// <summary>
        /// Payment transaction
        /// </summary>
        Payment,

        /// <summary>
        /// Investment transaction
        /// </summary>
        Investment,

        /// <summary>
        /// Electronic payee transaction
        /// </summary>
        ElectronicPayee
    }
}
namespace Altoviz.Qif.Models
{

    /// <summary>
    /// Represents the different account types supported by QIF format
    /// </summary>
    public enum QifAccountType
    {
        /// <summary>
        /// Bank account transactions
        /// </summary>
        Bank,

        /// <summary>
        /// Cash account transactions
        /// </summary>
        Cash,

        /// <summary>
        /// Credit card account transactions
        /// </summary>
        CreditCard,

        /// <summary>
        /// Investment account transactions
        /// </summary>
        Investment,

        /// <summary>
        /// Asset account transactions
        /// </summary>
        OtherAsset,

        /// <summary>
        /// Liability account transactions
        /// </summary>
        OtherLiability,

        /// <summary>
        /// Account list
        /// </summary>
        Account,

        /// <summary>
        /// Category list
        /// </summary>
        Category,

        /// <summary>
        /// Class list
        /// </summary>
        Class,

        /// <summary>
        /// Memorized transaction list
        /// </summary>
        Memorized
    }
}
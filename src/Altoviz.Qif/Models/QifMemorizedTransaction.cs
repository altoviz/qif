namespace Altoviz.Qif.Models
{

    /// <summary>
    /// Represents a memorized transaction in QIF format
    /// </summary>
    public class QifMemorizedTransaction : QifTransactionBase
    {
        /// <summary>
        /// Type of memorized transaction
        /// </summary>
        public QifMemorizedTransactionType TransactionType { get; set; }

        /// <summary>
        /// Amortization information (optional)
        /// </summary>
        public QifAmortization? Amortization { get; set; }
        /// <summary>
        /// Returns a string representation of the QifMemorizedTransaction
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Type: {TransactionType}, Amount: {Amount:C}, Payee: {Payee}, Category: {Category}";
        }
    }
}
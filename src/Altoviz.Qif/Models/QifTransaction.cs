namespace Altoviz.Qif.Models
{

    /// <summary>
    /// Represents a non-investment QIF transaction (Bank, Cash, Credit Card, Asset, Liability accounts)
    /// </summary>
    public class QifTransaction : QifTransactionBase
    {
        /// <summary>
        /// Returns a string representation of the transaction
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Date: {Date:d}, Amount: {Amount:C}, Payee: {Payee}, Category: {Category}";
        }
    }
}
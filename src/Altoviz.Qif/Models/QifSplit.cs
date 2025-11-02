namespace Altoviz.Qif.Models
{

    /// <summary>
    /// Represents a split transaction entry within a QIF transaction
    /// </summary>
    public class QifSplit
    {
        /// <summary>
        /// Category in split (Category/Transfer/Class)
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Memo in split
        /// </summary>
        public string? Memo { get; set; }

        /// <summary>
        /// Dollar amount of split
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Returns a string representation of the QifSplit
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Category: {Category}, Amount: {Amount:C}, Memo: {Memo}";
        }
    }
}
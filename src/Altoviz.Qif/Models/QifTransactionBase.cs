using System;
using System.Collections.Generic;

namespace Altoviz.Qif.Models
{

    /// <summary>
    /// Base class for all QIF transactions
    /// </summary>
    public abstract class QifTransactionBase
    {
        /// <summary>
        /// Transaction date
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Cleared status
        /// </summary>
        public QifClearedStatus ClearedStatus { get; set; }

        /// <summary>
        /// Check or reference number
        /// </summary>
        public string? Number { get; set; }

        /// <summary>
        /// Payee
        /// </summary>
        public string? Payee { get; set; }

        /// <summary>
        /// Memo
        /// </summary>
        public string? Memo { get; set; }

        /// <summary>
        /// Address lines (up to 6 lines)
        /// </summary>
        public List<string> Address { get; set; } = new List<string>();

        /// <summary>
        /// Category (Category/Subcategory/Transfer/Class)
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Split transactions
        /// </summary>
        public List<QifSplit> Splits { get; set; } = new List<QifSplit>();
    }
}
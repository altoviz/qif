using System;

namespace Altoviz.Qif.Models
{
    /// <summary>
    /// Represents account information in QIF format
    /// </summary>
    public class QifAccount
    {
        /// <summary>
        /// Account name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Type of account
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Account description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Credit limit (only for credit card accounts)
        /// </summary>
        public decimal? CreditLimit { get; set; }

        /// <summary>
        /// Statement balance date
        /// </summary>
        public DateTime? StatementBalanceDate { get; set; }

        /// <summary>
        /// Statement balance
        /// </summary>
        public decimal? StatementBalance { get; set; }
        /// <summary>
        /// Returns a string representation of the QifAccount
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Name: {Name}, Type: {Type}, Balance: {StatementBalance:C}";
        }
    }
}
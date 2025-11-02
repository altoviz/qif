using System.Collections.Generic;

namespace Altoviz.Qif.Models
{

    /// <summary>
    /// Represents a complete QIF file with all its data
    /// </summary>
    public class QifDocument
    {
        /// <summary>
        /// The type of account/data in this QIF document
        /// </summary>
        public QifAccountType AccountType { get; set; }

        /// <summary>
        /// Whether to import all transfers regardless of settings
        /// </summary>
        public bool ImportAllTransfers { get; set; }

        /// <summary>
        /// Regular transactions (for Bank, Cash, Credit Card, Asset, Liability accounts)
        /// </summary>
        public List<QifTransaction> Transactions { get; set; } = new List<QifTransaction>();

        /// <summary>
        /// Investment transactions
        /// </summary>
        public List<QifInvestmentTransaction> InvestmentTransactions { get; set; } = new List<QifInvestmentTransaction>();

        /// <summary>
        /// Memorized transactions
        /// </summary>
        public List<QifMemorizedTransaction> MemorizedTransactions { get; set; } = new List<QifMemorizedTransaction>();

        /// <summary>
        /// Account information
        /// </summary>
        public List<QifAccount> Accounts { get; set; } = new List<QifAccount>();

        /// <summary>
        /// Categories
        /// </summary>
        public List<QifCategory> Categories { get; set; } = new List<QifCategory>();

        /// <summary>
        /// Classes
        /// </summary>
        public List<QifClass> Classes { get; set; } = new List<QifClass>();

        /// <summary>
        /// Gets the total number of items in the document
        /// </summary>
        public int TotalItems => Transactions.Count + InvestmentTransactions.Count +
                                MemorizedTransactions.Count + Accounts.Count +
                                Categories.Count + Classes.Count;
        /// <summary>
        /// Returns a string representation of the QIF document
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"QIF Document - Type: {AccountType}, Total Items: {TotalItems}";
        }
    }
}
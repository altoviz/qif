namespace Altoviz.Qif.Models
{

    /// <summary>
    /// Represents a category in QIF format
    /// </summary>
    public class QifCategory
    {
        /// <summary>
        /// Category name (may include subcategory separated by colon)
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Category description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Whether the category is tax-related
        /// </summary>
        public bool IsTaxRelated { get; set; }

        /// <summary>
        /// Whether this is an income category (false = expense)
        /// </summary>
        public bool IsIncomeCategory { get; set; }

        /// <summary>
        /// Budget amount (only in Budget Amounts QIF file)
        /// </summary>
        public decimal? BudgetAmount { get; set; }

        /// <summary>
        /// Tax schedule information
        /// </summary>
        public string? TaxSchedule { get; set; }
        /// <summary>
        /// Returns a string representation of the QifCategory
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var type = IsIncomeCategory ? "Income" : "Expense";
            return $"Name: {Name}, Type: {type}, Description: {Description}";
        }
    }
}
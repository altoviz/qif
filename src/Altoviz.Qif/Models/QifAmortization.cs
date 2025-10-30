namespace Altoviz.Qif.Models;

/// <summary>
/// Represents amortization information for memorized transactions
/// </summary>
public class QifAmortization
{
    /// <summary>
    /// First payment date
    /// </summary>
    public DateTime? FirstPaymentDate { get; set; }
    
    /// <summary>
    /// Total years for loan
    /// </summary>
    public int? TotalYears { get; set; }
    
    /// <summary>
    /// Number of payments already made
    /// </summary>
    public int? PaymentsMade { get; set; }
    
    /// <summary>
    /// Number of periods per year
    /// </summary>
    public int? PeriodsPerYear { get; set; }
    
    /// <summary>
    /// Interest rate
    /// </summary>
    public decimal? InterestRate { get; set; }
    
    /// <summary>
    /// Current loan balance
    /// </summary>
    public decimal? CurrentBalance { get; set; }
    
    /// <summary>
    /// Original loan amount
    /// </summary>
    public decimal? OriginalAmount { get; set; }
}
namespace Altoviz.Qif.Models;

/// <summary>
/// Represents an investment transaction in QIF format
/// </summary>
public class QifInvestmentTransaction : QifTransactionBase
{
    /// <summary>
    /// Investment action (Buy, Sell, etc.)
    /// </summary>
    public string? Action { get; set; }
    
    /// <summary>
    /// Security name
    /// </summary>
    public string? Security { get; set; }
    
    /// <summary>
    /// Price per share
    /// </summary>
    public decimal? Price { get; set; }
    
    /// <summary>
    /// Quantity (number of shares or split ratio)
    /// </summary>
    public decimal? Quantity { get; set; }
    
    /// <summary>
    /// Commission
    /// </summary>
    public decimal? Commission { get; set; }
    
    /// <summary>
    /// Account for the transfer
    /// </summary>
    public string? TransferAccount { get; set; }
    
    /// <summary>
    /// Amount transferred
    /// </summary>
    public decimal? TransferAmount { get; set; }
    
    public override string ToString()
    {
        return $"Date: {Date:d}, Action: {Action}, Security: {Security}, Quantity: {Quantity}, Price: {Price:C}";
    }
}
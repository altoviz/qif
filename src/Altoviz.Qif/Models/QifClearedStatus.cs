namespace Altoviz.Qif.Models;

/// <summary>
/// Represents the cleared status of a transaction
/// </summary>
public enum QifClearedStatus
{
    /// <summary>
    /// Not specified or uncleared
    /// </summary>
    None,
    
    /// <summary>
    /// Cleared transaction
    /// </summary>
    Cleared,
    
    /// <summary>
    /// Reconciled transaction
    /// </summary>
    Reconciled
}
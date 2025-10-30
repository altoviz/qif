namespace Altoviz.Qif.Models;

/// <summary>
/// Represents a non-investment QIF transaction (Bank, Cash, Credit Card, Asset, Liability accounts)
/// </summary>
public class QifTransaction : QifTransactionBase
{
    public override string ToString()
    {
        return $"Date: {Date:d}, Amount: {Amount:C}, Payee: {Payee}, Category: {Category}";
    }
}
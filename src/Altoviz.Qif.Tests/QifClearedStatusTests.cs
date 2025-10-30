using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifClearedStatusTests
{
    private readonly QifParser _parser = new();

    [Theory]
    [InlineData("C", QifClearedStatus.Cleared)]
    [InlineData("c", QifClearedStatus.Cleared)]
    [InlineData("CLEARED", QifClearedStatus.Cleared)]
    [InlineData("X", QifClearedStatus.Cleared)]
    [InlineData("R", QifClearedStatus.Reconciled)]
    [InlineData("r", QifClearedStatus.Reconciled)]
    [InlineData("RECONCILED", QifClearedStatus.Reconciled)]
    [InlineData("", QifClearedStatus.None)]
    [InlineData("Unknown", QifClearedStatus.None)]
    public void ParseString_WithVariousClearedStatuses_ParsesCorrectly(string clearedString, QifClearedStatus expectedStatus)
    {
        var qifContent = $"""
            !Type:Bank
            D1/1/2023
            T100.00
            C{clearedString}
            PTest
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.Transactions);
        var transaction = result.Transactions[0];
        Assert.Equal(expectedStatus, transaction.ClearedStatus);
    }
}
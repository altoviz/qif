using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifDecimalParsingTests
{
    private readonly QifParser _parser = new();

    [Theory]
    [InlineData("100.00", 100.00)]
    [InlineData("-50.25", -50.25)]
    [InlineData("1,234.56", 1234.56)]
    [InlineData("$100.00", 100.00)]
    [InlineData("$-50.25", -50.25)]
    [InlineData("$1,234.56", 1234.56)]
    [InlineData("0", 0)]
    [InlineData(".50", 0.50)]
    [InlineData("-.75", -0.75)]
    public void ParseString_WithVariousAmountFormats_ParsesCorrectly(string amountString, decimal expectedAmount)
    {
        var qifContent = $"""
            !Type:Bank
            D1/1/2023
            T{amountString}
            PTest
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.Transactions);
        var transaction = result.Transactions[0];
        Assert.Equal(expectedAmount, transaction.Amount);
    }

    [Fact]
    public void ParseString_WithInvalidAmount_SetsAmountToNull()
    {
        var qifContent = """
            !Type:Bank
            D1/1/2023
            TInvalidAmount
            PTest
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.Transactions);
        Assert.Null(result.Transactions[0].Amount);
    }

    [Fact]
    public void ParseString_WithEmptyAmount_SetsAmountToNull()
    {
        var qifContent = """
            !Type:Bank
            D1/1/2023
            T
            PTest
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.Transactions);
        Assert.Null(result.Transactions[0].Amount);
    }
}
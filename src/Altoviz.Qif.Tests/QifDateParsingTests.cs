using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifDateParsingTests
{
    private readonly QifParser _parser = new();

    [Theory]
    [InlineData("6/1/94", 1994, 6, 1)]
    [InlineData("12/25/2023", 2023, 12, 25)]
    [InlineData("1/1/00", 2000, 1, 1)]
    [InlineData("6/1/'94", 1994, 6, 1)]
    [InlineData("06/01/94", 1994, 6, 1)]
    [InlineData("6-1-94", 1994, 6, 1)]
    [InlineData("6.1.94", 1994, 6, 1)]
    public void ParseString_WithVariousDateFormats_ParsesCorrectly(string dateString, int expectedYear, int expectedMonth, int expectedDay)
    {
        var qifContent = $"""
            !Type:Bank
            D{dateString}
            T100.00
            PTest
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.Transactions);
        var transaction = result.Transactions[0];
        Assert.NotNull(transaction.Date);
        Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay), transaction.Date);
    }

    [Fact]
    public void ParseString_WithInvalidDate_SetsDateToNull()
    {
        var qifContent = """
            !Type:Bank
            DInvalidDate
            T100.00
            PTest
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.Transactions);
        Assert.Null(result.Transactions[0].Date);
    }

    [Fact]
    public void ParseString_WithEmptyDate_SetsDateToNull()
    {
        var qifContent = """
            !Type:Bank
            D
            T100.00
            PTest
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.Transactions);
        Assert.Null(result.Transactions[0].Date);
    }
}
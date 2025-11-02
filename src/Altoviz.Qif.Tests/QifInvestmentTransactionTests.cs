using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifInvestmentTransactionTests
{
    private readonly QifReader _parser = new();

    [Fact]
    public void ReadString_WithInvestmentTransaction_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Invst
            D8/25/93
            NShrsIn
            Yibm4
            I11.260
            Q88.81
            CX
            T1,000.00
            MOpening Balance
            ^
            """;
        
        var result = _parser.ReadString(qifContent);
        
        Assert.Equal(QifAccountType.Investment, result.AccountType);
        Assert.Single(result.InvestmentTransactions);
        
        var transaction = result.InvestmentTransactions[0];
        Assert.Equal(new DateTime(1993, 8, 25), transaction.Date);
        Assert.Equal("ShrsIn", transaction.Action);
        Assert.Equal("ibm4", transaction.Security);
        Assert.Equal(11.260m, transaction.Price);
        Assert.Equal(88.81m, transaction.Quantity);
        Assert.Equal(QifClearedStatus.Cleared, transaction.ClearedStatus);
        Assert.Equal(1000.00m, transaction.Amount);
        Assert.Equal("Opening Balance", transaction.Memo);
    }

    [Fact]
    public void ReadString_WithInvestmentBuyTransaction_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Invst
            D8/25/93
            NBuyX
            Yibm4
            I11.030
            Q9.066
            T100.00
            MEst. price as of 8/25/93
            L[CHECKING]
            $100.00
            ^
            """;
        
        var result = _parser.ReadString(qifContent);
        
        Assert.Single(result.InvestmentTransactions);
        var transaction = result.InvestmentTransactions[0];
        
        Assert.Equal("BuyX", transaction.Action);
        Assert.Equal("ibm4", transaction.Security);
        Assert.Equal(11.030m, transaction.Price);
        Assert.Equal(9.066m, transaction.Quantity);
        Assert.Equal(100.00m, transaction.Amount);
        Assert.Equal("Est. price as of 8/25/93", transaction.Memo);
        Assert.Equal("[CHECKING]", transaction.TransferAccount);
        Assert.Equal(100.00m, transaction.TransferAmount);
    }

    [Fact]
    public void ReadString_WithInvestmentCommission_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Invst
            D8/25/93
            NBuy
            Yibm4
            I10.00
            Q100
            O25.00
            T1,025.00
            ^
            """;
        
        var result = _parser.ReadString(qifContent);
        
        Assert.Single(result.InvestmentTransactions);
        var transaction = result.InvestmentTransactions[0];
        
        Assert.Equal(25.00m, transaction.Commission);
        Assert.Equal(1025.00m, transaction.Amount);
    }
}
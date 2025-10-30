using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifAccountTests
{
    private readonly QifParser _parser = new();

    [Fact]
    public void ParseString_WithAccountInformation_ParsesCorrectly()
    {
        var qifContent = """
            !Account
            NChecking Account
            TBank
            DMain checking account
            L5000.00
            /12/31/2023
            $2500.00
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.Accounts);
        var account = result.Accounts[0];
        
        Assert.Equal("Checking Account", account.Name);
        Assert.Equal("Bank", account.Type);
        Assert.Equal("Main checking account", account.Description);
        Assert.Equal(5000.00m, account.CreditLimit);
        Assert.Equal(new DateTime(2023, 12, 31), account.StatementBalanceDate);
        Assert.Equal(2500.00m, account.StatementBalance);
    }

    [Fact]
    public void ParseString_WithCreditCardAccount_ParsesCorrectly()
    {
        var qifContent = """
            !Account
            NVisa Card
            TCredit Card
            DMain credit card
            L10000.00
            /11/30/2023
            $-1250.75
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.Accounts);
        var account = result.Accounts[0];
        
        Assert.Equal("Visa Card", account.Name);
        Assert.Equal("Credit Card", account.Type);
        Assert.Equal(10000.00m, account.CreditLimit);
        Assert.Equal(-1250.75m, account.StatementBalance);
    }

    [Fact]
    public void ParseString_WithMinimalAccountInfo_ParsesCorrectly()
    {
        var qifContent = """
            !Account
            NSavings
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.Accounts);
        var account = result.Accounts[0];
        
        Assert.Equal("Savings", account.Name);
        Assert.Null(account.Type);
        Assert.Null(account.Description);
        Assert.Null(account.CreditLimit);
        Assert.Null(account.StatementBalanceDate);
        Assert.Null(account.StatementBalance);
    }
}
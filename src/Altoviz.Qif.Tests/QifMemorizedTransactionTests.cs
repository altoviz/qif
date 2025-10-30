using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifMemorizedTransactionTests
{
    private readonly QifParser _parser = new();

    [Fact]
    public void ParseString_WithMemorizedCheckTransaction_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Memorized
            T-50.00
            POakwood Gardens
            MRent
            KC
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Equal(QifAccountType.Memorized, result.AccountType);
        Assert.Single(result.MemorizedTransactions);
        
        var transaction = result.MemorizedTransactions[0];
        Assert.Equal(-50.00m, transaction.Amount);
        Assert.Equal("Oakwood Gardens", transaction.Payee);
        Assert.Equal("Rent", transaction.Memo);
        Assert.Equal(QifMemorizedTransactionType.Check, transaction.TransactionType);
    }

    [Fact]
    public void ParseString_WithMemorizedDepositTransaction_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Memorized
            T500.00
            PSalary Deposit
            KD
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.MemorizedTransactions);
        var transaction = result.MemorizedTransactions[0];
        
        Assert.Equal(500.00m, transaction.Amount);
        Assert.Equal("Salary Deposit", transaction.Payee);
        Assert.Equal(QifMemorizedTransactionType.Deposit, transaction.TransactionType);
    }

    [Fact]
    public void ParseString_WithMemorizedTransactionWithAmortization_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Memorized
            T-1200.00
            PMortgage Payment
            MMortgage
            18/1/2023
            230
            312
            412
            56.5
            6150000.00
            7200000.00
            KC
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Single(result.MemorizedTransactions);
        var transaction = result.MemorizedTransactions[0];
        
        Assert.NotNull(transaction.Amortization);
        Assert.Equal(new DateTime(2023, 8, 1), transaction.Amortization.FirstPaymentDate);
        Assert.Equal(30, transaction.Amortization.TotalYears);
        Assert.Equal(12, transaction.Amortization.PaymentsMade);
        Assert.Equal(12, transaction.Amortization.PeriodsPerYear);
        Assert.Equal(6.5m, transaction.Amortization.InterestRate);
        Assert.Equal(150000.00m, transaction.Amortization.CurrentBalance);
        Assert.Equal(200000.00m, transaction.Amortization.OriginalAmount);
    }

    [Fact]
    public void ParseString_WithAllMemorizedTransactionTypes_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Memorized
            T-50.00
            PCheck Transaction
            KC
            ^
            T500.00
            PDeposit Transaction
            KD
            ^
            T-100.00
            PPayment Transaction
            KP
            ^
            T1000.00
            PInvestment Transaction
            KI
            ^
            T-75.00
            PElectronic Payee Transaction
            KE
            ^
            """;
        
        var result = _parser.ParseString(qifContent);
        
        Assert.Equal(5, result.MemorizedTransactions.Count);
        
        Assert.Equal(QifMemorizedTransactionType.Check, result.MemorizedTransactions[0].TransactionType);
        Assert.Equal(QifMemorizedTransactionType.Deposit, result.MemorizedTransactions[1].TransactionType);
        Assert.Equal(QifMemorizedTransactionType.Payment, result.MemorizedTransactions[2].TransactionType);
        Assert.Equal(QifMemorizedTransactionType.Investment, result.MemorizedTransactions[3].TransactionType);
        Assert.Equal(QifMemorizedTransactionType.ElectronicPayee, result.MemorizedTransactions[4].TransactionType);
    }
}
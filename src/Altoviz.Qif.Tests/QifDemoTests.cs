using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;
using Xunit.Abstractions;

namespace Altoviz.Qif.Tests;

public class QifDemoTests
{
    private readonly ITestOutputHelper _output;
    private readonly QifParser _parser = new();
    private readonly string _filesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "files");

    public QifDemoTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task ParseSampleQifFile_AndDisplaySignificantValues()
    {
        // Arrange
        var sampleFilePath = Path.Combine(_filesDirectory, "sample.qif");
        
        // Act
        var document = await _parser.ParseFileAsync(sampleFilePath);
        
        // Assert and Display Results
        Assert.NotNull(document);
        Assert.True(document.TotalItems > 0);
        
        _output.WriteLine("=== QIF Demo - Sample File Analysis ===");
        _output.WriteLine($"Account Type: {document.AccountType}");
        _output.WriteLine($"Total Transactions: {document.Transactions.Count}");
        _output.WriteLine($"Total Items: {document.TotalItems}");
        _output.WriteLine("");

        // Calculate summary statistics
        var transactions = document.Transactions;
        var totalIncome = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount ?? 0);
        var totalExpenses = transactions.Where(t => t.Amount < 0).Sum(t => t.Amount ?? 0);
        var netAmount = transactions.Sum(t => t.Amount ?? 0);
        
        _output.WriteLine("=== Financial Summary ===");
        _output.WriteLine($"Total Income: {totalIncome:C}");
        _output.WriteLine($"Total Expenses: {Math.Abs(totalExpenses):C}");
        _output.WriteLine($"Net Amount: {netAmount:C}");
        _output.WriteLine("");

        // Date range analysis
        var datedTransactions = transactions.Where(t => t.Date.HasValue).ToList();
        if (datedTransactions.Any())
        {
            var startDate = datedTransactions.Min(t => t.Date!.Value);
            var endDate = datedTransactions.Max(t => t.Date!.Value);
            _output.WriteLine($"Date Range: {startDate:MM/dd/yyyy} to {endDate:MM/dd/yyyy}");
            _output.WriteLine($"Period: {(endDate - startDate).Days} days");
            _output.WriteLine("");
        }

        // Category breakdown
        var categoryBreakdown = transactions
            .Where(t => !string.IsNullOrEmpty(t.Category))
            .GroupBy(t => t.Category)
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(t => t.Amount ?? 0),
                Count = g.Count(),
                Average = g.Average(t => t.Amount ?? 0)
            })
            .OrderByDescending(x => Math.Abs(x.Total))
            .ToList();

        _output.WriteLine("=== Category Breakdown ===");
        foreach (var category in categoryBreakdown)
        {
            var type = category.Total >= 0 ? "Income" : "Expense";
            _output.WriteLine($"{category.Category} ({type}):");
            _output.WriteLine($"  Total: {category.Total:C}");
            _output.WriteLine($"  Count: {category.Count} transactions");
            _output.WriteLine($"  Average: {category.Average:C}");
            _output.WriteLine("");
        }

        // Payee analysis
        var payeeBreakdown = transactions
            .Where(t => !string.IsNullOrEmpty(t.Payee))
            .GroupBy(t => t.Payee)
            .Select(g => new
            {
                Payee = g.Key,
                Total = g.Sum(t => t.Amount ?? 0),
                Count = g.Count(),
                FirstTransaction = g.Min(t => t.Date),
                LastTransaction = g.Max(t => t.Date)
            })
            .OrderByDescending(x => x.Count)
            .ThenByDescending(x => Math.Abs(x.Total))
            .ToList();

        _output.WriteLine("=== Top Payees ===");
        foreach (var payee in payeeBreakdown.Take(5))
        {
            _output.WriteLine($"{payee.Payee}:");
            _output.WriteLine($"  Total Amount: {payee.Total:C}");
            _output.WriteLine($"  Transaction Count: {payee.Count}");
            if (payee.FirstTransaction.HasValue && payee.LastTransaction.HasValue)
            {
                _output.WriteLine($"  Date Range: {payee.FirstTransaction:MM/dd/yyyy} to {payee.LastTransaction:MM/dd/yyyy}");
            }
            _output.WriteLine("");
        }

        // Monthly analysis
        var monthlyData = datedTransactions
            .GroupBy(t => new { Year = t.Date!.Value.Year, Month = t.Date!.Value.Month })
            .Select(g => new
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                Income = g.Where(t => t.Amount > 0).Sum(t => t.Amount ?? 0),
                Expenses = g.Where(t => t.Amount < 0).Sum(t => t.Amount ?? 0),
                Net = g.Sum(t => t.Amount ?? 0),
                TransactionCount = g.Count()
            })
            .OrderBy(x => x.Month)
            .ToList();

        _output.WriteLine("=== Monthly Breakdown ===");
        foreach (var month in monthlyData)
        {
            _output.WriteLine($"{month.Month:MMMM yyyy}:");
            _output.WriteLine($"  Income: {month.Income:C}");
            _output.WriteLine($"  Expenses: {Math.Abs(month.Expenses):C}");
            _output.WriteLine($"  Net: {month.Net:C}");
            _output.WriteLine($"  Transactions: {month.TransactionCount}");
            _output.WriteLine("");
        }

        // Individual transaction details (sample)
        _output.WriteLine("=== Sample Transactions ===");
        var sampleTransactions = transactions
            .OrderBy(t => t.Date)
            .Take(5)
            .ToList();

        foreach (var transaction in sampleTransactions)
        {
            _output.WriteLine($"Date: {transaction.Date:MM/dd/yyyy}");
            _output.WriteLine($"Amount: {transaction.Amount:C}");
            _output.WriteLine($"Payee: {transaction.Payee}");
            _output.WriteLine($"Category: {transaction.Category}");
            if (!string.IsNullOrEmpty(transaction.Memo))
            {
                _output.WriteLine($"Memo: {transaction.Memo}");
            }
            if (!string.IsNullOrEmpty(transaction.Number))
            {
                _output.WriteLine($"Number: {transaction.Number}");
            }
            _output.WriteLine($"Cleared Status: {transaction.ClearedStatus}");
            _output.WriteLine("---");
        }

        // Validation checks
        _output.WriteLine("=== Validation Results ===");
        
        // Check for transactions without dates
        var undatedTransactions = transactions.Where(t => !t.Date.HasValue).Count();
        if (undatedTransactions > 0)
        {
            _output.WriteLine($"Warning: {undatedTransactions} transactions without dates");
        }

        // Check for transactions without amounts
        var transactionsWithoutAmount = transactions.Where(t => !t.Amount.HasValue).Count();
        if (transactionsWithoutAmount > 0)
        {
            _output.WriteLine($"Warning: {transactionsWithoutAmount} transactions without amounts");
        }

        // Check for transactions without payees
        var transactionsWithoutPayee = transactions.Where(t => string.IsNullOrEmpty(t.Payee)).Count();
        if (transactionsWithoutPayee > 0)
        {
            _output.WriteLine($"Info: {transactionsWithoutPayee} transactions without payees");
        }

        // Check for transactions without categories
        var transactionsWithoutCategory = transactions.Where(t => string.IsNullOrEmpty(t.Category)).Count();
        if (transactionsWithoutCategory > 0)
        {
            _output.WriteLine($"Info: {transactionsWithoutCategory} transactions without categories");
        }

        if (undatedTransactions == 0 && transactionsWithoutAmount == 0)
        {
            _output.WriteLine("✓ All transactions have required date and amount fields");
        }

        _output.WriteLine("=== Demo Complete ===");
        
        // Final assertions to ensure the test validates the parser
        Assert.True(document.Transactions.Count > 0, "Should have parsed transactions");
        Assert.Equal(QifAccountType.Bank, document.AccountType);
        Assert.True(totalIncome > 0, "Should have income transactions");
        Assert.True(totalExpenses < 0, "Should have expense transactions");
    }
}
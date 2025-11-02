# QIF Parser Usage Examples

This document provides comprehensive examples of how to use the QIF Parser Library in various scenarios.

## Table of Contents

1. [Basic Usage](#basic-usage)
2. [Locale and Format Options](#locale-and-format-options)
3. [Async Operations](#async-operations)
4. [Error Handling](#error-handling)
5. [Working with Different Account Types](#working-with-different-account-types)
6. [Processing Transactions](#processing-transactions)
7. [Handling Split Transactions](#handling-split-transactions)
8. [Investment Transactions](#investment-transactions)
9. [Memorized Transactions](#memorized-transactions)
10. [Categories and Classes](#categories-and-classes)
11. [Performance Optimization](#performance-optimization)
12. [Testing Scenarios](#testing-scenarios)

## Basic Usage

### Simple File Parsing

```csharp
using QifLib;
using QifLib.Models;

// Create parser instance
var parser = new QifReader();

// Parse a QIF file
var document = await parser.ReadAsync("bank_transactions.qif");

// Display basic information
Console.WriteLine($"Account Type: {document.AccountType}");
Console.WriteLine($"Total Transactions: {document.Transactions.Count}");
Console.WriteLine($"Total Items: {document.TotalItems}");
```

### String Parsing

```csharp
var qifContent = """
    !Type:Bank
    D1/1/2023
    T1500.00
    PPaycheck
    LIncome:Salary
    ^
    D1/15/2023
    T-500.00
    PRent Payment
    LExpenses:Rent
    ^
    """;

var parser = new QifReader();
var document = parser.ReadString(qifContent);

Console.WriteLine($"Parsed {document.Transactions.Count} transactions");
```

## Locale and Format Options

The QIF Parser supports different locale-specific date and decimal formats through the `QifOptions` class.

### Using US Format (Default)

```csharp
using Altoviz.Qif;
using Altoviz.Qif.Models;

var parser = new QifReader();
// Default is US format: MM/dd/yyyy dates, period decimal separator

var qifContent = """
    !Type:Bank
    D12/31/2023
    T1,234.56
    PUS Transaction
    ^
    """;

var document = parser.ReadString(qifContent);
var transaction = document.Transactions[0];
Console.WriteLine($"Date: {transaction.Date}"); // 12/31/2023
Console.WriteLine($"Amount: {transaction.Amount}"); // 1234.56
```

### Using French Format

```csharp
var parser = new QifReader();
parser.Options = QifOptionsFR.Create(); // French format: dd/MM/yyyy dates, comma decimal separator

var qifContent = """
    !Type:Bank
    D31/12/2023
    T1 234,56
    PFrench Transaction
    ^
    """;

var document = parser.ReadString(qifContent);
var transaction = document.Transactions[0];
Console.WriteLine($"Date: {transaction.Date}"); // 12/31/2023
Console.WriteLine($"Amount: {transaction.Amount}"); // 1234.56
```

### Using Custom Format Options

```csharp
var customOptions = new QifOptions(
    dateFormats: new[] { "yyyy-MM-dd", "dd.MM.yyyy" },
    decimalSeparator: '.'
);

var parser = new QifReader();
parser.Options = customOptions;

var qifContent = """
    !Type:Bank
    D2023-12-31
    T1234.56
    PCustom Format Transaction
    ^
    """;

var document = parser.ReadString(qifContent);
```

### Predefined Options Comparison

````csharp
// US Options
var usOptions = QifOptionsUS.Create();
Console.WriteLine("US Date Formats: " + string.Join(", ", usOptions.DateFormats));
Console.WriteLine("US Decimal Separator: " + usOptions.DecimalSeparator);

// French Options
var frOptions = QifOptionsFR.Create();
Console.WriteLine("FR Date Formats: " + string.Join(", ", frOptions.DateFormats));
Console.WriteLine("FR Decimal Separator: " + frOptions.DecimalSeparator);
```## Async Operations

### File Parsing with Cancellation

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    var document = await parser.ReadAsync("large_file.qif", cts.Token);
    Console.WriteLine("Parsing completed successfully");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Parsing was cancelled due to timeout");
}
````

### Stream Parsing

```csharp
// From file stream
using var fileStream = File.OpenRead("transactions.qif");
var document = await parser.ReadAsync(fileStream);

// From memory stream
var bytes = Encoding.UTF8.GetBytes(qifContent);
using var memoryStream = new MemoryStream(bytes);
var document2 = await parser.ReadAsync(memoryStream);

// From network stream
using var httpClient = new HttpClient();
using var httpStream = await httpClient.GetStreamAsync("https://example.com/data.qif");
var document3 = await parser.ReadAsync(httpStream);
```

## Error Handling

### Comprehensive Error Handling

```csharp
public async Task<QifDocument?> SafeParseFile(string filePath)
{
    try
    {
        var parser = new QifReader();
        return await parser.ReadAsync(filePath);
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine($"File not found: {ex.FileName}");
        return null;
    }
    catch (ArgumentNullException ex)
    {
        Console.WriteLine($"Invalid argument: {ex.ParamName}");
        return null;
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"Access denied: {ex.Message}");
        return null;
    }
    catch (IOException ex)
    {
        Console.WriteLine($"IO error: {ex.Message}");
        return null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
        return null;
    }
}
```

### Validation and Error Recovery

```csharp
public void ValidateDocument(QifDocument document)
{
    // Check for basic validity
    if (document.TotalItems == 0)
    {
        Console.WriteLine("Warning: No data found in QIF file");
        return;
    }

    // Validate transactions
    var invalidTransactions = document.Transactions
        .Where(t => !t.Date.HasValue || !t.Amount.HasValue)
        .ToList();

    if (invalidTransactions.Any())
    {
        Console.WriteLine($"Warning: {invalidTransactions.Count} transactions have missing required data");
    }

    // Check for split transaction consistency
    foreach (var transaction in document.Transactions.Where(t => t.Splits.Any()))
    {
        var splitTotal = transaction.Splits.Sum(s => s.Amount);
        var transactionAmount = transaction.Amount ?? 0;

        if (Math.Abs(splitTotal - transactionAmount) > 0.01m)
        {
            Console.WriteLine($"Warning: Split amounts don't match transaction amount for transaction on {transaction.Date}");
        }
    }
}
```

## Working with Different Account Types

### Bank Account Processing

```csharp
public void ProcessBankAccount(QifDocument document)
{
    if (document.AccountType != QifAccountType.Bank)
    {
        Console.WriteLine("This is not a bank account file");
        return;
    }

    var deposits = document.Transactions
        .Where(t => t.Amount > 0)
        .OrderByDescending(t => t.Amount)
        .ToList();

    var withdrawals = document.Transactions
        .Where(t => t.Amount < 0)
        .OrderBy(t => t.Amount)
        .ToList();

    Console.WriteLine($"Total Deposits: {deposits.Sum(d => d.Amount):C}");
    Console.WriteLine($"Total Withdrawals: {Math.Abs(withdrawals.Sum(w => w.Amount ?? 0)):C}");
    Console.WriteLine($"Net Amount: {document.Transactions.Sum(t => t.Amount ?? 0):C}");
}
```

### Credit Card Processing

```csharp
public void ProcessCreditCard(QifDocument document)
{
    if (document.AccountType != QifAccountType.CreditCard)
        return;

    var charges = document.Transactions
        .Where(t => t.Amount < 0) // Charges are negative
        .ToList();

    var payments = document.Transactions
        .Where(t => t.Amount > 0) // Payments are positive
        .ToList();

    // Group charges by category
    var chargesByCategory = charges
        .Where(t => !string.IsNullOrEmpty(t.Category))
        .GroupBy(t => t.Category)
        .Select(g => new
        {
            Category = g.Key,
            Amount = Math.Abs(g.Sum(t => t.Amount ?? 0)),
            Count = g.Count()
        })
        .OrderByDescending(x => x.Amount)
        .ToList();

    Console.WriteLine("Top Spending Categories:");
    foreach (var category in chargesByCategory.Take(5))
    {
        Console.WriteLine($"  {category.Category}: {category.Amount:C} ({category.Count} transactions)");
    }
}
```

## Processing Transactions

### Transaction Analysis

```csharp
public class TransactionAnalyzer
{
    public void AnalyzeTransactions(QifDocument document)
    {
        var transactions = document.Transactions;

        // Basic statistics
        var totalTransactions = transactions.Count;
        var totalAmount = transactions.Sum(t => t.Amount ?? 0);
        var averageAmount = transactions.Where(t => t.Amount.HasValue).Average(t => t.Amount!.Value);

        Console.WriteLine($"Total Transactions: {totalTransactions}");
        Console.WriteLine($"Total Amount: {totalAmount:C}");
        Console.WriteLine($"Average Amount: {averageAmount:C}");

        // Date range
        var datedTransactions = transactions.Where(t => t.Date.HasValue).ToList();
        if (datedTransactions.Any())
        {
            var startDate = datedTransactions.Min(t => t.Date!.Value);
            var endDate = datedTransactions.Max(t => t.Date!.Value);
            Console.WriteLine($"Date Range: {startDate:d} to {endDate:d}");
        }

        // Monthly breakdown
        var monthlyTotals = datedTransactions
            .GroupBy(t => new { t.Date!.Value.Year, t.Date!.Value.Month })
            .Select(g => new
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                Amount = g.Sum(t => t.Amount ?? 0),
                Count = g.Count()
            })
            .OrderBy(x => x.Month)
            .ToList();

        Console.WriteLine("\nMonthly Breakdown:");
        foreach (var month in monthlyTotals)
        {
            Console.WriteLine($"  {month.Month:yyyy-MM}: {month.Amount:C} ({month.Count} transactions)");
        }
    }
}
```

### Payee Analysis

```csharp
public void AnalyzePayees(QifDocument document)
{
    var payeeStats = document.Transactions
        .Where(t => !string.IsNullOrEmpty(t.Payee))
        .GroupBy(t => t.Payee)
        .Select(g => new
        {
            Payee = g.Key,
            TotalAmount = g.Sum(t => t.Amount ?? 0),
            TransactionCount = g.Count(),
            AverageAmount = g.Average(t => t.Amount ?? 0),
            FirstTransaction = g.Min(t => t.Date),
            LastTransaction = g.Max(t => t.Date)
        })
        .OrderByDescending(x => Math.Abs(x.TotalAmount))
        .ToList();

    Console.WriteLine("Top Payees by Amount:");
    foreach (var payee in payeeStats.Take(10))
    {
        Console.WriteLine($"  {payee.Payee}: {payee.TotalAmount:C} ({payee.TransactionCount} transactions)");
    }
}
```

## Handling Split Transactions

### Processing Split Transactions

```csharp
public void ProcessSplitTransactions(QifDocument document)
{
    var splitTransactions = document.Transactions
        .Where(t => t.Splits.Any())
        .ToList();

    Console.WriteLine($"Found {splitTransactions.Count} split transactions");

    foreach (var transaction in splitTransactions)
    {
        Console.WriteLine($"\nTransaction on {transaction.Date:d} for {transaction.Amount:C}");
        Console.WriteLine($"Payee: {transaction.Payee}");
        Console.WriteLine("Splits:");

        foreach (var split in transaction.Splits)
        {
            Console.WriteLine($"  {split.Category}: {split.Amount:C}");
            if (!string.IsNullOrEmpty(split.Memo))
            {
                Console.WriteLine($"    Memo: {split.Memo}");
            }
        }

        // Verify split total
        var splitTotal = transaction.Splits.Sum(s => s.Amount);
        var difference = Math.Abs(splitTotal - (transaction.Amount ?? 0));
        if (difference > 0.01m)
        {
            Console.WriteLine($"  Warning: Split total ({splitTotal:C}) doesn't match transaction amount ({transaction.Amount:C})");
        }
    }
}
```

### Category Analysis from Splits

```csharp
public void AnalyzeCategoriesFromSplits(QifDocument document)
{
    var allCategories = new List<(string Category, decimal Amount)>();

    // Add main transaction categories
    foreach (var transaction in document.Transactions)
    {
        if (!string.IsNullOrEmpty(transaction.Category) && !transaction.Splits.Any())
        {
            allCategories.Add((transaction.Category, transaction.Amount ?? 0));
        }
    }

    // Add split categories
    foreach (var transaction in document.Transactions.Where(t => t.Splits.Any()))
    {
        foreach (var split in transaction.Splits)
        {
            if (!string.IsNullOrEmpty(split.Category))
            {
                allCategories.Add((split.Category, split.Amount));
            }
        }
    }

    var categoryTotals = allCategories
        .GroupBy(x => x.Category)
        .Select(g => new
        {
            Category = g.Key,
            Total = g.Sum(x => x.Amount),
            Count = g.Count()
        })
        .OrderBy(x => x.Total)
        .ToList();

    Console.WriteLine("Category Totals (including splits):");
    foreach (var category in categoryTotals)
    {
        Console.WriteLine($"  {category.Category}: {category.Total:C} ({category.Count} entries)");
    }
}
```

## Investment Transactions

### Processing Investment Data

```csharp
public void ProcessInvestmentTransactions(QifDocument document)
{
    if (document.AccountType != QifAccountType.Investment)
    {
        Console.WriteLine("This is not an investment account");
        return;
    }

    var investments = document.InvestmentTransactions;

    Console.WriteLine($"Total Investment Transactions: {investments.Count}");

    // Group by security
    var securityGroups = investments
        .Where(t => !string.IsNullOrEmpty(t.Security))
        .GroupBy(t => t.Security)
        .ToList();

    foreach (var group in securityGroups)
    {
        Console.WriteLine($"\nSecurity: {group.Key}");

        var buys = group.Where(t => t.Action?.ToLower().Contains("buy") == true).ToList();
        var sells = group.Where(t => t.Action?.ToLower().Contains("sell") == true).ToList();

        var totalShares = buys.Sum(t => t.Quantity ?? 0) - sells.Sum(t => t.Quantity ?? 0);
        var totalInvested = buys.Sum(t => (t.Quantity ?? 0) * (t.Price ?? 0));
        var totalCommissions = group.Sum(t => t.Commission ?? 0);

        Console.WriteLine($"  Total Shares: {totalShares}");
        Console.WriteLine($"  Total Invested: {totalInvested:C}");
        Console.WriteLine($"  Total Commissions: {totalCommissions:C}");

        if (totalShares > 0)
        {
            var averagePrice = totalInvested / totalShares;
            Console.WriteLine($"  Average Price: {averagePrice:C}");
        }
    }
}
```

### Investment Performance Analysis

```csharp
public class InvestmentAnalyzer
{
    public void AnalyzePortfolio(QifDocument document)
    {
        var investments = document.InvestmentTransactions
            .Where(t => t.Date.HasValue)
            .OrderBy(t => t.Date)
            .ToList();

        var portfolio = new Dictionary<string, PortfolioPosition>();

        foreach (var transaction in investments)
        {
            if (string.IsNullOrEmpty(transaction.Security)) continue;

            if (!portfolio.ContainsKey(transaction.Security))
            {
                portfolio[transaction.Security] = new PortfolioPosition
                {
                    Security = transaction.Security
                };
            }

            var position = portfolio[transaction.Security];

            switch (transaction.Action?.ToLower())
            {
                case "buy" when transaction.Quantity.HasValue && transaction.Price.HasValue:
                    position.Shares += transaction.Quantity.Value;
                    position.TotalCost += transaction.Quantity.Value * transaction.Price.Value;
                    position.TotalCommissions += transaction.Commission ?? 0;
                    break;

                case "sell" when transaction.Quantity.HasValue && transaction.Price.HasValue:
                    position.Shares -= transaction.Quantity.Value;
                    // Calculate realized gain/loss
                    var saleProceeds = transaction.Quantity.Value * transaction.Price.Value;
                    position.RealizedGainLoss += saleProceeds - (position.AverageCost * transaction.Quantity.Value);
                    position.TotalCommissions += transaction.Commission ?? 0;
                    break;
            }
        }

        Console.WriteLine("Portfolio Summary:");
        foreach (var position in portfolio.Values.Where(p => p.Shares > 0))
        {
            Console.WriteLine($"  {position.Security}:");
            Console.WriteLine($"    Shares: {position.Shares}");
            Console.WriteLine($"    Average Cost: {position.AverageCost:C}");
            Console.WriteLine($"    Total Investment: {position.TotalCost:C}");
            Console.WriteLine($"    Realized Gain/Loss: {position.RealizedGainLoss:C}");
        }
    }

    public class PortfolioPosition
    {
        public string Security { get; set; } = "";
        public decimal Shares { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalCommissions { get; set; }
        public decimal RealizedGainLoss { get; set; }
        public decimal AverageCost => Shares > 0 ? TotalCost / Shares : 0;
    }
}
```

## Performance Optimization

### Large File Processing

```csharp
public async Task ProcessLargeFileEfficiently(string filePath)
{
    var parser = new QifReader();

    // Use a CancellationToken to allow for cancellation
    using var cts = new CancellationTokenSource();

    // Set up progress reporting
    var progress = new Progress<string>(message => Console.WriteLine($"Progress: {message}"));

    try
    {
        // Parse the file
        var stopwatch = Stopwatch.StartNew();
        var document = await parser.ReadAsync(filePath, cts.Token);
        stopwatch.Stop();

        Console.WriteLine($"Parsed {document.TotalItems} items in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Memory usage: {GC.GetTotalMemory(false) / 1024 / 1024}MB");

        // Process in batches to avoid memory issues
        const int batchSize = 1000;
        var transactions = document.Transactions;

        for (int i = 0; i < transactions.Count; i += batchSize)
        {
            var batch = transactions.Skip(i).Take(batchSize);
            ProcessTransactionBatch(batch);

            // Optional: Force garbage collection between batches
            if (i % (batchSize * 10) == 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Processing was cancelled");
    }
}

private void ProcessTransactionBatch(IEnumerable<QifTransaction> transactions)
{
    // Process batch of transactions
    foreach (var transaction in transactions)
    {
        // Your processing logic here
    }
}
```

### Memory-Efficient Processing

```csharp
public async Task ProcessFileMemoryEfficient(string filePath)
{
    var parser = new QifReader();

    // Use streaming when possible
    using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 65536);
    var document = await parser.ReadAsync(stream);

    // Process data as you go, don't store everything in memory
    await ProcessTransactionsStreaming(document.Transactions);
}

private async Task ProcessTransactionsStreaming(IEnumerable<QifTransaction> transactions)
{
    await using var output = new FileStream("processed_output.csv", FileMode.Create);
    await using var writer = new StreamWriter(output);

    // Write CSV header
    await writer.WriteLineAsync("Date,Amount,Payee,Category");

    // Process one transaction at a time
    foreach (var transaction in transactions)
    {
        var line = $"{transaction.Date:yyyy-MM-dd},{transaction.Amount},{transaction.Payee},{transaction.Category}";
        await writer.WriteLineAsync(line);
    }
}
```

## Testing Scenarios

### Unit Test Examples

```csharp
[Test]
public async Task ParseFile_ValidBankFile_ReturnsCorrectData()
{
    // Arrange
    var qifContent = """
        !Type:Bank
        D1/1/2023
        T1000.00
        PTest Payee
        LTest Category
        ^
        """;

    var parser = new QifReader();

    // Act
    var result = parser.ReadString(qifContent);

    // Assert
    Assert.AreEqual(QifAccountType.Bank, result.AccountType);
    Assert.AreEqual(1, result.Transactions.Count);

    var transaction = result.Transactions[0];
    Assert.AreEqual(new DateTime(2023, 1, 1), transaction.Date);
    Assert.AreEqual(1000.00m, transaction.Amount);
    Assert.AreEqual("Test Payee", transaction.Payee);
    Assert.AreEqual("Test Category", transaction.Category);
}

[Test]
public void ReadString_NullInput_ThrowsArgumentNullException()
{
    // Arrange
    var parser = new QifReader();

    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => parser.ReadString(null));
}

[Test]
public async Task ReadAsync_NonExistentFile_ThrowsFileNotFoundException()
{
    // Arrange
    var parser = new QifReader();

    // Act & Assert
    await Assert.ThrowsAsync<FileNotFoundException>(() =>
        parser.ReadAsync("non_existent_file.qif"));
}
```

### Integration Test Examples

```csharp
[Test]
public async Task ParseRealWorldFile_ComplexQifFile_ParsesSuccessfully()
{
    // Arrange
    var parser = new QifReader();
    var testFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestFiles", "complex_transactions.qif");

    // Act
    var document = await parser.ReadAsync(testFilePath);

    // Assert
    Assert.IsNotNull(document);
    Assert.Greater(document.TotalItems, 0);

    // Verify specific business rules
    var splitTransactions = document.Transactions.Where(t => t.Splits.Any()).ToList();
    foreach (var transaction in splitTransactions)
    {
        var splitTotal = transaction.Splits.Sum(s => s.Amount);
        var difference = Math.Abs(splitTotal - (transaction.Amount ?? 0));
        Assert.LessOrEqual(difference, 0.01m, "Split amounts should sum to transaction amount");
    }
}
```

This comprehensive examples guide should help developers understand how to use the QIF Parser Library effectively in various scenarios, from basic parsing to complex financial analysis and performance optimization.

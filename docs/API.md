# QIF Parser API Reference

## Namespace: QifLib

### Interface: IQifReader

The main interface for QIF file parsing operations.

```csharp
public interface IQifReader
{
    Task<QifDocument> ReadAsync(string filePath, CancellationToken cancellationToken = default);
    Task<QifDocument> ReadAsync(Stream stream, CancellationToken cancellationToken = default);
    QifDocument ReadString(string content);
}
```

#### Methods

##### ReadAsync

Parses a QIF file from the specified file path asynchronously.

**Signature:**

```csharp
Task<QifDocument> ReadAsync(string filePath, CancellationToken cancellationToken = default)
```

**Parameters:**

- `filePath` (string): Path to the QIF file
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** Task<QifDocument> - Parsed QIF document

**Exceptions:**

- `ArgumentNullException`: Thrown when filePath is null or empty
- `FileNotFoundException`: Thrown when the file does not exist
- `OperationCanceledException`: Thrown when operation is cancelled

##### ReadAsync

Parses QIF content from a stream asynchronously.

**Signature:**

```csharp
Task<QifDocument> ReadAsync(Stream stream, CancellationToken cancellationToken = default)
```

**Parameters:**

- `stream` (Stream): Stream containing QIF data
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** Task<QifDocument> - Parsed QIF document

**Exceptions:**

- `ArgumentNullException`: Thrown when stream is null
- `OperationCanceledException`: Thrown when operation is cancelled

##### ReadString

Parses QIF content from a string synchronously.

**Signature:**

```csharp
QifDocument ReadString(string content)
```

**Parameters:**

- `content` (string): QIF content as string

**Returns:** QifDocument - Parsed QIF document

**Exceptions:**

- `ArgumentNullException`: Thrown when content is null

### Class: QifReader

The main implementation of the QIF parser.

```csharp
public class QifReader : IQifReader
```

Implements all methods from `IQifReader` interface.

## Namespace: QifLib.Models

### Class: QifDocument

Represents a complete QIF file with all its data.

```csharp
public class QifDocument
{
    public QifAccountType AccountType { get; set; }
    public bool ImportAllTransfers { get; set; }
    public List<QifTransaction> Transactions { get; set; }
    public List<QifInvestmentTransaction> InvestmentTransactions { get; set; }
    public List<QifMemorizedTransaction> MemorizedTransactions { get; set; }
    public List<QifAccount> Accounts { get; set; }
    public List<QifCategory> Categories { get; set; }
    public List<QifClass> Classes { get; set; }
    public int TotalItems { get; }
}
```

#### Properties

- **AccountType**: The type of account/data in this QIF document
- **ImportAllTransfers**: Whether to import all transfers regardless of settings
- **Transactions**: Regular transactions (for Bank, Cash, Credit Card, Asset, Liability accounts)
- **InvestmentTransactions**: Investment transactions
- **MemorizedTransactions**: Memorized transactions
- **Accounts**: Account information
- **Categories**: Categories
- **Classes**: Classes
- **TotalItems**: Gets the total number of items in the document (read-only)

### Enum: QifAccountType

Represents the different account types supported by QIF format.

```csharp
public enum QifAccountType
{
    Bank,
    Cash,
    CreditCard,
    Investment,
    OtherAsset,
    OtherLiability,
    Account,
    Category,
    Class,
    Memorized
}
```

### Enum: QifClearedStatus

Represents the cleared status of a transaction.

```csharp
public enum QifClearedStatus
{
    None,
    Cleared,
    Reconciled
}
```

### Abstract Class: QifTransactionBase

Base class for all QIF transactions.

```csharp
public abstract class QifTransactionBase
{
    public DateTime? Date { get; set; }
    public decimal? Amount { get; set; }
    public QifClearedStatus ClearedStatus { get; set; }
    public string? Number { get; set; }
    public string? Payee { get; set; }
    public string? Memo { get; set; }
    public List<string> Address { get; set; }
    public string? Category { get; set; }
    public List<QifSplit> Splits { get; set; }
}
```

### Class: QifTransaction

Represents a non-investment QIF transaction.

```csharp
public class QifTransaction : QifTransactionBase
{
    // Inherits all properties from QifTransactionBase
}
```

### Class: QifInvestmentTransaction

Represents an investment transaction in QIF format.

```csharp
public class QifInvestmentTransaction : QifTransactionBase
{
    public string? Action { get; set; }
    public string? Security { get; set; }
    public decimal? Price { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Commission { get; set; }
    public string? TransferAccount { get; set; }
    public decimal? TransferAmount { get; set; }
}
```

#### Properties

- **Action**: Investment action (Buy, Sell, etc.)
- **Security**: Security name
- **Price**: Price per share
- **Quantity**: Quantity (number of shares or split ratio)
- **Commission**: Commission
- **TransferAccount**: Account for the transfer
- **TransferAmount**: Amount transferred

### Class: QifMemorizedTransaction

Represents a memorized transaction in QIF format.

```csharp
public class QifMemorizedTransaction : QifTransactionBase
{
    public QifMemorizedTransactionType TransactionType { get; set; }
    public QifAmortization? Amortization { get; set; }
}
```

#### Properties

- **TransactionType**: Type of memorized transaction
- **Amortization**: Amortization information (optional)

### Enum: QifMemorizedTransactionType

Represents the transaction type for memorized transactions.

```csharp
public enum QifMemorizedTransactionType
{
    Check,
    Deposit,
    Payment,
    Investment,
    ElectronicPayee
}
```

### Class: QifSplit

Represents a split transaction entry within a QIF transaction.

```csharp
public class QifSplit
{
    public string? Category { get; set; }
    public string? Memo { get; set; }
    public decimal Amount { get; set; }
}
```

#### Properties

- **Category**: Category in split (Category/Transfer/Class)
- **Memo**: Memo in split
- **Amount**: Dollar amount of split

### Class: QifAmortization

Represents amortization information for memorized transactions.

```csharp
public class QifAmortization
{
    public DateTime? FirstPaymentDate { get; set; }
    public int? TotalYears { get; set; }
    public int? PaymentsMade { get; set; }
    public int? PeriodsPerYear { get; set; }
    public decimal? InterestRate { get; set; }
    public decimal? CurrentBalance { get; set; }
    public decimal? OriginalAmount { get; set; }
}
```

#### Properties

- **FirstPaymentDate**: First payment date
- **TotalYears**: Total years for loan
- **PaymentsMade**: Number of payments already made
- **PeriodsPerYear**: Number of periods per year
- **InterestRate**: Interest rate
- **CurrentBalance**: Current loan balance
- **OriginalAmount**: Original loan amount

### Class: QifAccount

Represents account information in QIF format.

```csharp
public class QifAccount
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public decimal? CreditLimit { get; set; }
    public DateTime? StatementBalanceDate { get; set; }
    public decimal? StatementBalance { get; set; }
}
```

#### Properties

- **Name**: Account name
- **Type**: Type of account
- **Description**: Account description
- **CreditLimit**: Credit limit (only for credit card accounts)
- **StatementBalanceDate**: Statement balance date
- **StatementBalance**: Statement balance

### Class: QifCategory

Represents a category in QIF format.

```csharp
public class QifCategory
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsTaxRelated { get; set; }
    public bool IsIncomeCategory { get; set; }
    public decimal? BudgetAmount { get; set; }
    public string? TaxSchedule { get; set; }
}
```

#### Properties

- **Name**: Category name (may include subcategory separated by colon)
- **Description**: Category description
- **IsTaxRelated**: Whether the category is tax-related
- **IsIncomeCategory**: Whether this is an income category (false = expense)
- **BudgetAmount**: Budget amount (only in Budget Amounts QIF file)
- **TaxSchedule**: Tax schedule information

### Class: QifClass

Represents a class in QIF format.

```csharp
public class QifClass
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
```

#### Properties

- **Name**: Class name
- **Description**: Class description

## Usage Examples

### Basic File Parsing

```csharp
using QifLib;

var parser = new QifReader();
var document = await parser.ReadAsync("transactions.qif");

Console.WriteLine($"Account Type: {document.AccountType}");
Console.WriteLine($"Total Items: {document.TotalItems}");
```

### Stream Parsing

```csharp
using QifLib;

var parser = new QifReader();
using var stream = File.OpenRead("transactions.qif");
var document = await parser.ReadAsync(stream);
```

### String Parsing

```csharp
using QifLib;

var qifContent = "!Type:Bank\nD1/1/2023\nT100.00\nPTest Payee\n^";
var parser = new QifReader();
var document = parser.ReadString(qifContent);
```

### Working with Transactions

```csharp
foreach (var transaction in document.Transactions)
{
    Console.WriteLine($"Date: {transaction.Date:d}");
    Console.WriteLine($"Amount: {transaction.Amount:C}");
    Console.WriteLine($"Payee: {transaction.Payee}");

    // Process splits
    foreach (var split in transaction.Splits)
    {
        Console.WriteLine($"  Split: {split.Category} - {split.Amount:C}");
    }
}
```

### Error Handling

```csharp
try
{
    var document = await parser.ReadAsync("file.qif");
}
catch (FileNotFoundException)
{
    Console.WriteLine("QIF file not found");
}
catch (ArgumentNullException)
{
    Console.WriteLine("Invalid file path");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled");
}
```

## Thread Safety

The `QifReader` class is thread-safe for read operations. You can safely use the same parser instance across multiple threads. However, the returned `QifDocument` and its contained objects are not thread-safe and should not be modified concurrently.

## Performance Considerations

- For large files, prefer `ReadAsync` or `ReadAsync` for better responsiveness
- The parser loads the entire file content into memory, so very large files (>1GB) may cause memory pressure
- Consider using cancellation tokens for long-running operations
- Parsing performance is approximately 10,000-50,000 transactions per second depending on hardware

## Versioning

This API follows semantic versioning:

- **Major version**: Breaking changes to public API
- **Minor version**: New features, backward compatible
- **Patch version**: Bug fixes, backward compatible

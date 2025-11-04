# QIF Library

A complete C# library for parsing Quicken Interchange Format (QIF) files with async/await support.

## Features

- **Complete QIF Support**: Supports all QIF record types including Bank, Cash, Credit Card, Investment, Memorized transactions, Accounts, Categories, and Classes
- **Async/Await**: Full async support for file and stream parsing
- **Flexible Input**: Parse from file paths, streams, or strings
- **Robust Parsing**: Handles various date and amount formats commonly found in QIF files
- **Split Transactions**: Full support for split transactions
- **Investment Transactions**: Complete support for investment account transactions
- **Memorized Transactions**: Support for memorized transactions with amortization
- **Type Safety**: Strongly typed models with proper enums for status and types

## Status

[![Build and publish](https://github.com/altoviz/qif/actions/workflows/build-qif.yaml/badge.svg)](https://github.com/altoviz/qif/actions/workflows/build-qif.yaml)

## Quick Start

### Basic Usage

```csharp
Altoviz.Qif;

// Create parser instance
var parser = new QifParser();

// Parse from file
var document = await parser.ParseAsync("transactions.qif");

// Parse from stream
using var stream = File.OpenRead("transactions.qif");
var document = await parser.ParseAsync(stream);

// Parse from string
var qifContent = "!Type:Bank\nD1/1/2023\nT100.00\nPTest Payee\n^";
var document = parser.ParseString(qifContent);
```

### Working with Results

```csharp
var parser = new QifParser();
var document = await parser.ParseAsync("bank.qif");

Console.WriteLine($"Account Type: {document.AccountType}");
Console.WriteLine($"Total Items: {document.TotalItems}");

// Process regular transactions
foreach (var transaction in document.Transactions)
{
    Console.WriteLine($"Date: {transaction.Date:d}");
    Console.WriteLine($"Amount: {transaction.Amount:C}");
    Console.WriteLine($"Payee: {transaction.Payee}");
    Console.WriteLine($"Category: {transaction.Category}");

    // Process splits if any
    foreach (var split in transaction.Splits)
    {
        Console.WriteLine($"  Split: {split.Category} - {split.Amount:C}");
    }
}

// Process investment transactions
foreach (var investment in document.InvestmentTransactions)
{
    Console.WriteLine($"Action: {investment.Action}");
    Console.WriteLine($"Security: {investment.Security}");
    Console.WriteLine($"Quantity: {investment.Quantity}");
    Console.WriteLine($"Price: {investment.Price:C}");
}
```

## Supported QIF Types

- **!Type:Bank** - Bank account transactions
- **!Type:Cash** - Cash account transactions
- **!Type:CCard** - Credit card account transactions
- **!Type:Invst** - Investment account transactions
- **!Type:Oth A** - Asset account transactions
- **!Type:Oth L** - Liability account transactions
- **!Type:Cat** - Category list
- **!Type:Class** - Class list
- **!Type:Memorized** - Memorized transaction list
- **!Account** - Account information

## Models

### QifDocument

The main container that holds all parsed data:

- `AccountType` - The type of QIF data
- `Transactions` - Regular transactions
- `InvestmentTransactions` - Investment transactions
- `MemorizedTransactions` - Memorized transactions
- `Accounts` - Account information
- `Categories` - Categories
- `Classes` - Classes

### QifTransaction

Regular transaction model with fields:

- `Date`, `Amount`, `Payee`, `Memo`, `Category`
- `Number` (check/reference number)
- `ClearedStatus` (None, Cleared, Reconciled)
- `Address` (multiple lines)
- `Splits` (split transactions)

### QifInvestmentTransaction

Investment transaction with additional fields:

- `Action`, `Security`, `Price`, `Quantity`
- `Commission`, `TransferAccount`, `TransferAmount`

## Error Handling

The library includes comprehensive error handling:

```csharp
try
{
    var document = await parser.ParseAsync("file.qif");
}
catch (FileNotFoundException)
{
    // Handle missing file
}
catch (ArgumentNullException)
{
    // Handle null arguments
}
catch (OperationCanceledException)
{
    // Handle cancellation
}
```

## Thread Safety

The `QifParser` class is thread-safe for read operations. You can safely use the same parser instance across multiple threads.

## Requirements

- .NET 9.0 or later
- No external dependencies

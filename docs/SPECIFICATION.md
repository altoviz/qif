# QIF Parser Library Specification

## Project Overview

The QIF Parser Library is a complete C# .NET library for parsing Quicken Interchange Format (QIF) files with full async/await support and comprehensive error handling.

## Version Information

- **Target Framework**: .NET 9.0
- **Language**: C# 12.0
- **Library Version**: 1.0.0
- **Specification Version**: 1.0
- **Last Updated**: October 2025

## Purpose

This library provides a robust, type-safe parser for QIF files that supports all standard QIF record types as defined in the official Quicken Interchange Format specification. It is designed for financial applications that need to import transaction data from Quicken or other QIF-compatible software.

## Key Features

- **Complete QIF Support**: All standard QIF record types (Bank, Cash, Credit Card, Investment, etc.)
- **Async/Await Support**: Full asynchronous parsing for files and streams
- **Type Safety**: Strongly typed models with proper enums and nullable reference types
- **Error Handling**: Comprehensive exception handling and graceful degradation
- **Performance**: Optimized for large files with streaming support
- **Extensibility**: Interface-based design for easy testing and extension
- **Thread Safety**: Safe for concurrent use
- **Memory Efficient**: Streaming support to handle large QIF files

## Architecture

### Project Structure

```
qif/
├── qif.sln                    # Solution file
├── qif-lib/                   # Main library project
│   ├── qif-lib.csproj        # Library project file
│   ├── IQifReader.cs         # Reader interface
│   ├── QifReader.cs          # Main reader implementation
│   └── Models/               # Data models
│       ├── QifAccountType.cs
│       ├── QifClearedStatus.cs
│       ├── QifSplit.cs
│       ├── QifAmortization.cs
│       ├── QifTransactionBase.cs
│       ├── QifTransaction.cs
│       ├── QifInvestmentTransaction.cs
│       ├── QifMemorizedTransaction.cs
│       ├── QifMemorizedTransactionType.cs
│       ├── QifAccount.cs
│       ├── QifCategory.cs
│       ├── QifClass.cs
│       └── QifDocument.cs
├── qif-test/                  # Test project
│   ├── qif-test.csproj       # Test project file
│   ├── files/                # Test QIF files
│   └── *Tests.cs             # Test classes
└── docs/                     # Documentation
    ├── SPECIFICATION.md
    ├── API.md
    ├── EXAMPLES.md
    └── QIF_FORMAT.md
```

### Design Patterns

- **Repository Pattern**: Interface-based design (`IQifReader`)
- **Factory Pattern**: Model creation through parsing methods
- **Strategy Pattern**: Different parsing strategies for different QIF record types
- **Null Object Pattern**: Graceful handling of missing data

### Dependencies

- **Runtime**: .NET 9.0 or later
- **Testing**: xUnit.net 2.8.2+
- **No External Dependencies**: Pure .NET implementation

## QIF Format Support

### Supported Account Types

| QIF Type          | Enum Value                      | Description                    |
| ----------------- | ------------------------------- | ------------------------------ |
| `!Type:Bank`      | `QifAccountType.Bank`           | Bank account transactions      |
| `!Type:Cash`      | `QifAccountType.Cash`           | Cash account transactions      |
| `!Type:CCard`     | `QifAccountType.CreditCard`     | Credit card transactions       |
| `!Type:Invst`     | `QifAccountType.Investment`     | Investment transactions        |
| `!Type:Oth A`     | `QifAccountType.OtherAsset`     | Asset account transactions     |
| `!Type:Oth L`     | `QifAccountType.OtherLiability` | Liability account transactions |
| `!Type:Cat`       | `QifAccountType.Category`       | Category definitions           |
| `!Type:Class`     | `QifAccountType.Class`          | Class definitions              |
| `!Type:Memorized` | `QifAccountType.Memorized`      | Memorized transactions         |
| `!Account`        | `QifAccountType.Account`        | Account information            |

### Supported Field Types

#### Non-Investment Transactions

- `D` - Date (various formats supported)
- `T` - Transaction amount
- `C` - Cleared status (None, Cleared, Reconciled)
- `N` - Number (check or reference number)
- `P` - Payee
- `M` - Memo
- `A` - Address (up to 6 lines)
- `L` - Category/Transfer/Class
- `S` - Split category
- `E` - Split memo
- `$` - Split amount
- `^` - End of entry

#### Investment Transactions

- `D` - Date
- `N` - Action
- `Y` - Security
- `I` - Price
- `Q` - Quantity
- `T` - Transaction amount
- `C` - Cleared status
- `P` - Text for transfers
- `M` - Memo
- `O` - Commission
- `L` - Transfer account
- `$` - Transfer amount
- `^` - End of entry

### Date Format Support

The parser supports multiple date formats:

- `M/d/yy`, `M/d/yyyy` (US format)
- `MM/dd/yy`, `MM/dd/yyyy` (US format with leading zeros)
- `M-d-yy`, `M-d-yyyy` (Dash separators)
- `M.d.yy`, `M.d.yyyy` (Dot separators)
- `M/d/'yy` (Apostrophe format)
- ISO format: `yyyy-MM-dd`

### Amount Format Support

- Decimal amounts: `100.00`, `-50.25`
- Currency symbols: `$100.00`, `$-50.25`
- Thousands separators: `1,234.56`
- No decimal: `100`, `-50`
- Decimal only: `.50`, `-.75`

## Error Handling Strategy

### Exception Types

1. **ArgumentNullException**: Thrown for null input parameters
2. **FileNotFoundException**: Thrown when QIF file doesn't exist
3. **OperationCanceledException**: Thrown when parsing is cancelled
4. **FormatException**: Thrown for severely malformed QIF data (rare)

### Graceful Degradation

- Invalid dates set to `null` instead of throwing exceptions
- Invalid amounts set to `null` instead of throwing exceptions
- Unknown field types are ignored
- Comment lines are skipped
- Malformed entries are skipped

## Performance Requirements

### Parsing Performance

- **Small files** (< 1MB): < 100ms
- **Medium files** (1-10MB): < 1 second
- **Large files** (10-100MB): < 10 seconds
- **Memory usage**: O(n) where n is the number of records

### Scalability

- Support for files with 100,000+ transactions
- Streaming support for very large files
- Thread-safe for concurrent access

## Testing Requirements

### Test Coverage

- **Minimum Code Coverage**: 90%
- **Branch Coverage**: 85%
- **All Public APIs**: 100% tested

### Test Categories

1. **Unit Tests**: Individual component testing
2. **Integration Tests**: File parsing end-to-end
3. **Performance Tests**: Large file handling
4. **Edge Case Tests**: Malformed data handling
5. **Regression Tests**: Real-world QIF files

### Test Files Required

- Empty files
- Single transaction files
- Multi-transaction files
- Split transaction files
- Investment transaction files
- Memorized transaction files
- Category/Class definition files
- Files with comments
- Files with various date formats
- Files with various amount formats
- Malformed files
- Large files (1000+ transactions)

## Security Considerations

### Input Validation

- All string inputs are validated for length and content
- File paths are validated for existence and accessibility
- Stream inputs are validated for readability

### Memory Safety

- No unsafe code blocks
- Proper disposal of streams and file handles
- Protection against memory exhaustion attacks

### Data Privacy

- No logging of sensitive financial data
- No caching of parsed content beyond method scope
- Clear memory patterns for sensitive data

## Compatibility Requirements

### .NET Compatibility

- **Minimum**: .NET 9.0
- **Target**: Latest LTS version
- **Package Support**: NuGet package compatible

### Platform Support

- Windows 10/11
- macOS 10.15+
- Linux (Ubuntu 20.04+, RHEL 8+)

### Interoperability

- Compatible with Quicken 2010-2024
- Compatible with other QIF-exporting software
- Standard QIF format compliance

## Future Considerations

### Potential Enhancements

1. **Multi-section Files**: Better support for files with multiple `!Type:` sections
2. **Validation**: Optional strict validation mode
3. **Writing Support**: QIF file generation capabilities
4. **Localization**: Support for non-English QIF files
5. **Performance**: Parallel parsing for very large files
6. **Streaming**: True streaming parser for infinite files

### Backward Compatibility

- Semantic versioning (SemVer) compliance
- Deprecation warnings for breaking changes
- Migration guides for major version updates

## Quality Assurance

### Code Quality Standards

- Follow Microsoft C# coding conventions
- XML documentation for all public APIs
- Static analysis with no warnings
- Nullable reference types enabled
- Code analysis rules enforced

### Review Process

- Peer review for all changes
- Automated testing on all commits
- Performance regression testing
- Security review for external inputs

## Deployment

### Package Distribution

- NuGet package publication
- Symbol packages for debugging
- Documentation package
- Example projects

### Versioning Strategy

- Semantic versioning (Major.Minor.Patch)
- Release notes for all versions
- Migration guides for breaking changes
- Long-term support (LTS) versions

using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifOptionsTests
{
  [Fact]
  public void QifOptions_DefaultConstructor_UsesUSFormats()
  {
    var options = new QifOptions();

    Assert.Equal(QifOptionsUS.DefaultDateFormats, options.DateFormats);
    Assert.Equal(QifOptionsUS.DefaultDecimalSeparator, options.DecimalSeparator);
  }

  [Fact]
  public void QifOptions_CustomConstructor_UsesProvidedFormats()
  {
    var customDateFormats = new[] { "dd/MM/yyyy" };
    var customDecimalSeparator = ',';

    var options = new QifOptions(customDateFormats, customDecimalSeparator);

    Assert.Equal(customDateFormats, options.DateFormats);
    Assert.Equal(customDecimalSeparator, options.DecimalSeparator);
  }

  [Fact]
  public void QifOptionsUS_Create_ReturnsUSFormattedOptions()
  {
    var options = QifOptionsUS.Create();

    Assert.Equal(QifOptionsUS.DefaultDateFormats, options.DateFormats);
    Assert.Equal(QifOptionsUS.DefaultDecimalSeparator, options.DecimalSeparator);
  }

  [Fact]
  public void QifOptionsFR_Create_ReturnsFrenchFormattedOptions()
  {
    var options = QifOptionsFR.Create();

    Assert.Equal(QifOptionsFR.DefaultDateFormats, options.DateFormats);
    Assert.Equal(QifOptionsFR.DefaultDecimalSeparator, options.DecimalSeparator);
  }

  [Fact]
  public void QifReader_WithUSOptions_ParsesUSFormatDates()
  {
    var parser = new QifReader();
    parser.Options = QifOptionsUS.Create();

    var qifContent = @"!Type:Bank
D12/31/2023
T1234.56
^";

    var result = parser.ReadString(qifContent);

    Assert.Single(result.Transactions);
    var transaction = result.Transactions[0];
    Assert.Equal(new DateTime(2023, 12, 31), transaction.Date);
    Assert.Equal(1234.56m, transaction.Amount);
  }

  [Fact]
  public void QifReader_WithFrenchOptions_ParsesFrenchFormatDates()
  {
    var parser = new QifReader();
    parser.Options = QifOptionsFR.Create();

    var qifContent = @"!Type:Bank
D31/12/2023
T1234,56
^";

    var result = parser.ReadString(qifContent);

    Assert.Single(result.Transactions);
    var transaction = result.Transactions[0];
    Assert.Equal(new DateTime(2023, 12, 31), transaction.Date);
    Assert.Equal(1234.56m, transaction.Amount);
  }

  [Fact]
  public void QifReader_WithFrenchOptions_ParsesFrenchDecimalFormat()
  {
    var parser = new QifReader();
    parser.Options = QifOptionsFR.Create();

    var qifContent = @"!Type:Bank
D31/12/2023
T2 345,67
^";

    var result = parser.ReadString(qifContent);

    Assert.Single(result.Transactions);
    var transaction = result.Transactions[0];
    Assert.Equal(2345.67m, transaction.Amount);
  }

  [Fact]
  public void QifReader_WithCustomOptions_UsesCustomFormats()
  {
    var parser = new QifReader();
    parser.Options = new QifOptions(
      new[] { "yyyy-MM-dd" }, // Custom date format
      '.' // Custom decimal separator
    );

    var qifContent = @"!Type:Bank
D2023-12-31
T1234.56
^";

    var result = parser.ReadString(qifContent);

    Assert.Single(result.Transactions);
    var transaction = result.Transactions[0];
    Assert.Equal(new DateTime(2023, 12, 31), transaction.Date);
    Assert.Equal(1234.56m, transaction.Amount);
  }

  [Fact]
  public void QifReader_DefaultOptions_UsesUSFormats()
  {
    var parser = new QifReader();

    var qifContent = @"!Type:Bank
D12/31/2023
T1,234.56
^";

    var result = parser.ReadString(qifContent);

    Assert.Single(result.Transactions);
    var transaction = result.Transactions[0];
    Assert.Equal(new DateTime(2023, 12, 31), transaction.Date);
    Assert.Equal(1234.56m, transaction.Amount);
  }
}
using System;

namespace Altoviz.Qif.Models
{
  /// <summary>
  /// Configuration options for parsing QIF files, including date and decimal formats
  /// </summary>
  public class QifOptions
  {
    /// <summary>
    /// Date formats to try when parsing dates in QIF files.
    /// Formats should be compatible with DateTime.TryParseExact.
    /// </summary>
    public string[] DateFormats { get; set; }

    /// <summary>
    /// Decimal separator to try when parsing decimal values in QIF files.
    /// </summary>
    public char DecimalSeparator { get; set; }

    /// <summary>
    /// Initializes a new instance of QifOptions with default US formats
    /// </summary>
    public QifOptions()
    {
      DateFormats = QifOptionsUS.DefaultDateFormats;
      DecimalSeparator = QifOptionsUS.DefaultDecimalSeparator;
    }

    /// <summary>
    /// Initializes a new instance of QifOptions with custom formats
    /// </summary>
    /// <param name="dateFormats">Array of date format strings</param>
    /// <param name="decimalSeparator">Decimal separator string</param>
    public QifOptions(string[] dateFormats, char decimalSeparator = '.')
    {
      DateFormats = dateFormats ?? throw new ArgumentNullException(nameof(dateFormats));
      DecimalSeparator = decimalSeparator;
    }
  }
}
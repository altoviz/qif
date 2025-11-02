namespace Altoviz.Qif.Models
{
  /// <summary>
  /// Predefined QIF options for French locale (dd/MM/yyyy dates, comma decimal separator)
  /// </summary>
  public static class QifOptionsFR
  {
    /// <summary>
    /// Default French date formats used in QIF files
    /// </summary>
    public static readonly string[] DefaultDateFormats = new[]
    {
      "d/M/yy", "d/M/yyyy", "dd/MM/yy", "dd/MM/yyyy",
      "d-M-yy", "d-M-yyyy", "dd-MM-yy", "dd-MM-yyyy",
      "d.M.yy", "d.M.yyyy", "dd.MM.yy", "dd.MM.yyyy"
    };

    /// <summary>
    /// Default French decimal separator
    /// </summary>
    public static readonly char DefaultDecimalSeparator = ',';

    /// <summary>
    /// Creates a new QifOptions instance with French formatting
    /// </summary>
    /// <returns>QifOptions configured for French locale</returns>
    public static QifOptions Create()
    {
      return new QifOptions(DefaultDateFormats, DefaultDecimalSeparator);
    }
  }
}
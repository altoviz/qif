namespace Altoviz.Qif.Models
{
  /// <summary>
  /// Predefined QIF options for US locale (MM/dd/yyyy dates, period decimal separator)
  /// </summary>
  public static class QifOptionsUS
  {
    /// <summary>
    /// Default US date formats used in QIF files
    /// </summary>
    public static readonly string[] DefaultDateFormats = new[]
    {
      "M/d/yy", "M/d/yyyy", "MM/dd/yy", "MM/dd/yyyy",
      "M-d-yy", "M-d-yyyy", "MM-dd-yy", "MM-dd-yyyy",
      "M.d.yy", "M.d.yyyy", "MM.dd.yy", "MM.dd.yyyy"
    };

    /// <summary>
    /// Default US decimal separators
    /// </summary>
    public static readonly char DefaultDecimalSeparator = '.';

    /// <summary>
    /// Creates a new QifOptions instance with US formatting
    /// </summary>
    /// <returns>QifOptions configured for US locale</returns>
    public static QifOptions Create()
    {
      return new QifOptions(DefaultDateFormats, DefaultDecimalSeparator);
    }
  }
}
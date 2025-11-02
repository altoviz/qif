using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altoviz.Qif.Models;

namespace Altoviz.Qif
{

    /// <summary>
    /// QIF file reader implementation with async/await support
    /// </summary>
    public class QifReader : IQifReader
    {
        private const string EndOfEntry = "^";

        /// <summary>
        /// Configuration options for parsing QIF files, including date and decimal formats.
        /// Defaults to US formatting (MM/dd/yyyy dates, period decimal separator).
        /// Use QifOptionsUS.Create() or QifOptionsFR.Create() for predefined locale settings.
        /// </summary>
        public QifOptions Options { get; set; } = new QifOptions();

        /// <summary>
        /// Reads a QIF file from the specified file path asynchronously
        /// </summary>
        /// <param name="filePath">Path to the QIF file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Parsed QIF document</returns>
        /// <exception cref="ArgumentNullException">Thrown when filePath is null or empty</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist</exception>
        public async Task<QifDocument> ReadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"QIF file not found: {filePath}");

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return await ReadAsync(fileStream, cancellationToken);
        }

        /// <summary>
        /// Reads QIF content from a stream asynchronously
        /// </summary>
        /// <param name="stream">Stream containing QIF data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Parsed QIF document</returns>
        /// <exception cref="ArgumentNullException">Thrown when stream is null</exception>
        public async Task<QifDocument> ReadAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using var reader = new StreamReader(stream, Encoding.UTF8);
            var content = await reader.ReadToEndAsync();
            return ReadString(content);
        }

        /// <summary>
        /// Reads QIF content from a string
        /// </summary>
        /// <param name="content">QIF content as string</param>
        /// <returns>Parsed QIF document</returns>
        /// <exception cref="ArgumentNullException">Thrown when content is null</exception>
        public QifDocument ReadString(string content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var document = new QifDocument();
            var lines = content.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
                return document;

            var currentIndex = 0;

            // Skip comment lines and find the first !Type: header
            while (currentIndex < lines.Length &&
                   (lines[currentIndex].StartsWith("#") ||
                    lines[currentIndex].StartsWith("###") ||
                    string.IsNullOrWhiteSpace(lines[currentIndex])))
            {
                currentIndex++;
            }

            // Parse header
            if (currentIndex < lines.Length && lines[currentIndex].StartsWith("!Type:"))
            {
                document.AccountType = ParseAccountType(lines[currentIndex]);
                currentIndex++;
            }

            // Check for AllXfr option
            if (currentIndex < lines.Length && lines[currentIndex] == "!Option:AllXfr")
            {
                document.ImportAllTransfers = true;
                currentIndex++;
            }

            // Parse entries
            while (currentIndex < lines.Length)
            {
                // Skip comment lines
                if (lines[currentIndex].StartsWith("#") || lines[currentIndex].StartsWith("###"))
                {
                    currentIndex++;
                    continue;
                }

                var entry = ExtractEntry(lines, ref currentIndex);
                if (entry.Count > 0)
                {
                    ParseEntry(document, entry);
                }
            }

            return document;
        }

        private static QifAccountType ParseAccountType(string headerLine)
        {
            var type = headerLine.Substring(6); // Remove "!Type:"
            return type switch
            {
                "Bank" => QifAccountType.Bank,
                "Cash" => QifAccountType.Cash,
                "CCard" => QifAccountType.CreditCard,
                "Invst" => QifAccountType.Investment,
                "Oth A" => QifAccountType.OtherAsset,
                "Oth L" => QifAccountType.OtherLiability,
                "Cat" => QifAccountType.Category,
                "Class" => QifAccountType.Class,
                "Memorized" => QifAccountType.Memorized,
                _ => QifAccountType.Bank
            };
        }

        private static List<string> ExtractEntry(string[] lines, ref int currentIndex)
        {
            var entry = new List<string>();

            while (currentIndex < lines.Length)
            {
                var line = lines[currentIndex++];

                // Skip comment lines
                if (line.StartsWith("#") || line.StartsWith("###"))
                    continue;

                if (line == EndOfEntry)
                    break;

                if (line.StartsWith("!Account"))
                {
                    // Handle account header
                    entry.Add(line);
                    continue;
                }

                entry.Add(line);
            }

            return entry;
        }

        private void ParseEntry(QifDocument document, List<string> entry)
        {
            if (entry.Count == 0) return;

            // Check if this is an account header
            if (entry[0].StartsWith("!Account"))
            {
                var account = ParseAccount(entry);
                if (account != null)
                    document.Accounts.Add(account);
                return;
            }

            switch (document.AccountType)
            {
                case QifAccountType.Investment:
                    var investmentTransaction = ParseInvestmentTransaction(entry);
                    if (investmentTransaction != null)
                        document.InvestmentTransactions.Add(investmentTransaction);
                    break;

                case QifAccountType.Memorized:
                    var memorizedTransaction = ParseMemorizedTransaction(entry);
                    if (memorizedTransaction != null)
                        document.MemorizedTransactions.Add(memorizedTransaction);
                    break;

                case QifAccountType.Category:
                    var category = ParseCategory(entry);
                    if (category != null)
                        document.Categories.Add(category);
                    break;

                case QifAccountType.Class:
                    var qifClass = ParseClass(entry);
                    if (qifClass != null)
                        document.Classes.Add(qifClass);
                    break;

                default:
                    var transaction = ParseTransaction(entry);
                    if (transaction != null)
                        document.Transactions.Add(transaction);
                    break;
            }
        }

        private QifTransaction? ParseTransaction(List<string> entry)
        {
            var transaction = new QifTransaction();
            var hasData = false;

            foreach (var line in entry)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var indicator = line[0];
                var value = line.Length > 1 ? line[1..] : string.Empty;

                switch (indicator)
                {
                    case 'D':
                        transaction.Date = ParseDate(value);
                        hasData = true;
                        break;
                    case 'T':
                        transaction.Amount = ParseDecimal(value);
                        hasData = true;
                        break;
                    case 'C':
                        transaction.ClearedStatus = ParseClearedStatus(value);
                        break;
                    case 'N':
                        transaction.Number = value;
                        break;
                    case 'P':
                        transaction.Payee = value;
                        break;
                    case 'M':
                        transaction.Memo = value;
                        break;
                    case 'A':
                        transaction.Address.Add(value);
                        break;
                    case 'L':
                        transaction.Category = value;
                        break;
                    case 'S':
                        // Start of split - we'll collect these separately
                        break;
                    case 'E':
                    case '$':
                        // These are handled with splits
                        break;
                }
            }

            // Parse splits
            ParseSplits(entry, transaction.Splits);

            return hasData ? transaction : null;
        }

        private QifInvestmentTransaction? ParseInvestmentTransaction(List<string> entry)
        {
            var transaction = new QifInvestmentTransaction();
            var hasData = false;

            foreach (var line in entry)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var indicator = line[0];
                var value = line.Length > 1 ? line[1..] : string.Empty;

                switch (indicator)
                {
                    case 'D':
                        transaction.Date = ParseDate(value);
                        hasData = true;
                        break;
                    case 'N':
                        transaction.Action = value;
                        break;
                    case 'Y':
                        transaction.Security = value;
                        break;
                    case 'I':
                        transaction.Price = ParseDecimal(value);
                        break;
                    case 'Q':
                        transaction.Quantity = ParseDecimal(value);
                        break;
                    case 'T':
                        transaction.Amount = ParseDecimal(value);
                        hasData = true;
                        break;
                    case 'C':
                        transaction.ClearedStatus = ParseClearedStatus(value);
                        break;
                    case 'P':
                        transaction.Payee = value;
                        break;
                    case 'M':
                        transaction.Memo = value;
                        break;
                    case 'O':
                        transaction.Commission = ParseDecimal(value);
                        break;
                    case 'L':
                        transaction.TransferAccount = value;
                        break;
                    case '$':
                        transaction.TransferAmount = ParseDecimal(value);
                        break;
                }
            }

            return hasData ? transaction : null;
        }

        private QifMemorizedTransaction? ParseMemorizedTransaction(List<string> entry)
        {
            var transaction = new QifMemorizedTransaction();
            var hasData = false;
            QifAmortization? amortization = null;

            foreach (var line in entry)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var indicator = line[0];
                var value = line.Length > 1 ? line[1..] : string.Empty;

                switch (indicator)
                {
                    case 'T':
                        transaction.Amount = ParseDecimal(value);
                        hasData = true;
                        break;
                    case 'C':
                        transaction.ClearedStatus = ParseClearedStatus(value);
                        break;
                    case 'P':
                        transaction.Payee = value;
                        break;
                    case 'M':
                        transaction.Memo = value;
                        break;
                    case 'A':
                        transaction.Address.Add(value);
                        break;
                    case 'L':
                        transaction.Category = value;
                        break;
                    case 'S':
                    case 'E':
                    case '$':
                        // Handle splits
                        break;
                    case 'K':
                        if (line.Length >= 2)
                        {
                            transaction.TransactionType = line[1] switch
                            {
                                'C' => QifMemorizedTransactionType.Check,
                                'D' => QifMemorizedTransactionType.Deposit,
                                'P' => QifMemorizedTransactionType.Payment,
                                'I' => QifMemorizedTransactionType.Investment,
                                'E' => QifMemorizedTransactionType.ElectronicPayee,
                                _ => QifMemorizedTransactionType.Check
                            };
                            hasData = true;
                        }
                        break;
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                        // Amortization fields
                        amortization ??= new QifAmortization();
                        ParseAmortizationField(amortization, indicator, value);
                        break;
                }
            }

            if (amortization != null)
                transaction.Amortization = amortization;

            // Parse splits
            ParseSplits(entry, transaction.Splits);

            return hasData ? transaction : null;
        }

        private void ParseAmortizationField(QifAmortization amortization, char field, string value)
        {
            switch (field)
            {
                case '1':
                    amortization.FirstPaymentDate = ParseDate(value);
                    break;
                case '2':
                    amortization.TotalYears = ParseInt(value);
                    break;
                case '3':
                    amortization.PaymentsMade = ParseInt(value);
                    break;
                case '4':
                    amortization.PeriodsPerYear = ParseInt(value);
                    break;
                case '5':
                    amortization.InterestRate = ParseDecimal(value);
                    break;
                case '6':
                    amortization.CurrentBalance = ParseDecimal(value);
                    break;
                case '7':
                    amortization.OriginalAmount = ParseDecimal(value);
                    break;
            }
        }

        private QifAccount? ParseAccount(List<string> entry)
        {
            var account = new QifAccount();
            var hasData = false;

            foreach (var line in entry)
            {
                if (string.IsNullOrEmpty(line)) continue;

                if (line.StartsWith("!Account"))
                    continue;

                var indicator = line[0];
                var value = line.Length > 1 ? line[1..] : string.Empty;

                switch (indicator)
                {
                    case 'N':
                        account.Name = value;
                        hasData = true;
                        break;
                    case 'T':
                        account.Type = value;
                        break;
                    case 'D':
                        account.Description = value;
                        break;
                    case 'L':
                        account.CreditLimit = ParseDecimal(value);
                        break;
                    case '/':
                        account.StatementBalanceDate = ParseDate(value);
                        break;
                    case '$':
                        account.StatementBalance = ParseDecimal(value);
                        break;
                }
            }

            return hasData ? account : null;
        }

        private QifCategory? ParseCategory(List<string> entry)
        {
            var category = new QifCategory();
            var hasData = false;

            foreach (var line in entry)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var indicator = line[0];
                var value = line.Length > 1 ? line[1..] : string.Empty;

                switch (indicator)
                {
                    case 'N':
                        category.Name = value;
                        hasData = true;
                        break;
                    case 'D':
                        category.Description = value;
                        break;
                    case 'T':
                        category.IsTaxRelated = true;
                        break;
                    case 'I':
                        category.IsIncomeCategory = true;
                        break;
                    case 'E':
                        category.IsIncomeCategory = false;
                        break;
                    case 'B':
                        category.BudgetAmount = ParseDecimal(value);
                        break;
                    case 'R':
                        category.TaxSchedule = value;
                        break;
                }
            }

            return hasData ? category : null;
        }

        private QifClass? ParseClass(List<string> entry)
        {
            var qifClass = new QifClass();
            var hasData = false;

            foreach (var line in entry)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var indicator = line[0];
                var value = line.Length > 1 ? line[1..] : string.Empty;

                switch (indicator)
                {
                    case 'N':
                        qifClass.Name = value;
                        hasData = true;
                        break;
                    case 'D':
                        qifClass.Description = value;
                        break;
                }
            }

            return hasData ? qifClass : null;
        }

        private void ParseSplits(List<string> entry, List<QifSplit> splits)
        {
            QifSplit? currentSplit = null;

            foreach (var line in entry)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var indicator = line[0];
                var value = line.Length > 1 ? line[1..] : string.Empty;

                switch (indicator)
                {
                    case 'S':
                        // Start new split
                        if (currentSplit != null)
                            splits.Add(currentSplit);
                        currentSplit = new QifSplit { Category = value };
                        break;
                    case 'E':
                        if (currentSplit != null)
                            currentSplit.Memo = value;
                        break;
                    case '$':
                        if (currentSplit != null)
                            currentSplit.Amount = ParseDecimal(value) ?? 0;
                        break;
                }
            }

            // Add the last split
            if (currentSplit != null)
                splits.Add(currentSplit);
        }

        private DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            // QIF dates can be in various formats: M/d/yy, M/d/yyyy, M/d/'yy
            var cleanValue = value.Replace("'", "").Trim();
            var formats = Options?.DateFormats ?? QifOptionsUS.DefaultDateFormats;
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(cleanValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    // Handle 2-digit years
                    if (date.Year < 100)
                        date = date.AddYears(2000);
                    return date;
                }
            }
            // Fallback to standard parsing
            if (DateTime.TryParse(cleanValue, out var fallbackDate))
            {
                return fallbackDate;
            }
            return null;
        }
        private decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var decimalSeparator = Options?.DecimalSeparator ?? QifOptionsUS.DefaultDecimalSeparator;
            var numbers = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            // Clean the value by keeping only numbers, decimal separator, and minus sign
            var cleanValue = new string(value.Where(c => c == decimalSeparator || numbers.Contains(c) || c == '-').ToArray());

            // If using comma as decimal separator, replace it with period for parsing
            if (decimalSeparator == ',')
            {
                cleanValue = cleanValue.Replace(',', '.');
            }

            // Try to parse the cleaned value
            if (decimal.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        private static int? ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if (int.TryParse(value.Trim(), out var result))
            {
                return result;
            }
            return null;
        }

        private static QifClearedStatus ParseClearedStatus(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return QifClearedStatus.None;
            switch (value.ToUpperInvariant())
            {
                case "C":
                case "CLEARED":
                    return QifClearedStatus.Cleared;
                case "R":
                case "RECONCILED":
                    return QifClearedStatus.Reconciled;
                case "X":
                    return QifClearedStatus.Cleared; // X is also used for cleared
                default:
                    return QifClearedStatus.None;
            }
            ;
        }
    }
}
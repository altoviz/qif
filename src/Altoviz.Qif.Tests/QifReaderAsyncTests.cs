using System.Text;
using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifReaderAsyncTests
{
    private readonly QifReader _parser = new();

    [Fact]
    public async Task ReadAsync_WithValidStream_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Bank
            D6/1/94
            T-1,000.00
            N1005
            PBank Of Mortgage
            LMortgage
            ^
            """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(qifContent));
        var result = await _parser.ReadAsync(stream);

        Assert.Equal(QifAccountType.Bank, result.AccountType);
        Assert.Single(result.Transactions);
        Assert.Equal(-1000.00m, result.Transactions[0].Amount);
    }

    [Fact]
    public async Task ReadAsync_WithValidTempFile_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Cash
            D12/25/2023
            T25.50
            PGift
            LMisc
            ^
            """;

        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, qifContent);
            var result = await _parser.ReadAsync(tempFile);

            Assert.Equal(QifAccountType.Cash, result.AccountType);
            Assert.Single(result.Transactions);
            Assert.Equal(25.50m, result.Transactions[0].Amount);
            Assert.Equal("Gift", result.Transactions[0].Payee);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ReadAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        var qifContent = """
            !Type:Bank
            D6/1/94
            T100.00
            PTest
            ^
            """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(qifContent));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            _parser.ReadAsync(stream, cts.Token));
    }

    [Fact]
    public async Task ReadAsync_File_WithCancellation_ThrowsOperationCanceledException()
    {
        var qifContent = "!Type:Bank\nD6/1/94\nT100.00\nPTest\n^";
        var tempFile = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(tempFile, qifContent);

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                _parser.ReadAsync(tempFile, cts.Token));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ReadAsync_Stream_WithLargeFile_HandlesCorrectly()
    {
        var sb = new StringBuilder();
        sb.AppendLine("!Type:Bank");

        // Create 1000 transactions
        for (int i = 1; i <= 1000; i++)
        {
            sb.AppendLine($"D1/{i % 28 + 1}/2023");
            sb.AppendLine($"T{i * 10}.00");
            sb.AppendLine($"PPayee {i}");
            sb.AppendLine($"LCategory {i % 10}");
            sb.AppendLine("^");
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        var result = await _parser.ReadAsync(stream);

        Assert.Equal(QifAccountType.Bank, result.AccountType);
        Assert.Equal(1000, result.Transactions.Count);

        // Verify first and last transactions
        Assert.Equal(10.00m, result.Transactions[0].Amount);
        Assert.Equal("Payee 1", result.Transactions[0].Payee);

        Assert.Equal(10000.00m, result.Transactions[999].Amount);
        Assert.Equal("Payee 1000", result.Transactions[999].Payee);
    }
}
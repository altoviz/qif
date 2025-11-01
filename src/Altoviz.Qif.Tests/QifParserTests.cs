using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifParserTests
{
    private readonly QifParser _parser = new();

    [Fact]
    public void ParseString_WithNullContent_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _parser.ParseString(null!));
    }

    [Fact]
    public void ParseString_WithEmptyContent_ReturnsEmptyDocument()
    {
        var result = _parser.ParseString(string.Empty);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalItems);
    }

    [Fact]
    public async Task ParseAsync_WithNullFilePath_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _parser.ParseAsync((Stream)null!));
    }

    [Fact]
    public async Task ParseAsync_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(() => _parser.ParseAsync("nonexistent.qif"));
    }

    [Fact]
    public async Task ParseAsync_WithNullStream_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _parser.ParseAsync((Stream)null!));
    }

    [Fact]
    public void ParseString_WithBankHeaderOnly_ReturnsBankDocument()
    {
        var qifContent = "!Type:Bank";
        var result = _parser.ParseString(qifContent);

        Assert.Equal(QifAccountType.Bank, result.AccountType);
        Assert.Equal(0, result.TotalItems);
    }

    [Fact]
    public void ParseString_WithAllTransferOption_SetsImportAllTransfers()
    {
        var qifContent = """
            !Type:Bank
            !Option:AllXfr
            """;

        var result = _parser.ParseString(qifContent);

        Assert.Equal(QifAccountType.Bank, result.AccountType);
        Assert.True(result.ImportAllTransfers);
    }

    [Fact]
    public void ParseString_WithSimpleBankTransaction_ParsesCorrectly()
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

        var result = _parser.ParseString(qifContent);

        Assert.Equal(QifAccountType.Bank, result.AccountType);
        Assert.Single(result.Transactions);

        var transaction = result.Transactions[0];
        Assert.Equal(new DateTime(1994, 6, 1), transaction.Date);
        Assert.Equal(-1000.00m, transaction.Amount);
        Assert.Equal("1005", transaction.Number);
        Assert.Equal("Bank Of Mortgage", transaction.Payee);
        Assert.Equal("Mortgage", transaction.Category);
    }

    [Fact]
    public void ParseString_WithTransactionWithSplits_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Bank
            D6/1/94
            T-1,000.00
            N1005
            PBank Of Mortgage
            L[linda]
            S[linda]
            $-253.64
            SMort Int
            $-746.36
            ^
            """;

        var result = _parser.ParseString(qifContent);

        Assert.Single(result.Transactions);
        var transaction = result.Transactions[0];
        Assert.Equal(2, transaction.Splits.Count);

        Assert.Equal("[linda]", transaction.Splits[0].Category);
        Assert.Equal(-253.64m, transaction.Splits[0].Amount);

        Assert.Equal("Mort Int", transaction.Splits[1].Category);
        Assert.Equal(-746.36m, transaction.Splits[1].Amount);
    }

    [Fact]
    public void ParseString_WithClearedStatus_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Bank
            D6/1/94
            T100.00
            CC
            PTest Payee
            ^
            """;

        var result = _parser.ParseString(qifContent);

        Assert.Single(result.Transactions);
        Assert.Equal(QifClearedStatus.Cleared, result.Transactions[0].ClearedStatus);
    }

    [Fact]
    public void ParseString_WithAddress_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Bank
            D6/3/94
            T-10.00
            PAnthony Hopkins
            MFilm
            LEntertain
            AP.O. Box 27027
            ATucson, AZ
            A85726
            A
            A
            A
            ^
            """;

        var result = _parser.ParseString(qifContent);

        Assert.Single(result.Transactions);
        var transaction = result.Transactions[0];
        Assert.Equal("Film", transaction.Memo);
        Assert.Equal("Entertain", transaction.Category);
        Assert.Equal(6, transaction.Address.Count);
        Assert.Equal("P.O. Box 27027", transaction.Address[0]);
        Assert.Equal("Tucson, AZ", transaction.Address[1]);
        Assert.Equal("85726", transaction.Address[2]);
        Assert.Equal("", transaction.Address[3]);
    }
}
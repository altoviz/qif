using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifClassTests
{
    private readonly QifReader _parser = new();

    [Fact]
    public void ReadString_WithClass_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Class
            NBusiness
            DBusiness related expenses
            ^
            """;
        
        var result = _parser.ReadString(qifContent);
        
        Assert.Equal(QifAccountType.Class, result.AccountType);
        Assert.Single(result.Classes);
        
        var qifClass = result.Classes[0];
        Assert.Equal("Business", qifClass.Name);
        Assert.Equal("Business related expenses", qifClass.Description);
    }

    [Fact]
    public void ReadString_WithMinimalClass_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Class
            NPersonal
            ^
            """;
        
        var result = _parser.ReadString(qifContent);
        
        Assert.Single(result.Classes);
        var qifClass = result.Classes[0];
        
        Assert.Equal("Personal", qifClass.Name);
        Assert.Null(qifClass.Description);
    }

    [Fact]
    public void ReadString_WithMultipleClasses_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Class
            NBusiness
            DBusiness expenses
            ^
            NPersonal
            DPersonal expenses
            ^
            NRental
            DRental property
            ^
            """;
        
        var result = _parser.ReadString(qifContent);
        
        Assert.Equal(3, result.Classes.Count);
        Assert.Equal("Business", result.Classes[0].Name);
        Assert.Equal("Personal", result.Classes[1].Name);
        Assert.Equal("Rental", result.Classes[2].Name);
    }
}
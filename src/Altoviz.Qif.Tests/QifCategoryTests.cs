using Altoviz.Qif;
using Altoviz.Qif.Models;
using Xunit;

namespace Altoviz.Qif.Tests;

public class QifCategoryTests
{
    private readonly QifReader _parser = new();

    [Fact]
    public void ReadString_WithIncomeCategory_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Cat
            NSalary:Bonus
            DSalary and bonus income
            T
            I
            B5000.00
            RW-2
            ^
            """;
        
        var result = _parser.ReadString(qifContent);
        
        Assert.Equal(QifAccountType.Category, result.AccountType);
        Assert.Single(result.Categories);
        
        var category = result.Categories[0];
        Assert.Equal("Salary:Bonus", category.Name);
        Assert.Equal("Salary and bonus income", category.Description);
        Assert.True(category.IsTaxRelated);
        Assert.True(category.IsIncomeCategory);
        Assert.Equal(5000.00m, category.BudgetAmount);
        Assert.Equal("W-2", category.TaxSchedule);
    }

    [Fact]
    public void ReadString_WithExpenseCategory_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Cat
            NGroceries
            DFood and household items
            E
            B300.00
            ^
            """;
        
        var result = _parser.ReadString(qifContent);
        
        Assert.Single(result.Categories);
        var category = result.Categories[0];
        
        Assert.Equal("Groceries", category.Name);
        Assert.Equal("Food and household items", category.Description);
        Assert.False(category.IsTaxRelated);
        Assert.False(category.IsIncomeCategory);
        Assert.Equal(300.00m, category.BudgetAmount);
    }

    [Fact]
    public void ReadString_WithMinimalCategory_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Cat
            NUtilities
            ^
            """;
        
        var result = _parser.ReadString(qifContent);
        
        Assert.Single(result.Categories);
        var category = result.Categories[0];
        
        Assert.Equal("Utilities", category.Name);
        Assert.Null(category.Description);
        Assert.False(category.IsTaxRelated);
        Assert.False(category.IsIncomeCategory);
        Assert.Null(category.BudgetAmount);
        Assert.Null(category.TaxSchedule);
    }

    [Fact]
    public void ReadString_WithMultipleCategories_ParsesCorrectly()
    {
        var qifContent = """
            !Type:Cat
            NSalary
            DSalary income
            I
            ^
            NGroceries
            DFood expenses
            E
            ^
            NRent
            DHousing expenses
            E
            ^
            """;
        
        var result = _parser.ReadString(qifContent);
        
        Assert.Equal(3, result.Categories.Count);
        Assert.True(result.Categories[0].IsIncomeCategory);
        Assert.False(result.Categories[1].IsIncomeCategory);
        Assert.False(result.Categories[2].IsIncomeCategory);
    }
}
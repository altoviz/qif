namespace Altoviz.Qif.Models;

/// <summary>
/// Represents a class in QIF format
/// </summary>
public class QifClass
{
    /// <summary>
    /// Class name
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Class description
    /// </summary>
    public string? Description { get; set; }
    
    public override string ToString()
    {
        return $"Name: {Name}, Description: {Description}";
    }
}
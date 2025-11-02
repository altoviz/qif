namespace Altoviz.Qif.Models
{

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
        /// <summary>
        /// Returns a string representation of the QifClass
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Name: {Name}, Description: {Description}";
        }
    }
}
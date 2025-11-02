using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Altoviz.Qif.Models;

namespace Altoviz.Qif
{

    /// <summary>
    /// Interface for QIF file reading operations
    /// </summary>
    public interface IQifReader
    {
        /// <summary>
        /// Reads a QIF file from the specified file path asynchronously
        /// </summary>
        /// <param name="filePath">Path to the QIF file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Parsed QIF document</returns>
        Task<QifDocument> ReadAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads QIF content from a stream asynchronously
        /// </summary>
        /// <param name="stream">Stream containing QIF data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Parsed QIF document</returns>
        Task<QifDocument> ReadAsync(Stream stream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads QIF content from a string
        /// </summary>
        /// <param name="content">QIF content as string</param>
        /// <returns>Parsed QIF document</returns>
        QifDocument ReadString(string content);
    }
}
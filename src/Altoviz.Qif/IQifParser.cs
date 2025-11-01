using Altoviz.Qif.Models;

namespace Altoviz.Qif;

/// <summary>
/// Interface for QIF file parsing operations
/// </summary>
public interface IQifParser
{
    /// <summary>
    /// Parses a QIF file from the specified file path asynchronously
    /// </summary>
    /// <param name="filePath">Path to the QIF file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parsed QIF document</returns>
    Task<QifDocument> ParseAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses QIF content from a stream asynchronously
    /// </summary>
    /// <param name="stream">Stream containing QIF data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parsed QIF document</returns>
    Task<QifDocument> ParseAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses QIF content from a string
    /// </summary>
    /// <param name="content">QIF content as string</param>
    /// <returns>Parsed QIF document</returns>
    QifDocument ParseString(string content);
}
using System;
using Unity.Services.Multiplayer;
using UnityEngine;

/// <summary>
/// The custom exception for session operations
/// </summary>
public class CustomSessionException : Exception
{
    /// <summary>
    /// Gets the error type
    /// </summary>
    public SessionError Error { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomSessionException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="error">The exception type.</param>
    public CustomSessionException(string message, SessionError error) : base(message)
    {
        Error = error;
    }

    /// <summary>
    /// Returns a string representation of the
    /// <see cref="CustomSessionException"/>instance.
    /// </summary>
    /// <returns>A string that represents the current exception with its
    /// <see cref="SessionError"/> and its <see cref="Exception.Message"/>.</returns>
    public override string ToString()
    {
        return $"CustomSessionException: [Error: {Error}] [Message: {Message}]";
    }
}

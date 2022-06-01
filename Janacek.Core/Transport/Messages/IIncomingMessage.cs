//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System.Collections.Generic;

namespace Janacek
{
    /// <summary>
    /// Interface for incoming message.
    /// </summary>
    public interface IIncomingMessage : IReadOnlyCollection<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Determines whether the message contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The element key to locate.</param>
        /// <returns>
        /// <c>True</c> if the message contains a top level element that has the specified key; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets the value that is associated with the specified top level element key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, 
        /// if the key is found; otherwise, the default value for the type of the value parameter. 
        /// This parameter is passed uninitialized.</param>
        /// <returns>
        /// <c>True</c> if the message contains an element that has the specified key; otherwise, <c>false</c>.
        /// </returns>
        bool TryGetValue(string key, out object value);

        /// <summary>
        /// Gets the top level element that has the specified key in the message.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        /// The top level element that has the specified key in the message.
        /// </returns>
        object this[string key] { get; }
    }
}
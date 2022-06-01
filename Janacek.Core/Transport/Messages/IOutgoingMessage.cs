//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System.Collections.Generic;

namespace Janacek
{
    /// <summary>
    /// Interface for outgoing message.
    /// </summary>
    public interface IOutgoingMessage : IIncomingMessage
    {
        /// <summary>
        /// Adds a top level element to the message.
        /// </summary>
        /// <param name="item">The top level element to add.</param>
        void Add(KeyValuePair<string, object> item);

        /// <summary>
        /// Adds a top level element with the provided key and value to the message.
        /// </summary>
        /// <param name="key">The key of the top level element to add.</param>
        /// <param name="value">The value of the top level element to add.</param>
        void Add(string key, object value);

        /// <summary>
        /// Removes all top level elements from the message.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets or sets the value associated with the specified top level element key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, 
        /// a get operation throws a <see cref="KeyNotFoundException"/>, 
        /// and a set operation creates a new element with the specified key.
        /// </returns>
        new object this[string key] { get; set; }

        /// <summary>
        /// Serialize the message into Json format..
        /// </summary>
        /// <returns>
        /// A string representing the value of the message in Json format.
        /// </returns>
        string Serialize();
    }
}
//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Janacek
{
    /// <summary>
    /// Class representing message sent between Janacek components and microservices.
    /// </summary>
    /// <seealso cref="T:Janacek.IOutgoingMessage"/>
    /// <seealso cref="T:Janacek.IIncomingMessage"/>
    public class Message : IOutgoingMessage, IIncomingMessage
    {
        /// <summary>
        /// Empty message.
        /// </summary>
        public static readonly Message Empty = new Message(new Dictionary<string, object>(0));

        /// <summary>
        /// Message data (top level elements).
        /// </summary>
        protected Dictionary<string, object> _data;

        /// <summary>
        /// Initializes a new instance of the Janacek.Message class.
        /// </summary>
        public Message()
        {
            _data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the Janacek.Message class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Message(Dictionary<string, object> data)
        {
            _data = data;
        }

        /// <summary>
        /// Initializes a new instance of the Janacek.Message class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Message(IEnumerable<KeyValuePair<string, object>> data)
        {
            _data = new Dictionary<string, object>(data, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns an enumerator that iterates through top level elements in the message.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        /// <seealso cref="M:Janacek.IIncomingMessage.GetEnumerator()"/>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a top level element to the message.
        /// </summary>
        /// <param name="item">The top level element to add.</param>
        /// <seealso cref="M:Janacek.IOutgoingMessage.Add(KeyValuePair{string,object})"/>
        public void Add(KeyValuePair<string, object> item)
        {
            _data.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds a top level element with the provided key and value to the message.
        /// </summary>
        /// <param name="key">The key of the top level element to add.</param>
        /// <param name="value">The value of the top level element to add.</param>
        /// <seealso cref="M:Janacek.IOutgoingMessage.Add(string,object)"/>
        public void Add(string key, object value)
        {
            _data.Add(key, value);
        }

        /// <summary>
        /// Removes all top level elements from the message.
        /// </summary>
        /// <seealso cref="M:Janacek.IOutgoingMessage.Clear()"/>
        public void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// Gets the number of top level elements in the message.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        /// <seealso cref="P:Janacek.IIncomingMessage.Count"/>
        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

        /// <summary>
        /// Determines whether the message contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The element key to locate.</param>
        /// <returns>
        /// <c>True</c> if the message contains a top level element that has the specified key; otherwise, <c>false</c>.
        /// </returns>
        /// <seealso cref="M:Janacek.IIncomingMessage.ContainsKey(string)"/>
        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value that is associated with the specified top level element key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">[out] When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value
        /// parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// <c>True</c> if the message contains an element that has the specified key; otherwise, <c>false</c>.
        /// </returns>
        /// <seealso cref="M:Janacek.IIncomingMessage.TryGetValue(string,out object)"/>
        public bool TryGetValue(string key, out object value)
        {
            return _data.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets or sets the value associated with the specified top level element key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="T:System.Collections.Generic.KeyNotFoundException" />,
        /// and a set operation creates a new element with the specified key.
        /// </returns>
        /// <seealso cref="M:Janacek.IOutgoingMessage.this(string)"/>
        public object this[string key]
        {
            get
            {
                return _data[key];
            }
            set
            {
                _data[key] = value;
            }
        }

        /// <summary>
        /// Serialize the message into Json format.
        /// </summary>
        /// <returns>
        /// A string representing the value of the message in Json format.
        /// </returns>
        /// <seealso cref="M:Janacek.IOutgoingMessage.Serialize()"/>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(_data);
        }

        /// <summary>
        /// Deserialize this Message to the given stream.
        /// </summary>
        /// <param name="serialized">The serialized.</param>
        /// <returns>
        /// A Message.
        /// </returns>
        public static Message Deserialize(string serialized)
        {
            return new Message(JsonConvert.DeserializeObject<Dictionary<string, object>>(serialized));
        }
    }
}

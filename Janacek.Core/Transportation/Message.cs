using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Janacek
{
    public class Message : IReadOnlyCollection<KeyValuePair<string, object>>
    {
        public static readonly Message Empty = new Message(new Dictionary<string, object>(0));

        protected Dictionary<string, object> _data;

        public Message()
        {
            _data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public Message(Dictionary<string, object> data)
        {
            _data = data;
        }

        public Message(IEnumerable<KeyValuePair<string, object>> data)
        {
            _data = new Dictionary<string, object>(data, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _data.Add(item.Key, item.Value);
        }

        public void Add(string key, object value)
        {
            _data.Add(key, value);
        }

        public void Clear()
        {
            _data.Clear();
        }

        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _data.TryGetValue(key, out value);
        }

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

        public string Serialize()
        {
            return JsonConvert.SerializeObject(_data);
        }

        public static Message Deserialize(string serialized)
        {
            return new Message(JsonConvert.DeserializeObject<Dictionary<string, object>>(serialized));
        }
    }
}

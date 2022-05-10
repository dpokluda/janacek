// -------------------------------------------------------------------------
// <copyright file="Message.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <project>Delivery Catalog Service</project>
// <summary>
//  
//    Object representing the Message.cs.
//  
// </summary>
// -------------------------------------------------------------------------

using System.Collections.Generic;

namespace JanacekClient
{
    public class Message : Dictionary<string, object>
    {
        public Message()
        { }

        public Message(IDictionary<string, object> dictionary)
            : base(dictionary)
        { }
    }
}

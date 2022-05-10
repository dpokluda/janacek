using System;

namespace JanacekClient
{
    public class Janacek
    {
        private PatternMatching _patterns;

        public Janacek()
        {
            _patterns = new PatternMatching();
        }

        public Janacek Add(string pattern, Func<Message, Message> action)
        {
            _patterns.Add(pattern, action);
            return this;
        }

        public Message Act(Message msg)
        {
            var action = (Func<Message, Message>) _patterns.Match(msg);
            var result = action(msg);
            return result;
        }

        public void Listen() //type, message
        { }

        public void Client() //type, message
        { }
    }
}

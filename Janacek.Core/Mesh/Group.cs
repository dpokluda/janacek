using System;
using System.Collections.Generic;

namespace Janacek.Mesh
{
    public class Group
    {
        private readonly Random _generator = new Random((int)DateTime.UtcNow.Ticks);
        private List<Member> _members;
        private int _index;

        public Group()
        {
            _members = new List<Member>();
            _index = -1;
        }

        public IReadOnlyList<Member> Members
        {
            get
            {
                // we should probably sort it in some way
                return _members;
            }
        }

        public int Index
        {

            get
            {
                return _index;
            }
        }

        public void Join(Member member)
        {
            var newMember = new Member(member.Address, member.Name)
            {
                State = MemberState.Active,
                Incarnation = 1,
            };

            _members.Add(newMember);
        }

        public void Shuffle()
        {
            for (int i = _members.Count - 1; i >= 0; i--)
            {
                int index = _generator.Next(i + 1);
                Member swapValue = _members[index];
                _members[index] = _members[i];
                _members[i] = swapValue;
            }
        }
    }
}

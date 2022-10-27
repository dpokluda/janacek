namespace Janacek.Mesh
{
    public class Member
    {
        public Member(string address, string name)
        {
            Address = address;
            Name = name;
        }

        public Member(string address)
        {
            Address = address;
        }

        public string Name { get; set; }

        public string Address { get; set; }

        public MemberState State { get; set; }

        public int Incarnation { get; set; }
    }
}

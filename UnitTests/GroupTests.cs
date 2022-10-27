using System;
using System.Diagnostics;
using System.Linq;
using Janacek.Mesh;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GroupTests
    {
        private static readonly Random Generator = new Random((int)DateTime.UtcNow.Ticks);

        [TestMethod]
        public void Empty()
        {
            var node = new Group();
            Assert.IsNotNull(node);

            Assert.IsNotNull(node.Members);
            Assert.AreEqual(0, node.Members.Count);
            
            // initial index = 0
            Assert.AreEqual(-1, node.Index);
        }

        [TestMethod]
        public void Join()
        {
            // empty group
            var node = new Group();
            Assert.AreEqual(0, node.Members.Count);

            // join new member
            var member = new Member(GetServiceAddress())
            {
                Name = "A",
            };
            node.Join(member);

            // test
            Assert.AreEqual(1, node.Members.Count);
            var nodeMember = node.Members[0];
            Assert.AreEqual("A", nodeMember.Name);
            Assert.AreEqual(member.Address, nodeMember.Address);
            Assert.AreEqual(MemberState.Active, nodeMember.State);
            Assert.AreEqual(1, nodeMember.Incarnation);
            
            // initial index = 0
            Assert.AreEqual(-1, node.Index);
        }

        [TestMethod]
        public void Shuffle()
        {
            // empty group
            var node = new Group();
            Assert.AreEqual(0, node.Members.Count);

            // add members
            for (int i = 0; i < 10; i++)
            {
                node.Join(new Member(GetServiceAddress(), (i+1).ToString()));
            }

            // test
            Assert.AreEqual(10, node.Members.Count);
            var names = new string[10];
            for (int i = 0; i < 10; i++)
            {
                names[i] = node.Members[i].Name;

                // check also that Members are not shuffled
                Assert.AreEqual((i+1).ToString(), node.Members[i].Name);
            }

            // first shuffle
            node.Shuffle();

            bool isSame = true;
            for (int i = 0; i < 10; i++)
            {
                if (names[i] != node.Members[i].Name)
                {
                    isSame = false;
                }

                names[i] = node.Members[i].Name;

            }
            Assert.IsFalse(isSame);

            // second shuffle
            node.Shuffle();

            isSame = true;
            for (int i = 0; i < 10; i++)
            {
                if (names[i] != node.Members[i].Name)
                {
                    isSame = false;
                    break;
                }
            }
            Assert.IsFalse(isSame);
        }

        [TestMethod]
        public void UpdateMember()
        {
            // empty group
            var node = new Group();
            Assert.AreEqual(0, node.Members.Count);

            // join new member
            var member = new Member(GetServiceAddress())
            {
                Name = "A",
            };
            node.Join(member);

            // test
            Assert.AreEqual(1, node.Members.Count);
            var nodeMember = node.Members[0];
            Assert.AreEqual(MemberState.Active, nodeMember.State);

            node.Members[0].State = MemberState.Suspected;
            Assert.AreEqual(MemberState.Suspected, nodeMember.State);
        }

        private static string GetServiceAddress()
        {
            var port = Generator.Next(62000, 63000);
            return $"http://localhost:{port}/";
        }
    }
}

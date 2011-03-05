using System;
using System.Linq;
using System.Reflection;
using Ncqrs.Domain;
using NUnit.Framework;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot.Tests
{
    [TestFixture]
    public class DynamicSnapshotTests
    {
        [SetUp]
        public void TestFixtureSetUp()
        {
            var target = Assembly.LoadFrom("Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot.Tests.dll");
            DynamicSnapshot.CreateAssemblyFrom(target);
        }

        [Test]
        public void CanCreateDynamicSnapshotForInheritanceTree()
        {
            Type snapshotType = DynamicSnapshot.FindSnapshotType(new SnapshotableObject());

            Assert.IsNotNull(snapshotType);
            Assert.AreEqual(snapshotType.Name, "SnapshotableObject_Snapshot");
            var f = snapshotType.GetFields();
            Assert.AreEqual(4, snapshotType.GetFields().Count());
        }

        [Test]
        public void CanCreateDynamicSnapshtotType()
        {
            Type snapshotType = DynamicSnapshot.FindSnapshotType(new ParentSnapshotableObject());

            Assert.IsNotNull(snapshotType);
            Assert.AreEqual(snapshotType.Name, "ParentSnapshotableObject_Snapshot");
            Assert.AreEqual(2, snapshotType.GetFields().Count());
        }
    }

    [DynamicSnapshot]
    public class ParentSnapshotableObject : AggregateRoot
    {
        protected string _name = "ParentSnapshotableObject";
        private readonly bool _isActive = true;

        [ExcludeFromSnapshot]
        private int _excludedInt = -123;

        [ExcludeFromSnapshot]
        private DateTime _excludedDate = DateTime.Now;

    }

    [DynamicSnapshot]
    public class SnapshotableObject : ParentSnapshotableObject
    {
        protected string _name = "SnapshotableObject";
        private int _penchoInt = 1245;

        [ExcludeFromSnapshot]
        private bool _excludedBool;
    }

    public class ParentNonSnapshotableObect : AggregateRoot
    {
        private string _name = "ParentNonSnapshotableObect";
        DateTime _date = DateTime.Now;
    }

    public class NonSnapshotableObject : ParentNonSnapshotableObect
    {
        private string _name = "NonSnapshotableObject";
        DateTime _date = DateTime.Now;
    }
}

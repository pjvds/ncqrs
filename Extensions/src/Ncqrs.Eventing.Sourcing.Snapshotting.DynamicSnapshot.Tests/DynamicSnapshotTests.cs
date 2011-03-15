using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Ncqrs.Domain;
using NUnit.Framework;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot.Tests
{
    [TestFixture]
    public class SnapshotableProxyTests
    {
        private const string ChangedTitle = "ChangedTitle";
        private const string OriginalTitle = "OriginalTitle";
        private IWindsorContainer _container;

        [Test]
        public void CanBuildDynamicSnapshotAssembly()
        {
            var assembly = _container.Resolve<IDynamicSnapshotAssembly>();
            var snapshotTypesCount = assembly.ActualAssembly.GetTypes().Length;
            Assert.AreEqual(3, snapshotTypesCount);
        }

        [Test]
        public void CanCreateSnapshotableAggregate()
        {
            var proxy = _container.Resolve<Foo>();
            Assert.IsNotNull(proxy);
        }

        [Test]
        public void CanCreateSnapshot()
        {
            dynamic proxy = _container.Resolve<Foo>();
            var snapshot = proxy.CreateSnapshot();
            Assert.IsNotNull(snapshot);
        }

        [Test]
        public void CanProperlyRestoreFormDynamicSnapshot()
        {
            //  We assert in each step!!!

            dynamic proxy = _container.Resolve<Foo>();
            proxy.ChangeTitle(OriginalTitle);
            Assert.AreEqual(OriginalTitle, proxy.Tittle);

            var snapshot = proxy.CreateSnapshot();
            Assert.AreEqual(OriginalTitle, proxy.Tittle);

            proxy.ChangeTitle(ChangedTitle);
            Assert.AreEqual(ChangedTitle, proxy.Tittle);

            proxy.RestoreFromSnapshot(snapshot);
            Assert.AreEqual(OriginalTitle, proxy.Tittle);
        }

        [SetUp]
        public void SetUp()
        {
            var target = Assembly.LoadFrom("Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot.Tests.dll");
            _container = new WindsorContainer();
            _container.AddFacility("Ncqrs.DynamicSnapshot", new DynamicSnapshotFacility(target));
            _container.Register(Component.For<Foo>().AsSnapshotable());
        }

    }

    [DynamicSnapshot]
    public class Foo : AggregateRoot
    {
        private string _title;
        public string Tittle { get { return _title; } }

        public Foo() { }
        private Foo(string title) { _title = title; }
        public static Foo CreateNew(string title) { return new Foo(title); }

        public void ChangeTitle(string newTitle) { _title = newTitle; }
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

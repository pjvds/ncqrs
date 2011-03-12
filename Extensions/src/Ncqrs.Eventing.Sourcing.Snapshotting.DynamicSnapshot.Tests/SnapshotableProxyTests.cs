using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Proxy;
using Castle.MicroKernel.Registration;
using Ncqrs.Domain;
using NUnit.Framework;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot.Tests
{
    [TestFixture]
    public class DynamicSnapshotTests
    {
        private const string OriginalTitle = "OriginalTitle";
        private const string ChangedTitle = "ChangedTitle";

        [Test]
        public void CanRestoreFormDynamicSnapshot()
        {
            //  We assert in each step!!!

            var target = Assembly.LoadFrom("Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot.Tests.dll");
            var snapshotsAsm = DynamicSnapshot.CreateAssemblyFrom(target);

            Castle.Windsor.IWindsorContainer container = new Castle.Windsor.WindsorContainer();
            container.Register(Component.For<Foo>().AsSnapshotable());

            dynamic proxy = container.Resolve<Foo>();
            proxy.ChangeTitle(OriginalTitle);
            Assert.AreEqual(OriginalTitle, proxy.Tittle);

            var snapshot = proxy.CreateSnapshot();
            Assert.AreEqual(OriginalTitle, proxy.Tittle);

            proxy.ChangeTitle(ChangedTitle);
            Assert.AreEqual(ChangedTitle, proxy.Tittle);

            proxy.RestoreFromSnapshot(snapshot);
            Assert.AreEqual(OriginalTitle, proxy.Tittle);
        }

        [Test]
        public void BuildDynamicSnapshotAssembly()
        {
            var target = Assembly.LoadFrom("Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot.Tests.dll");
            var snapshotsAsm = DynamicSnapshot.CreateAssemblyFrom(target);
            var snapshotTypesCount = snapshotsAsm.GetTypes().Length;
            Assert.AreEqual(3, snapshotTypesCount);
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

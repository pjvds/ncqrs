using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs.Domain;
using NUnit.Framework;
using System.Reflection;
using Castle.DynamicProxy;
using System.Reflection.Emit;
using System.Threading;

namespace Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot.Tests
{

    [TestFixture]
    public class AutoSnapshotTests
    {
        private const string OriginalTitle = "OroginalTitle";
        private const string ChangedTitle = "ChangedTitle";

        [Test]
        public void CanRestoreFormDynamicSnapshot()
        {
            //  We assert in each step!!!

            var target = Assembly.LoadFrom("Ncqrs.Eventing.Sourcing.Snapshotting.DynamicSnapshot.Tests.dll");
            var snapshotsAsm = DynamicSnapshot.CreateAssemblyFrom(target);

            dynamic proxy = SnapshotableProxy.Create(new Foo());
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
}

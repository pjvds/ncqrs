using System;
using System.Threading;
using System.Transactions;
using EventStore;
using EventStore.Serialization;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Eventing.Storage.JOliver;
using Ncqrs.Eventing.Storage.JOliver.SqlPersistence;
using Ncqrs.Tests.Integration.Domain;
using NUnit.Framework;

namespace Ncqrs.Tests.Integration
{
    [TestFixture]
    public class TransactionalSqlPersistenceEngineTests
    {
        [Test]
        public void Aggregates_should_be_persisted_in_one_transaction()
        {
            var factory = new AbsoluteOrderingSqlPersistenceFactory("SqlJoesEventStore", new BinarySerializer(), true);
            var streamPersister = factory.Build();
            streamPersister.Initialize();
            var store = new OptimisticEventStore(streamPersister, new NullDispatcher());
            var uowFactory = new JoesUnitOfWorkFactory(store);
            NcqrsEnvironment.SetDefault<IUnitOfWorkFactory>(uowFactory);

            var note1Id = Guid.NewGuid();
            var note2Id = Guid.NewGuid();

            //Create
            using (var uow = uowFactory.CreateUnitOfWork(Guid.NewGuid()))
            {
                var note1 = new Note(note1Id, "Text 1");
                var note2 = new Note(note2Id, "Text 2");
                uow.Accept();
            }

            try
            {
                using (var tx = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    using (var uow = uowFactory.CreateUnitOfWork(Guid.NewGuid()))
                    {
                        var note1 = (Note) uow.GetById(typeof (Note), note1Id, null);
                        note1.ChangeText("Text 1 Modified");
                        var note2 = (Note) uow.GetById(typeof (Note), note2Id, null);
                        note2.ChangeText("Text 2 Modified");

                        var t = new Thread(() =>
                                               {
                                                   using (var nestedUow = uowFactory.CreateUnitOfWork(Guid.NewGuid()))
                                                   {
                                                       note2 = (Note) nestedUow.GetById(typeof (Note), note2Id, null);
                                                       note2.ChangeText("Text 2 Modified from mested UoW");
                                                       nestedUow.Accept();
                                                   }
                                               });
                        t.Start();
                        t.Join();

                        uow.Accept(); //Throws
                    }
                    tx.Complete();
                }
            }
            catch (Exception)
            {
                //Swallow
            }

            //Nothing should be modified
            using (var uow = uowFactory.CreateUnitOfWork(Guid.NewGuid()))
            {
                var note1 = (Note) uow.GetById(typeof (Note), note1Id, null);
                note1.Text.Should().Be("Text 1");
                var note2 = (Note) uow.GetById(typeof (Note), note2Id, null);
                note2.Text.Should().Be("Text 2 Modified from mested UoW");
            }
        }
    }
}
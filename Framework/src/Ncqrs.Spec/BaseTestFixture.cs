using System;

namespace Ncqrs.Spec
{
    [Specification]
    public abstract class BaseTestFixture
    {
        protected Exception CaughtException;
        protected virtual void Given() { }
        protected abstract void When();
        protected virtual void Finally() { }

        [Given]
        public void Setup()
        {
            Given();

            try
            {
                When();
            }
            catch (Exception exception)
            {
                CaughtException = exception;
            }
            finally
            {
                Finally();
            }
        }
    }

    [Specification]
    public abstract class BaseTestFixture<TSubjectUnderTest>
    {
        protected TSubjectUnderTest SubjectUnderTest;
        protected Exception CaughtException;
        protected virtual void SetupDependencies() { }
        protected virtual void Given() { }
        protected abstract void When();
        protected virtual void Finally() { }

        [Given]
        public void Setup()
        {
            SetupDependencies();
            
            Given();

            try
            {
                When();
            }
            catch (Exception exception)
            {
                CaughtException = exception;
            }
            finally
            {
                Finally();
            }
        }
    }
}
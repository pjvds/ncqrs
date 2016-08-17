using System;

namespace Ncqrs.Spec
{
    public abstract class BaseTestFixture
    {
        protected Exception CaughtException;
        protected virtual void Given() { }
        protected abstract void When();
        protected virtual void Finally() { }
        protected virtual void SetupDependencies() { }

        public BaseTestFixture() 
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

    public abstract class BaseTestFixture<TSubjectUnderTest>
    {
        protected TSubjectUnderTest SubjectUnderTest;
        protected Exception CaughtException;
        protected virtual void SetupDependencies() { }
        protected virtual void Given() { }
        protected abstract void When();
        protected virtual void Finally() { }

        public BaseTestFixture()
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
using System;
using System.Diagnostics;

namespace Ncqrs
{
    internal class TraceLogger : ILog
    {
        private TraceSwitch _switch = new TraceSwitch("ncqrs", "Ncqrs Framework trace switch");

        public void Debug(object message)
        {
            if(_switch.TraceVerbose)
                Trace.TraceInformation(message.ToString());
        }

        public void Debug(object message, Exception exception)
        {
            if(_switch.TraceVerbose)
                Trace.TraceInformation(message.ToString());
        }

        public void DebugFormat(string format, params object[] args)
        {
            if(_switch.TraceVerbose)
                Trace.TraceInformation(format, args);
        }

        public void Info(object message)
        {
            if(_switch.TraceInfo)
                Trace.TraceInformation(message.ToString());
        }

        public void Info(object message, Exception exception)
        {
            if(_switch.TraceInfo)
                Trace.TraceInformation(message.ToString());  
        }

        public void InfoFormat(string format, params object[] args)
        {
            if(_switch.TraceInfo)
                Trace.TraceInformation(format, args);
        }

        public void Warn(object message)
        {
            if(_switch.TraceWarning)
                Trace.TraceWarning(message.ToString());   
        }

        public void Warn(object message, Exception exception)
        {
            if(_switch.TraceWarning)
                Trace.TraceWarning(message.ToString());  
        }

        public void WarnFormat(string format, params object[] args)
        {
            if(_switch.TraceWarning)
                Trace.TraceWarning(format, args); 
        }

        public void Error(object message)
        {
            if(_switch.TraceError)
                Trace.TraceError(message.ToString());
        }

        public void Error(object message, Exception exception)
        {
            if(_switch.TraceError)
                Trace.TraceError(message.ToString());
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if(_switch.TraceError)
                Trace.TraceError(format, args);
        }

        public void Fatal(object message)
        {
            Error(message);
        }

        public void Fatal(object message, Exception exception)
        {
            Error(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            ErrorFormat(format, args);
        }

        public bool IsDebugEnabled
        {
            get { return _switch.TraceVerbose; }
        }

        public bool IsInfoEnabled
        {
            get { return _switch.TraceInfo; }
        }

        public bool IsWarnEnabled
        {
            get { return _switch.TraceWarning; }
        }

        public bool IsErrorEnabled
        {
            get { return _switch.TraceError; }
        }

        public bool IsFatalEnabled
        {
            get { return IsErrorEnabled; }
        }
    }
}

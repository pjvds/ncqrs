using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Ncqrs
{
    public class LogManager
    {
        private static bool _isLog4NetAvailable = false;
        private static Dictionary<Type, ILog> _loggerCache = new Dictionary<Type, ILog>();

        static LogManager()
        {
            try
            {
                Assembly.Load("log4net");
                _isLog4NetAvailable = true;
            }
            catch (FileNotFoundException)
            {
                _isLog4NetAvailable = false;
            }
        }

        public static ILog GetLogger(Type type)
        {
            ILog logger = null;

            if(!_loggerCache.TryGetValue(type, out logger))
            {
                logger = CreateLoggerForType(type);
                _loggerCache.Add(type, logger);
            }

            return logger;
        }

        private static ILog CreateLoggerForType(Type type)
        {
            if (_isLog4NetAvailable)
            {
                return (ILog)Activator.CreateInstance(typeof(Log4NetLogger), new object[] { type });
            }
            else
            {
                return new TraceLogger();
            }
        }
    }
}
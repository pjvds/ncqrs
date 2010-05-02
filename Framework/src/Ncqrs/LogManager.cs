using System;
using System.IO;
using System.Reflection;

namespace Ncqrs
{
    public class LogManager
    {
        private static bool? _isLog4NetAvailable = null;

        public static ILog GetLogger(Type type)
        {
            if(IsLog4NetAvailable())
            {
                return new Log4NetLogger(log4net.LogManager.GetLogger(type));
            }
            else
            {
                return new TraceLogger();
            }
        }

        private static bool IsLog4NetAvailable()
        {
            if (!_isLog4NetAvailable.HasValue)
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

            return _isLog4NetAvailable.Value;
        }
    }
}
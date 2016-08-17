using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Ncqrs
{
    /// <summary>
    /// A manager class to use to get a logger for a certain type.
    /// </summary>
    public static class LogManager
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();

        public static ILogger GetLogger<T>() => LoggerFactory.CreateLogger<T>();

        public static ILogger GetLogger(Type type)
        {
            return LoggerFactory.CreateLogger(type);
        }
    }
}
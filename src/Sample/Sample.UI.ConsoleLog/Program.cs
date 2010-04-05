using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using log4net.Config;

namespace Sample.UI.ConsoleLog
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            var endPoint = new IPEndPoint(IPAddress.Any, 0);

            using (var client = new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 43284)))
            {
                while (true)
                {
                    byte[] buffer = client.Receive(ref endPoint);
                    var logLine = Encoding.Default.GetString(buffer);

                    if (logLine.Contains("{INFO}"))
                    {
                        Log.Info(logLine);
                    }
                    else if (logLine.Contains("{DEBUG}"))
                    {
                        Log.Debug(logLine);
                    }
                    else if (logLine.Contains("{ERROR}"))
                    {
                        Log.Error(logLine);
                    }
                    else if (logLine.Contains("{WARN}"))
                    {
                        Log.Warn(logLine);
                    }
                    else
                    {
                        Log.Error("NO HANDLER FOUND FOR LOGLINE: " + logLine);
                    }
                }
            }
        }
    }
}

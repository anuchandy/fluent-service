// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace AnuChandy.Fluent.Service.BackEnd
{
    public class TracingInterceptor : IServiceClientTracingInterceptor
    {
        private ILog _logger;

        public TracingInterceptor()
        {
            PatternLayout layout = new PatternLayout();

            ConsoleAppender appender = new ConsoleAppender();
            appender.Layout = layout;
            appender.ActivateOptions();

            ILoggerRepository repository = LoggerManager.CreateRepository("MyRepository");
            BasicConfigurator.Configure(repository, appender);

            _logger = LogManager.GetLogger("MyRepository", "MyLogger");
        }

        public TracingInterceptor(string filePath)
        {
            PatternLayout layout = new PatternLayout("% date{ MMM / dd / yyyy HH:mm: ss,fff} [%thread] %-5level %logger %ndc – %message%newline");

            LevelMatchFilter filter = new LevelMatchFilter();
            filter.LevelToMatch = Level.All;
            filter.ActivateOptions();

            RollingFileAppender appender = new RollingFileAppender();
            appender.File = filePath;
            appender.ImmediateFlush = true;
            appender.AppendToFile = true;
            appender.RollingStyle = RollingFileAppender.RollingMode.Date;
            appender.DatePattern = "-yyyy - MM - dd";
            appender.LockingModel = new FileAppender.MinimalLock();
            appender.Name = "MyAppender";
            appender.AddFilter(filter);
            appender.Layout = layout;
            appender.ActivateOptions();

            ILoggerRepository repository = LoggerManager.CreateRepository("MyRepository");
            BasicConfigurator.Configure(repository, appender);

            _logger = LogManager.GetLogger("MyRepository", "MyLogger");
        }

        public void Information(string message)
        {
            _logger.Info(message);
        }

        public void Configuration(string source, string name, string value)
        {
            _logger.DebugFormat(CultureInfo.InvariantCulture,
                "Configuration: source={0}, name={1}, value={2}", source, name, value);
        }

        public void EnterMethod(string invocationId, object instance, string method,
            IDictionary<string, object> parameters)
        {
            _logger.DebugFormat(CultureInfo.InvariantCulture,
                "invocationId: {0}\r\ninstance: {1}\r\nmethod: {2}\r\nparameters: {3}",
                invocationId, instance, method, parameters.AsFormattedString());
        }

        public void SendRequest(string invocationId, HttpRequestMessage request)
        {
            string requestAsString = (request == null ? string.Empty : request.AsFormattedString());
            _logger.DebugFormat(CultureInfo.InvariantCulture,
                "invocationId: {0}\r\nrequest: {1}", invocationId, requestAsString);
        }

        public void ReceiveResponse(string invocationId, HttpResponseMessage response)
        {
            string requestAsString = (response == null ? string.Empty : response.AsFormattedString());
            _logger.DebugFormat(CultureInfo.InvariantCulture,
                "invocationId: {0}\r\nresponse: {1}", invocationId, requestAsString);
        }

        public void TraceError(string invocationId, Exception exception)
        {
            _logger.Error("invocationId: " + invocationId, exception);
        }

        public void ExitMethod(string invocationId, object returnValue)
        {
            string returnValueAsString = (returnValue == null ? string.Empty : returnValue.ToString());
            _logger.DebugFormat(CultureInfo.InvariantCulture,
                "Exit with invocation id {0}, the return value is {1}",
                invocationId,
                returnValueAsString);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using log4net;

using NLog;
using NLog.Config;

namespace Time.Invoicing.Logging
{
    public class NLogger : ILogger
    {
        private readonly Logger _logger;

        public NLogger()
        {
            // test for null to bypass mocking errors since there is no real HttpContext
            if (HttpContext.Current != null)
            {
                //ThreadContext.Properties["ip_address"] = HttpContext.Current.Request.UserHostAddress;
                //ThreadContext.Properties["user_agent"] = HttpContext.Current.Request.UserAgent;
            }
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("web_variables", typeof(Logging.WebVariablesRenderer));
            _logger = LogManager.GetCurrentClassLogger();
        }

        public NLogger(string currentClassName)
        {
            _logger = LogManager.GetLogger(currentClassName);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(Exception x)
        {
            Error(BuildExceptionMessage(x));
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(Exception x)
        {
            Fatal(BuildExceptionMessage(x));
        }

        private string BuildExceptionMessage(Exception x)
        {
            Exception logException = x;
            if (x.InnerException != null)
            {
                logException = x.InnerException;
            }

            string strErrorMsg = Environment.NewLine + "Error in Path :" + System.Web.HttpContext.Current.Request.Path;

            // get the QueryString along with the Virtual Path
            strErrorMsg += Environment.NewLine + "Raw Url :" + System.Web.HttpContext.Current.Request.RawUrl;

            // Get the error message
            strErrorMsg += Environment.NewLine + "Message :" + logException.Message;

            // Source of the message
            strErrorMsg += Environment.NewLine + "Source :" + logException.Source;

            // Stack Trace of the error
            strErrorMsg += Environment.NewLine + "Stack Trace :" + logException.StackTrace;

            // Method where the error occurred
            strErrorMsg += Environment.NewLine + "TargetSite :" + logException.TargetSite;

            return strErrorMsg;
        }
    }
}
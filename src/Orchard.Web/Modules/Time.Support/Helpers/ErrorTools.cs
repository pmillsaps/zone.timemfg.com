using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Time.Support.Helpers
{
    public class ErrorTools
    {
        public static void SendEmail(Uri URL, Exception ex)
        {
            SendEmail(URL, ex, null);
        }

        public static void SendEmail(Uri URL, Exception ex, string username)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add("itadmin@timemfg.com");
            mail.From = new MailAddress("intranet@timemfg.com");
            mail.Subject = String.Format("Oops, {0} broke something!", ((username) ?? "someone"));
            StringBuilder Body = new StringBuilder();
            Body.Append(@"An error has occurred <br />");
            Body.Append(String.Format("URL: {0}<br />\n", URL));
            Body.Append("<br />\n");
            Body.Append(String.Format("Error: {0}<br />\n", ex.Message));
            Body.Append(String.Format("Inner Error: {0}<br />\n", (ex.InnerException != null) ? 
                ex.InnerException.Message : "No Inner Error Available..k?..thks!"));
            Body.Append(String.Format("Raw Error Data: {0}", ex.ToString()));
            mail.Body = Body.ToString();
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            var h = ConfigurationManager.AppSettings["emailServer"];
            smtp.Host = h;
            smtp.Send(mail);
        }
    }
}
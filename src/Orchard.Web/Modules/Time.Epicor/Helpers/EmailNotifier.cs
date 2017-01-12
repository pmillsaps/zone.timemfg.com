using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Time.Data.Models.MessageQueue;

namespace Time.Support.Helpers
{
    public class EmailNotifier
    {
        public static string FromAddress { get; set; }
        public static string FromName { get; set; }
        private const string MailServerAddress = "mail.timemfg.com";

        public EmailNotifier()
        {
            FromAddress = "DealerConnectManager@timemfg.com";
            FromName = "DealerConnectManager";
        }

        public static void SendMail(string p_Subject, string p_Body, List<string> p_ToAddress, bool p_Priority, bool isBodyHtml = false, string attachmentFile = "")
        {
            // New section to use MSMQ queue to send the email message
            var message = new EmailMessage();

            message.From = FromAddress;
            message.To = string.Join(",", p_ToAddress);
            message.BCC = "paulm@timemfg.com";
            message.Message = p_Body;
            message.HTML = true;
            message.Subject = p_Subject;
            var success = MSMQ.SendQueueMessage(message, MessageType.EmailMessage.Value);

            //var svr = new SmtpClient();
            //svr.DeliveryMethod = SmtpDeliveryMethod.Network;
            //svr.Host = MailServerAddress;
            //svr.Credentials = new NetworkCredential("Test2", "Versalift1", "Time");
            //svr.UseDefaultCredentials = false;

            //var msg = new MailMessage();
            //if (!String.IsNullOrEmpty(FromName))
            //    msg.From = new MailAddress(FromAddress, FromName);
            //else
            //    msg.From = new MailAddress(FromAddress);

            //List<string> ccAddress = new List<string>();

            //foreach (var item in p_ToAddress)
            //{
            //    msg.To.Add(new MailAddress(item));
            //}

            //var ccList = "paulm@timemfg.com";
            //msg.Bcc.Add(ccList);

            //msg.Body = p_Body;
            //msg.Subject = p_Subject;
            //msg.Priority = (p_Priority) ? MailPriority.High : MailPriority.Normal;
            //msg.IsBodyHtml = isBodyHtml;

            //if (!string.IsNullOrEmpty(attachmentFile))
            //{
            //    msg.Attachments.Add(new Attachment(attachmentFile));
            //}

            //try
            //{
            //    svr.Send(msg);
            //}
            //catch (Exception ex)
            //{
            //    Console.Write(String.Format("Could not send mail. ({0})", ex.Message));
            //}
        }

        public static void SendMailDirect(string p_Subject, string p_Body, List<string> p_ToAddress, bool p_Priority, bool isBodyHtml = false, string attachmentFile = "")
        {
            var svr = new SmtpClient();
            svr.DeliveryMethod = SmtpDeliveryMethod.Network;
            svr.Host = MailServerAddress;
            svr.Credentials = new NetworkCredential("NoReply", "M!n!D0nuts14", "Time");
            svr.UseDefaultCredentials = false;

            var msg = new MailMessage();
            if (!String.IsNullOrEmpty(FromName))
                msg.From = new MailAddress(FromAddress, FromName);
            else
                msg.From = new MailAddress(FromAddress);

            List<string> ccAddress = new List<string>();

            foreach (var item in p_ToAddress)
            {
                msg.To.Add(new MailAddress(item));
            }

            // Debugging code to trace issues
            //var ccList = "paulm@timemfg.com";
            //msg.Bcc.Add(ccList);

            msg.Body = p_Body;
            msg.Subject = p_Subject;
            msg.Priority = (p_Priority) ? MailPriority.High : MailPriority.Normal;
            msg.IsBodyHtml = isBodyHtml;

            if (!string.IsNullOrEmpty(attachmentFile))
            {
                msg.Attachments.Add(new Attachment(attachmentFile));
            }

            try
            {
                svr.Send(msg);
            }
            catch (Exception ex)
            {
                Console.Write(String.Format("Could not send mail. ({0})", ex.Message));
            }
        }

        public static void SendMail(string p_Subject, string p_Body, string p_ToAddress, bool p_Priority, bool isBodyHtml = false)
        {
            List<string> toAddress = new List<string>();
            toAddress.Add(p_ToAddress);
            SendMail(p_Subject, p_Body, toAddress, p_Priority, isBodyHtml);
        }
    }
}
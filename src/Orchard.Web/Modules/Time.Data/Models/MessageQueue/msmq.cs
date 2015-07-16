using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using msmq = System.Messaging;

namespace Time.Data.Models.MessageQueue
{
    public class MSMQ
    {
        public static string ErrorMessage { get; set; }
        private static string queueAddress = @"Aruba-Connect\time.messagequeue";

        public enum MessageType
        {
            BuildComplexLookups,
            CheckWaterReports,
            CustomManualCheckProblemJobs,
            CustomManualJobQueue,
            EmailMessage,
            EmailProblemJobs,
            MoveFile,
            Refresh_Drawings_DB,
            SendRTIMessage,
            WOL
        }

        public static bool SendQueueMessage(object command, MessageType messageType)
        {
            string messageLabel = messageType.ToString();
            return SendQueueMessage(command, messageLabel);
        }

        public static bool SendQueueMessage(object incomingMessage, string MessageLabel)
        {
            bool success = false;
            ErrorMessage = String.Empty;
            using (var queue = new msmq.MessageQueue(queueAddress))
            {
                //var message = new msmq.Message(incomingMessage);
                //ErrorMessage += incomingMessage;
                // ErrorMessage += queueAddress;
                var message = new msmq.Message();
                var jsonBody = JsonConvert.SerializeObject(incomingMessage);
                message.BodyStream = new MemoryStream(Encoding.Default.GetBytes(jsonBody));
                message.Label = MessageLabel;
                var tx = new msmq.MessageQueueTransaction();
                // ErrorMessage += "Started Transaction";
                tx.Begin();
                try
                {
                    // ErrorMessage += "Fixing to send<br />";
                    queue.Send(message, tx);
                    // ErrorMessage += "Fixing to commit<br />";
                    tx.Commit();
                    success = true;
                    // ErrorMessage += "Finished<br />";
                }
                catch (Exception ex)
                {
                    // ErrorMessage += "Fixing to abort<br />";
                    tx.Abort();
                    var msg = String.Format("Message Not Sent {0}Error: {1}", Environment.NewLine, ex.Message);
                    if (ex.InnerException != null) msg += Environment.NewLine + ex.InnerException.Message;
                    success = false;
                    ErrorMessage += msg;
                    //MessageBox.Show(msg);
                }
            }
            return success;
        }
    }
}
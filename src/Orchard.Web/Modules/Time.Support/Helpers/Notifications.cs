using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Time.Data.EntityModels.TimeMFG;
using Time.Data.Models.MessageQueue;
using Time.Data.Models.TimeMFG;

namespace Time.Support.Helpers
{
    public interface INotifications
    {
        void SetupEmail();

        void SendEmail();
    }

    public abstract class EmailNotifications : INotifications
    {
        private SettingRepository sr;

        public EmailNotifications()
        {
            sr = new SettingRepository();
        }

        #region Properties

        private string _TemplatePath;

        public string TemplatePath
        {
            get
            {
                if (String.IsNullOrEmpty(_TemplatePath))
                {
                    // _TemplatePath = sr.GetSettings("TemplatePath");
                    _TemplatePath = HttpContext.Current.Server.MapPath("~/Modules/Time.Support/Content/EmailTemplates");
                    if (String.IsNullOrEmpty(_TemplatePath))
                        throw new Exception("Template Path was not set");
                }

                return _TemplatePath;
            }
            set
            {
                _TemplatePath = value;
            }
        }

        public List<string> SendTo { get; set; }

        internal string _SentFrom;

        public string SentFrom
        {
            get { return _SentFrom; }
        }

        internal string _Server;

        public string EmailServer
        {
            get { return _Server; }
        }

        internal string _HtmlBody;

        public string HtmlBody
        {
            get { return _HtmlBody; }
        }

        private string _Subject;

        public string Subject
        {
            get
            {
                if (String.IsNullOrEmpty(_TemplatePath))
                    throw new Exception("Email Subject was not set");
                return _Subject;
            }
            set
            {
                _Subject = value;
            }
        }

        internal TicketProject tp;

        public string Additionalbody { get; set; }

        public List<string> BCCTo { get; set; }

        #endregion Properties

        internal EmailNotifications(TicketProject ticket, string additionalBody = "")
        {
            this.tp = ticket;

            //this._Server = ConfigurationManager.AppSettings["emailServer"];
            this._Server = "mail.timemfg.com";
            // this._Server = sr.GetSettings("EmailServer");
            this._SentFrom = "noreply@timemfg.com";
            // this._SentFrom = sr.GetSettings("MailFrom");
            this._HtmlBody = string.Empty;
            this.SendTo = new List<string>();
            this.BCCTo = new List<string>();
            this.Additionalbody = additionalBody;
            SetupEmail();
            BuildEmailBody();
        }

        public void SendEmail()
        {
            var message = new EmailMessage();
            message.To = string.Join(",", SendTo);
            message.BCC = string.Join(",", BCCTo);
            message.Message = _HtmlBody;
            message.HTML = true;
            message.Subject = _Subject;
            var success = MSMQ.SendQueueMessage(message, MSMQ.MessageType.EmailMessage);

            //var mail = new MailMessage();
            //if (SendTo.Count == 0 && BCCTo.Count == 0)
            //    throw new Exception("Email could not be sent. There is no one to send to");
            //foreach (var e in SendTo)
            //    mail.To.Add(new MailAddress(e));
            //foreach (var e in BCCTo)
            //    mail.Bcc.Add(new MailAddress(e));
            //mail.From = new MailAddress(_SentFrom, "My Time Intranet");
            //mail.Body = _HtmlBody;
            //MailAddress replyTo = new MailAddress("NoReplyHere@timemfg.com");
            //mail.ReplyToList.Add(replyTo);
            //mail.IsBodyHtml = true;
            //mail.Subject = _Subject;
            //mail.Sender = new MailAddress(_SentFrom, "My Time Intranet");
            //var smtp = new SmtpClient();
            //smtp.Host = _Server;

            //smtp.SendCompleted += new SendCompletedEventHandler(mail_SendCompleted);
            //string userState = String.Format("Sending Message: To: '{0}'  Subject: '{1}'  ", mail.To, mail.Subject);

            //smtp.SendAsync(mail, userState);
        }

        private void mail_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // TODO: Add logging framework to track issues
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
        }

        public string GetEmailforNTUser(string ntUser)
        {
            if (!String.IsNullOrEmpty(ntUser))
            {
                // var ih = new Internals();
                try
                {
                    var ctx = new PrincipalContext(ContextType.Domain, "timemfg.prv");
                    var ctx2 = new PrincipalContext(ContextType.Domain, "versaliftsouthwest.prv");
                    var user = UserPrincipal.FindByIdentity(ctx, ntUser);
                    if (user == null)
                        user = UserPrincipal.FindByIdentity(ctx2, ntUser);
                    if (user == null)
                        throw new Exception(String.Format("Could not find Email in either domain for {0}", ntUser));
                    //var user = ih.SearchUser(ntUser).First();
                    //return user.EmailAddress;
                    if (user != null && user.EmailAddress == null) throw new Exception(String.Format("Error With AD Account: ({0}) AD Account found, but there was no email attached to the account.", ntUser));
                    // This will return null for an empty email addresss within AD, so leave it as
                    // is and handle the error within the return procedure.
                    if (user != null) return user.EmailAddress;
                }
                catch (Exception err)
                {
                    throw new Exception(String.Format("Error Sending email, Did not find user ({1}) in AD: {0}", err.Message, ntUser), err.InnerException);
                }
            }
            throw new Exception(String.Format("Could not find Email for an Empty Email user"));
        }

        public void AddEmailSendToBcc(string e)
        {
            //var sendTo = new List<string>(ConfigurationManager.AppSettings["emailSendTo"].Split(','));
            var sendTo = new List<string>(sr.GetSettings("EmailBCC").Split(','));
            // Section for CC'ing ITAdmin group
            foreach (var email in sendTo)
            {
                if (e != email)
                {
                    BCCTo.Add(email);
                }
            }
        }

        public abstract void SetupEmail();

        private void BuildEmailBody()
        {
            var header = HttpContext.Current.Server.MapPath("~/Modules/Time.Support/Content/EmailTemplates/NotificationHeader.htm");
            // HttpContext.Current.Server.MapPath(Path.Combine(TemplatePath, "NotificationHeader.htm"));
            var footer = HttpContext.Current.Server.MapPath("~/Modules/Time.Support/Content/EmailTemplates/NotificationFooter.htm");
            // HttpContext.Current.Server.MapPath(Path.Combine(TemplatePath, "NotificationFooter.htm"));
            string h, f;

            using (var path = new StreamReader(header))
            {
                var blah = path.ReadToEnd();
                h = blah;
            }

            using (var path = new StreamReader(footer))
            {
                var blah = path.ReadToEnd();
                f = blah;
            }

            using (var path = new StreamReader(TemplatePath))
            {
                //if (!tp.TicketStatusesReference.IsLoaded)
                //    tp.TicketStatusesReference.Load();
                //if (!tp.TicketEmployeesReference.IsLoaded)
                //    tp.TicketEmployeesReference.Load();

                var blah = path.ReadToEnd();
                this._HtmlBody += blah
                    .Replace("[ticketnumber]", tp.TicketID.ToString())
                    .Replace("[ticketurl]", String.Format("http://zone.timemfg.com/Support/Ticket/Info/{0}", tp.TicketID))
                    .Replace("[tickettitle]", tp.Title)
                    .Replace("[ticketdescription]", tp.Description)
                    .Replace("[ticketapprovalcode]", tp.ApprovalCode)
                    .Replace("[additionalbody]", Additionalbody)
                    .Replace("[ticketrequestedby]", tp.RequestedBy)
                    .Replace("[ticketstatus]", tp.TicketStatus.Name)
                    .Replace("[ticketrequesteddate]", (tp.RequestedDate != null) ? ((DateTime)tp.RequestedDate).ToLongDateString() : "N/A")
                    .Replace("[header]", h)
                    .Replace("[footer]", f);

                if (tp.TicketEmployee != null)
                {
                    this._HtmlBody = this._HtmlBody.Replace("[ticketemployee]",
                                          String.Format("{0} {1}", tp.TicketEmployee.FirstName,
                                                        tp.TicketEmployee.LastName));
                }
                //this._HtmlBody += Additionalbody;
            }
        }
    }

    public class AssignmentNotification : EmailNotifications
    {
        public AssignmentNotification(TicketProject ticket)
            : base(ticket)
        {
        }

        public override void SetupEmail()
        {
            //if (!tp.TicketEmployeesReference.IsLoaded)
            //    tp.TicketEmployeesReference.Load();
            TemplatePath = HttpContext.Current.Server.MapPath("~/Modules/Time.Support/Content/EmailTemplates/AssignmentNotification.htm");

            // TemplatePath = HttpContext.Current.Server.MapPath(@"~\Content\EmailTemplates\AssignmentNotification.htm");
            SendTo.Add(GetEmailforNTUser(tp.TicketEmployee.NTLogin));
            Subject = String.Format("[Ticket #{0}] Assigned - {1}", tp.TicketID, tp.Title);
        }
    }

    public class AssignmentNotificationUser : EmailNotifications
    {
        public AssignmentNotificationUser(TicketProject ticket)
            : base(ticket)
        {
        }

        public override void SetupEmail()
        {
            //if (!tp.TicketEmployeesReference.IsLoaded)
            //    tp.TicketEmployeesReference.Load();
            TemplatePath = HttpContext.Current.Server.MapPath("~/Modules/Time.Support/Content/EmailTemplates/AssignmentNotificationUser.htm");

            // TemplatePath = HttpContext.Current.Server.MapPath(@"~\Content\EmailTemplates\AssignmentNotificationUser.htm");
            var e = GetEmailforNTUser(tp.RequestedBy);
            SendTo.Add(e);
            //AddEmailSendToBcc(e);

            Subject = String.Format("[Ticket #{0}] Assigned - {1} to {2}", tp.TicketID, tp.Title, tp.TicketEmployee.FirstName);
        }
    }

    public class NewTicketNotification : EmailNotifications
    {
        public NewTicketNotification(TicketProject ticket)
            : base(ticket)
        {
        }

        public override void SetupEmail()
        {
            TemplatePath = HttpContext.Current.Server.MapPath(@"~/Modules/Time.Support/Content/EmailTemplates/NewTicketNotification.htm");
            var e = GetEmailforNTUser(tp.RequestedBy);
            SendTo.Add(e);
            AddEmailSendToBcc(e);

            Subject = String.Format("[Ticket #{0}] New - {1}", tp.TicketID, tp.Title);
        }
    }

    public class SupervisorNewTicketNotification : EmailNotifications
    {
        public SupervisorNewTicketNotification(TicketProject ticket)
            : base(ticket)
        {
        }

        public override void SetupEmail()
        {
            TemplatePath = HttpContext.Current.Server.MapPath(@"~/Modules/Time.Support/Content/EmailTemplates/SupervisorNewTicketNotification.htm");
            var e = GetEmailforNTUser(tp.TicketDepartment.TicketEmployee.NTLogin);
            SendTo.Add(e);
            AddEmailSendToBcc(e);

            Subject = String.Format("[Ticket #{0}] New - {1}", tp.TicketID, tp.Title);
        }
    }

    public class CompletionPendingNotification : EmailNotifications
    {
        public CompletionPendingNotification(TicketProject ticket)
            : base(ticket)
        {
        }

        public override void SetupEmail()
        {
            TemplatePath = HttpContext.Current.Server.MapPath(@"~/Modules/Time.Support/Content/EmailTemplates/CompletionPendingNotification.htm");
            var e = GetEmailforNTUser(tp.RequestedBy);
            SendTo.Add(e);
            AddEmailSendToBcc(e);

            Subject = String.Format("[Ticket #{0}] Awaiting Completion - {1}", tp.TicketID, tp.Title);
        }
    }

    public class ApprovedNotification : EmailNotifications
    {
        public ApprovedNotification(TicketProject ticket)
            : base(ticket)
        {
        }

        public override void SetupEmail()
        {
            TemplatePath = HttpContext.Current.Server.MapPath(@"~/Modules/Time.Support/Content/EmailTemplates/ApprovedNotification.htm");
            var e = GetEmailforNTUser(tp.RequestedBy);
            AddEmailSendToBcc(e);

            Subject = String.Format("[Ticket #{0}] has been approved or not approved by the supervisor - {1}", tp.TicketID, tp.Title);
        }
    }

    public class UpdateNotification : EmailNotifications
    {
        public UpdateNotification(TicketProject ticket)
            : base(ticket)
        {
            var e = GetEmailforNTUser(tp.RequestedBy);
            SendTo.Add(e);
        }

        public UpdateNotification(TicketProject ticket, string userName)
            : base(ticket)
        {
            var e = GetEmailforNTUser(userName);
            SendTo.Add(e);
        }

        public UpdateNotification(TicketProject ticket, string userName, string statusMessage = "")
            : base(ticket, statusMessage)
        {
            var e = GetEmailforNTUser(userName);
            SendTo.Add(e);
            // Additionalbody = note.Note;
            if (!String.IsNullOrEmpty(statusMessage)) Additionalbody = statusMessage;
        }

        public override void SetupEmail()
        {
            TemplatePath = HttpContext.Current.Server.MapPath(@"~/Modules/Time.Support/Content/EmailTemplates/UpdateNotification.htm");
            Subject = String.Format("[Ticket #{0}] Updated - {1}", tp.TicketID, tp.Title);
        }
    }

    public class TaskAssignmentNotification : EmailNotifications
    {
        public TaskAssignmentNotification(TicketProject ticket)
            : base(ticket)
        {
            var e = GetEmailforNTUser(tp.RequestedBy);
            SendTo.Add(e);
        }

        public TaskAssignmentNotification(TicketProject ticket, string userName)
            : base(ticket)
        {
            var e = GetEmailforNTUser(userName);
            SendTo.Add(e);
        }

        public TaskAssignmentNotification(TicketProject ticket, string userName, string statusMessage = "")
            : base(ticket, statusMessage)
        {
            var e = GetEmailforNTUser(userName);
            SendTo.Add(e);
            // Additionalbody = note.Note;
            if (!String.IsNullOrEmpty(statusMessage)) Additionalbody = statusMessage;
        }

        public override void SetupEmail()
        {
            TemplatePath = HttpContext.Current.Server.MapPath(@"~/Modules/Time.Support/Content/EmailTemplates/TaskAssignmentNotification.htm");
            Subject = String.Format("[Ticket #{0}] {1} A New Task Has Been Assigned To You", tp.TicketID, tp.Title);
        }
    }

    public class TaskCompletedNotification : EmailNotifications
    {
        public TaskCompletedNotification(TicketProject ticket)
            : base(ticket)
        {
            var e = GetEmailforNTUser(tp.RequestedBy);
            SendTo.Add(e);
        }

        public TaskCompletedNotification(TicketProject ticket, string userName)
            : base(ticket)
        {
            var e = GetEmailforNTUser(userName);
            SendTo.Add(e);
        }

        public TaskCompletedNotification(TicketProject ticket, string userName, string statusMessage = "")
            : base(ticket, statusMessage)
        {
            var e = GetEmailforNTUser(userName);
            SendTo.Add(e);
            // Additionalbody = note.Note;
            if (!String.IsNullOrEmpty(statusMessage)) Additionalbody = statusMessage;
        }

        public override void SetupEmail()
        {
            TemplatePath = HttpContext.Current.Server.MapPath(@"~/Modules/Time.Support/Content/EmailTemplates/TaskCompletedNotification.htm");
            Subject = String.Format("[Ticket #{0}] {1} The Task Below has been completed", tp.TicketID, tp.Title);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Support.Helpers
{
    public static class TicketExtensions
    {
        public static void SendNewTicketNotification(this TicketProject ticket)
        {
            try
            {
                var msg = new NewTicketNotification(ticket);
                msg.SendEmail();
            }
            catch (Exception err)
            {
                ErrorTools.SendEmail(HttpContext.Current.Request.Url, err, HttpContext.Current.User.Identity.Name);
                Debug.Print(err.Message);
            }
        }

        public static void SendApprovedNotification(this TicketProject ticket)
        {
            try
            {
                var msg = new ApprovedNotification(ticket);
                msg.SendEmail();
            }
            catch (Exception err)
            {
                ErrorTools.SendEmail(HttpContext.Current.Request.Url, err, HttpContext.Current.User.Identity.Name);
                Debug.Print(err.Message);
            }
        }

        public static void SendSupervisorNewTicketNotification(this TicketProject ticket)
        {
            if (!String.IsNullOrEmpty(ticket.TicketEmployee.NTLogin))
            {
                try
                {
                    var msg = new SupervisorNewTicketNotification(ticket);
                    msg.SendEmail();
                }
                catch (Exception err)
                {
                    ErrorTools.SendEmail(HttpContext.Current.Request.Url, err, HttpContext.Current.User.Identity.Name);
                    Debug.Print(err.Message);
                }
            }
        }

        public static void SendAssignmentNotification(this TicketProject ticket)
        {
            //if (!ticket.TicketEmployeesReference.IsLoaded)
            //    ticket.TicketEmployeesReference.Load();
            if (!String.IsNullOrEmpty(ticket.TicketEmployee.NTLogin))
            {
                try
                {
                    var msg = new AssignmentNotification(ticket);
                    msg.SendEmail();
                    var msg2 = new AssignmentNotificationUser(ticket);
                    msg2.SendEmail();
                }
                catch (Exception err)
                {
                    ErrorTools.SendEmail(System.Web.HttpContext.Current.Request.Url, err,
                                                               System.Web.HttpContext.Current.User.Identity.Name);
                    Debug.Print(err.Message);
                }
            }
        }

        public static void SendUpdateNotification(this TicketProject ticket, string statusMessage = "")
        {
            if (ticket.RequestedBy.Contains("TIME\\") || ticket.RequestedBy.Contains("TIMEMFG\\") || ticket.RequestedBy.Contains("VERSALIFTSOUTHW\\"))
            {
                try
                {
                    var msg = new UpdateNotification(ticket, ticket.RequestedBy, statusMessage);
                    msg.SendEmail();
                }
                catch (Exception err)
                {
                    ErrorTools.SendEmail(System.Web.HttpContext.Current.Request.Url, err,
                                                               System.Web.HttpContext.Current.User.Identity.Name);
                    Debug.Print(err.Message);
                }
            }
        }

        public static void SendUpdateNotificationToAssigned(this TicketProject ticket, string statusMessage = "")
        {
            if (ticket.TicketEmployee != null)
            {
                if (ticket.TicketEmployee.NTLogin.Contains("TIME\\") || ticket.TicketEmployee.NTLogin.Contains("TIMEMFG\\") || ticket.TicketEmployee.NTLogin.Contains("VERSALIFTSOUTHW\\"))
                {
                    try
                    {
                        var msg = new UpdateNotification(ticket, ticket.TicketEmployee.NTLogin, statusMessage, ticket.TicketEmployee.EmailName);
                        msg.SendEmail();
                    }
                    catch (Exception err)
                    {
                        ErrorTools.SendEmail(System.Web.HttpContext.Current.Request.Url, err,
                                                                   System.Web.HttpContext.Current.User.Identity.Name);
                        Debug.Print(err.Message);
                    }
                }
            }
        }

        public static void SendCompletionPendingNotification(this TicketProject ticket)
        {
            if (ticket.RequestedBy.Contains("TIME\\") || ticket.RequestedBy.Contains("TIMEMFG\\") || ticket.RequestedBy.Contains("VERSALIFTSOUTHW\\"))
            {
                try
                {
                    var msg = new CompletionPendingNotification(ticket);
                    msg.SendEmail();
                }
                catch (Exception err)
                {
                    ErrorTools.SendEmail(System.Web.HttpContext.Current.Request.Url, err,
                                                               System.Web.HttpContext.Current.User.Identity.Name);
                    Debug.Print(err.Message);
                }
            }
        }

        #region TicketTask

        public static void SendTaskAssignmentNotification(this TicketProject ticket, TicketTask task)
        {
            if (!String.IsNullOrEmpty(task.TicketEmployee.NTLogin))
            {
                try
                {
                    var msg = new TaskAssignmentNotification(ticket, task.TicketEmployee.NTLogin, String.Format("{0}<br />{1}", task.Task, task.Notes));
                    msg.SendEmail();
                    //var msg2 = new AssignmentNotificationUser(ticket);
                    //msg2.SendEmail();
                }
                catch (Exception err)
                {
                    ErrorTools.SendEmail(System.Web.HttpContext.Current.Request.Url, err,
                                                               System.Web.HttpContext.Current.User.Identity.Name);
                    Debug.Print(err.Message);
                }
            }
        }

        public static void SendTaskCompletedNotification(this TicketProject ticket, TicketTask task)
        {
            if (!String.IsNullOrEmpty(ticket.TicketEmployee.NTLogin))
            {
                try
                {
                    var msg = new TaskCompletedNotification(ticket, ticket.TicketEmployee.NTLogin, String.Format("{0}<br />{1}", task.Task, task.Notes));
                    msg.SendEmail();
                    //var msg2 = new AssignmentNotificationUser(ticket);
                    //msg2.SendEmail();
                }
                catch (Exception err)
                {
                    ErrorTools.SendEmail(System.Web.HttpContext.Current.Request.Url, err,
                                                               System.Web.HttpContext.Current.User.Identity.Name);
                    Debug.Print(err.Message);
                }
            }
        }

        #endregion TicketTask

        #region TicketNotes

        public static void SendUpdateNotification(this TicketNote note)
        {
            //if (!note.TicketProjectsReference.IsLoaded)
            //    note.TicketProjectsReference.Load();
            //if (!note.TicketProject.TicketEmployeesReference.IsLoaded)
            //    note.TicketProject.TicketEmployeesReference.Load();
            if (note.Visibility > 3 && note.CreatedBy != note.TicketProject.RequestedBy)
                SendUpdateNotification(note.TicketProject, note.Note);
            if (note.TicketProject.TicketEmployee != null && note.Visibility > 3 && note.CreatedBy != note.TicketProject.TicketEmployee.NTLogin)
                SendUpdateNotificationToAssigned(note.TicketProject, note.Note);
        }

        #endregion TicketNotes

        public static void StoreAttachmentFile(this HttpPostedFileBase file, int ticketid)
        {
            var buf = new byte[file.ContentLength];
            file.InputStream.Read(buf, 0, file.ContentLength);
            var fn = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1).ToLower();

            var _AttachmentPath = HttpContext.Current.Server.MapPath(String.Format(@"~\Modules\Time.Support\Content\AttachmentFiles\{0}\", ticketid));
            if (!Directory.Exists(_AttachmentPath)) Directory.CreateDirectory(_AttachmentPath);
            var fullpath = Path.Combine(_AttachmentPath, fn);
            System.IO.File.WriteAllBytes(fullpath, buf);
        }
    }
}
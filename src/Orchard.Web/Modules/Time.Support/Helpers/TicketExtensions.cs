using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Support.Helpers
{
    public static class TicketExtensions
    {
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
    }
}
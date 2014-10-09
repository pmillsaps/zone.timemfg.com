<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="EpicWeb.Controllers" %>

<% if (ViewData["MyOpenTicketsbyStatus"] != null)
   { %>
<h3>My Tickets</h3>
<ul>
    <% foreach (KeyValuePair<string, int> d in ((Dictionary<String, int>)ViewData["MyOpenTicketsbyStatus"]))
       { %>
    <li><a href="/Support/MyStatus/<%= Html.Encode(d.Key.Replace("/", "+")) %>"><%= d.Key %> (<%=d.Value %>)</a></li>
    <%} %>
</ul>
<h3>Tickets I Wrote</h3>
<ul>
    <li><%= Html.ActionLink("My Open Tickets", "MyTickets", "Support") %></li>
    <li><%= Html.ActionLink("All My Tickets", "MyTicketsAll", "Support" )%></li>
</ul>
<% } %>

<% if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Support Admin"))
   { %>
<h3>Assigned To</h3>
<ul>
    <% foreach (var d in ((List<AssignedTo>)ViewData["OpenTicketsbyAssignedTo"]))
       { %>
    <li><a href="/support/AssignedTo/<%= Html.Encode(d.Employee.EmployeeID) %>"><%= d.Employee.FirstName + " " + d.Employee.LastName %> (<%=d.Count %>)</a></li>
    <%} %>
    <% } %>
</ul>

<% Html.RenderAction("_Reports", "Reports"); %>

<%--<% Html.RenderPartial("~/Views/Reports/_reports.ascx"); %>--%>

<h3>Status</h3>
<ul>
    <% foreach (KeyValuePair<string, int> d in ((Dictionary<String, int>)ViewData["OpenTicketsbyStatus"]))
       { %>
    <li><a href="/Support/Status/<%= Html.Encode(d.Key.Replace("/", "+")) %>"><%= d.Key %> (<%=d.Value %>)</a></li>
    <%} %>
</ul>
<h3>Category</h3>
<ul>
    <% foreach (KeyValuePair<string, int> d in ((Dictionary<String, int>)ViewData["OpenTicketsbyCategory"]))
       { %>
    <li><a href="/Support/Category/<%= Html.Encode(d.Key) %>"><%= d.Key %> (<%=d.Value %>)</a></li>
    <%} %>
</ul>
<h3>Departments</h3>
<ul>
    <% foreach (KeyValuePair<string, int> d in ((Dictionary<String, int>)ViewData["OpenTicketsbyDepartment"]))
       { %>
    <li><a href="/support/Department/<%= Html.Encode(d.Key) %>"><%= d.Key %> (<%=d.Value %>)</a></li>
    <%} %>
</ul>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/shared/support.master" Inherits="System.Web.Mvc.ViewPage<List<VersaliftDataServices.EntityModels.TimeMfg.TicketProject>>" %>

<%--<script src="../../Scripts/jquery.timeago.js" type="text/javascript"></script>--%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Support Index
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="cphHead" runat="server">
    <script src="../../Scripts/jquery.timeago.js" type="text/javascript"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            jQuery('abbr[class*=timeago]').timeago();
        })
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContentMain" runat="server">

    <h2>Overview</h2>
    <h3>Latest Tickets</h3>
    <em><%= ViewData["OpenCount"] %> tickets
        <% if (ViewData["TotalCount"] != null)
           { %>
                &nbsp;/ <%= ViewData["TotalCount"] %> open tickets
        <% } %>
    </em>

    <br />
    <% if (TempData["NewTicketID"] != null)
       { %>
    <div class="info">Ticket <a href="/support/info/<%= TempData["NewTicketID"].ToString() %>">#<%= TempData["NewTicketID"].ToString() %></a> has been successfully submitted.</div>
    <%} %>

    <div id="TicketSearch">
        <% using (Html.BeginForm("Search", "Support"))
           { %>
        <label for="txtSearch">Search:</label><%=Html.TextBox("txtSearch") %><input type="submit" value="Find" />
        <br />
        <%= Html.CheckBox("chkComplete", true) %> Include Completed Requests ?
        <br />
        <em>Use any keyword to search details and notes! Use a # to go straight to a ticket.. (ex: #10075)</em>
        <%} %>
    </div>
    <div id="SortMenu">
        <p>Sort by:</p>
        <ul>
            <li><a href="?<%= ViewData["currentpage"] %>">Ticket ID</a></li>
            <li><a href="?sort=requestor&<%= ViewData["currentpage"] %>">Requestor</a></li>
            <li><a href="?sort=department&<%= ViewData["currentpage"] %>">Department</a></li>
            <li><a href="?sort=priority&<%= ViewData["currentpage"] %>">Priority</a></li>
            <li><a href="?sort=assigned&<%= ViewData["currentpage"] %>">Assigned Priority</a></li>
        </ul>
    </div>
    <br />
    <div id="ticketpages">
        <% var count = (int)ViewData["OpenCount"];
           var showcount = (int)ViewData["showCount"];
           for (int i = 0; i < count; i += showcount)
           {%>
        <abbr class="pages"><a href="?page=<%=(i/showcount) + 1 %>&<%= ViewData["SortBy"] %>"><%=(i / showcount) + 1%></a></abbr>
        <%} %>
    </div>
    <div id="LatestTickets">
        <% foreach (var r in Model)
           { %>
        <div class="TicketItem">
            <h5><a href="/support/info/<%= r.TicketID %>"><%= r.Title %></a></h5>
            <div class="TicketDetails">
                <div class="TicketNumber">
                    Ticket #: <%= r.TicketID %>
                    <div class="TicketStatus">Status: <%= r.TicketStatus.Name %></div>
                </div>
                <div class="TicketPriority">
                    Priority: <%= r.TicketPriority.Name %>
                    <span class="TicketDepartment">Dept: <%= r.TicketDepartment.Name %></span>
                </div>
                <div class="TicketPriority">
                    <%--Priority: <%= r.TicketPriority.Name %>--%>
                    <span class="TicketDepartment">Category: <%= r.TicketCategory.Name %></span>
                </div>
            </div>
            <div class="clear"></div>
            <div class="TicketDescription"><%= r.Description %></div>
            <div class="RequestedBy">
                <p>
                    Requested by:
                            <a href="/Support/RequestedBy/<%= Html.Encode(r.RequestedBy.Replace(@"\","~")) %>"><%= r.RequestedByFriendly %></a> @
                    <abbr class="timeago" title="<%= r.RequestedDate %>"><%= r.RequestedDate %></abbr>
                    <% if (!(r.TicketSequence == null))
                       { %>
                    <span class="AssignedPriority">Assigned Priority: <%: r.TicketSequence %></span>
                    <% } %>
                </p>
            </div>
        </div>
        <%} %>
    </div>
    <div id="ticketpages">
        <% for (int i = 0; i < count; i += showcount)
           {%>
        <abbr class="pages"><a href="?page=<%=(i/showcount) + 1 %>&<%= ViewData["SortBy"] %>"><%=(i / showcount) + 1%></a></abbr>
        <%} %>
    </div>
</asp:Content>
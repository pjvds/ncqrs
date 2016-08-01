<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<ReadModel.TotalsPerDayItem>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Report</h2>

    <table>
        <tr>
            <th>
                Date
            </th>
            <th>
                NewCount
            </th>
            <th>
                EditCount
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%: String.Format("{0:g}", item.Date) %>
            </td>
            <td>
                <%: item.NewCount %>
            </td>
            <td>
                <%: item.EditCount %>
            </td>
        </tr>
    
    <% } %>

    </table>
</asp:Content>


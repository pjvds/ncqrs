<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<ReadModel.NoteItem>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <table>
        <tr>
            <th></th>
            <th>
                CreationDate
            </th>
            <th>
                Text
            </th>

        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%: Html.ActionLink("Edit", "Edit", "Note", new {id=item.Id}, null)%>
            </td>
            <td>
                <%: String.Format("{0:g}", item.CreationDate) %>
            </td>
            <td>
                <%: item.Text %>
            </td>

        </tr>
    
    <% } %>

    </table>

    <p>
        <%: Html.ActionLink("Add New", "Add", "Note") %>
    </p>

</asp:Content>
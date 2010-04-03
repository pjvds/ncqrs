<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Sample.ReadModel.MessageModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        All messages from the world!</h2>
    <table>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <span class="MessageText">
                    <%= Html.Encode(item.Text) %></span><br />
                <small>
                    <%= Html.Encode(item.CreationDate.ToString()) %></small>
            </td>
        </tr>
        <% } %>
    </table>
</asp:Content>

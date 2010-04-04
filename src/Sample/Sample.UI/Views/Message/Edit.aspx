<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Sample.ReadModel.EditMessageModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Edit message text</h2>
    <% using (Html.BeginForm())
       { %>
    <div>
        <p>
            <%= Html.TextArea("Text", "", 3, 45, new { @class = "messageinput" } )%>
            <%= Html.ValidationMessage("Text")%>
        </p>
        <p>
            <input type="submit" value="Update your share" />
        </p>
        <h3>
            Current message text</h3>
        <blockquote>
            <%= Html.Encode(Model.Text) %></blockquote>
        <h3>
            Previous text, if any</h3>
        <% foreach (var previousText in Model.TextChanges)
           {
        %>
        <p>
            <%= Html.Encode(previousText.ChangeDate.ToString()) %>:</p>
        <blockquote>
            <%= Html.Encode(previousText.Text) %></blockquote>
        <% } %>
    </div>
    <% } %>
</asp:Content>

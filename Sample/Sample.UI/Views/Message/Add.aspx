<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Sample.Commands.AddNewMessageCommand>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Add
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Add some love to the world!</h2>
    <p>
        Use the area below to drop in your message and hit the <i>Share it</i> button to spread it.
    </p>
    <%= Html.ValidationSummary("Adding the message was unsuccessful. Please correct the errors and try again.") %>
    <% using (Html.BeginForm())
       { %>
    <div>
        <p>
            <%= Html.TextArea("Text", "", 3, 45, new { @class = "messageinput" } )%>
            <%= Html.ValidationMessage("Text")%>
        </p>
        <p>
            <input type="submit" value="Share it" />
        </p>
    </div>
    <% } %>
</asp:Content>

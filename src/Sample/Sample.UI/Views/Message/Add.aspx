<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Sample.Commands.AddNewMessageCommand>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Add
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Add</h2>
    <p>
        Add a new message now!
    </p>
    <%= Html.ValidationSummary("Adding the message was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) { %>
        <div>
            <fieldset>
                <legend>The message you'd like to share</legend>
                <p>
                    <label for="username">Msg</label>
                    <%= Html.TextBox("Text")%>
                    <%= Html.ValidationMessage("Text")%>
                </p>
                <p>
                    <input type="submit" value="Share it" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>

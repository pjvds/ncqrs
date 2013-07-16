<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MyNotes.Commands.CreateNewNote>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Add
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Add</h2>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true)%>

    <fieldset>
        <legend>Fields</legend>

        <%= Html.HiddenFor(model => model.CommandIdentifier) %>
        <%= Html.HiddenFor(model => model.Id)%>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.Text)%>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Text)%>
            <%: Html.ValidationMessageFor(model => model.Text)%>
        </div>

        <p>
            <input type="submit" value="Create" />
        </p>
    </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index")%>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Commands.ChangeNoteText>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>
        
        <fieldset>
            <legend>Fields</legend>

            <%= Html.HiddenFor(model => model.CommandIdentifier) %>
            <%= Html.HiddenFor(model => model.NoteId)%>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.NewText) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.NewText) %>
                <%: Html.ValidationMessageFor(model => model.NewText) %>
            </div>
            
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>


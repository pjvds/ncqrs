<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Welcome
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Welcome to this new adventure</h2>
    <p class="MessageText">
    So, it seems the website is running But did you double checked that MongoDB is running? Since you will get nasty error messages if you didn't.
    </p>

</asp:Content>

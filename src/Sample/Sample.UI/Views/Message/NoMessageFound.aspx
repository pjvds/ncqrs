<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	NoMessageFound
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>NoMessageFound</h2>
    <p>Oops! There are no messages found. This is the first time you run this sample app so that there is no
    data available, or MongoDB just dropped all documents.</p>
    <p>Now go and <a href="/Message/Add">add a message</a>!</p>
</asp:Content>

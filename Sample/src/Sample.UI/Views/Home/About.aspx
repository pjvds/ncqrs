<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    About Us
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>About</h2>
    <p>
        This application is a sample application that demostrates the power, easyness and more impressive markting terms, of the <a href="http://ncqrs.codeplex.com/">Ncqrs Framework</a>.
    </p>
    <p>In this sample app you can post messages, list them all or edit them. All the pages have there own denormalized view model sitting there in MongoDB in a nice optimized way.</p>
    <h2>Feedback</h2>
    <p>
        We are always looking for feedback. Please visit <a href="http://ncqrs.org">ncqrs.org</a> for more information.
    </p>
</asp:Content>

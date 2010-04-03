<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Welcome
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Welcome to this new adventure</h2>
    <p class="MessageText">
        So, it seems the website is running But did you double checked that MongoDB is running?
        Since you will get nasty error messages if you didn't.
    </p>
    <h3>How do I double check?</h3>
    <p class="MessageText">
        For the fact you forgot how to double check:
        <ol>
            <li>Download MongoDB 1.4.0 for windows <a href="http://downloads.mongodb.org/win32/mongodb-win32-i386-1.4.0.zip">
                here</a>.</li>
            <li>Unpack all the content in the <strong>bin</strong> folder to <strong>c:\mongodb\</strong>.</li>
            <li>Start a <strong>Command Shell</strong> (cmd.exe) and <strong>cd</strong> to <strong>
                c:\mongodb\</strong>.</li>
            <li>Now enter <strong>mongod.exe</strong> and <strong>hit enter</strong>.</li>
        </ol>
    </p>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Exception>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Error
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Error page</h2>
    <% if (Model == null)
       { %>
    <p>
        Now this is funny... there is an error on the error page. Since we expected an error,
        but he wasn't there. Hmm... did you surfed directly to this page?</p>
    <% }
       else
       { %>
    <p>
        Oh no! A error occured. Now some application say that there way an unexpected exception,
        but he... who expects exceptions anyway?</p>
    <% if (Model is System.Net.Sockets.SocketException)
       { %>
    <h2>
        What can we say about the exception?</h2>
    <p>
        Let me see, the error is of the type <strong>SocketException</strong>. There is
        a good change that <strong>MongoDB</strong> is not running.</p>
    <p>
        Double check if it is running by following the following the next steps:</p>
    <ol>
        <li>Download MongoDB 1.4.0 for windows <a href="http://downloads.mongodb.org/win32/mongodb-win32-i386-1.4.0.zip">
            here</a>.</li>
        <li>Unpack all the content in the <strong>bin</strong> folder to <strong>c:\mongodb\</strong>.</li>
        <li>Start a <strong>Command Shell</strong> (cmd.exe) and <strong>cd</strong> to <strong>
            c:\mongodb\</strong>.</li>
        <li>Now enter <strong>mongod.exe</strong> and <strong>hit enter</strong>.</li>
    </ol>
    <% } %>
    <h2>
        Exception details</h2>
    <p>
        Anyhow, since you are a software developer. Here are the error details, now go fix
        it.</p>
    <table>
        <tr>
            <td>
                Message
            </td>
            <td>
                <%= Html.Encode(Model.Message) %>
            </td>
        </tr>
        <tr>
            <td>
                Source
            </td>
            <td>
                <%= Html.Encode(Model.Source) %>
            </td>
        </tr>
        <tr>
            <td>
                Stack trace
            </td>
            <td>
                <%= Html.Encode(Model.StackTrace) %>
            </td>
        </tr>
        <tr>
            <td>
                Full details
            </td>
            <td>
                <%= Html.Encode(Model.ToString()) %>
            </td>
        </tr>
    </table>
    <% } %>
</asp:Content>

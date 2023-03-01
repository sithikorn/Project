<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="SSL___ProjectMaintenance.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="App_Themes/formTheme.css" />
    <link rel="stylesheet" type="text/css" href="App_Themes/popupTheme.css" />
</head>
<body>
    <form id="form1" runat="server">
        <center>
             <br />
             <br />
             <br />
             <br />
             <br />
        <div id="divPanelPopUpLogin" class="popupLogin">
            <div class="popup_Container">
                <div id="divPopupHeaderLogin" class="popup_Titlebar">
                    <div id="divTitlePopupLogin" class="TitlebarLeft">
                        <asp:Label ID="lblTitlePopupLogin" runat="server" Text="Log in"></asp:Label>
                    </div>
                    <%--                    <div id="divClosePopupLogin" class="TitlebarRight"></div>--%>
                </div>
                <div class="popup_Body">
                    <table class="popUp" width="250px">
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblTitle" runat="server">Please log in</asp:Label>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="td-lable">
                                <asp:Label ID="lblPopupLoginEmployee" runat="server">User Name &nbsp;</asp:Label>
                            </td>
                            <td class="td-input">
                                <asp:TextBox ID="txtUserName" runat="server" Width="150px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="td-lable">
                                <asp:Label ID="lblPopupTimeDate" runat="server">Password &nbsp;</asp:Label>
                            </td>
                            <td class="td-input">
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Width="150px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <br />
                                <asp:Button ID="btnLogin" runat="server" Text="Log In" />
                                &nbsp;&nbsp;&nbsp;
                                  <asp:Button ID="btnClose" runat="server" Text="Close"  OnClientClick="javaScript:window.close(); return false;"  />
                                &nbsp;&nbsp;</td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:Label ID="lblPopUpLoginResult" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <%--  <tr>
                            <td align="center" colspan="2">
                                <asp:Label ID="Label1" runat="server" Text ="Please open finger server application" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>--%>
                    </table>
                    <br />
                </div>
            </div>
        </div>
        </center>
    </form>
</body>
</html>

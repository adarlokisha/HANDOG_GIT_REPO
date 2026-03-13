<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Handog.web._default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Login</title>
    <link href="~/stylesheet/default.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <header class="top-header">
            <div class="header-content">
                <asp:Image ID="imgSmallLogo" runat="server" ImageUrl="~/images/HandogLogo1.png" AlternateText="Handog Logo" CssClass="small-logo" />
                <span class="home-text">Home</span>
            </div>
        </header>

        <main class="main-container">
    
            <div class="logo-container">
                <asp:Image ID="imgMainLogo" runat="server" ImageUrl="~/images/HandogBig1.png" AlternateText="Handog Para Sa Komunidad" CssClass="main-logo" />
            </div>

            <div class="login-card">
                
                <div class="input-group">
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input" placeholder="Email Address"></asp:TextBox>
                </div>
                
                <div class="input-group">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-input" TextMode="Password" placeholder="Password"></asp:TextBox>
                </div>

                <div class="options-row">
                    <div class="checkbox-container">
                        <asp:CheckBox ID="chkRememberMe" runat="server" Text=" Remember Me" CssClass="remember-checkbox" />
                    </div>
                    <asp:HyperLink ID="lnkForgotPassword" runat="server" NavigateUrl="~/ForgotPassword.aspx" CssClass="forgot-link">Forgot Password</asp:HyperLink>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="LOGIN" CssClass="btn btn-gradient" OnClick="btnLogin_Click"/>

                <div class="signup-prompt">
                    Don't have an account? <asp:HyperLink ID="lnkSignUp" runat="server" NavigateUrl="~/SignUp.aspx" CssClass="signup-link">Sign up.</asp:HyperLink>
                </div>

            </div>
        </main>
    </form>
</body>
</html>

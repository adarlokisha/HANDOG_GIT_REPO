<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Handog.web._default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Login</title>
    <link href="~/stylesheet/default.css" rel="stylesheet" type="text/css" />
    <style>
        .modal-overlay {
            display: none;
            position: fixed;
            top:0; left:0; right:0; bottom:0;
            background-color: rgba(0,0,0,0.5);
            z-index: 1000;
        }
        .modal-content {
            background-color: #fff;
            width: 600px;
            max-width: 90%;
            margin: 100px auto;
            padding: 30px;
            border-radius: 10px;
            position: relative;
        }
        .close-btn {
            position: absolute;
            top: 10px;
            right: 15px;
            font-size: 18px;
            font-weight: bold;
            cursor: pointer;
        }
    </style>
    <script type="text/javascript">
        function showSignup() {
            document.getElementById('<%= signupPanel.ClientID %>').style.display = 'block';
        }
        function closeSignup() {
            document.getElementById('<%= signupPanel.ClientID %>').style.display = 'none';
        }

        function togglePassword() {
            const pwdInput = document.getElementById('<%= txtPassword.ClientID %>');
            const chkShow = document.getElementById('<%= chkShowPassword.ClientID %>');

            if (chkShow.checked) {
                pwdInput.type = 'text';
            } else {
                pwdInput.type = 'password';
            }
        }

        window.onload = function () {
            const churchLabel = document.getElementById('<%= lblChurchID.ClientID %>');
            const churchInput = document.getElementById('<%= txtChurchID.ClientID %>');
            const rbOrganizer = document.getElementById('<%= rbOrganizer.ClientID %>');
            const rbVolunteer = document.getElementById('<%= rbVolunteer.ClientID %>');

            function updateChurchVisibility() {
                if (rbOrganizer.checked) {
                    churchLabel.style.display = 'inline-block';
                    churchInput.style.display = 'block';
                    churchInput.setAttribute('required', 'required');
                } else {
                    churchLabel.style.display = 'none';
                    churchInput.style.display = 'none';
                    churchInput.removeAttribute('required');
                    churchInput.value = '';
                }
            }
            updateChurchVisibility();

            rbOrganizer.addEventListener('change', updateChurchVisibility);
            rbVolunteer.addEventListener('change', updateChurchVisibility);
        };
    </script>
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
                <asp:Image ID="imgMainLogo" runat="server" ImageUrl="~/images/HandogBig3.png" AlternateText="Handog Para Sa Komunidad" CssClass="main-logo" />
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
                        <asp:CheckBox ID="chkShowPassword" runat="server" Text=" Show Password" 
                            CssClass="showPassword-checkbox" onclick="togglePassword()" />
                    </div>
                    <asp:HyperLink ID="lnkForgotPassword" runat="server" NavigateUrl="~/ForgotPassword.aspx" CssClass="forgot-link">Forgot Password</asp:HyperLink>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="LOGIN" CssClass="btn btn-gradient" OnClick="btnLogin_Click" />

                <div class="signup-prompt">
                    Don't have an account? 
                    <a href="javascript:void(0);" class="signup-link" onclick="showSignup();">Sign up.</a>
                </div>
            </div>

            <asp:Panel ID="signupPanel" runat="server" CssClass="modal-overlay">
                <div class="modal-content">
                    <span class="close-btn" onclick="closeSignup()">&times;</span>
                    <h2>Create Account</h2>

                    <asp:Label ID="lblRole" runat="server" Text="Role *"></asp:Label><br/>
                    <asp:RadioButton ID="rbVolunteer" runat="server" Text=" Volunteer" GroupName="role" Checked="true" />
                    <asp:RadioButton ID="rbOrganizer" runat="server" Text=" Organizer" GroupName="role" /><br/><br/>

                    <asp:Label ID="lblFirstName" runat="server" Text="First Name *"></asp:Label>
                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-input"></asp:TextBox>

                    <asp:Label ID="lblLastName" runat="server" Text="Last Name *"></asp:Label>
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-input"></asp:TextBox>

                    <asp:Label ID="lblSignupEmail" runat="server" Text="Email *"></asp:Label>
                    <asp:TextBox ID="txtSignupEmail" runat="server" CssClass="form-input"></asp:TextBox>

                    <asp:Label ID="lblContact" runat="server" Text="Contact Number *"></asp:Label>
                    <asp:TextBox ID="txtContact" runat="server" CssClass="form-input"></asp:TextBox>

                    <asp:Label ID="lblSignupPassword" runat="server" Text="Password *"></asp:Label>
                    <asp:TextBox ID="txtSignupPassword" runat="server" TextMode="Password" CssClass="form-input"></asp:TextBox>

                    <asp:Label ID="lblChurchID" runat="server" Text="Church ID *" style="display:none;"></asp:Label>
                    <asp:TextBox ID="txtChurchID" runat="server" CssClass="form-input" style="display:none;"></asp:TextBox>

                    <asp:Label ID="lblSignupMessage" runat="server" ForeColor="Red"></asp:Label><br/><br />

                    <asp:Button ID="btnSignup" runat="server" Text="Create Account" CssClass="btn btn-gradient" OnClick="btnSignup_Click"/>
                </div>
            </asp:Panel>
        </main>
    </form>
</body>
</html>
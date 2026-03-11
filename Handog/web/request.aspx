<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="request.aspx.cs" Inherits="Handog.web.request" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Requests</title>
    <link href="~/stylesheet/request.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <header class="top-header">
            <div class="nav-left">
                <asp:Image ID="imgSmallLogo" runat="server" ImageUrl="~/images/HandogLogo1.png" AlternateText="Handog Logo" CssClass="small-logo" />
                <a href="home.aspx" class="nav-link">Home</a>
                
                <a href="request.aspx" class="nav-link active">Request</a>
                
                <a href="events.aspx" class="nav-link">Events</a>
            </div>
            <div class="nav-right">
                <asp:LinkButton ID="btnLogout" runat="server" CssClass="nav-link logout-link">LOGOUT</asp:LinkButton>
            </div>
        </header>

        <main class="main-content">
            
            <div class="page-header">
                <div class="title-area">
                    <h1 class="page-title">We are in need of help!</h1>
                    <p class="page-subtitle">Request for a Community Engagement to be held in your area:</p>
                </div>
                <div class="icon-area">
                    <asp:Image ID="imgBell" runat="server" ImageUrl="~/images/bell-icon.png" AlternateText="Notifications" CssClass="bell-icon" />
                </div>
            </div>

            <hr class="title-divider" />

            <div class="cards-scroll-container">
                
                <div class="request-card">
                    <p class="requestor-name">NAME OF REQUESTORS</p>
                    <hr class="card-divider" />
                    <p class="request-text">
                        <span class="bold-label">REASON FOR REQUEST:</span> Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                    </p>
                </div>

                <div class="request-card">
                    <p class="requestor-name">NAME OF REQUESTORS</p>
                    <hr class="card-divider" />
                    <p class="request-text">
                        <span class="bold-label">INQUIRY:</span> Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                    </p>
                </div>

            </div>

            <div class="button-container">
                <asp:Button ID="btnAddRequest" runat="server" Text="+ ADD REQUEST" CssClass="btn-add" />
            </div>

        </main>
    </form>
</body>
</html>
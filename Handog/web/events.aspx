<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="events.aspx.cs" Inherits="Handog.web.events" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Events</title>
    <link href="~/stylesheet/events.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <header class="top-header">
            <div class="nav-left">
                <asp:Image ID="imgSmallLogo" runat="server" ImageUrl="~/images/HandogLogo1.png" AlternateText="Handog Logo" CssClass="small-logo" />
                <a href="home.aspx" class="nav-link">Home</a>
                <a href="request.aspx" class="nav-link">Request</a>
                
                <a href="events.aspx" class="nav-link active">Events</a>
            </div>
            <div class="nav-right">
                <asp:LinkButton ID="btnLogout" runat="server" CssClass="nav-link logout-link">LOGOUT</asp:LinkButton>
            </div>
        </header>

        <main class="main-content">
            
            <div class="search-header">
                <div class="search-bar-container">
                    <asp:Image ID="imgSearchIcon" runat="server" ImageUrl="~/images/search-icon.png" CssClass="search-icon" />
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search for an event..."></asp:TextBox>
                </div>
                <div class="bell-container">
                    <asp:Image ID="imgBell" runat="server" ImageUrl="~/images/bell-icon.png" AlternateText="Notifications" CssClass="bell-icon" />
                </div>
            </div>

            <h1 class="page-title">EVENTS</h1>

            <div class="events-list">
                
                <div class="event-card">
                    <div class="event-info">
                        <h2 class="event-name">COMMUNITY ENGAGEMENT</h2>
                        <ul class="event-details">
                            <li>Mapua Malayan Colleges Laguna</li>
                            <li>January 23, 2026</li>
                            <li>7:00AM - 6:00PM</li>
                        </ul>
                        <p class="event-desc">
                            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris tincidunt mauris sed ipsum auctor venenatis.
                        </p>
                    </div>
                   <div class="event-image image-1">
                        <asp:Button ID="btnViewDetails" runat="server" Text="VIEW DETAILS" CssClass="btn-action" />
                    </div>
                </div>

                <div class="event-card">
                    <div class="event-info">
                        <h2 class="event-name">COMMUNITY ENGAGEMENT</h2>
                        <ul class="event-details">
                            <li>Mapua Malayan Colleges Laguna</li>
                            <li>January 23, 2026</li>
                            <li>7:00AM - 6:00PM</li>
                        </ul>
                        <p class="event-desc">
                            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris tincidunt mauris sed ipsum auctor venenatis.
                        </p>
                    </div>
                    <div class="event-image image-2">
                        <asp:Button ID="btnRegister" runat="server" Text="+ REGISTER" CssClass="btn-action" />
                    </div>
                </div>

            </div>

        </main>
    </form>
</body>
</html>
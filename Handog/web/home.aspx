<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="Handog.web.home" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Home</title>
    <link href="~/stylesheet/home.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <header class="top-header">
            <div class="nav-left">
                <asp:Image ID="imgSmallLogo" runat="server" ImageUrl="~/images/HandogLogo1.png" AlternateText="Handog Logo" CssClass="small-logo" />
                <a href="home.aspx" class="nav-link active">Home</a>
                <a href="request.aspx" class="nav-link">Request</a>
                <a href="events.aspx" class="nav-link">Events</a>
            </div>
            <div class="nav-right">
                <asp:LinkButton ID="btnLogout" runat="server" onClick="btnLogout_Click" CssClass="nav-link logout-link">LOGOUT</asp:LinkButton>
            </div>
        </header>

        <main>
            <section class="hero-section">
                <div class="bell-container">
                    <asp:LinkButton ID="btnBell" runat="server" OnClick="btnBell_Click">
                        <asp:Image ID="imgBell" runat="server" ImageUrl="~/images/bell-icon.png" AlternateText="Notifications" CssClass="bell-icon" />
                    </asp:LinkButton>
                </div>

                <div class="hero-content">
                    <div class="hero-text">
                        <h1>Connecting hearts and strengthening communities.</h1>
                        <p>Whether you are a volunteer, a requester, a business, or an organization, Handog is the perfect place for you to manage your events.</p>
                    </div>
                    <div class="hero-image-container">
                        <asp:Image ID="imgVolunteers" runat="server" ImageUrl="~/images/Volunteer.jpeg" AlternateText="Volunteers" CssClass="hero-image" />
                    </div>
                </div>
            </section>

            <section class="locales-section">
                <h2 class="section-title">List of Locales</h2>
                <div class="table-container">
                    <table class="locales-table">
                        <thead><tr><th></th></tr></thead>
                        <tbody>
                            <tr><td></td></tr>
                            <tr><td></td></tr>
                        </tbody>
                    </table>
                </div>
            </section>

            <section class="story-section">
                <h2 class="story-title">Our Story</h2>
                <div class="story-card">
                    <asp:Image ID="imgStoryLogo" runat="server" ImageUrl="~/images/HandogBig1.png" AlternateText="Handog Logo" CssClass="story-logo" />
                    <p>Our platform was created with one simple goal — to make community engagement easier, more organized, and more accessible for everyone. 
                        We saw the gap between people who want to serve, those who need support, and organizers who are willing to lead initiatives, but lack a centralized system to connect them all.</p>
                </div>
            </section>
        </main>

        <asp:Panel ID="pnlNotifications" runat="server" CssClass="modal-overlay" Visible="false">
            <div class="notification-card">
                <div class="modal-header">
                    <asp:Image ID="imgModalBell" runat="server" ImageUrl="~/images/bell-icon.png" CssClass="modal-bell-icon" />
                    <span class="modal-title">NOTIFICATIONS</span>
                </div>
                <div class="notification-list">
                    <div class="notif-item">
                        <i class="info-icon">ⓘ</i> <span>New event has been added!</span>
                    </div>
                    <div class="notif-item success">
                        <i class="info-icon">ⓘ</i> <span>You've been accepted to volunteer!</span>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnCloseNotif" runat="server" OnClick="btnCloseNotif_Click" CssClass="modal-close">
                        <span class="close-arrow">➔</span> CLOSE
                    </asp:LinkButton>
                </div>
            </div>
         </asp:Panel>

    </form>
</body>
</html>
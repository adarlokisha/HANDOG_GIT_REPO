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
                <asp:LinkButton ID="btnLogout" runat="server" onClick="btnLogout_Click" CssClass="nav-link logout-link">LOGOUT</asp:LinkButton>
            </div>
        </header>

        <main class="main-content">
            <div class="search-header">
                <div class="search-bar-container">
                    <asp:Image ID="imgSearchIcon" runat="server" ImageUrl="~/images/search-icon.png" CssClass="search-icon" />
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search for an event..."></asp:TextBox>
                </div>
                <div class="bell-container">
                    <asp:LinkButton ID="btnBell" runat="server" OnClick="btnBell_Click">
                        <asp:Image ID="imgBell" runat="server" ImageUrl="~/images/bell-icon.png" AlternateText="Notifications" CssClass="bell-icon" />
                    </asp:LinkButton>
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
                        <p class="event-desc"> Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                            Mauris tincidunt mauris sed ipsum auctor venenatis.p>
                    </div>
                    <div class="event-image image-1">
                        <asp:Button ID="btnViewDetails" runat="server" Text="VIEW DETAILS" CssClass="btn-action" OnClick="btnViewDetails_Click" />
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
                        <p class="event-desc"> Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                            Mauris tincidunt mauris sed ipsum auctor venenatis.</p>
                    </div>
                    <div class="event-image image-2">
                        <asp:Button ID="btnRegister" runat="server" Text="+ REGISTER" CssClass="btn-action" OnClick="btnRegister_Click" />
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnlRegistration" runat="server" CssClass="modal-overlay" Visible="false">
                <div class="registration-card">
                    <div class="reg-modal-header">
                        <div class="header-left">
                            <asp:LinkButton ID="btnCloseReg" runat="server" OnClick="btnCloseModals_Click" CssClass="back-btn-link">
                                <div>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/back-icon.png" CssClass="back-icon-img" />
                                </div>
                            </asp:LinkButton>
                            <div class="header-titles">
                                <h1 class="reg-title">REGISTRATION</h1>
                                <p class="reg-subtitle">COMMUNITY ENGAGEMENT TITLE</p>
                            </div>
                        </div>
                    </div>

                    <hr class="reg-divider" />

                    <div class="reg-content-split">
                        <div class="reg-details-side">
                            <h3 class="section-underline-title">EVENT DETAILS</h3>
                            <div class="details-list">
                                <p><strong>TITLE:</strong> Event Title</p>
                                <p><strong>ORGANIZER NAME:</strong> Juan dela Cruz</p>
                                <p><strong>ADDRESS:</strong> Pulo, Diezmo Road</p>
                                <p><strong>VENUE:</strong> Brgy. Hall</p>
                                <p><strong>EMAIL:</strong> email@gmail.com</p>
                                <p><strong>CONTACT NUMBER:</strong> +63***********</p>
                                <p><strong>DATE:</strong> January 31, 2026</p>
                                <p><strong>START TIME:</strong> 7:00AM</p>
                                <p><strong>END TIME:</strong> 12:00PM</p>
                                <p><strong>MAX VOLUNTEERS:</strong> 50</p>
                            </div>
                        </div>

                        <div class="reg-form-side">
                            <div class="floating-reg-box">
                                <label class="field-label">REGISTER AS *</label>
                                <div class="reg-radio-group">
                                    <asp:RadioButton ID="rbVolunteer" runat="server" GroupName="RegRole" Text="VOLUNTEER" />
                                    <asp:RadioButton ID="rbParticipant" runat="server" GroupName="RegRole" Text="PARTICIPANT" />
                                </div>

                                <label class="field-label">FULL NAME *</label>
                                <asp:TextBox ID="txtRegName" runat="server" CssClass="reg-input-full"></asp:TextBox>

                                <div class="reg-input-row">
                                    <div class="input-half">
                                        <label class="field-label">EMAIL *</label>
                                        <asp:TextBox ID="txtRegEmail" runat="server" CssClass="reg-input-full"></asp:TextBox>
                                    </div>
                                    <div class="input-half">
                                        <label class="field-label">CONTACT NUMBER *</label>
                                        <asp:TextBox ID="txtRegPhone" runat="server" CssClass="reg-input-full"></asp:TextBox>
                                    </div>
                                </div>

                                <label class="field-label">I AGREE TO THE <a href="#">TERMS AND CONDITIONS</a> *</label>
                                <div class="reg-radio-group">
                                    <asp:RadioButton ID="rbAgreeYes" runat="server" GroupName="Consent" Text="YES" />
                                    <asp:RadioButton ID="rbAgreeNo" runat="server" GroupName="Consent" Text="NO" />
                        
                                    <asp:Button ID="btnConfirmReg" runat="server" Text="+ REGISTER" CssClass="btn-action reg-float-btn" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEventDetails" runat="server" CssClass="modal-overlay" Visible="false">
                <div class="details-card">
                    <div class="modal-header-row">
                       <asp:LinkButton ID="btnCloseDetails" runat="server" OnClick="btnCloseModals_Click" CssClass="back-btn-link">
                            <div>
                                <asp:Image ID="imgBackArrow" runat="server" ImageUrl="~/images/back-icon.png" CssClass="back-icon-img" />
                            </div>
                        </asp:LinkButton>
                        <h1 class="modal-main-title">EVENTS</h1>
                    </div>

                    <div class="details-container-box">
                        <div class="details-header-yellow">
                            <div class="category-tag">COMMUNITY ENGAGEMENT</div>
                            <div class="engagement-title">TITLE OF COMMUNITY ENGAGEMENT</div>
                        </div>
            
                        <div class="details-body-white">
                            <h3 class="details-section-title">EVENT DETAILS</h3>
                            <div class="details-grid">
                                <p><strong>TITLE:</strong> Event Title</p>
                                <p><strong>ORGANIZER NAME:</strong> Juan dela Cruz</p>
                                <p><strong>ADDRESS:</strong> Pulo, Diezmo Road</p>
                                <p><strong>VENUE:</strong> Brgy. Hall</p>
                                <p><strong>EMAIL:</strong> email@gmail.com</p>
                                <p><strong>CONTACT NUMBER:</strong> +63***********</p>
                                <p><strong>DATE:</strong> January 31, 2026</p>
                                <p><strong>START TIME:</strong> 7:00AM</p>
                                <p><strong>END TIME:</strong> 12:00PM</p>
                                <p><strong>MAX VOLUNTEERS:</strong> 50</p>
                                <p class="announcement-row">
                                    <strong>ANNOUNCEMENT:</strong> Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do 
                                    eiusmod tempor incididunt ut labore et dolore magna aliqua.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
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
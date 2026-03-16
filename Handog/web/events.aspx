<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="events.aspx.cs" Inherits="Handog.web.events" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Events</title>
    <link rel="icon" type="image/png" href="~/images/HandogLogo2.png" runat="server" />
    <link href="~/stylesheet/events.css?v=1" rel="stylesheet" type="text/css" />
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

        <!-- SEARCH AND NOTIFICATION -->
        <main class="main-content">
            <div class="search-header">
                <div class="search-bar-container">
                    <asp:ImageButton ID="btnSearchIcon" runat="server" ImageUrl="~/images/search-icon.png" CssClass="search-icon" OnClick="btnSearchIcon_Click" />
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search for an event..." AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                </div>
                <div class="bell-container">
                    <asp:LinkButton ID="btnBell" runat="server" OnClick="btnBell_Click">
                        <asp:Image ID="imgBell" runat="server" ImageUrl="~/images/bell-icon.png" AlternateText="Notifications" CssClass="bell-icon" />
                    </asp:LinkButton>
                </div>
            </div>

            <!-- MAIN CONTENT -->
            <h1 class="page-title">EVENTS</h1>

            <!-- EVENTS PANEL -->
            <div class="events-list">
                <asp:Repeater ID="rptEvents" runat="server">
                    <ItemTemplate>
                        <!-- Event Information Summary -->
                        <div class="event-card">
                            <div class="event-info">
                                <h2 class="event-name"><%# Eval("EventTitle") %></h2>
                                <ul class="event-details">
                                    <li><%# Eval("EventAddress") %></li>
                                    <li><%# Eval("ImplementationDate", "{0:MMMM dd, yyyy}") %></li>
                                    <li><%# Eval("EventStartTime", "{0:t}") %> - <%# Eval("EventEndTime", "{0:t}") %></li>
                                </ul>
                                <p class="event-desc"><%# Eval("Announcement") %></p>
                            </div>

                            <!-- View Details and Joined -->
                            <div class="event-image">
                                <div class="event-card-bg image-<%# Container.ItemIndex % 2 + 1 %>"></div>
                                <div class="status-container">
                                    <%-- Shows VIEW DETAILS and JOINED status only if user IS registered --%>
                                    <asp:PlaceHolder ID="phJoined" runat="server" Visible='<%# IsUserRegistered(Eval("PublishedEventNum")) %>'>
                                        <asp:Button ID="btnViewDetails" runat="server" Text="VIEW DETAILS" CssClass="btn-action" 
                                            OnClick="btnViewDetails_Click" CommandArgument='<%# Eval("PublishedEventNum") %>' />
                                        <div class="joined-status">JOINED</div>
                                    </asp:PlaceHolder>

                                    <%-- Shows + REGISTER button only if user is NOT registered --%>
                                    <asp:Button ID="btnRegister" runat="server" Text="+ REGISTER" CssClass="btn-action" 
                                        OnClick="btnRegister_Click" CommandArgument='<%# Eval("PublishedEventNum") %>'
                                        Visible='<%# !IsUserRegistered(Eval("PublishedEventNum")) %>' />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div> 

            <!-- VOLUNTEER REGISTRATION MODAL -->
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
                                <p class="reg-subtitle"><asp:Label ID="lblRegEventTitleHeader" runat="server" /></p>
                            </div>
                        </div>
                    </div>

                    <hr class="reg-divider" />

                    <!-- Event Details -->
                    <div class="reg-content-split">
                        <div class="reg-details-side">
                            <h3 class="section-underline-title">EVENT DETAILS</h3>
                           <div class="details-list">
                                <p><strong>TITLE:</strong> <asp:Label ID="lblRegTitle" runat="server" /></p>
                                <p><strong>ORGANIZER NAME:</strong> <asp:Label ID="lblRegOrganizer" runat="server" /></p>
                                <p><strong>ADDRESS:</strong> <asp:Label ID="lblRegAddress" runat="server" /></p>
                                <p><strong>VENUE:</strong> <asp:Label ID="lblRegVenue" runat="server" /></p>
                                <p><strong>EMAIL:</strong> <asp:Label ID="lblRegEmail" runat="server" /></p>
                                <p><strong>CONTACT NUMBER:</strong> <asp:Label ID="lblRegContact" runat="server" /></p>
                                <p><strong>DATE:</strong> <asp:Label ID="lblRegDate" runat="server" /></p>
                                <p><strong>START TIME:</strong> <asp:Label ID="lblRegStart" runat="server" /></p>
                                <p><strong>END TIME:</strong> <asp:Label ID="lblRegEnd" runat="server" /></p>
                                <p><strong>MAX VOLUNTEERS:</strong> <asp:Label ID="lblRegMax" runat="server" /></p>
                            </div>
                        </div>

                        <!-- Registration Form -->
                        <div class="reg-form-side">
                            <div class="floating-reg-box">
                                <label class="field-label">REGISTER AS *</label>
                                <!-- Volunteer or Participant -->
                                <div class="reg-radio-group">
                                    <asp:RadioButton ID="rbVolunteer" runat="server" GroupName="RegRole" Text="VOLUNTEER" />
                                    <asp:RadioButton ID="rbParticipant" runat="server" GroupName="RegRole" Text="PARTICIPANT" />
                                </div>

                                <!-- Read-only (Contents from database) -->
                                <label class="field-label">FULL NAME</label>
                                <asp:TextBox ID="txtRegName" runat="server" CssClass="reg-input-full"></asp:TextBox>

                                <div class="reg-input-row">
                                    <div class="input-half">
                                        <label class="field-label">EMAIL</label>
                                        <asp:TextBox ID="txtRegEmail" runat="server" CssClass="reg-input-full"></asp:TextBox>
                                    </div>
                                    <div class="input-half">
                                        <label class="field-label">CONTACT NUMBER</label>
                                        <asp:TextBox ID="txtRegPhone" runat="server" CssClass="reg-input-full"></asp:TextBox>
                                    </div>
                                </div>

                                <!-- Terms and Conditions -->
                                <label class="field-label">I AGREE TO THE <a href="#">TERMS AND CONDITIONS</a> *</label>
                                <div class="reg-radio-group">
                                    <asp:RadioButton ID="rbAgreeYes" runat="server" GroupName="Consent" Text="YES" />
                                    <asp:RadioButton ID="rbAgreeNo" runat="server" GroupName="Consent" Text="NO" />
                        
                                    <asp:Button ID="btnConfirmReg" runat="server" Text="+ REGISTER" CssClass="btn-action reg-float-btn" OnClick="btnConfirmReg_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- VIEW EVENT DETAILS MODAL-->
            <asp:Panel ID="pnlEventDetails" runat="server" CssClass="modal-overlay" Visible="false">
                <div class="details-card">
                    <!-- Modal Header -->
                    <div class="modal-header-row">
                       <asp:LinkButton ID="btnCloseDetails" runat="server" OnClick="btnCloseModals_Click" CssClass="back-btn-link">
                            <div>
                                <asp:Image ID="imgBackArrow" runat="server" ImageUrl="~/images/back-icon.png" CssClass="back-icon-img" />
                            </div>
                        </asp:LinkButton>
                        <h1 class="modal-main-title">EVENTS</h1>
                    </div>

                     <!-- Event Details -->
                    <div class="details-container-box">
                        <div class="details-header-yellow">
                            <div class="category-tag">COMMUNITY ENGAGEMENT</div>
                            <div class="engagement-title"><asp:Label ID="lblDetEventTitleHeader" runat="server" /></div>
                        </div>
            
                        <div class="details-body-white">
                            <h3 class="details-section-title">EVENT DETAILS</h3>
                            <div class="details-grid">
                                <p><strong>TITLE:</strong> <asp:Label ID="lblDetTitle" runat="server" /></p>
                                <p><strong>ORGANIZER NAME:</strong> <asp:Label ID="lblDetOrganizer" runat="server" /></p>
                                <p><strong>ADDRESS:</strong> <asp:Label ID="lblDetAddress" runat="server" /></p>
                                <p><strong>VENUE:</strong> <asp:Label ID="lblDetVenue" runat="server" /></p>
                                <p><strong>EMAIL:</strong> <asp:Label ID="lblDetEmail" runat="server" /></p>
                                <p><strong>CONTACT NUMBER:</strong> <asp:Label ID="lblDetContact" runat="server" /></p>
                                <p><strong>DATE:</strong> <asp:Label ID="lblDetDate" runat="server" /></p>
                                <p><strong>START TIME:</strong> <asp:Label ID="lblDetStart" runat="server" /></p>
                                <p><strong>END TIME:</strong> <asp:Label ID="lblDetEnd" runat="server" /></p>
                                <p><strong>MAX VOLUNTEERS:</strong> <asp:Label ID="lblDetMax" runat="server" /></p>
                                <p class="announcement-row">
                                    <strong>ANNOUNCEMENT:</strong> <asp:Label ID="lblDetAnnouncement" runat="server" />
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </main>

         <!-- NOTIFICATION MODAL -->
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
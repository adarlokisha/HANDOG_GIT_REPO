<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="events.aspx.cs" Inherits="Handog.org._events" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Organize Events</title>
    <link href="~/stylesheet/orgevent.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <header class="top-header">
            <div class="nav-left">
                <asp:Image ID="imgSmallLogo" runat="server" ImageUrl="~/images/HandogLogo1.png" CssClass="small-logo" />
                <a href="home.aspx" class="nav-link">Home</a>
                <a href="request.aspx" class="nav-link">Requests</a>
                <a href="events.aspx" class="nav-link active">Events</a>
            </div>
            <div class="nav-right">
                <asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click" CssClass="nav-link logout-link">LOGOUT</asp:LinkButton>
            </div>
        </header>

        <main class="main-content">
            
            <div class="top-actions">
                <div class="search-bar-container">
                    <asp:Image ID="imgSearchIcon" runat="server" ImageUrl="~/images/search-icon.png" CssClass="search-icon" />
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search for an event..."></asp:TextBox>
                </div>
                <asp:LinkButton ID="btnBell" runat="server" OnClick="btnBell_Click">
                    <asp:Image ID="imgBell" runat="server" ImageUrl="~/images/bell-icon.png" CssClass="bell-icon" />
                </asp:LinkButton>
            </div>

            <asp:Panel ID="pnlMainEvents" runat="server" Visible="true">
                <h1 class="page-title">MY EVENTS</h1>

                <div class="events-list">
                    <asp:Repeater ID="rptEvents" runat="server" OnItemCommand="rptEvents_ItemCommand">
                        <ItemTemplate>
                            <div class="event-card">
                                <div class="event-info">
                                    <h2 class="event-name"><%# Eval("EventTitle") %></h2>
                                    <ul class="event-details">
                                        <li><%# Eval("Venue") %></li>
                                        <li><%# Eval("EventDate") %></li>
                                        <li><%# Eval("TimeStr") %></li>
                                    </ul>
                                    <p class="event-desc"><%# Eval("Description") %></p>
                                </div>
                                <div class="event-image">
                                    <asp:Button ID="btnViewDetails" runat="server" Text="VIEW DETAILS" CommandName="ViewDetails" CommandArgument='<%# Eval("EventID") %>' CssClass="btn-view" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="center-button">
                    <asp:Button ID="btnOpenCreateModal" runat="server" Text="+ ORGANIZE EVENT" OnClick="btnOpenCreateModal_Click" CssClass="btn-primary" />
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlManageEvent" runat="server" Visible="false">
                
                <div class="manage-header">
                    <asp:LinkButton ID="btnBackToMain" runat="server" OnClick="btnBackToMain_Click" CssClass="btn-back-arrow">←</asp:LinkButton>
                    <h1 class="page-title" style="margin-bottom: 0;">MY EVENTS</h1>
                </div>

                <div class="manage-title-box">
                    <div class="box-left">COMMUNITY ENGAGEMENT</div>
                    <div class="box-right"><asp:Label ID="lblTopTitle" runat="server" Text="TITLE OF COMMUNITY ENGAGEMENT"></asp:Label></div>
                </div>

                <div class="manage-tabs">
                    <asp:LinkButton ID="btnTabDetails" runat="server" OnClick="btnTabDetails_Click" CssClass="tab-link active">EVENT DETAILS</asp:LinkButton>
                    <span class="tab-divider">|</span>
                    <asp:LinkButton ID="btnTabSignups" runat="server" OnClick="btnTabSignups_Click" CssClass="tab-link">VOLUNTEER SIGNUPS <span class="red-dot">•</span></asp:LinkButton>
                </div>

                <asp:Panel ID="pnlEventDetailsTab" runat="server" Visible="true">
                    <h2 class="section-heading">EVENT DETAILS</h2>
                    <div class="details-grid">
                        <div class="detail-row"><asp:LinkButton ID="editTitle" runat="server" CssClass="edit-link">EDIT</asp:LinkButton> <span class="detail-label">TITLE:</span> <asp:Label ID="lblTitle" runat="server" Text="Event Title"></asp:Label></div>
                        <div class="detail-row"><asp:LinkButton ID="editOrg" runat="server" CssClass="edit-link">EDIT</asp:LinkButton> <span class="detail-label">ORGANIZER NAME:</span> <asp:Label ID="lblOrg" runat="server" Text="Juan dela Cruz"></asp:Label></div>
                        <div class="detail-row"><asp:LinkButton ID="editAddress" runat="server" CssClass="edit-link">EDIT</asp:LinkButton> <span class="detail-label">ADDRESS:</span> <asp:Label ID="lblAddress" runat="server" Text="Brgy. Hall"></asp:Label></div>
                    </div>

                    <h2 class="section-heading">REGISTERED VOLUNTEERS</h2>
                    <asp:GridView ID="gvVolunteers" runat="server" AutoGenerateColumns="False" CssClass="volunteers-grid">
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" />
                            <asp:BoundField DataField="Name" HeaderText="NAME" />
                            <asp:BoundField DataField="Email" HeaderText="EMAIL" />
                            <asp:BoundField DataField="Contact" HeaderText="CONTACTNUM" />
                        </Columns>
                    </asp:GridView>
                    <h3 class="expected-participants">EXPECTED NUMBER OF PARTICIPANTS: 50</h3>
                </asp:Panel>

                <asp:Panel ID="pnlVolunteerSignupsTab" runat="server" Visible="false">
                    <h2 class="section-heading">VOLUNTEER SIGNUPS</h2>
                    
                    <div class="signups-list">
                        <asp:Repeater ID="rptSignups" runat="server" OnItemCommand="rptSignups_ItemCommand">
                            <ItemTemplate>
                                <div class="signup-card">
                                    <asp:Image ID="imgAvatar" runat="server" ImageUrl="~/images/profile-icon.png" CssClass="signup-avatar" />
                                    
                                    <div class="signup-info">
                                        <h3 class="signup-name"><%# Eval("Name") %></h3>
                                        <p class="signup-meta">
                                            <i>Church ID: <%# Eval("ChurchID") %></i> &nbsp;&nbsp; 
                                            <i><%# Eval("Email") %></i> &nbsp;&nbsp; 
                                            <i><%# Eval("Phone") %></i>
                                        </p>
                                    </div>
                                    
                                    <div class="signup-actions">
                                        <asp:ImageButton ID="btnAccept" runat="server" ImageUrl="~/images/accept-icon.png" CommandName="Accept" CommandArgument='<%# Eval("SignupID") %>' CssClass="action-icon" />
                                        <asp:ImageButton ID="btnReject" runat="server" ImageUrl="~/images/reject-icon.png" CommandName="Reject" CommandArgument='<%# Eval("SignupID") %>' CssClass="action-icon" />
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:Panel>

            </asp:Panel>
        </main>

        <asp:Panel ID="pnlCreateEventModal" runat="server" CssClass="modal-overlay" Visible="false">
            <div class="create-card">
                
                <div class="modal-top">
                    <h2 class="create-title">ORGANIZE EVENT <asp:Image ID="imgModalBell" runat="server" ImageUrl="~/images/bell-icon.png" CssClass="small-bell" /></h2>
                    <asp:LinkButton ID="btnCloseCreate" runat="server" OnClick="btnCloseCreate_Click" CssClass="close-x">X</asp:LinkButton>
                </div>
                <hr class="create-divider" />

                <asp:Panel ID="pnlStep1" runat="server" Visible="true">
                    <div class="form-row"><span class="form-label">EVENT TITLE *</span><asp:TextBox ID="txtTitle" runat="server" CssClass="form-input"></asp:TextBox></div>
                    <div class="form-row"><span class="form-label">ORGANIZER'S FULL NAME *</span><asp:TextBox ID="txtOrgName" runat="server" CssClass="form-input"></asp:TextBox></div>
                    <div class="form-row"><span class="form-label">VENUE NAME *</span><asp:TextBox ID="txtVenue" runat="server" CssClass="form-input"></asp:TextBox></div>
                    <div class="form-row"><span class="form-label">VENUE ADDRESS *</span><asp:TextBox ID="txtAddress" runat="server" CssClass="form-input"></asp:TextBox></div>
                    
                    <div class="form-row-split">
                        <div class="form-half"><span class="form-label">EMAIL *</span><asp:TextBox ID="txtEmail" runat="server" CssClass="form-input"></asp:TextBox></div>
                        <div class="form-half"><span class="form-label">CONTACT NUMBER</span><asp:TextBox ID="txtContact" runat="server" CssClass="form-input"></asp:TextBox></div>
                    </div>

                    <div class="modal-footer-right">
                        <asp:LinkButton ID="btnNextStep" runat="server" OnClick="btnNextStep_Click" CssClass="btn-circle-yellow">→</asp:LinkButton>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlStep2" runat="server" Visible="false">
                    <div class="form-row-split">
                        <div class="form-third"><span class="form-label">IMPLEMENTATION DATE *</span><asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-input"></asp:TextBox></div>
                        <div class="form-third"><span class="form-label">START TIME *</span><asp:TextBox ID="txtStart" runat="server" TextMode="Time" CssClass="form-input"></asp:TextBox></div>
                        <div class="form-third"><span class="form-label">END TIME *</span><asp:TextBox ID="txtEnd" runat="server" TextMode="Time" CssClass="form-input"></asp:TextBox></div>
                    </div>
                    
                    <div class="form-row"><span class="form-label">MAXIMUM VOLUNTEERS *</span><asp:TextBox ID="txtMaxVol" runat="server" CssClass="form-input"></asp:TextBox></div>
                    <div class="form-row"><span class="form-label">ADD GENERAL ANNOUNCEMENT:</span><asp:TextBox ID="txtAnnouncement" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-input"></asp:TextBox></div>

                    <div class="modal-footer-split">
                        <asp:LinkButton ID="btnPrevStep" runat="server" OnClick="btnPrevStep_Click" CssClass="btn-circle-grey">←</asp:LinkButton>
                        <asp:Button ID="btnPublish" runat="server" Text="+ PUBLISH EVENT" OnClick="btnPublish_Click" CssClass="btn-primary" />
                    </div>
                </asp:Panel>

            </div>
        </asp:Panel>

        <asp:Panel ID="pnlNotifications" runat="server" CssClass="modal-overlay" Visible="false">
            <div class="notification-card">
                
                <div class="modal-header">
                    <asp:Image ID="imgYellowBell" runat="server" ImageUrl="~/images/yellow-bell.png" CssClass="modal-bell-icon" />
                    <span class="modal-title">NOTIFICATIONS</span>
                </div>

                <div class="notification-list">
                    <div class="notif-item">
                        <asp:Image ID="imgExclam1" runat="server" ImageUrl="~/images/exclam-icon.png" CssClass="exclam-icon" /> 
                        <span>New volunteer sign-up for Coastal Cleanup!</span>
                    </div>
                    <div class="notif-item">
                        <asp:Image ID="imgExclam2" runat="server" ImageUrl="~/images/exclam-icon.png" CssClass="exclam-icon" /> 
                        <span>New inquiry regarding Community Engagement.</span>
                    </div>
                </div>

                <div class="modal-footer" style="justify-content: flex-end;">
                    <asp:LinkButton ID="btnCloseNotif" runat="server" OnClick="btnCloseNotif_Click" CssClass="modal-close">
                        <asp:Image ID="imgCloseArrow" runat="server" ImageUrl="~/images/red-arrow-icon.png" CssClass="logout-icon" /> CLOSE
                    </asp:LinkButton>
                </div>

            </div>
        </asp:Panel>

    </form>
</body>
</html>
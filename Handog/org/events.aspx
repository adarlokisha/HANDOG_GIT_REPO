<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="events.aspx.cs" Inherits="Handog.org._events" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Organize Events</title>
    <link href="../stylesheet/orgevent.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <header class="top-header">
            <div class="nav-left">
                <asp:Image ID="imgSmallLogo" runat="server" ImageUrl="~/images/HandogLogo1.png" CssClass="small-logo" />
                <a href="~/org/home.aspx" runat="server" class="nav-link">Home</a>
                <a href="~/org/request.aspx" runat="server" class="nav-link">Request</a>
                <a href="~/org/events.aspx" runat="server" class="nav-link active">Events</a>
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
                            <!-- DELETE BUTTON -->
                            <asp:LinkButton 
                                ID="btnDeleteEvent" 
                                runat="server"
                                CommandName="DeleteEvent"
                                CommandArgument='<%# Eval("EventID") %>'
                                CssClass="event-delete"
                                OnClientClick="return confirm('Are you sure you want to delete this event?');">X</asp:LinkButton>

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
                                <asp:Button 
                                    ID="btnViewDetails" 
                                    runat="server" 
                                    Text="VIEW DETAILS" 
                                    CommandName="ViewDetails" 
                                    CommandArgument='<%# Eval("EventID") %>' 
                                    CssClass="btn-view" />
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

                <asp:Panel ID="pnlEventDetailsTab" runat="server" Visible="true">
                    <div style="display: flex; justify-content: space-between; align-items: center;">
                        <h2 class="section-heading">EVENT DETAILS</h2>
                        <asp:LinkButton ID="btnEditAll" runat="server" OnClick="btnEdit_Click" CssClass="edit-link" style="font-weight: 900;">[ EDIT ALL DETAILS ]</asp:LinkButton>
                    </div>

                    <div class="details-grid">
                        <div class="detail-row"><span class="detail-label">TITLE:</span> <asp:Label ID="lblTitle" runat="server" /></div>
                        <div class="detail-row"><span class="detail-label">ORGANIZER NAME:</span> <asp:Label ID="lblOrg" runat="server" /></div>
                        <div class="detail-row"><span class="detail-label">VENUE ADDRESS:</span> <asp:Label ID="lblAddress" runat="server" /></div>
                        <div class="detail-row"><span class="detail-label">VENUE NAME:</span> <asp:Label ID="lblVenue" runat="server" /></div>
                        <div class="detail-row"><span class="detail-label">EMAIL:</span> <asp:Label ID="lblEmail" runat="server" /></div>
                        <div class="detail-row"><span class="detail-label">CONTACT NUMBER:</span> <asp:Label ID="lblContact" runat="server" /></div>
                        <div class="detail-row"><span class="detail-label">DATE:</span> <asp:Label ID="lblDate" runat="server" /></div>
                        <div class="detail-row"><span class="detail-label">START TIME:</span> <asp:Label ID="lblStart" runat="server" /></div>
                        <div class="detail-row"><span class="detail-label">END TIME:</span> <asp:Label ID="lblEnd" runat="server" /></div>
                        <div class="detail-row"><span class="detail-label">MAX VOLUNTEERS:</span> <asp:Label ID="lblMax" runat="server" /></div>
                        <div class="detail-row"><span class="detail-label">ANNOUNCEMENT:</span> <asp:Label ID="lblAnnouncement" runat="server" /></div>
                    </div>

                    <h2 class="section-heading">REGISTERED VOLUNTEERS: </h2>
                    <div class="table-container" style="border: 1px solid #333; margin-top: 10px;">
                        <asp:GridView ID="gvVolunteers" runat="server" AutoGenerateColumns="False" CssClass="volunteers-grid" GridLines="Horizontal">
                            <Columns>
                                <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="100px" />
                                <asp:BoundField DataField="Name" HeaderText="NAME" ItemStyle-Width="250px" />
                                <asp:BoundField DataField="Email" HeaderText="EMAIL" ItemStyle-Width="250px" />
                                <asp:BoundField DataField="Contact" HeaderText="PHONE" />
                                <asp:BoundField DataField="VolunteerType" HeaderText="TYPE" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    <h3 class="expected-participants" style="margin-top: 20px;">
                        EXPECTED NUMBER OF PARTICIPANTS: <asp:Label ID="lblExpectedPart" runat="server" Text="0" />
                    </h3>
                    </asp:Panel>
             </asp:Panel>      
   
        </main>

        <asp:HiddenField ID="hfEditingEventID" runat="server" />

        <asp:Panel ID="pnlEditEvent" runat="server" CssClass="modal-overlay" Visible="false">
            <div class="create-card" style="max-width: 700px; max-height: 90vh; overflow-y: auto;">
                <h2 class="create-title">EDIT EVENT DETAILS</h2>
                <hr class="create-divider" />
        
                <div class="form-row">
                    <span class="form-label">EVENT TITLE</span>
                    <asp:TextBox ID="txtEditTitle" runat="server" CssClass="form-input"></asp:TextBox>
                </div>

                <div class="form-row-split">
                    <div class="form-half">
                        <span class="form-label">VENUE NAME</span>
                        <asp:TextBox ID="txtEditVenueName" runat="server" CssClass="form-input"></asp:TextBox>
                    </div>
                    <div class="form-half">
                        <span class="form-label">VENUE ADDRESS</span>
                        <asp:TextBox ID="txtEditAddress" runat="server" CssClass="form-input"></asp:TextBox>
                    </div>
                </div>

                <div class="form-row-split">
                    <div class="form-half">
                        <span class="form-label">IMPLEMENTATION DATE</span>
                        <asp:TextBox ID="txtEditDate" runat="server" TextMode="Date" CssClass="form-input"></asp:TextBox>
                    </div>
                    <div class="form-half">
                        <span class="form-label">MAX VOLUNTEERS</span>
                        <asp:TextBox ID="txtEditMax" runat="server" CssClass="form-input" TextMode="Number"></asp:TextBox>
                    </div>
                </div>

                <div class="form-row-split">
                    <div class="form-half">
                        <span class="form-label">START TIME</span>
                        <asp:TextBox ID="txtEditStart" runat="server" TextMode="Time" CssClass="form-input"></asp:TextBox>
                    </div>
                    <div class="form-half">
                        <span class="form-label">END TIME</span>
                        <asp:TextBox ID="txtEditEnd" runat="server" TextMode="Time" CssClass="form-input"></asp:TextBox>
                    </div>
                </div>

                <div class="form-row">
                    <span class="form-label">GENERAL ANNOUNCEMENT</span>
                    <asp:TextBox ID="txtEditAnnounce" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-input"></asp:TextBox>
                </div>

                <div class="modal-footer-split">
                    <asp:LinkButton ID="btnCancelEdit" runat="server" OnClick="btnCloseEdit_Click" CssClass="btn-circle-grey" style="text-decoration:none;">X</asp:LinkButton>
                    <asp:Button ID="btnSaveEdit" runat="server" Text="SAVE CHANGES" OnClick="btnSaveEdit_Click" CssClass="btn-primary" />
                </div>
            </div>
        </asp:Panel>

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

                    <div style="display:flex; justify-content:flex-start; margin-bottom:12px;">
                        <asp:Label ID="lblCreateMsg" runat="server" ForeColor="Red" Visible="false" CssClass="form-error" />
                    </div>

                    <br />

                    <div class="modal-footer-split">
                        <asp:LinkButton ID="btnPrevStep" runat="server" OnClick="btnPrevStep_Click" CssClass="btn-circle-grey">←</asp:LinkButton>
                        <asp:Button ID="btnPublish" runat="server" Text="+ PUBLISH EVENT" OnClick="btnPublish_Click" CssClass="btn-primary" />
                    </div>
                </asp:Panel>

            </div>
        </asp:Panel>

         <!-- NOTIFICATION MODAL -->
        <asp:Panel ID="pnlNotifications" runat="server" CssClass="modal-overlay-notif" Visible="false">
            <div class="notification-card">
                <!-- Header -->
                <div class="modal-header">
                    <asp:Image ID="imgYellowBell" runat="server" ImageUrl="~/images/yellow-bell.png" CssClass="modal-bell-icon" />
                    <span class="modal-title">NOTIFICATIONS</span>
                </div>

                <!-- Notification List -->
                <div class="notification-list">
                    <div class="notif-item">
                        <asp:Image ID="imgExclam1" runat="server" ImageUrl="~/images/exclam-icon.png" CssClass="exclam-icon" />
                        <span>New request has been added!</span>
                    </div>
                    <div class="notif-item">
                        <asp:Image ID="imgExclam2" runat="server" ImageUrl="~/images/exclam-icon.png" CssClass="exclam-icon" />
                        <span>New volunteer registration!</span>
                    </div>
                    <div class="notif-item">
                        <asp:Image ID="imgExclam3" runat="server" ImageUrl="~/images/exclam-icon.png" CssClass="exclam-icon" />
                        <span>New event has been added!</span>
                    </div>
                </div>

                <!-- Footer / Close Button -->
                <div class="modal-footer">
                    <asp:LinkButton ID="btnCloseNotif" runat="server" OnClick="btnCloseNotif_Click" CssClass="modal-close">
                        <asp:Image ID="imgCloseArrow" runat="server" ImageUrl="~/images/red-arrow-icon.png" CssClass="logout-icon" /> CLOSE
                    </asp:LinkButton>
                </div>
            </div>
        </asp:Panel>

    </form>
</body>
</html>
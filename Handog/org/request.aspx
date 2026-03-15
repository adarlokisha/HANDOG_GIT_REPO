<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="request.aspx.cs" Inherits="Handog.org.request" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Organizer Requests</title>
    <link href="~/stylesheet/orgrequest.css?v=1" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <header class="top-header">
            <div class="nav-left">
                <asp:Image ID="imgSmallLogo" runat="server" ImageUrl="~/images/HandogLogo1.png" AlternateText="Handog Logo" CssClass="small-logo" />
                <a href="~/org/home.aspx" runat="server" class="nav-link">Home</a>
                <a href="~/org/request.aspx" runat="server" class="nav-link active">Request</a>
                <a href="~/org/events.aspx" runat="server" class="nav-link">Events</a>
            </div>
            <div class="nav-right">
                <asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click" CssClass="nav-link logout-link">LOGOUT</asp:LinkButton>
            </div>
        </header>

        <main class="main-content">
            
            <div class="bell-container">
                <asp:LinkButton ID="btnBell" runat="server" OnClick="btnBell_Click">
                    <asp:Image ID="imgBell" runat="server" ImageUrl="~/images/bell-icon.png" AlternateText="Notifications" CssClass="bell-icon" />
                </asp:LinkButton>
            </div>

            <div class="page-header">
                <div class="title-area">
                    <h1 class="page-title">We are in need of help!</h1>
                    <p class="page-subtitle">These are the requests currently sent by your volunteers and requesters.</p>
                </div>
            </div>

            <hr class="title-divider" />
            <h3 class="pending-label">PENDING REQUESTS</h3>

            <div class="cards-scroll-container">
                <asp:Repeater ID="rptRequests" runat="server" OnItemCommand="rptRequests_ItemCommand">
                    <ItemTemplate>
            
                        <div class="request-card">
                            <p class="requestor-name"><%# Eval("RequestorName") %></p>
                            <hr class="card-divider" />
                
                            <p class="request-text">
                                <span class="bold-label"><%# Eval("RequestType").ToString().ToUpper() %>:</span> 
                                <br />
                                <%# Eval("MessageText") %>
                            </p>

                            <div class="action-buttons-container">
                                <asp:ImageButton ID="btnAccept" runat="server" ImageUrl="~/images/accept-icon.png" CommandName="Accept" CommandArgument='<%# Eval("RequestID") %>' CssClass="action-icon" AlternateText="Accept" />
                                <asp:ImageButton ID="btnReject" runat="server" ImageUrl="~/images/red-cross.png" CommandName="Reject" CommandArgument='<%# Eval("RequestID") %>' CssClass="action-icon" AlternateText="Reject" />
                            </div>
                        </div>

                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </main>

        <asp:HiddenField ID="hfAcceptedRequestID" runat="server" />

        <asp:Panel ID="pnlCreateEventModal" runat="server" CssClass="modal-overlay" Visible="false">
            <div class="create-card">
                
                <div class="modal-top">
                    <h2 class="create-title">ORGANIZE EVENT <asp:Image ID="imgCreateBell" runat="server" ImageUrl="~/images/bell-icon.png" CssClass="small-bell" /></h2>
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

        <asp:Panel ID="pnlNotifications" runat="server" CssClass="modal-overlay" Visible="false">
            <div class="notification-card">
                <div class="modal-header">
                    <asp:Image ID="imgYellowBell" runat="server" ImageUrl="~/images/yellow-bell.png" CssClass="modal-bell-icon" />
                    <span class="modal-title">NOTIFICATIONS</span>
                </div>
                <div class="notification-list">
                    <div class="notif-item">
                        <asp:Image ID="imgExclam1" runat="server" ImageUrl="~/images/exclam-icon.png" CssClass="exclam-icon" /> 
                        <span>New request has been added!</span>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnCloseNotif" runat="server" OnClick="btnCloseNotif_Click" CssClass="modal-close">
                        <asp:Image ID="imgCloseArrow" runat="server" ImageUrl="~/images/red-cross.png" CssClass="logout-icon" /> CLOSE
                    </asp:LinkButton>
                </div>
            </div>
        </asp:Panel>

    </form>
</body>
</html>
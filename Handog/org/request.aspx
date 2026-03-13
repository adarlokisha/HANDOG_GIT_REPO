<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="request.aspx.cs" Inherits="Handog.org.request" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Organizer Requests</title>
    <link href="~/stylesheet/orgrequest.css" rel="stylesheet" type="text/css" />
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
                                <span class="bold-label">REASON FOR REQUEST:</span> 
                                <%# Eval("MessageText") %>
                            </p>

                            <div class="action-buttons-container">
                                <asp:ImageButton ID="btnAccept" runat="server" ImageUrl="~/images/accept-icon.png" CommandName="Accept" CommandArgument='<%# Eval("RequestID") %>' CssClass="action-icon" AlternateText="Accept" />
                                <asp:ImageButton ID="btnReject" runat="server" ImageUrl="~/images/reject-icon.png" CommandName="Reject" CommandArgument='<%# Eval("RequestID") %>' CssClass="action-icon" AlternateText="Reject" />
                            </div>
                        </div>

        </ItemTemplate>
    </asp:Repeater>
</div>
        </main>

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
                        <asp:Image ID="imgCloseArrow" runat="server" ImageUrl="~/images/red-arrow-icon.png" CssClass="logout-icon" /> CLOSE
                    </asp:LinkButton>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlAnswerModal" runat="server" CssClass="modal-overlay" Visible="false">
            <div class="notification-card"> <div class="modal-header">
                    <span class="modal-title">ANSWER INQUIRY</span>
                </div>
                
                <div class="answer-body">
                    <p class="answer-instructions">Type your response to the user below:</p>
                    <asp:TextBox ID="txtAnswerBody" runat="server" TextMode="MultiLine" CssClass="answer-textbox" Rows="6"></asp:TextBox>
                    
                    <asp:HiddenField ID="hfCurrentRequestID" runat="server" />
                </div>
            </div>
        </asp:Panel>

    </form>
</body>
</html>
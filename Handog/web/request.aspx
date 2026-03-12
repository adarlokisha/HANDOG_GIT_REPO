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
                <asp:LinkButton ID="btnLogout" runat="server" onClick="btnLogout_Click" CssClass="nav-link logout-link">LOGOUT</asp:LinkButton>
            </div>
        </header>

        <main class="main-content">
            <div class="page-header">
                <div class="title-area">
                    <h1 class="page-title">We are in need of help!</h1>
                    <p class="page-subtitle">Request for a Community Engagement to be held in your area:</p>
                </div>
                <div class="icon-area">
                    <asp:LinkButton ID="btnBell" runat="server" OnClick="btnBell_Click">
                        <asp:Image ID="imgBell" runat="server" ImageUrl="~/images/bell-icon.png" AlternateText="Notifications" CssClass="bell-icon" />
                    </asp:LinkButton>
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

            <div class="button-container">
                <asp:LinkButton ID="btnAddRequestTrigger" runat="server" OnClick="btnAddRequest_Click" CssClass="btn-add">+ ADD REQUEST</asp:LinkButton>
            </div>
        </main>

        <asp:Panel ID="pnlAddRequest" runat="server" CssClass="modal-overlay" Visible="false">
            <div class="request-modal-card">
                <div class="modal-close-header">
                    <asp:LinkButton ID="btnCancelRequest" runat="server" OnClick="btnCancelRequest_Click" CssClass="close-x">✕</asp:LinkButton>
                </div>

                <div class="form-group">
                    <label class="form-question">IS THIS REQUEST AN INQUIRY OR A REQUEST FOR ASSISTANCE?</label>
                    <div class="radio-group">
                        <asp:RadioButtonList ID="rblRequestType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblRequestType_SelectedIndexChanged" CssClass="custom-radio-list">
                            <asp:ListItem Text="INQUIRY" Value="Inquiry"></asp:ListItem>
                            <asp:ListItem Text="ASSISTANCE" Value="Assistance"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>

                <asp:PlaceHolder ID="phFormFields" runat="server" Visible="false">
                    <div class="form-group">
                        <asp:Label ID="lblSubject" runat="server" CssClass="form-label" Text="INQUIRY SUBJECT*"></asp:Label>
                        <asp:TextBox ID="txtSubject" runat="server" CssClass="form-input"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblDetails" runat="server" CssClass="form-label" Text="INQUIRY DETAILS*"></asp:Label>
                        <asp:TextBox ID="txtDetails" runat="server" TextMode="MultiLine" Rows="6" CssClass="form-textarea"></asp:TextBox>
                    </div>

                    <div class="modal-action-container">
                        <asp:Button ID="btnPostRequest" runat="server" Text="+ POST INQUIRY" CssClass="btn-post" />
                    </div>
                </asp:PlaceHolder>
            </div>
        </asp:Panel>
               

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

        <asp:Panel ID="pnlRegistration" runat="server" CssClass="modal-overlay" Visible="false">
            <div class="registration-card">
                <div class="modal-header-row">
                    <asp:LinkButton ID="btnBackReg" runat="server" OnClick="btnCloseModals_Click" CssClass="back-button">
                        <asp:Image ID="imgBack" runat="server" ImageUrl="~/images/back-icon.png" CssClass="back-icon-img" />
                    </asp:LinkButton>
                    <h1 class="modal-main-title">REGISTRATION</h1>
                </div>
                <p class="sub-title">COMMUNITY ENGAGEMENT TITLE</p>
                <hr class="divider" />

                <div class="registration-content">
                    <div class="event-details-text">
                        <h3>EVENT DETAILS</h3>
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

                    <div class="registration-form-box">
                        <label>REGISTER AS *</label>
                        <div class="radio-group">
                            <asp:RadioButton ID="rbVolunteer" runat="server" GroupName="RegType" Text="VOLUNTEER" />
                            <asp:RadioButton ID="rbParticipant" runat="server" GroupName="RegType" Text="PARTICIPANT" />
                        </div>

                        <label>FULL NAME *</label>
                        <asp:TextBox ID="txtFullName" runat="server" CssClass="form-input"></asp:TextBox>

                        <div class="form-row">
                            <div class="form-group">
                                <label>EMAIL *</label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>CONTACT NUMBER *</label>
                                <asp:TextBox ID="txtContact" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>

                        <label>I AGREE TO THE <a href="#">TERMS AND CONDITIONS</a> *</label>
                        <div class="radio-group">
                            <asp:RadioButton ID="rbYes" runat="server" GroupName="Terms" Text="YES" />
                            <asp:RadioButton ID="rbNo" runat="server" GroupName="Terms" Text="NO" />
                        </div>

                        <asp:Button ID="btnSubmitRegistration" runat="server" Text="+ REGISTER" CssClass="btn-action reg-submit" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlEventDetails" runat="server" CssClass="modal-overlay" Visible="false">
            <div class="details-card">
                 <div class="modal-header-row">
                    <asp:LinkButton ID="btnBackDetails" runat="server" OnClick="btnCloseModals_Click" CssClass="back-button">
                        <asp:Image ID="imgBack2" runat="server" ImageUrl="~/images/back-icon.png" CssClass="back-icon-img" />
                    </asp:LinkButton>
                    <h1 class="modal-main-title">EVENTS</h1>
                </div>
                <div class="details-inner-content">
                     <div class="details-header-yellow">
                         <span class="category-tag">COMMUNITY ENGAGEMENT</span>
                         <span class="engagement-title">TITLE OF COMMUNITY ENGAGEMENT</span>
                     </div>
                     <div class="details-body">
                         <h3>EVENT DETAILS</h3>
                         <p><strong>TITLE:</strong> Event Title</p>
                         <p><strong>ORGANIZER NAME:</strong> Juan dela Cruz</p>
                         <p><strong>DATE:</strong> January 31, 2026</p>
                         <p><strong>ANNOUNCEMENT:</strong> Lorem ipsum dolor sit amet, consectetur adipiscing elit...</p>
                     </div>
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
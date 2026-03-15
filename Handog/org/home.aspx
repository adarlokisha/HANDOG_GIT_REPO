<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="Handog.org.home" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Handog - Organizer Home</title>
    <link href="../stylesheet/orghome.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <header class="top-header">
            <div class="nav-left">
                <asp:Image ID="imgSmallLogo" runat="server" ImageUrl="~/images/HandogLogo1.png" AlternateText="Handog Logo" CssClass="small-logo" />
                <a href="~/org/home.aspx" runat="server" class="nav-link active">Home</a>
                <a href="~/org/request.aspx" runat="server" class="nav-link">Request</a>
                <a href="~/org/events.aspx" runat="server" class="nav-link">Events</a>
            </div>
            <div class="nav-right">
                <asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click" CssClass="nav-link logout-link">LOGOUT</asp:LinkButton>
            </div>
        </header>

        <!-- MAIN CONTENT -->
        <main>
            <section class="hero-section">
                <div class="bell-container">
                    <asp:LinkButton ID="btnBell" runat="server" OnClick="btnBell_Click">
                        <asp:Image ID="imgBell" runat="server" ImageUrl="~/images/bell-icon.png" CssClass="bell-icon" />
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

            <!-- LOCALE GRID -->
            <section class="locales-section">
                <h2 class="section-title">List of Locales</h2>

                <div class="entry-row">
                    <span class="input-label">Locale:</span> <asp:TextBox ID="txtLocale" runat="server" CssClass="input-box"></asp:TextBox>
                    <span class="input-label">City:</span> <asp:TextBox ID="txtCity" runat="server" CssClass="input-box"></asp:TextBox>
                    <span class="input-label">Barangay:</span> <asp:TextBox ID="txtBarangay" runat="server" CssClass="input-box"></asp:TextBox>
                    <asp:Button ID="btnAddLocale" runat="server" Text="+ ADD LOCALE" CssClass="btn-add-blue" OnClick="btnAddLocale_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="CLEAR" CssClass="btn-clear-blue" OnClick="btnClear_Click" />
                </div>

                <div class="table-container">
                    <asp:GridView ID="gvLocales" runat="server" AutoGenerateColumns="False" 
                                  CssClass="locales-table" GridLines="None" 
                                  DataKeyNames="Locale_ID" 
                                  OnRowCommand="gvLocales_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="LocaleName" HeaderText="LOCALE" HeaderStyle-CssClass="grid-header" />
                            <asp:BoundField DataField="City" HeaderText="CITY" HeaderStyle-CssClass="grid-header" />
                            <asp:BoundField DataField="Barangay" HeaderText="BARANGAY" HeaderStyle-CssClass="grid-header" />
                            <asp:TemplateField HeaderText="ACTIONS" HeaderStyle-CssClass="grid-header">
                                <ItemTemplate>
                                    <div class="action-links-container">
                                        <asp:LinkButton ID="btnEdit" runat="server" Text="EDIT" 
                                            CommandName="EditLocale" CommandArgument='<%# Eval("Locale_ID") %>' 
                                            Visible='<%# IsOwner(Eval("AccountNum")) %>' CssClass="action-link-item" />
            
                                        <asp:Literal ID="litSeparator" runat="server" Text=" | " 
                                            Visible='<%# IsOwner(Eval("AccountNum")) %>'></asp:Literal>

                                        <asp:LinkButton ID="btnDelete" runat="server" Text="DELETE" 
                                            CommandName="DeleteLocale" CommandArgument='<%# Eval("Locale_ID") %>' 
                                            OnClientClick="return confirm('Are you sure?');" 
                                            Visible='<%# IsOwner(Eval("AccountNum")) %>' CssClass="action-link-item" />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </section>

            <!-- EDIT LOCALE -->
            <asp:Panel ID="pnlEditLocaleModal" runat="server" CssClass="modal-overlay" Visible="false">
                <div class="edit-modal-card">
                    <h2 class="modal-title-yellow">EDIT LOCALE DETAILS</h2>
                    <hr />

                    <div class="modal-body">
                        <span class="input-label-modal">Locale Name:</span>
                        <asp:TextBox ID="txtEditLocaleName" runat="server" CssClass="modal-input"></asp:TextBox>
            
                        <span class="input-label-modal">City:</span>
                        <asp:TextBox ID="txtEditCity" runat="server" CssClass="modal-input"></asp:TextBox>
            
                        <span class="input-label-modal">Barangay:</span>
                        <asp:TextBox ID="txtEditBarangay" runat="server" CssClass="modal-input"></asp:TextBox>
                    </div>

                    <div class="modal-footer-btns">
                        <asp:LinkButton ID="btnCancelLocaleEdit" runat="server" OnClick="btnCancelLocaleEdit_Click" CssClass="btn-close-circle">X</asp:LinkButton>
            
                        <asp:HiddenField ID="hfSelectedLocaleID" runat="server" />
            
                        <asp:Button ID="btnUpdateLocale" runat="server" Text="SAVE CHANGES" CssClass="btn-save-yellow" OnClick="btnUpdateLocale_Click" />
                    </div>
                </div>
            </asp:Panel>

            <!-- STORY SECTION -->
            <section class="story-section">
                <h2 class="story-title">Our Story</h2>
                <div class="story-card">
                    <asp:Image ID="imgStoryLogo" runat="server" ImageUrl="~/images/HandogBig1.png" AlternateText="Handog Logo" CssClass="story-logo" />
                    <p>Our platform was created with one simple goal — to make community engagement easier, more organized, and more accessible for everyone. We saw the gap between people who want to serve, those who need support, and organizers who are willing to lead initiatives, but lack a centralized system to connect them all.</p>
                </div>
            </section>
        </main>

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
                        <asp:Image ID="imgCloseArrow" runat="server" ImageUrl="~/images/red-cross.png" CssClass="logout-icon" /> CLOSE
                    </asp:LinkButton>
                </div>
            </div>
        </asp:Panel>

    </form>
</body>
</html>
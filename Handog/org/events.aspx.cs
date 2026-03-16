using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Handog.org
{
    public partial class _events : System.Web.UI.Page
    {
        private string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure user is logged in
            if (Session["AccountNum"] == null)
            {
                Response.Redirect("~/web/default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindMyEvents();
            }
        }

        // ==============================
        // 1. MAIN EVENTS BINDING
        // ==============================
        private void BindMyEvents()
        {
            int accountNum = Convert.ToInt32(Session["AccountNum"]);

            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT PublishedEventNum as EventID, 
                       EventTitle, 
                       EventAddress, 
                       Venue,
                       ImplementationDate as EventDate,
                       (FORMAT(EventStartTime, 'hh:mm tt') + ' - ' + FORMAT(EventEndTime, 'hh:mm tt')) as TimeStr,
                       VolunteerCapacity,
                       Announcement as Description,
                       1 as IsOwner
                FROM PublishedEvent
                WHERE AccountNum = @accNum
                ORDER BY ImplementationDate DESC", conn))
            {
                cmd.Parameters.AddWithValue("@accNum", accountNum);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptEvents.DataSource = dt;
                rptEvents.DataBind();
            }
        }

        private void BindAllEvents()
        {
            int accountNum = Convert.ToInt32(Session["AccountNum"]);

            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT PublishedEventNum as EventID,
                       EventTitle,
                       EventAddress,
                       Venue,
                       ImplementationDate as EventDate,
                       (FORMAT(EventStartTime, 'hh:mm tt') + ' - ' + FORMAT(EventEndTime, 'hh:mm tt')) as TimeStr,
                        VolunteerCapacity,
                       Announcement as Description,
                       CASE WHEN AccountNum = @accNum THEN 1 ELSE 0 END as IsOwner
                FROM PublishedEvent
                ORDER BY ImplementationDate DESC", conn))
            {
                cmd.Parameters.AddWithValue("@accNum", accountNum);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptEvents.DataSource = dt;
                rptEvents.DataBind();
            }
        }

        protected void btnAllEvents_Click(object sender, EventArgs e)
        {
            BindAllEvents();
            btnAllEvents.CssClass = "tab-link active";
            btnMyEvents.CssClass = "tab-link";
        }

        protected void btnMyEvents_Click(object sender, EventArgs e)
        {
            BindMyEvents();
            btnMyEvents.CssClass = "tab-link active";
            btnAllEvents.CssClass = "tab-link";
        }

        // ==============================
        // SEARCH FUNCTIONALITY
        // ==============================
        private void PerformSearch()
        {
            int accountNum = Convert.ToInt32(Session["AccountNum"]);
            string keyword = txtSearch.Text.Trim();
            bool isMyEventsTab = btnMyEvents.CssClass.Contains("active");

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT PublishedEventNum as EventID, 
                           EventTitle, EventAddress, Venue,
                           ImplementationDate as EventDate,
                           (FORMAT(EventStartTime, 'hh:mm tt') + ' - ' + FORMAT(EventEndTime, 'hh:mm tt')) as TimeStr,
                           Announcement as Description,
                           CASE WHEN AccountNum = @accNum THEN 1 ELSE 0 END as IsOwner
                    FROM PublishedEvent
                    WHERE EventTitle LIKE @keyword";

                if (isMyEventsTab)
                    query += " AND AccountNum = @accNum";

                query += " ORDER BY ImplementationDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@accNum", accountNum);
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptEvents.DataSource = dt;
                    rptEvents.DataBind();
                }
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e) => PerformSearch();
        protected void btnSearchIcon_Click(object sender, ImageClickEventArgs e) => PerformSearch();

        // ==============================
        // EVENT MANAGEMENT - Load / Delete
        // ==============================
        protected void rptEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int accountNum = Convert.ToInt32(Session["AccountNum"]);

            if (e.CommandName == "ViewDetails")
            {
                string eventID = e.CommandArgument.ToString();
                ViewState["SelectedEventID"] = eventID;

                pnlMainEvents.Visible = false;
                pnlManageEvent.Visible = true;

                LoadEventManagementData(eventID);
            }
            else if (e.CommandName == "DeleteEvent")
            {
                string eventID = e.CommandArgument.ToString();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string deleteVolunteers = "DELETE FROM EventVolunteers WHERE PublishedEventNum = @id";
                    using (SqlCommand cmdVol = new SqlCommand(deleteVolunteers, conn))
                    {
                        cmdVol.Parameters.AddWithValue("@id", eventID);
                        cmdVol.ExecuteNonQuery();
                    }

                    string deleteEvent = "DELETE FROM PublishedEvent WHERE PublishedEventNum = @id AND AccountNum = @accNum";
                    using (SqlCommand cmdEvent = new SqlCommand(deleteEvent, conn))
                    {
                        cmdEvent.Parameters.AddWithValue("@id", eventID);
                        cmdEvent.Parameters.AddWithValue("@accNum", accountNum);
                        cmdEvent.ExecuteNonQuery();
                    }
                }

                BindMyEvents();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Event deleted successfully!');", true);
            }
        }

        private void LoadEventManagementData(string eventID)
        {
            int accountNum = Convert.ToInt32(Session["AccountNum"]);

            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT E.*, 
                       (A.Lastname + ', ' + A.Firstname) as OrganizerName,
                       A.Email as OrgEmail, A.ContactNum as OrgPhone,
                       (SELECT COUNT(*) FROM EventVolunteers WHERE PublishedEventNum = @id AND Is_Accepted = 1) as AcceptedCount,
                       CASE WHEN E.AccountNum = @accNum THEN 1 ELSE 0 END as CanEdit
                FROM PublishedEvent E
                INNER JOIN Account A ON E.AccountNum = A.AccountNum
                WHERE PublishedEventNum = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", eventID);
                cmd.Parameters.AddWithValue("@accNum", accountNum);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblTopTitle.Text = reader["EventTitle"].ToString().ToUpper();
                        lblTitle.Text = reader["EventTitle"].ToString();
                        lblOrg.Text = reader["OrganizerName"].ToString();
                        lblVenue.Text = reader["Venue"].ToString();
                        lblAddress.Text = reader["EventAddress"].ToString();
                        lblEmail.Text = reader["OrgEmail"].ToString();
                        lblContact.Text = reader["OrgPhone"].ToString();
                        lblDate.Text = Convert.ToDateTime(reader["ImplementationDate"]).ToString("MMMM dd, yyyy");
                        lblStart.Text = Convert.ToDateTime(reader["EventStartTime"]).ToString("t");
                        lblEnd.Text = Convert.ToDateTime(reader["EventEndTime"]).ToString("t");
                        lblMax.Text = reader["VolunteerCapacity"].ToString();
                        lblAnnouncement.Text = reader["Announcement"].ToString();
                        lblExpectedPart.Text = reader["AcceptedCount"].ToString();

                        btnEditAll.Visible = Convert.ToInt32(reader["CanEdit"]) == 1;
                    }
                }
            }

            LoadVolunteerGrid(eventID);
        }

        private void LoadVolunteerGrid(string eventID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT A.AccountNum as ID, (A.Firstname + ' ' + A.Lastname) as Name,
                       A.Email, A.ContactNum as Contact, EV.VolunteerType
                FROM EventVolunteers EV
                INNER JOIN Account A ON EV.AccountNum = A.AccountNum
                WHERE EV.PublishedEventNum = @id AND EV.Is_Accepted = 1", conn))
            {
                cmd.Parameters.AddWithValue("@id", eventID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvVolunteers.DataSource = dt;
                gvVolunteers.DataBind();
            }
        }
        // ==============================
        // 3. CREATE EVENT MODAL
        // ==============================
        protected void btnOpenCreateModal_Click(object sender, EventArgs e)
        {
            pnlCreateEventModal.Visible = true;
            pnlStep1.Visible = true;
            pnlStep2.Visible = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT Lastname, Firstname, Email, ContactNum
                    FROM Account
                    WHERE AccountNum = @accountNum", conn))
                {
                    cmd.Parameters.AddWithValue("@accountNum", Session["AccountNum"]);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtOrgName.Text = $"{reader["Lastname"]}, {reader["Firstname"]}";
                            txtEmail.Text = reader["Email"].ToString();
                            txtContact.Text = reader["ContactNum"].ToString();

                            txtOrgName.ReadOnly = true;
                            txtEmail.ReadOnly = true;
                            txtContact.ReadOnly = true;
                        }
                    }
                }
            }
            catch
            {
                lblCreateMsg.Text = "Unable to load organizer info.";
                lblCreateMsg.Visible = true;
            }
        }

        protected void btnCloseCreate_Click(object sender, EventArgs e) => pnlCreateEventModal.Visible = false;
        protected void btnNextStep_Click(object sender, EventArgs e) { pnlStep1.Visible = false; pnlStep2.Visible = true; }
        protected void btnPrevStep_Click(object sender, EventArgs e) { pnlStep1.Visible = true; pnlStep2.Visible = false; }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            lblCreateMsg.Visible = false;

            string title = txtTitle.Text.Trim();
            string venue = txtVenue.Text.Trim();
            string address = txtAddress.Text.Trim();
            string note = txtAnnouncement.Text.Trim();
            string maxStr = txtMaxVol.Text.Trim();
            string dateStr = txtDate.Text.Trim();
            string startStr = txtStart.Text.Trim();
            string endStr = txtEnd.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(venue) || string.IsNullOrEmpty(address) ||
                string.IsNullOrEmpty(dateStr) || string.IsNullOrEmpty(startStr) || string.IsNullOrEmpty(endStr) ||
                string.IsNullOrEmpty(maxStr))
            {
                lblCreateMsg.Text = "Please fill in all required fields.";
                lblCreateMsg.Visible = true;
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;
                return;
            }

            if (!DateTime.TryParse(dateStr, out DateTime implDate))
            {
                lblCreateMsg.Text = "Implementation date is invalid.";
                lblCreateMsg.Visible = true;
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;
                return;
            }

            if (!TimeSpan.TryParse(startStr, out TimeSpan startTime))
                startTime = DateTime.TryParse(startStr, out DateTime tmpStart) ? tmpStart.TimeOfDay : TimeSpan.Zero;

            if (!TimeSpan.TryParse(endStr, out TimeSpan endTime))
                endTime = DateTime.TryParse(endStr, out DateTime tmpEnd) ? tmpEnd.TimeOfDay : TimeSpan.Zero;

            if (!int.TryParse(maxStr, out int maxVol))
            {
                lblCreateMsg.Text = "Maximum volunteers must be a number.";
                lblCreateMsg.Visible = true;
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    int accountNum = Convert.ToInt32(Session["AccountNum"]);

                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO PublishedEvent
                        (AccountNum, EventTitle, Venue, EventAddress, ImplementationDate, EventStartTime, EventEndTime, VolunteerCapacity, Announcement)
                        VALUES
                        (@accountNum, @title, @venue, @address, @date, @start, @end, @max, @note)", conn))
                    {
                        cmd.Parameters.AddWithValue("@accountNum", accountNum);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@venue", venue);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.Add("@date", SqlDbType.Date).Value = implDate.Date;
                        cmd.Parameters.Add("@start", SqlDbType.Time).Value = startTime;
                        cmd.Parameters.Add("@end", SqlDbType.Time).Value = endTime;
                        cmd.Parameters.Add("@max", SqlDbType.Int).Value = maxVol;
                        cmd.Parameters.AddWithValue("@note", string.IsNullOrWhiteSpace(note) ? DBNull.Value : (object)note);

                        cmd.ExecuteNonQuery();
                    }
                }

                pnlCreateEventModal.Visible = false;
                BindMyEvents();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Event published successfully!');", true);
            }
            catch (Exception ex)
            {
                lblCreateMsg.Text = "Database error: " + ex.Message;
                lblCreateMsg.Visible = true;
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;
            }
        }

        // ==============================
        // 4. EDIT / SAVE EVENT
        // ==============================
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedEventID"] == null) return;

            hfEditingEventID.Value = ViewState["SelectedEventID"].ToString();

            txtEditTitle.Text = lblTitle.Text;
            txtEditVenueName.Text = lblVenue.Text;
            txtEditAddress.Text = lblAddress.Text;
            txtEditMax.Text = lblMax.Text;
            txtEditAnnounce.Text = lblAnnouncement.Text;

            txtEditDate.Text = DateTime.TryParse(lblDate.Text, out DateTime d) ? d.ToString("yyyy-MM-dd") : "";
            txtEditStart.Text = DateTime.TryParse(lblStart.Text, out DateTime s) ? s.ToString("HH:mm") : "";
            txtEditEnd.Text = DateTime.TryParse(lblEnd.Text, out DateTime eT) ? eT.ToString("HH:mm") : "";

            pnlEditEvent.Visible = true;
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfEditingEventID.Value)) return;

            if (!DateTime.TryParse(txtEditDate.Text, out DateTime implDate) ||
                !TimeSpan.TryParse(txtEditStart.Text, out TimeSpan startTime) ||
                !TimeSpan.TryParse(txtEditEnd.Text, out TimeSpan endTime) ||
                !int.TryParse(txtEditMax.Text, out int maxVol))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid date/time/volunteer count.');", true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE PublishedEvent
                    SET EventTitle = @title,
                        Venue = @venueName,
                        EventAddress = @address,
                        ImplementationDate = @date,
                        EventStartTime = @start,
                        EventEndTime = @end,
                        VolunteerCapacity = @max,
                        Announcement = @announce
                    WHERE PublishedEventNum = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@title", txtEditTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@venueName", txtEditVenueName.Text.Trim());
                    cmd.Parameters.AddWithValue("@address", txtEditAddress.Text.Trim());
                    cmd.Parameters.Add("@date", SqlDbType.Date).Value = implDate.Date;
                    cmd.Parameters.Add("@start", SqlDbType.Time).Value = startTime;
                    cmd.Parameters.Add("@end", SqlDbType.Time).Value = endTime;
                    cmd.Parameters.Add("@max", SqlDbType.Int).Value = maxVol;
                    cmd.Parameters.AddWithValue("@announce", string.IsNullOrWhiteSpace(txtEditAnnounce.Text) ? DBNull.Value : (object)txtEditAnnounce.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", hfEditingEventID.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                pnlEditEvent.Visible = false;
                LoadEventManagementData(hfEditingEventID.Value);
                BindMyEvents();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Event updated successfully!');", true);
            }
            catch
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error updating event.');", true);
            }
        }

        protected void btnCloseEdit_Click(object sender, EventArgs e) => pnlEditEvent.Visible = false;

        protected void btnBackToMain_Click(object sender, EventArgs e)
        {
            pnlMainEvents.Visible = true;
            pnlManageEvent.Visible = false;
            BindMyEvents();
        }

        // ==============================
        // 5. NOTIFICATIONS / LOGOUT
        // ==============================
        protected void btnBell_Click(object sender, EventArgs e) => pnlNotifications.Visible = true;
        protected void btnCloseNotif_Click(object sender, EventArgs e) => pnlNotifications.Visible = false;

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/web/default.aspx");
        }
    }
}
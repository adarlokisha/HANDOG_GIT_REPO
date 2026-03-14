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
        string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null)
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
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT PublishedEventNum as EventID, EventTitle, EventAddress, Venue,
                           ImplementationDate as EventDate,
                           (FORMAT(EventStartTime, 'hh:mm tt') + ' - ' + FORMAT(EventEndTime, 'hh:mm tt')) as TimeStr,
                           Announcement as Description
                    FROM PublishedEvent
                    WHERE AccountNum = (SELECT AccountNum FROM Account WHERE Account_ID = @accID)";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@accID", Session["AccountID"]);
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptEvents.DataSource = dt;
                rptEvents.DataBind();
            }
        }

        protected void rptEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
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

                // Delete the event and all its registered volunteers
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // 1. Delete registered volunteers first
                    string deleteVolunteers = "DELETE FROM EventVolunteers WHERE PublishedEventNum = @id";
                    using (SqlCommand cmdVol = new SqlCommand(deleteVolunteers, conn))
                    {
                        cmdVol.Parameters.AddWithValue("@id", eventID);
                        cmdVol.ExecuteNonQuery();
                    }

                    // 2. Delete the event itself
                    string deleteEvent = "DELETE FROM PublishedEvent WHERE PublishedEventNum = @id";
                    using (SqlCommand cmdEvent = new SqlCommand(deleteEvent, conn))
                    {
                        cmdEvent.Parameters.AddWithValue("@id", eventID);
                        cmdEvent.ExecuteNonQuery();
                    }
                }

                // Refresh the Repeater
                BindMyEvents();

                // Optional: show alert
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Event deleted successfully!');", true);
            }
        }

        // ==============================
        // 2. MANAGE & EDIT EVENT
        // ==============================
        private void LoadEventManagementData(string eventID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    SELECT E.*, Venue, EventAddress,
                           (A.Firstname + ' ' + A.Lastname) as OrganizerName,
                           A.Email as OrgEmail, A.ContactNum as OrgPhone
                    FROM PublishedEvent E
                    INNER JOIN Account A ON E.AccountNum = A.AccountNum
                    WHERE PublishedEventNum = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", eventID);
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
                    }
                }
            }

            LoadVolunteerGrid(eventID);
        }

        private void LoadVolunteerGrid(string eventID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                SELECT A.AccountNum as ID, (A.Firstname + ' ' + A.Lastname) as Name,
                       A.Email, A.ContactNum as Contact, EV.VolunteerType
                FROM EventVolunteers EV
                INNER JOIN Account A ON EV.AccountNum = A.AccountNum
                WHERE EV.PublishedEventNum = @id AND EV.Is_Accepted = 1";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@id", eventID);
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
            ClearCreateEventFields();

            // Pre-fill organizer info from the logged-in account and make fields read-only
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string userQuery = "SELECT Firstname, Lastname, Email, ContactNum FROM Account WHERE Account_ID = @accID";
                    using (SqlCommand cmd = new SqlCommand(userQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@accID", Session["AccountID"]);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtOrgName.Text = reader["Firstname"].ToString() + " " + reader["Lastname"].ToString();
                                txtEmail.Text = reader["Email"].ToString();
                                txtContact.Text = reader["ContactNum"].ToString();

                                // Make inputs uneditable but still submitted with the form
                                txtOrgName.ReadOnly = true;
                                txtEmail.ReadOnly = true;
                                txtContact.ReadOnly = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Escape single quotes in the message to prevent JS errors
                string safeMessage = ex.Message.Replace("'", "");
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Unable to load organizer info: " + safeMessage + "');", true);
            }
            pnlStep1.Visible = true;
            pnlStep2.Visible = false;
            pnlCreateEventModal.Visible = true;
        }

        protected void btnCloseCreate_Click(object sender, EventArgs e)
        {
            pnlCreateEventModal.Visible = false;
        }

        protected void btnNextStep_Click(object sender, EventArgs e)
        {
            pnlStep1.Visible = false;
            pnlStep2.Visible = true;
        }
        protected void btnPrevStep_Click(object sender, EventArgs e)
        {
            pnlStep1.Visible = true;
            pnlStep2.Visible = false;
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            // Clear previous message
            lblCreateMsg.Text = "";
            lblCreateMsg.Visible = false;

            // Gather inputs
            string title = txtTitle.Text.Trim();
            string venue = txtVenue.Text.Trim();
            string address = txtAddress.Text.Trim();
            string note = txtAnnouncement.Text.Trim();
            string maxStr = txtMaxVol.Text.Trim();
            string dateStr = txtDate.Text.Trim();
            string startStr = txtStart.Text.Trim();
            string endStr = txtEnd.Text.Trim();

            // Basic validation
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(venue) || string.IsNullOrEmpty(address) ||
                string.IsNullOrEmpty(dateStr) || string.IsNullOrEmpty(startStr) || string.IsNullOrEmpty(endStr) ||
                string.IsNullOrEmpty(maxStr))
            {
                lblCreateMsg.Text = "Please fill in all required fields.";
                lblCreateMsg.Visible = true;
                pnlCreateEventModal.Visible = true;
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;
                return;
            }

            if (!DateTime.TryParse(dateStr, out DateTime implDate))
            {
                lblCreateMsg.Text = "Implementation date is invalid.";
                lblCreateMsg.Visible = true;
                pnlCreateEventModal.Visible = true;
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;
                return;
            }

            // parse times (try TimeSpan first, then DateTime)
            TimeSpan startTime, endTime;
            if (!TimeSpan.TryParse(startStr, out startTime))
            {
                if (DateTime.TryParse(startStr, out DateTime tmpStart)) startTime = tmpStart.TimeOfDay;
                else
                {
                    lblCreateMsg.Text = "Start time is invalid.";
                    lblCreateMsg.Visible = true;
                    pnlCreateEventModal.Visible = true;
                    pnlStep1.Visible = false;
                    pnlStep2.Visible = true;
                    return;
                }
            }

            if (!TimeSpan.TryParse(endStr, out endTime))
            {
                if (DateTime.TryParse(endStr, out DateTime tmpEnd)) endTime = tmpEnd.TimeOfDay;
                else
                {
                    lblCreateMsg.Text = "End time is invalid.";
                    lblCreateMsg.Visible = true;
                    pnlCreateEventModal.Visible = true;
                    pnlStep1.Visible = false;
                    pnlStep2.Visible = true;
                    return;
                }
            }

            if (!int.TryParse(maxStr, out int maxVol))
            {
                lblCreateMsg.Text = "Maximum volunteers must be a number.";
                lblCreateMsg.Visible = true;
                pnlCreateEventModal.Visible = true;
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;
                return;
            }

            // Insert using typed parameters
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"
                        INSERT INTO PublishedEvent
                        (AccountNum, EventTitle, Venue, EventAddress, ImplementationDate, EventStartTime, EventEndTime, VolunteerCapacity, Announcement)
                        VALUES
                        ((SELECT AccountNum FROM Account WHERE Account_ID = @accID),
                         @title, @venue, @address, @date, @start, @end, @max, @note)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@accID", Session["AccountID"]);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@venue", venue);
                        cmd.Parameters.AddWithValue("@address", address);

                        var pDate = new SqlParameter("@date", System.Data.SqlDbType.Date) { Value = implDate.Date };
                        var pStart = new SqlParameter("@start", System.Data.SqlDbType.Time) { Value = startTime };
                        var pEnd = new SqlParameter("@end", System.Data.SqlDbType.Time) { Value = endTime };
                        var pMax = new SqlParameter("@max", System.Data.SqlDbType.Int) { Value = maxVol };

                        cmd.Parameters.Add(pDate);
                        cmd.Parameters.Add(pStart);
                        cmd.Parameters.Add(pEnd);
                        cmd.Parameters.Add(pMax);

                        cmd.Parameters.AddWithValue("@note", string.IsNullOrWhiteSpace(note) ? DBNull.Value : (object)note);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Close modal, clear message and refresh
                lblCreateMsg.Text = "";
                lblCreateMsg.Visible = false;
                pnlCreateEventModal.Visible = false;
                BindMyEvents();
            }
            catch (Exception)
            {
                lblCreateMsg.Text = "Unable to publish event. Please try again.";
                lblCreateMsg.Visible = true;
                pnlCreateEventModal.Visible = true;
                pnlStep1.Visible = false;
                pnlStep2.Visible = true;
            }
        }

        private void ClearCreateEventFields()
        {
            txtTitle.Text = "";
            txtOrgName.Text = "";
            txtVenue.Text = "";
            txtAddress.Text = "";
            txtEmail.Text = "";
            txtContact.Text = "";
            txtDate.Text = "";
            txtStart.Text = "";
            txtEnd.Text = "";
            txtMaxVol.Text = "";
            txtAnnouncement.Text = "";
        }

        // ==============================
        // 4. HEADER BUTTONS
        // ==============================
        protected void btnBell_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = true;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/web/default.aspx");
        }

        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = false;
        }

        // ==============================
        // 5. EDIT / SAVE EVENT
        // ==============================
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedEventID"] != null)
            {
                hfEditingEventID.Value = ViewState["SelectedEventID"].ToString();

                txtEditTitle.Text = lblTitle.Text;
                txtEditVenueName.Text = lblVenue.Text;
                txtEditAddress.Text = lblAddress.Text;
                txtEditMax.Text = lblMax.Text;
                txtEditAnnounce.Text = lblAnnouncement.Text;

                try
                {
                    txtEditDate.Text = DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd");
                    txtEditStart.Text = DateTime.Parse(lblStart.Text).ToString("HH:mm");
                    txtEditEnd.Text = DateTime.Parse(lblEnd.Text).ToString("HH:mm");
                }
                catch { }

                pnlEditEvent.Visible = true;
            }
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                    UPDATE PublishedEvent
                    SET EventTitle = @title,
                        Venue = @venueName,
                        EventAddress = @address,
                        ImplementationDate = @date,
                        EventStartTime = @start,
                        EventEndTime = @end,
                        VolunteerCapacity = @max,
                        Announcement = @announce
                    WHERE PublishedEventNum = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@title", txtEditTitle.Text);
                cmd.Parameters.AddWithValue("@address", txtEditAddress.Text);
                cmd.Parameters.AddWithValue("@venueName", txtEditVenueName.Text);
                cmd.Parameters.AddWithValue("@date", txtEditDate.Text);
                cmd.Parameters.AddWithValue("@start", txtEditStart.Text);
                cmd.Parameters.AddWithValue("@end", txtEditEnd.Text);
                cmd.Parameters.AddWithValue("@max", txtEditMax.Text);
                cmd.Parameters.AddWithValue("@announce", txtEditAnnounce.Text);
                cmd.Parameters.AddWithValue("@id", hfEditingEventID.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            pnlEditEvent.Visible = false;
            LoadEventManagementData(hfEditingEventID.Value);
            BindMyEvents();

            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Database updated successfully!');", true);
        }

        protected void btnCloseEdit_Click(object sender, EventArgs e) => pnlEditEvent.Visible = false;

        protected void btnBackToMain_Click(object sender, EventArgs e)
        {
            pnlMainEvents.Visible = true;
            pnlManageEvent.Visible = false;
            BindMyEvents();
        }
    }
}
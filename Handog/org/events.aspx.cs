using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Handog.org
{
    public partial class _events : System.Web.UI.Page
    {
        // Connection string from Web.config
        string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMyEvents();
            }
        }

        // ==============================================================
        // 1. DATA BINDING FOR MAIN EVENTS
        // ==============================================================
        private void BindMyEvents()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Selects events created by the logged-in organizer
                string query = @"SELECT PublishedEventNum as EventID, EventTitle, EventAddress as Adress, Venue as Venue,
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

                // Hide Main View, Show Manage View
                pnlMainEvents.Visible = false;
                pnlManageEvent.Visible = true;

                LoadEventManagementData(eventID);
            }
        }

        // ==============================================================
        // 2. TAB SWITCHING LOGIC
        // ==============================================================
        protected void btnTabDetails_Click(object sender, EventArgs e)
        {
            // Show Details, Hide Signups
            pnlEventDetailsTab.Visible = true;
            pnlVolunteerSignupsTab.Visible = false;

            // Update Tab UI
            btnTabDetails.CssClass = "tab-link active";
            btnTabSignups.CssClass = "tab-link";
        }

        protected void btnTabSignups_Click(object sender, EventArgs e)
        {
            // Show Signups, Hide Details
            pnlEventDetailsTab.Visible = false;
            pnlVolunteerSignupsTab.Visible = true;

            // Update Tab UI
            btnTabDetails.CssClass = "tab-link";
            btnTabSignups.CssClass = "tab-link active";

            if (ViewState["SelectedEventID"] != null)
            {
                LoadVolunteerSignups(ViewState["SelectedEventID"].ToString());
            }
        }

        // ==============================================================
        // 3. MANAGE & EDIT LOGIC
        // ==============================================================
        private void LoadVolunteerSignups(string eventID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"SELECT EV.EventVolunteerNum as SignupID, (A.Firstname + ' ' + A.Lastname) as Name, 
                                 A.Account_ID as ChurchID, A.Email, A.ContactNum as Phone
                                 FROM EventVolunteers EV
                                 INNER JOIN Account A ON EV.AccountNum = A.AccountNum
                                 WHERE EV.PublishedEventNum = @id AND EV.Is_Accepted = 0";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@id", eventID);
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptSignups.DataSource = dt;
                rptSignups.DataBind();
            }
        }

        protected void rptSignups_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string signupID = e.CommandArgument.ToString();
            int status = (e.CommandName == "Accept") ? 1 : -1;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "UPDATE EventVolunteers SET Is_Accepted = @status WHERE EventVolunteerNum = @signupID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@signupID", signupID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadVolunteerSignups(ViewState["SelectedEventID"].ToString());
        }

        protected void btnBackToMain_Click(object sender, EventArgs e)
        {
            pnlMainEvents.Visible = true;
            pnlManageEvent.Visible = false;
            BindMyEvents();
        }

        // ==============================================================
        // 4. CREATE EVENT MODAL LOGIC
        // ==============================================================
        protected void btnOpenCreateModal_Click(object sender, EventArgs e)
        {
            // 1. Clear the fields first
            ClearCreateEventFields();

            // 2. Reset the step panels
            pnlStep1.Visible = true;
            pnlStep2.Visible = false;

            // 3. Show the modal
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
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"INSERT INTO PublishedEvent (AccountNum, EventTitle, VenueAddress, ImplementationDate, 
                                 EventStartTime, EventEndTime, VolunteerCapacity, Announcement) 
                                 VALUES ((SELECT AccountNum FROM Account WHERE Account_ID = @accID), 
                                 @title, @venue, @date, @start, @end, @max, @note)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@accID", Session["AccountID"]);
                cmd.Parameters.AddWithValue("@title", txtTitle.Text);
                cmd.Parameters.AddWithValue("@venue", txtVenue.Text);
                cmd.Parameters.AddWithValue("@date", txtDate.Text);
                cmd.Parameters.AddWithValue("@start", txtStart.Text);
                cmd.Parameters.AddWithValue("@end", txtEnd.Text);
                cmd.Parameters.AddWithValue("@max", txtMaxVol.Text);
                cmd.Parameters.AddWithValue("@note", txtAnnouncement.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            pnlCreateEventModal.Visible = false;
            BindMyEvents();
        }

        // ==============================================================
        // 5. HEADER ACTIONS & NOTIFICATIONS
        // ==============================================================
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

        // ==============================================================
        // 6. HELPER METHODS
        // ==============================================================
        private void LoadEventManagementData(string eventID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Join with Account to get Organizer Details
                string query = @"SELECT E.*, Venue, EventAddress, (A.Firstname + ' ' + A.Lastname) as OrganizerName, A.Email as OrgEmail, A.ContactNum as OrgPhone
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

                        // For the Expected participants (you can adjust this logic based on your DB)
                        lblExpectedPart.Text = reader["VolunteerCapacity"].ToString();
                    }
                }
            }
            // Also load the GridView for this specific event
            LoadVolunteerGrid(eventID);
        }

        private void LoadVolunteerGrid(string eventID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"SELECT A.AccountNum as ID, (A.Firstname + ' ' + A.Lastname) as Name, 
                         A.Email, A.ContactNum as Contact 
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
        // ==============================================================
        // 7. MANAGE & EDIT LOGIC
        // ==============================================================
        protected void btnUpdateEvent_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedEventID"] != null)
            {
                string eventID = ViewState["SelectedEventID"].ToString();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"UPDATE PublishedEvent 
                                     SET EventTitle = @title, 
                                         VenueAddress = @venue, 
                                         Announcement = @note 
                                     WHERE PublishedEventNum = @id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    
                    // Uses current textbox values to update database
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text); 
                    cmd.Parameters.AddWithValue("@venue", txtVenue.Text);
                    cmd.Parameters.AddWithValue("@note", txtAnnouncement.Text);
                    cmd.Parameters.AddWithValue("@id", eventID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                BindMyEvents();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Changes saved successfully!');", true);
            }
        }
        // This method is triggered when any "EDIT" link in your image is clicked
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedEventID"] != null)
            {
                hfEditingEventID.Value = ViewState["SelectedEventID"].ToString();

                // 1. Map current on-screen labels to Edit TextBoxes
                txtEditTitle.Text = lblTitle.Text;
                txtEditVenueName.Text = lblVenue.Text;
                txtEditAddress.Text = lblAddress.Text;
                txtEditMax.Text = lblMax.Text;
                txtEditAnnounce.Text = lblAnnouncement.Text;

                // 2. Handle Date/Time formatting for HTML5 inputs
                try
                {
                    txtEditDate.Text = DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd");
                    txtEditStart.Text = DateTime.Parse(lblStart.Text).ToString("HH:mm");
                    txtEditEnd.Text = DateTime.Parse(lblEnd.Text).ToString("HH:mm");
                }
                catch { /* Fallback if date format is custom */ }

                pnlEditEvent.Visible = true;
            }
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Update query including Date, Time, and Venue Name
                string query = @"UPDATE PublishedEvent 
                         SET EventTitle = @title, 
                             VenueAddress = @address,
                             VenueName = @venueName,
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
            LoadEventManagementData(hfEditingEventID.Value); // Refresh UI
            BindMyEvents(); // Refresh main list

            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Database updated successfully!');", true);
        }

        protected void btnCloseEdit_Click(object sender, EventArgs e)
        {
            pnlEditEvent.Visible = true;
        }
    }
}
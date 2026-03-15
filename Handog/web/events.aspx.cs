using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Handog.web
{
    public partial class events : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEvents();
            }
        }
        private void BindEvents()
        {
            string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT PublishedEventNum, EventTitle, EventAddress, Venue, ImplementationDate, EventStartTime, EventEndTime, Announcement FROM PublishedEvent";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptEvents.DataSource = dt;
                rptEvents.DataBind();
            }
        }

        // ==============================
        // SEARCH FUNCTIONALITY
        // ==============================
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        protected void btnSearchIcon_Click(object sender, ImageClickEventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();

            using (SqlConnection conn = new SqlConnection(connString))
            {

                string query = @"
                    SELECT PublishedEventNum, EventTitle, EventAddress, Venue, 
                           ImplementationDate, EventStartTime, EventEndTime, Announcement 
                    FROM PublishedEvent
                    WHERE EventTitle LIKE @keyword
                    ORDER BY ImplementationDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptEvents.DataSource = dt;
                    rptEvents.DataBind();
                }
            }
        }


        public bool IsUserRegistered(object eventNum)
        {
            // Use AccountID if that is what you set during login
            if (Session["AccountID"] == null) return false;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"SELECT COUNT(*) FROM EventVolunteers 
                         WHERE PublishedEventNum = @eventID 
                         AND AccountNum = (SELECT AccountNum FROM Account WHERE Account_ID = @accID)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@eventID", eventNum);
                cmd.Parameters.AddWithValue("@accID", Session["AccountID"]);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Clears the user session
            Response.Redirect("default.aspx"); // Sends them back to login
        }
        protected void btnBell_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = true;
            // This ensures the overlay uses flexbox to center the card
            pnlNotifications.Style.Add("display", "flex");
        }

        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = false;
            // Remove the style when closing
            pnlNotifications.Style.Remove("display");
        }
        // Update the existing "View Details" and "Register" buttons in your events list to call these:
        protected void btnViewDetails_Click(object sender, EventArgs e)
        {
            string eventID = ((Button)sender).CommandArgument;
            LoadModalData(eventID);
            pnlEventDetails.Visible = true;
            pnlEventDetails.Style.Add("display", "flex");
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string eventID = btn.CommandArgument;
            ViewState["SelectedEventID"] = eventID;

            LoadModalData(eventID);

            // Load event data into the modal labels (Similar to your ViewDetails logic)
            pnlRegistration.Visible = true;
            pnlRegistration.Style.Add("display", "flex");
        }
        protected void btnConfirmReg_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["SelectedEventID"] == null)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error: Event ID lost. Please try again.');", true);
                    return;
                }

                string eventID = ViewState["SelectedEventID"].ToString();
                string role = rbVolunteer.Checked ? "Volunteer" : "Participant";

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    // Insert using the subquery to find the correct AccountNum
                    string query = @"INSERT INTO EventVolunteers (PublishedEventNum, AccountNum, VolunteerType, Is_Accepted) 
                             VALUES (@eventID, (SELECT AccountNum FROM Account WHERE Account_ID = @accID), @role, 1)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@eventID", eventID);
                    cmd.Parameters.AddWithValue("@accID", Session["AccountID"]);
                    cmd.Parameters.AddWithValue("@role", role);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Success: Hide modal and Refresh list
                pnlRegistration.Visible = false;
                pnlRegistration.Style.Remove("display");
                BindEvents();

                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Successfully joined!');", true);
            }
            catch (Exception ex)
            {
                // This will tell you if it's a Database error (like a primary key violation)
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Database Error: {ex.Message}');", true);
            }
        }
        // Global close for all modals
        protected void btnCloseModals_Click(object sender, EventArgs e)
        {
            pnlRegistration.Visible = false;
            pnlEventDetails.Visible = false;
            pnlNotifications.Visible = false;

            pnlRegistration.Style.Remove("display");
            pnlEventDetails.Style.Remove("display");
            pnlNotifications.Style.Remove("display");
        }
        private void LoadModalData(string eventID)
        {
            // 1. Session Check
            if (Session["AccountID"] == null)
            {
                Response.Redirect("default.aspx");
                return;
            }

            string accountID = Session["AccountID"].ToString();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                // 2. Define Queries
                string eventQuery = @"SELECT E.*, (A.Firstname + ' ' + A.Lastname) as OrganizerName, A.Email as OrgEmail, A.ContactNum as OrgPhone 
                             FROM PublishedEvent E 
                             INNER JOIN Account A ON E.AccountNum = A.AccountNum 
                             WHERE E.PublishedEventNum = @id";

                string userQuery = "SELECT Firstname, Lastname, Email, ContactNum FROM Account WHERE Account_ID = @accID";

                conn.Open();

                // 3. Populate Event Details (Registration & View Details Modals)
                using (SqlCommand cmd = new SqlCommand(eventQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", eventID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string title = reader["EventTitle"].ToString();
                            string organizer = reader["OrganizerName"].ToString();
                            string address = reader["EventAddress"].ToString();
                            string venue = reader["Venue"].ToString();
                            string email = reader["OrgEmail"].ToString();
                            string contact = reader["OrgPhone"].ToString();
                            string date = Convert.ToDateTime(reader["ImplementationDate"]).ToString("MMMM dd, yyyy");
                            string start = Convert.ToDateTime(reader["EventStartTime"]).ToString("t");
                            string end = Convert.ToDateTime(reader["EventEndTime"]).ToString("t");
                            string capacity = reader["VolunteerCapacity"].ToString();
                            string announcement = reader["Announcement"].ToString();

                            // Populate Registration Modal Labels
                            lblRegEventTitleHeader.Text = title;
                            lblRegTitle.Text = title;
                            lblRegOrganizer.Text = organizer;
                            lblRegAddress.Text = address;
                            lblRegVenue.Text = venue;
                            lblRegEmail.Text = email;
                            lblRegContact.Text = contact;
                            lblRegDate.Text = date;
                            lblRegStart.Text = start;
                            lblRegEnd.Text = end;
                            lblRegMax.Text = capacity;
                        // --- POPULATE REGISTRATION MODAL (lblReg) ---
                        lblRegEventTitleHeader.Text = title;
                        lblRegTitle.Text = title;
                        lblRegOrganizer.Text = organizer;
                        lblRegAddress.Text = address;
                        lblRegVenue.Text = venue; 
                        lblRegEmail.Text = email;
                        lblRegContact.Text = contact;
                        lblRegDate.Text = date;                            
                        lblRegStart.Text = start;
                        lblRegEnd.Text = end;
                        lblRegMax.Text = capacity;

                            // Populate View Details Modal Labels
                            lblDetEventTitleHeader.Text = title;
                            lblDetTitle.Text = title;
                            lblDetOrganizer.Text = organizer;
                            lblDetAddress.Text = address;
                            lblDetVenue.Text = venue;
                            lblDetEmail.Text = email;
                            lblDetContact.Text = contact;
                            lblDetDate.Text = date;
                            lblDetStart.Text = start;
                            lblDetEnd.Text = end;
                            lblDetMax.Text = capacity;
                            lblDetAnnouncement.Text = announcement;
                        }
                    } // Reader 1 closes here
                }

                // 4. Populate Logged-in User Information (Registration Modal Fields)
                using (SqlCommand userCmd = new SqlCommand(userQuery, conn))
                {
                    userCmd.Parameters.AddWithValue("@accID", accountID);
                    using (SqlDataReader userReader = userCmd.ExecuteReader())
                    {
                        if (userReader.Read())
                        {
                            txtRegName.Text = userReader["Firstname"].ToString() + " " + userReader["Lastname"].ToString();
                            txtRegEmail.Text = userReader["Email"].ToString();
                            txtRegPhone.Text = userReader["ContactNum"].ToString();

                            // Make fields read-only as per standard registration behavior
                            txtRegName.ReadOnly = true;
                            txtRegEmail.ReadOnly = true;
                            txtRegPhone.ReadOnly = true;
                        }
                    } // Reader 2 closes here
                }
            }
        }
    }
}
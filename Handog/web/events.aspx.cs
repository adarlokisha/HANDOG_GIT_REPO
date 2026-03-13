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
                string query = "SELECT PublishedEventNum, EventTitle, VenueAddress, ImplementationDate, EventStartTime, EventEndTime, Announcement FROM PublishedEvent";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptEvents.DataSource = dt;
                rptEvents.DataBind();
            }
        }
        public bool IsUserRegistered(object eventNumObj)
        {
            if (Session["AccountID"] == null) return false;

            int eventNum = Convert.ToInt32(eventNumObj);
            string accountID = Session["AccountID"].ToString();
            bool joined = false;

            string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT COUNT(*) FROM EventVolunteers WHERE PublishedEventNum = @eventNum AND AccountNum = (SELECT AccountNum FROM Account WHERE Account_ID = @accID)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@eventNum", eventNum);
                cmd.Parameters.AddWithValue("@accID", accountID);
                conn.Open();
                joined = (int)cmd.ExecuteScalar() > 0;
            }
            return joined;
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
            string eventID = ((Button)sender).CommandArgument;
            ViewState["SelectedEventNum"] = eventID;
            LoadModalData(eventID);
            pnlRegistration.Visible = true;
            pnlRegistration.Style.Add("display", "flex");
        }
        protected void btnConfirmReg_Click(object sender, EventArgs e)
        {
            // Check if the user selected a role and agreed to terms
            if (!rbVolunteer.Checked && !rbParticipant.Checked)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a role (Volunteer or Participant)');", true);
                return;
            }

            if (!rbAgreeYes.Checked)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('You must agree to the Terms and Conditions');", true);
                return;
            }

            string accountID = Session["AccountID"].ToString();
            string eventNum = ViewState["SelectedEventNum"].ToString();
            string type = rbVolunteer.Checked ? "Volunteer" : "Participant";

            string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"INSERT INTO EventVolunteers (AccountNum, PublishedEventNum, VolunteerType, Is_Accepted) 
                         VALUES ((SELECT AccountNum FROM Account WHERE Account_ID = @accID), @eventNum, @type, 0)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@accID", accountID);
                cmd.Parameters.AddWithValue("@eventNum", eventNum);
                cmd.Parameters.AddWithValue("@type", type);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            pnlRegistration.Visible = false;
            BindEvents(); // This will refresh the card to show "JOINED"
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Successfully registered!');", true);
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
            if (Session["AccountID"] == null)
            {
                Response.Redirect("default.aspx");
                return;
            }

            string accountID = Session["AccountID"].ToString();
            string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                // 1. Fetch Event Details
                string eventQuery = @"SELECT E.*, (A.Firstname + ' ' + A.Lastname) as OrganizerName, A.Email as OrgEmail, A.ContactNum as OrgPhone 
                             FROM PublishedEvent E 
                             INNER JOIN Account A ON E.AccountNum = A.AccountNum 
                             WHERE E.PublishedEventNum = @id";

                // 2. Fetch Logged-in User Info
                string userQuery = "SELECT Firstname, Lastname, Email, ContactNum FROM Account WHERE Account_ID = @accID";

                SqlCommand cmd = new SqlCommand(eventQuery, conn);
                cmd.Parameters.AddWithValue("@id", eventID);

                SqlCommand userCmd = new SqlCommand(userQuery, conn);
                userCmd.Parameters.AddWithValue("@accID", accountID);

                conn.Open();

                // Populate BOTH Registration and View Details labels
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string title = reader["EventTitle"].ToString();
                        string organizer = reader["OrganizerName"].ToString();
                        string address = reader["VenueAddress"].ToString();
                        string email = reader["OrgEmail"].ToString();
                        string contact = reader["OrgPhone"].ToString();
                        string date = Convert.ToDateTime(reader["ImplementationDate"]).ToString("MMMM dd, yyyy");
                        string start = Convert.ToDateTime(reader["EventStartTime"]).ToString("t");
                        string end = Convert.ToDateTime(reader["EventEndTime"]).ToString("t");
                        string capacity = reader["VolunteerCapacity"].ToString();
                        string announcement = reader["Announcement"].ToString();

                        // --- POPULATE REGISTRATION MODAL (lblReg) ---
                        lblRegEventTitleHeader.Text = title;
                        lblRegTitle.Text = title;
                        lblRegOrganizer.Text = organizer;
                        lblRegAddress.Text = address;
                        lblRegEmail.Text = email;
                        lblRegContact.Text = contact;
                        lblRegDate.Text = date;
                        lblRegStart.Text = start;
                        lblRegEnd.Text = end;
                        lblRegMax.Text = capacity;

                        // --- POPULATE VIEW DETAILS MODAL (lblDet) ---
                        lblDetEventTitleHeader.Text = title;
                        lblDetTitle.Text = title;
                        lblDetOrganizer.Text = organizer;
                        lblDetAddress.Text = address;
                        lblDetVenue.Text = address; // Using address as venue per your SQL results
                        lblDetEmail.Text = email;
                        lblDetContact.Text = contact;
                        lblDetDate.Text = date;
                        lblDetStart.Text = start;
                        lblDetEnd.Text = end;
                        lblDetMax.Text = capacity;
                        lblDetAnnouncement.Text = announcement;
                    }
                }

                // Populate User TextBoxes (Only for Registration Modal)
                using (SqlDataReader userReader = userCmd.ExecuteReader())
                {
                    if (userReader.Read())
                    {
                        txtRegName.Text = userReader["Firstname"].ToString() + " " + userReader["Lastname"].ToString();
                        txtRegEmail.Text = userReader["Email"].ToString();
                        txtRegPhone.Text = userReader["ContactNum"].ToString();

                        txtRegName.ReadOnly = true;
                        txtRegEmail.ReadOnly = true;
                        txtRegPhone.ReadOnly = true;
                    }
                }
            }
        }
    }
}
using System;
using System.Data;
using System.Web.UI.WebControls;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Handog.org
{
    public partial class request : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null || Session["UserRole"].ToString() != "Organizer")
            {
                //Boot them out if they aren't logged in as an Organizer
                Response.Redirect("~/web/default.aspx");
            }

            if (!IsPostBack)
            {
                BindRequests();
            }
        }

        // ==============================================================
        // 1. DATA BINDING
        // ==============================================================
        private void BindRequests()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"SELECT r.RequestNum AS RequestID, 
                                        rt.Type_of_Request AS RequestType, 
                                        (a.FirstName + ' ' + a.LastName) AS RequestorName, 
                                        r.RequestDetails AS MessageText 
                                 FROM Request r
                                 INNER JOIN RequestType rt ON r.RequestTypeNum = rt.RequestTypeNum
                                 INNER JOIN Account a ON r.AccountNum = a.AccountNum
                                 WHERE r.Is_Accepted IS NULL OR r.Is_Accepted = 0
                                 ORDER BY r.RequestNum ASC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptRequests.DataSource = dt;
                rptRequests.DataBind();
            }
        }

        // ==============================================================
        // 2. REPEATER BUTTON CLICKS
        // ==============================================================
        protected void rptRequests_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int requestID = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Accept")
            {
                // Store the ID so we can update it when they hit Publish
                hfAcceptedRequestID.Value = requestID.ToString();

                ClearCreateEventFields();
                PreFillModalData(requestID);

                // Open the modal to start creating the event!
                pnlStep1.Visible = true;
                pnlStep2.Visible = false;
                pnlCreateEventModal.Visible = true;
            }
            else if (e.CommandName == "Reject")
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "DELETE FROM Request WHERE RequestNum = @RequestNum";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RequestNum", requestID);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                BindRequests();
            }
        }

        // ==============================================================
        // 3. CREATE EVENT MODAL LOGIC
        // ==============================================================
        private void PreFillModalData(int requestID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // 1. Fetch Organizer Details to lock them in
                string orgQuery = "SELECT Firstname, Lastname, Email, ContactNum FROM Account WHERE AccountNum = @AccNum";
                using (SqlCommand cmd = new SqlCommand(orgQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@AccNum", Session["AccountID"]);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtOrgName.Text = reader["Firstname"].ToString() + " " + reader["Lastname"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            txtContact.Text = reader["ContactNum"].ToString();

                            // Make them uneditable!
                            txtOrgName.ReadOnly = true;
                            txtEmail.ReadOnly = true;
                            txtContact.ReadOnly = true;
                        }
                    }
                }

                // 2. Fetch Request Nature for the Title
                string reqQuery = @"SELECT rt.Type_of_Request 
                                    FROM Request r 
                                    INNER JOIN RequestType rt ON r.RequestTypeNum = rt.RequestTypeNum 
                                    WHERE r.RequestNum = @ReqNum";
                using (SqlCommand cmd = new SqlCommand(reqQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ReqNum", requestID);
                    object reqType = cmd.ExecuteScalar();
                    if (reqType != null)
                    {
                        txtTitle.Text = reqType.ToString() + " Event";
                    }
                }
            }
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

        protected void btnCloseCreate_Click(object sender, EventArgs e)
        {
            pnlCreateEventModal.Visible = false;
            hfAcceptedRequestID.Value = ""; // Clear out the request ID since they cancelled
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            lblCreateMsg.Text = "";
            lblCreateMsg.Visible = false;

            // Gathering inputs
            string title = txtTitle.Text.Trim();
            string venue = txtVenue.Text.Trim();
            string address = txtAddress.Text.Trim();
            string note = txtAnnouncement.Text.Trim();
            string maxStr = txtMaxVol.Text.Trim();
            string dateStr = txtDate.Text.Trim();
            string startStr = txtStart.Text.Trim();
            string endStr = txtEnd.Text.Trim();

            // Validation checks
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(venue) || string.IsNullOrEmpty(address) ||
                string.IsNullOrEmpty(dateStr) || string.IsNullOrEmpty(startStr) || string.IsNullOrEmpty(endStr) || string.IsNullOrEmpty(maxStr))
            {
                lblCreateMsg.Text = "Please fill in all required fields.";
                lblCreateMsg.Visible = true;
                return;
            }

            if (!DateTime.TryParse(dateStr, out DateTime implDate) || !int.TryParse(maxStr, out int maxVol))
            {
                lblCreateMsg.Text = "Invalid Date or Maximum Volunteers.";
                lblCreateMsg.Visible = true;
                return;
            }

            TimeSpan startTime, endTime;

            // Safely parse Start Time
            if (!TimeSpan.TryParse(startStr, out startTime))
            {
                if (DateTime.TryParse(startStr, out DateTime dtStart))
                    startTime = dtStart.TimeOfDay;
                else
                {
                    lblCreateMsg.Text = "Invalid Start Time."; lblCreateMsg.Visible = true; return;
                }
            }

            // Safely parse End Time
            if (!TimeSpan.TryParse(endStr, out endTime))
            {
                if (DateTime.TryParse(endStr, out DateTime dtEnd))
                    endTime = dtEnd.TimeOfDay;
                else
                {
                    lblCreateMsg.Text = "Invalid End Time."; lblCreateMsg.Visible = true; return;
                }
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // 1. Publish the new Event
                    string insertQuery = @"
                        INSERT INTO PublishedEvent
                        (AccountNum, EventTitle, Venue, EventAddress, ImplementationDate, EventStartTime, EventEndTime, VolunteerCapacity, Announcement)
                        VALUES
                        (@accID, @title, @venue, @address, @date, @start, @end, @max, @note)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@accID", Session["AccountID"]);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@venue", venue);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.Date) { Value = implDate.Date });
                        cmd.Parameters.Add(new SqlParameter("@start", SqlDbType.Time) { Value = startTime });
                        cmd.Parameters.Add(new SqlParameter("@end", SqlDbType.Time) { Value = endTime });
                        cmd.Parameters.Add(new SqlParameter("@max", SqlDbType.Int) { Value = maxVol });
                        cmd.Parameters.AddWithValue("@note", string.IsNullOrWhiteSpace(note) ? DBNull.Value : (object)note);

                        cmd.ExecuteNonQuery();
                    }

                    // 2. Mark the original request as accepted!
                    if (!string.IsNullOrEmpty(hfAcceptedRequestID.Value))
                    {
                        string updateReqQuery = "UPDATE Request SET Is_Accepted = 1 WHERE RequestNum = @ReqNum";
                        using (SqlCommand cmd = new SqlCommand(updateReqQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@ReqNum", hfAcceptedRequestID.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Cleanup and Refresh
                pnlCreateEventModal.Visible = false;
                hfAcceptedRequestID.Value = "";
                BindRequests();

                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Event successfully published and request accepted!');", true);
            }
            catch (Exception ex)
            {
                lblCreateMsg.Text = "Database Error. " + ex.Message;
                lblCreateMsg.Visible = true;
            }
        }

        private void ClearCreateEventFields()
        {
            txtTitle.Text = txtOrgName.Text = txtVenue.Text = txtAddress.Text = "";
            txtEmail.Text = txtContact.Text = txtDate.Text = txtStart.Text = "";
            txtEnd.Text = txtMaxVol.Text = txtAnnouncement.Text = "";
            lblCreateMsg.Visible = false;
        }

        // ==============================================================
        // 4. HEADER ACTIONS & NOTIFICATIONS
        // ==============================================================
        protected void btnBell_Click(object sender, EventArgs e) => pnlNotifications.Visible = true;
        protected void btnCloseNotif_Click(object sender, EventArgs e) => pnlNotifications.Visible = false;

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Session.Clear();
            Response.Redirect("~/web/default.aspx");
        }
    }
}
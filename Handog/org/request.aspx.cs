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
                Response.Redirect("~/web/default.aspx");
            }

            if (!IsPostBack)
            {
                BindRequests();
            }
        }

        // ==============================================================
        //  DATA BINDING
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
        //  REPEATER BUTTON CLICKS
        // ==============================================================
        protected void rptRequests_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int requestID = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Accept")
            {

                hfAcceptedRequestID.Value = requestID.ToString();

                ClearCreateEventFields();
                PreFillModalData(requestID);

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
        //  CREATE EVENT LOGIC
        // ==============================================================
        private void PreFillModalData(int requestID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string orgQuery = "SELECT Firstname, Lastname, Email, ContactNum FROM Account WHERE Account_ID = @AccID";
                using (SqlCommand cmd = new SqlCommand(orgQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@AccID", Session["AccountID"]);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtOrgName.Text = reader["Firstname"].ToString() + " " + reader["Lastname"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            txtContact.Text = reader["ContactNum"].ToString();

                            txtOrgName.ReadOnly = true;
                            txtEmail.ReadOnly = true;
                            txtContact.ReadOnly = true;
                        }
                    }
                }

                string reqQuery = @"SELECT rt.Type_of_Request, 
                                           (a.Firstname + ' ' + a.Lastname) AS RequestorName 
                                    FROM Request r 
                                    INNER JOIN RequestType rt ON r.RequestTypeNum = rt.RequestTypeNum 
                                    INNER JOIN Account a ON r.AccountNum = a.AccountNum
                                    WHERE r.RequestNum = @ReqNum";

                using (SqlCommand cmd = new SqlCommand(reqQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ReqNum", requestID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string reqType = reader["Type_of_Request"].ToString();
                            string requestorName = reader["RequestorName"].ToString();
                            txtTitle.Text = "";
                            txtAnnouncement.Text = $"This event was created because of a request having a purpose of '{reqType}' for {requestorName}.\n\n-Add description here or modify if you like-";
                        }
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
            hfAcceptedRequestID.Value = "";
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            lblCreateMsg.Text = "";
            lblCreateMsg.Visible = false;

            // Gathering inputs
            string title = txtTitle.Text.Trim();
            string headOrganizer = txtOrgName.Text.Trim();
            string venue = txtVenue.Text.Trim();
            string address = txtAddress.Text.Trim();
            string note = txtAnnouncement.Text.Trim();
            string maxStr = txtMaxVol.Text.Trim();
            string dateStr = txtDate.Text.Trim();
            string startStr = txtStart.Text.Trim();
            string endStr = txtEnd.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(headOrganizer) || string.IsNullOrEmpty(venue) || string.IsNullOrEmpty(address) ||
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

            if (!TimeSpan.TryParse(startStr, out startTime))
            {
                if (DateTime.TryParse(startStr, out DateTime dtStart))
                    startTime = dtStart.TimeOfDay;
                else
                {
                    lblCreateMsg.Text = "Invalid Start Time."; lblCreateMsg.Visible = true; return;
                }
            }

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

                    string insertQuery = @"
                        INSERT INTO PublishedEvent
                        (AccountNum, HeadOrganizer, EventTitle, Venue, EventAddress, ImplementationDate, EventStartTime, EventEndTime, VolunteerCapacity, Announcement)
                        VALUES
                        ((SELECT AccountNum FROM Account WHERE Account_ID = @accID), 
                         @headOrg, @title, @venue, @address, @date, @start, @end, @max, @note)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@accID", Session["AccountID"]);
                        cmd.Parameters.AddWithValue("@headOrg", headOrganizer);
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
        //  HEADER ACTIONS & NOTIFICATIONS
        // ==============================================================
        // TODO: Notifications
        // CONNECT TO DATABASE
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
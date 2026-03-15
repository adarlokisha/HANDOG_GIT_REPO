using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Data.SqlClient; 
using System.Configuration;

namespace Handog.web
{
    public partial class request : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null || Session["UserRole"].ToString() != "Volunteer")
            {
                //Boot them out if they aren't logged in as an Volunteer
                Response.Redirect("~/web/default.aspx");
            }

            if (!IsPostBack)
            {
                BindRequestTypes();
                BindRequests();
            }
        }
        private void BindRequestTypes()
        {
            // Replace with your actual connection string
            string connStr = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT RequestTypeNum, Type_of_Request FROM RequestType";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                ddlRequestType.DataSource = cmd.ExecuteReader();
                ddlRequestType.DataTextField = "Type_of_Request";
                ddlRequestType.DataValueField = "RequestTypeNum";
                ddlRequestType.DataBind();

                ddlRequestType.Items.Insert(0, new ListItem("-- Select Purpose --", "0"));
            }
        }
        private void BindRequests()
        {
            string connStr = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // Added a WHERE clause to filter out accepted requests!
                string query = @"SELECT r.RequestDetails, 
                                        rt.Type_of_Request, 
                                        (a.FirstName + ' ' + a.LastName) as AccountName 
                                 FROM Request r
                                 INNER JOIN RequestType rt ON r.RequestTypeNum = rt.RequestTypeNum
                                 INNER JOIN Account a ON r.AccountNum = a.AccountNum
                                 WHERE r.Is_Accepted IS NULL OR r.Is_Accepted = 0
                                 ORDER BY r.RequestNum DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptRequests.DataSource = dt;
                rptRequests.DataBind();
            }
        }
        protected void btnPostRequest_Click(object sender, EventArgs e)
        {
            // 1. Safety Check: If session is null, the user isn't logged in properly
            if (Session["UserAccountNum"] == null)
            {
                // Redirect to login or show an error
                Response.Redirect("default.aspx");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "INSERT INTO Request (AccountNum, RequestTypeNum, RequestDetails, Is_Accepted) VALUES (@Acc, @Type, @Details, 0)";
                SqlCommand cmd = new SqlCommand(query, conn);

                // Now @Acc will definitely have a value
                cmd.Parameters.AddWithValue("@Acc", Session["UserAccountNum"]);
                cmd.Parameters.AddWithValue("@Type", ddlRequestType.SelectedValue);
                cmd.Parameters.AddWithValue("@Details", txtDetails.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            pnlAddRequest.Visible = false;
            BindRequests();
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Clears the user session
            Response.Redirect("default.aspx"); // Sends them back to login
        }
        protected void btnBell_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = true;
        }

        // New logic to hide the notification panel
        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = false;
        }
        protected void btnAddRequest_Click(object sender, EventArgs e)
        {
            txtDetails.Text = "";
            if (ddlRequestType.Items.Count > 0) ddlRequestType.SelectedIndex = 0;

            // Show the modal
            pnlAddRequest.Visible = true;
        }

        protected void btnCancelRequest_Click(object sender, EventArgs e)
        {
            pnlAddRequest.Visible = false;
        }
        protected void btnCloseModals_Click(object sender, EventArgs e)
        {
            // Hide the registration panel
            pnlRegistration.Visible = false;

            // Also hide notifications if they are open
            if (pnlNotifications != null)
            {
                pnlNotifications.Visible = false;
                pnlNotifications.Style.Remove("display");
            }

            // Clean up any flex styling if applied
            pnlRegistration.Style.Remove("display");
        }
    }
}
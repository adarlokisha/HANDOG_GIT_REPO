using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Handog.web
{
    public partial class request : Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure user is logged in and is a Volunteer
            if (Session["AccountNum"] == null || Session["UserRole"]?.ToString() != "Volunteer")
            {
                Response.Redirect("~/web/default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindRequestTypes();
                BindRequests();
            }
        }

        private void BindRequestTypes()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT RequestTypeNum, Type_of_Request FROM RequestType", conn))
            {
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
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT r.RequestDetails, 
                       rt.Type_of_Request, 
                       (a.FirstName + ' ' + a.LastName) AS AccountName 
                FROM Request r
                INNER JOIN RequestType rt ON r.RequestTypeNum = rt.RequestTypeNum
                INNER JOIN Account a ON r.AccountNum = a.AccountNum
                WHERE r.Is_Accepted IS NULL OR r.Is_Accepted = 0
                ORDER BY r.RequestNum DESC", conn))
            {
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    da.Fill(dt);

                rptRequests.DataSource = dt;
                rptRequests.DataBind();
            }
        }

        protected void btnPostRequest_Click(object sender, EventArgs e)
        {
            if (Session["AccountNum"] == null)
            {
                Response.Redirect("~/web/default.aspx");
                return;
            }

            int accNum = Convert.ToInt32(Session["AccountNum"]); // numeric PK

            if (ddlRequestType.SelectedIndex == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a request type.');", true);
                return;
            }

            string details = txtDetails.Text.Trim();
            if (string.IsNullOrEmpty(details))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter request details.');", true);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Request (AccountNum, RequestTypeNum, RequestDetails, Is_Accepted) 
                VALUES (@Acc, @Type, @Details, 0)", conn))
            {
                cmd.Parameters.AddWithValue("@Acc", accNum);
                cmd.Parameters.AddWithValue("@Type", int.Parse(ddlRequestType.SelectedValue));
                cmd.Parameters.AddWithValue("@Details", details);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            pnlAddRequest.Visible = false;
            BindRequests();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/web/default.aspx");
        }

        protected void btnBell_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = true;
        }

        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = false;
        }

        protected void btnAddRequest_Click(object sender, EventArgs e)
        {
            txtDetails.Text = "";
            ddlRequestType.SelectedIndex = 0;
            pnlAddRequest.Visible = true;
        }

        protected void btnCancelRequest_Click(object sender, EventArgs e)
        {
            pnlAddRequest.Visible = false;
        }

        protected void btnCloseModals_Click(object sender, EventArgs e)
        {
            pnlRegistration.Visible = false;
            if (pnlNotifications != null)
            {
                pnlNotifications.Visible = false;
                pnlNotifications.Style.Remove("display");
            }
            pnlRegistration.Style.Remove("display");
        }
    }
}
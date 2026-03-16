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
    public partial class home : System.Web.UI.Page
    {
        string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null || Session["UserRole"].ToString() != "Volunteer")
            {
                Response.Redirect("~/web/default.aspx");
            }

            if (!IsPostBack)
            {
                BindLocales();
            }
        }
        private void BindLocales()
        {
            string query = "SELECT Locale_ID, LocaleName, Barangay, City FROM Locale ORDER BY LocaleName ASC";

            using (SqlConnection con = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvLocales.DataSource = dt;
                        gvLocales.DataBind();
                    }
                    catch (Exception ex)
                    {
                         Response.Write(ex.Message);
                    }
                }
            }
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon(); 
            Response.Redirect("default.aspx"); 
        }
        protected void btnBell_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = true;
        }

        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Handog.org
{
    public partial class home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // This is where you will eventually call your "LoadData()" function
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/web/default.aspx"); // Go up one level to root for login
        }

        protected void btnBell_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = true;
        }

        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = false;
        }

        // --- ORGANIZER HANDLERS ---

        protected void btnAddLocale_Click(object sender, EventArgs e)
        {
            // Logic to save to Azure DB will go here
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtLocale.Text = "";
            txtCity.Text = "";
            txtBarangay.Text = "";
        }
    }
}
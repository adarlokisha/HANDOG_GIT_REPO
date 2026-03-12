using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Handog.web
{
    public partial class events : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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
            pnlEventDetails.Visible = true;
            pnlEventDetails.Style.Add("display", "flex");
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            pnlRegistration.Visible = true;
            pnlRegistration.Style.Add("display", "flex");
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
    }
}
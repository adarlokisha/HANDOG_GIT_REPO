using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Handog.web
{
    public partial class request : System.Web.UI.Page
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
        }

        // New logic to hide the notification panel
        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = false;
        }
        protected void btnAddRequest_Click(object sender, EventArgs e)
        {
            pnlAddRequest.Visible = true;
        }

        protected void btnCancelRequest_Click(object sender, EventArgs e)
        {
            pnlAddRequest.Visible = false;
            rblRequestType.ClearSelection();
            phFormFields.Visible = false;
        }
        protected void rblRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            phFormFields.Visible = true;
            if (rblRequestType.SelectedValue == "Inquiry")
            {
                lblSubject.Text = "INQUIRY SUBJECT*";
                lblDetails.Text = "INQUIRY DETAILS*";
                btnPostRequest.Text = "+ POST INQUIRY";
            }
            else
            {
                lblSubject.Text = "REQUEST NATURE*";
                lblDetails.Text = "REQUEST DETAILS*";
                btnPostRequest.Text = "+ POST ASSISTANCE";
            }
        }
    }
}
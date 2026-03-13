using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Handog.org
{
    public partial class request : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load temporary data so you can see the layout!
                LoadDummyData();
            }
        }

        private void LoadDummyData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("RequestID", typeof(int));
            dt.Columns.Add("RequestorName", typeof(string));
            dt.Columns.Add("MessageText", typeof(string));

            dt.Rows.Add(1, "JUAN DELA CRUZ", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");
            dt.Rows.Add(2, "BARANGAY SAN JOSE", "We are requesting assistance for a cleanup drive this coming weekend. We need 20 volunteers.");

            rptRequests.DataSource = dt;
            rptRequests.DataBind();
        }

        // ==============================================================
        // 2. REPEATER BUTTON CLICKS (Accept, Reject)
        // ==============================================================
        protected void rptRequests_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // e.CommandArgument holds the specific RequestID from the database
            string requestID = e.CommandArgument.ToString();

            if (e.CommandName == "Accept")
            {
                // TODO: Write SQL UPDATE to set status to 'Accepted'
                System.Diagnostics.Debug.WriteLine("Accepted request: " + requestID);
            }
            else if (e.CommandName == "Reject")
            {
                // TODO: Write SQL UPDATE to set status to 'Rejected'
                System.Diagnostics.Debug.WriteLine("Rejected request: " + requestID);
            }
        }

        protected void btnBell_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = true;
        }

        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = false;
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Session.Clear();
            Response.Redirect("~/web/default.aspx");
        }
    }
}
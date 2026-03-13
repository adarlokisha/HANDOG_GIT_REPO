using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Handog.org
{
    public partial class _events : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDummyEvents();
            }
        }

        // ==============================================================
        // 1. DATA BINDING FOR MAIN EVENTS
        // ==============================================================
        private void LoadDummyEvents()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EventID", typeof(int));
            dt.Columns.Add("EventTitle", typeof(string));
            dt.Columns.Add("Venue", typeof(string));
            dt.Columns.Add("EventDate", typeof(string));
            dt.Columns.Add("TimeStr", typeof(string));
            dt.Columns.Add("Description", typeof(string));

            dt.Rows.Add(1, "COMMUNITY ENGAGEMENT", "Mapua Malayan Colleges Laguna", "January 23, 2026", "7:00AM - 6:00PM", "Lorem ipsum dolor sit amet, consectetur adipiscing elit.");
            dt.Rows.Add(2, "COASTAL CLEANUP", "Batangas Bay", "February 14, 2026", "6:00AM - 12:00PM", "Join us to clean the shores and help marine life.");

            rptEvents.DataSource = dt;
            rptEvents.DataBind();
        }

        protected void rptEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                string eventID = e.CommandArgument.ToString();

                // Hide Main View, Show Manage View
                pnlMainEvents.Visible = false;
                pnlManageEvent.Visible = true;

                // Load the fake volunteer data when we open the details!
                LoadDummySignups();
            }
        }

        // ==============================================================
        // TAB SWITCHING LOGIC
        // ==============================================================
        protected void btnTabDetails_Click(object sender, EventArgs e)
        {
            // Show Details, Hide Signups
            pnlEventDetailsTab.Visible = true;
            pnlVolunteerSignupsTab.Visible = false;

            // Update Tab UI (Yellow Highlight)
            btnTabDetails.CssClass = "tab-link active";
            btnTabSignups.CssClass = "tab-link";
        }

        protected void btnTabSignups_Click(object sender, EventArgs e)
        {
            // Show Signups, Hide Details
            pnlEventDetailsTab.Visible = false;
            pnlVolunteerSignupsTab.Visible = true;

            // Update Tab UI (Yellow Highlight)
            btnTabDetails.CssClass = "tab-link";
            btnTabSignups.CssClass = "tab-link active";

            // Load data into the repeater when the tab is clicked
            LoadDummySignups();
        }

        // ==============================================================
        // SIGNUPS DATA & ACTIONS
        // ==============================================================
        private void LoadDummySignups()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SignupID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("ChurchID", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Phone", typeof(string));

            dt.Rows.Add(1, "JUAN DELA CRUZ", "M2000000000", "email@gmail.com", "09*********");
            dt.Rows.Add(2, "MARIA SANTOS", "M2000000000", "email@gmail.com", "09*********");
            dt.Rows.Add(3, "JOSE REYES", "M2000000000", "email@gmail.com", "09*********");
            dt.Rows.Add(4, "MARY ANN", "M2000000000", "email@gmail.com", "09*********");

            rptSignups.DataSource = dt;
            rptSignups.DataBind();
        }

        protected void rptSignups_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string signupID = e.CommandArgument.ToString();

            if (e.CommandName == "Accept")
            {
                // TODO: SQL UPDATE to accept volunteer into event
                System.Diagnostics.Debug.WriteLine("Accepted Signup: " + signupID);
            }
            else if (e.CommandName == "Reject")
            {
                // TODO: SQL UPDATE to reject volunteer
                System.Diagnostics.Debug.WriteLine("Rejected Signup: " + signupID);
            }
        }

        protected void btnBackToMain_Click(object sender, EventArgs e)
        {
            pnlMainEvents.Visible = true;
            pnlManageEvent.Visible = false;
        }

        // ==============================================================
        // 3. CREATE EVENT MODAL LOGIC
        // ==============================================================
        protected void btnOpenCreateModal_Click(object sender, EventArgs e)
        {
            // Reset form to Step 1
            pnlStep1.Visible = true;
            pnlStep2.Visible = false;
            pnlCreateEventModal.Visible = true;
        }

        protected void btnCloseCreate_Click(object sender, EventArgs e)
        {
            pnlCreateEventModal.Visible = false;
        }

        protected void btnNextStep_Click(object sender, EventArgs e)
        {
            // Transition to Step 2
            pnlStep1.Visible = false;
            pnlStep2.Visible = true;
        }

        protected void btnPrevStep_Click(object sender, EventArgs e)
        {
            // Back to Step 1
            pnlStep1.Visible = true;
            pnlStep2.Visible = false;
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            // TODO: Collect data from txtTitle.Text, txtOrgName.Text, etc.
            // Write SQL INSERT query to create event in Azure DB

            pnlCreateEventModal.Visible = false;
        }

        // ==============================================================
        // 4. HEADER ACTIONS
        // ==============================================================
        protected void btnBell_Click(object sender, EventArgs e)
        {
            // TODO: Implement Notifications Modal
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/web/default.aspx");
        }

        // ==============================================================
        // 4. HEADER ACTIONS & NOTIFICATIONS
        // ==============================================================

        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            // Hide the Notifications modal
            pnlNotifications.Visible = false;
        }

    }
}
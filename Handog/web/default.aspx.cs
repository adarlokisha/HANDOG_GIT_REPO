using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Handog.web
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Page initialization logic here
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // 1. Get the values from the TextBoxes
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            // 2. Add your validation logic here 
            // (e.g., check against a database)
            if (IsValidUser(email, password))
            {
                // 3. Redirect to home.aspx
                Response.Redirect("home.aspx");
            }
            else
            {
                // Optional: Show an error message if login fails
                // ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid Credentials');", true);
            }
        }

        private bool IsValidUser(string email, string password)
        {
            // Dummy validation for now - replace with database logic
            return !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password);
        }
    }
}


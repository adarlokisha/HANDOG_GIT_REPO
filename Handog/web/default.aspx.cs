using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Handog.web
{
    public partial class _default : System.Web.UI.Page
    {

        string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;
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
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid Credentials');", true);
            }
        }

        private bool IsValidUser(string email, string password)
        {
            bool isValid = false;
            string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Use the exact column names from your screenshot: Account_ID, AccRole, Email
                string query = "SELECT Account_ID, AccRole, Email FROM Account WHERE Email = @email AND AccPassword = @pass";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@pass", password);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isValid = true;
                            // This is the part that fixes your NullReferenceException!
                            Session["AccountID"] = reader["Account_ID"].ToString();
                            Session["UserRole"] = reader["AccRole"].ToString();
                            Session["UserEmail"] = reader["Email"].ToString();
                        }
                    }
                }
            }
            return isValid;
        }
    }
}


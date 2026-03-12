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
            // Get connection string from Web.config
            string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Joining Person -> Registration -> Account
                string query = @"
            SELECT A.account_ID, A.Role, P.Email 
            FROM Account A
            INNER JOIN Registration R ON A.registration_ID = R.Registration_ID
            INNER JOIN Person P ON R.Person_ID = P.Person_ID
            WHERE P.Email = @email AND A.Password = @pass";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email.Trim());
                    cmd.Parameters.AddWithValue("@pass", password); // In production, use hashing!

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isValid = true;
                                // Store details in Session to use on the Home page
                                Session["AccountID"] = reader["account_ID"].ToString();
                                Session["UserRole"] = reader["Role"].ToString();
                                Session["UserEmail"] = reader["Email"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // This helps you see if there's a specific Azure/SQL error
                        string errorMsg = ex.Message.Replace("'", "");
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {errorMsg}');", true);
                    }
                }
            }
            return isValid;
        }
    }
}


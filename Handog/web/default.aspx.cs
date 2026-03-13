using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Web.UI;

namespace Handog.web
{
    public partial class _default : Page
    {
        string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            string role = GetUserRole(email, password);

            if (!string.IsNullOrEmpty(role))
            {
                Session["UserEmail"] = email;
                Session["UserRole"] = role;
                Session["AccountID"] = GetAccountID(email);

                if (role == "Organizer")
                    Response.Redirect("orghome.aspx");
                else
                    Response.Redirect("home.aspx");
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid Credentials');", true);
            }
        }

        private string GetUserRole(string email, string password)
        {
            string role = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT AccRole FROM Account WHERE Email = @Email AND AccPassword = @Password";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            role = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message.Replace("'", "")}');", true);
            }
            return role;
        }

        private string GetAccountID(string email)
        {
            string accountID = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT Account_ID FROM Account WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            accountID = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message.Replace("'", "")}');", true);
            }
            return accountID;
        }
    }
}

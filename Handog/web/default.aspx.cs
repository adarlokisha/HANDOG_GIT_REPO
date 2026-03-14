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
            // nothing needed here for modal; login remains same
        }

        // =========================
        // LOGIN LOGIC
        // =========================
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            string role = GetUserRole(email, password);

            if (!string.IsNullOrEmpty(role))
            {
                Session["UserEmail"] = email;
                Session["UserRole"] = role;

                // Retrieve the ID and store it in the session variable 
                // that your request page expects: "UserAccountNum"
                Session["UserAccountNum"] = GetAccountID(email);

                if (role == "Organizer")
                    Response.Redirect("~/org/home.aspx");
                else
                    Response.Redirect("~/web/home.aspx");
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
                    string query = "SELECT AccRole FROM [Account] WHERE Email=@Email AND AccPassword=@Password";

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
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Error: {ex.Message.Replace("'", "")}');", true);
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
                    // Change 'Account_ID' to 'AccountNum' if that is your actual column name
                    string query = "SELECT AccountNum FROM [Account] WHERE Email=@Email";

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
                // Error handling
            }
            return accountID;
        }

        // =========================
        // SIGNUP LOGIC
        // =========================
        protected void btnSignup_Click(object sender, EventArgs e)
        {
            string fname = txtFirstName.Text.Trim();
            string lname = txtLastName.Text.Trim();
            string email = txtSignupEmail.Text.Trim();
            string contact = txtContact.Text.Trim();
            string password = txtSignupPassword.Text;
            string churchID = txtChurchID.Text.Trim();

            string role = rbVolunteer.Checked ? "Volunteer" : "Organizer";

            // Simple validation
            if (string.IsNullOrEmpty(fname) || string.IsNullOrEmpty(lname) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contact) ||
                string.IsNullOrEmpty(password))
            {
                lblSignupMessage.Text = "Please fill in all required fields.";
                // Re-open the modal after postback
                ClientScript.RegisterStartupScript(this.GetType(), "showSignup", "showSignup();", true);
                return;
            }

            if (rbOrganizer.Checked && string.IsNullOrEmpty(churchID))
            {
                lblSignupMessage.Text = "Organizer must provide a Church ID.";
                // Re-open the modal after postback
                ClientScript.RegisterStartupScript(this.GetType(), "showSignup", "showSignup();", true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"INSERT INTO [Account]
                (Lastname, Firstname, Email, ContactNum, AccPassword, AccRole, ChurchID)
                VALUES
                (@Last,@First,@Email,@Contact,@Pass,@Role,@ChurchID)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Last", lname);
                        cmd.Parameters.AddWithValue("@First", fname);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Contact", contact);
                        cmd.Parameters.AddWithValue("@Pass", password);
                        cmd.Parameters.AddWithValue("@Role", role);
                        cmd.Parameters.AddWithValue("@ChurchID", string.IsNullOrEmpty(churchID) ? DBNull.Value : (object)churchID);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Clear form and show success alert
                txtFirstName.Text = txtLastName.Text = txtSignupEmail.Text = "";
                txtContact.Text = txtSignupPassword.Text = txtChurchID.Text = "";
                rbVolunteer.Checked = true;
                rbOrganizer.Checked = false;

                lblSignupMessage.Text = "";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Sign up successful!');closeSignup();", true);
            }
            catch (Exception ex)
            {
                lblSignupMessage.Text = "Error: " + ex.Message;
                // Re-open the modal so user can see the error
                ClientScript.RegisterStartupScript(this.GetType(), "showSignup", "showSignup();", true);
            }
        }
    }
}

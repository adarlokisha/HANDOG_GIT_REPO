using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Web.UI;

namespace Handog.web
{
    public partial class _default : Page
    {
        private string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // No specific Page_Load logic needed for modal/login
        }

        // =========================
        // LOGIN LOGIC
        // =========================
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter email and password.');", true);
                return;
            }

            string role = GetUserRole(email, password);

            if (!string.IsNullOrEmpty(role))
            {
                Session["UserEmail"] = email;
                Session["UserRole"] = role;

                // Store both the database AccountNum and the user-facing AccountID (like A000#)
                Session["AccountNum"] = GetAccountNum(email);
                Session["AccountID"] = GetUserFacingAccountID(email);

                if (role == "Organizer")
                    Response.Redirect("~/org/home.aspx");
                else
                    Response.Redirect("~/web/home.aspx");
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid credentials.');", true);
            }
        }

        private string GetUserRole(string email, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT AccRole FROM [Account] WHERE Email=@Email AND AccPassword=@Password", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Login Error: {ex.Message.Replace("'", "")}');", true);
                return null;
            }
        }

        private int GetAccountNum(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand("SELECT AccountNum FROM [Account] WHERE Email=@Email", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                return 0; // default/fallback
            }
        }

        private string GetUserFacingAccountID(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand("SELECT Account_ID FROM [Account] WHERE Email=@Email", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
            catch
            {
                return null;
            }
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

            // Validation
            if (string.IsNullOrEmpty(fname) || string.IsNullOrEmpty(lname) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contact) ||
                string.IsNullOrEmpty(password))
            {
                lblSignupMessage.Text = "Please fill in all required fields.";
                ClientScript.RegisterStartupScript(this.GetType(), "showSignup", "showSignup();", true);
                return;
            }

            if (rbOrganizer.Checked && string.IsNullOrEmpty(churchID))
            {
                lblSignupMessage.Text = "Organizer must provide a Church ID.";
                ClientScript.RegisterStartupScript(this.GetType(), "showSignup", "showSignup();", true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO [Account] 
                        (Lastname, Firstname, Email, ContactNum, AccPassword, AccRole, ChurchID)
                      VALUES (@Last, @First, @Email, @Contact, @Pass, @Role, @ChurchID)", conn))
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

                // Reset form
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
                ClientScript.RegisterStartupScript(this.GetType(), "showSignup", "showSignup();", true);
            }
        }
    }
}
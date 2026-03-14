using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Handog.org
{
    public partial class home : System.Web.UI.Page
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["HandogDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null || Session["UserRole"].ToString() != "Organizer")
            {
                //Boot them out if they aren't logged in as an Organizer
                Response.Redirect("~/web/default.aspx");
            }

            if (!IsPostBack)
            {
                BindLocaleGrid();
            }


        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/web/default.aspx"); // Go up one level to root for login
        }

        protected void btnBell_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = true;
        }

        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = false;
        }

        // --- ORGANIZER HANDLERS ---

        private void BindLocaleGrid()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Note: Ensure your table columns match: LocaleName, City, Barangay
                string query = "SELECT LocaleName, City, Barangay FROM Locale ORDER BY LocaleName ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        gvLocales.DataSource = dt;
                        gvLocales.DataBind();
                    }
                }
            }
        }


        // Logic to save locale details to Azure DB 
        protected void btnAddLocale_Click(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null) return;

            string stringID = Session["AccountID"].ToString(); // This is "A0001"
            int numericNum = 0;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // 1. Get the actual AccountNum (INT) based on the Account_ID (A0001)
                string getIntQuery = "SELECT AccountNum FROM Account WHERE Account_ID = @AccountID";
                using (SqlCommand cmdGet = new SqlCommand(getIntQuery, conn))
                {
                    cmdGet.Parameters.AddWithValue("@AccountID", stringID);
                    object result = cmdGet.ExecuteScalar();

                    if (result != null)
                    {
                        numericNum = Convert.ToInt32(result);
                    }
                    else
                    {
                        // Handle case where account isn't found
                        return;
                    }
                }

                // 2. Now Insert into Locale using the INT numericNum
                string insertQuery = "INSERT INTO Locale (LocaleName, City, Barangay, AccountNum) " +
                                     "VALUES (@Locale, @City, @Barangay, @AccNum)";

                using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@Locale", txtLocale.Text.Trim());
                    cmdInsert.Parameters.AddWithValue("@City", txtCity.Text.Trim());
                    cmdInsert.Parameters.AddWithValue("@Barangay", txtBarangay.Text.Trim());
                    cmdInsert.Parameters.AddWithValue("@AccNum", numericNum);

                    cmdInsert.ExecuteNonQuery();
                }
            }

            ClearInputs();
            BindLocaleGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void ClearInputs()
        {
            txtLocale.Text = "";
            txtCity.Text = "";
            txtBarangay.Text = "";
        }

    }
}
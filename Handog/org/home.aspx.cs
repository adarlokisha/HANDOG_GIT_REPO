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

        // this is for checking who inputted locale
        // This property stores the ID for the duration of the page session
        private int CurrentUserNumericID
        {
            get { return ViewState["UserNumericID"] != null ? (int)ViewState["UserNumericID"] : -1; }
            set { ViewState["UserNumericID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AccountID"] == null || Session["UserRole"].ToString() != "Organizer")
            {
                //Boot them out if they aren't logged in as an Organizer
                Response.Redirect("~/web/default.aspx");
            }

            if (!IsPostBack)
            { 
                SetUserNumericID();
                BindLocaleGrid();
            }
        }

        private void SetUserNumericID()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT AccountNum FROM Account WHERE Account_ID = @AccountID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", Session["AccountID"].ToString());
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        this.CurrentUserNumericID = Convert.ToInt32(result);
                    }
                }
            }
        }

        //checks if user is the one who added the locale

        protected bool IsOwner(object accountNumFromRow)
        {
            if (accountNumFromRow == DBNull.Value || this.CurrentUserNumericID == -1) return false;

            return this.CurrentUserNumericID.ToString() == accountNumFromRow.ToString();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/web/default.aspx"); // Go up one level to root for login
        }

        // --- ORGANIZER HANDLERS ---

        private void BindLocaleGrid()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Note: Ensure your table columns match: LocaleName, City, Barangay
                string query = "SELECT Locale_ID, AccountNum, LocaleName, City, Barangay FROM Locale ORDER BY LocaleName ASC";
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


        // EDIT LOCALE BUTTON FUNCTION
        protected void gvLocales_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // 1. Single safety check: if argument is empty, stop immediately.
            if (e.CommandArgument == null || string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                return;
            }

            // 2. Extract the ID
            string localeID = e.CommandArgument.ToString();

           
            if (e.CommandName == "EditLocale")
            {
                try
                {
                    hfSelectedLocaleID.Value = localeID;
                    LoadLocaleData(localeID);
                    pnlEditLocaleModal.Visible = true;
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Error loading data: " + ex.Message + "');</script>");
                }
            }
            else if (e.CommandName == "DeleteLocale")
            {
                DeleteLocale(localeID);
            }
        }

        private void LoadLocaleData(string localeID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = "SELECT LocaleName, City, Barangay FROM Locale WHERE Locale_ID = @ID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", localeID);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtEditLocaleName.Text = dr["LocaleName"].ToString();
                        txtEditCity.Text = dr["City"].ToString();
                        txtEditBarangay.Text = dr["Barangay"].ToString();
                    }
                }
            }
        }

        protected void btnUpdateLocale_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Safety: Ensure only the owner can update
                string query = "UPDATE Locale SET LocaleName=@Name, City=@City, Barangay=@Brgy " +
                               "WHERE Locale_ID=@ID AND AccountNum=@AccNum";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", txtEditLocaleName.Text.Trim());
                    cmd.Parameters.AddWithValue("@City", txtEditCity.Text.Trim());
                    cmd.Parameters.AddWithValue("@Brgy", txtEditBarangay.Text.Trim());
                    cmd.Parameters.AddWithValue("@ID", hfSelectedLocaleID.Value);
                    cmd.Parameters.AddWithValue("@AccNum", this.CurrentUserNumericID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            pnlEditLocaleModal.Visible = false;
            BindLocaleGrid();
        }

        protected void btnCancelLocaleEdit_Click(object sender, EventArgs e)
        {
            pnlEditLocaleModal.Visible = false;
        }

        // DELETE LOCALE FUNCTIONSS
        private void DeleteLocale(string localeID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                // Add a check in the WHERE clause: AccountNum must match current user
                string query = "DELETE FROM Locale WHERE Locale_ID = @ID AND AccountNum = @AccNum";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", localeID);
                    cmd.Parameters.AddWithValue("@AccNum", this.CurrentUserNumericID);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Unauthorized: You do not have permission to delete this.');", true);
                    }
                }
            }
            BindLocaleGrid();
        }
        protected void btnBell_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = true;
        }

        protected void btnCloseNotif_Click(object sender, EventArgs e)
        {
            pnlNotifications.Visible = false;
        }
    }   
}
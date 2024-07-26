using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace JobSpeakJobPortal.User
{
    public partial class Login : System.Web.UI.Page
    {
        string str = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlLoginType.SelectedValue == "Admin")
                {
                    AdminLogin();
                }
                else
                {
                    UserLogin();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }

        private void AdminLogin()
        {
            string username = ConfigurationManager.AppSettings["username"];
            string password = ConfigurationManager.AppSettings["password"];

            if (username == txtUserName.Text.Trim() && password == txtPassword.Text.Trim())
            {
                Session["admin"] = username;
                Response.Redirect("../Admin/Dashboard.aspx", false);
            }
            else
            {
                showErrorMsg("Admin");
            }
        }

        private void UserLogin()
        {
            using (SqlConnection con = new SqlConnection(str))
            {
                string query = "SELECT * FROM useer WHERE Username = @Username AND Pass = @Password";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", txtUserName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            Session["user"] = sdr["Username"].ToString();
                            Session["userId"] = sdr["UserId"].ToString();
                            Response.Redirect("Default.aspx", false);
                        }
                        else
                        {
                            showErrorMsg("User");
                        }
                    }
                }
            }
        }

        private void showErrorMsg(string userType)
        {
            lblMsg.Visible = true;
            lblMsg.Text = "<b>" + userType + "</b> credentials are incorrect..!";
            lblMsg.CssClass = "alert alert-danger";
        }
    }
}

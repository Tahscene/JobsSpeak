using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace JobSpeakJobPortal.User
{
    public partial class Profile : System.Web.UI.Page
    {
        private string str = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                ShowUserProfile();
            }
        }

        private void ShowUserProfile()
        {
            using (SqlConnection con = new SqlConnection(str))
            {
                string query = "SELECT UserId, Username, Name, Addres, Mobile, Email, Country, Resum FROM [Useer] WHERE Username = @username";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", Session["user"].ToString());
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            dlProfile.DataSource = dt;
                            dlProfile.DataBind();
                        }
                        else
                        {
                            Response.Write("<script>alert('Please do login again with your latest username');</script>");
                        }
                    }
                }
            }
        }

        protected void dlProfile_ItemCommand(object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "EditUserProfile")
            {
                Response.Redirect("ResumeBuild.aspx?id=" + e.CommandArgument.ToString());
            }
        }
    }
}

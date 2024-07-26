using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JobSpeakJobPortal.Admin
{
    public partial class UserList : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        DataTable dt;
        string str = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null)
            {
                Response.Redirect("../User/Login.aspx");
            }

            if (!IsPostBack)
            {
                ShowUsers();
            }

        }

        private void ShowUsers()
        {
            string query = @"SELECT Row_Number() OVER(ORDER BY (SELECT 1)) AS [Sr.No], UserId, Name, Email, Mobile, Country from Useer";
            using (con = new SqlConnection(str))
            {
                using (cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    sda.Fill(dt);
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
        }
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            ShowUsers();
        }
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GridViewRow row = GridView1.Rows[e.RowIndex];
                int userId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
                using (con = new SqlConnection(str))
                {
                    using (cmd = new SqlCommand("DELETE FROM Useer WHERE UserId = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        con.Open();
                        int r = cmd.ExecuteNonQuery();
                        if (r > 0)
                        {
                            lblMsg.Text = "User deleted successfully!";
                            lblMsg.CssClass = "alert alert-success";
                        }
                        else
                        {
                            lblMsg.Text = "Cannot delete this record!";
                            lblMsg.CssClass = "alert alert-danger";
                        }
                    }
                }
                ShowUsers();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }
        }

    }
}
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System;

namespace JobSpeakJobPortal.User
{
    public partial class ResumeBuild : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader sdr;
        string str = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        string query;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    showUserInfo();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        private void showUserInfo()
        {
            try
            {
                con = new SqlConnection(str);
                string query = "SELECT * FROM Useer WHERE UserId=@UserId";
                cmd = new SqlCommand(query, con);
                string userId = Request.QueryString["id"];

                if (string.IsNullOrEmpty(userId))
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Invalid User ID!";
                    lblMsg.CssClass = "alert alert-danger";
                    return;
                }

                cmd.Parameters.AddWithValue("@UserId", Request.QueryString["id"]);
                con.Open();
                sdr = cmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    if (sdr.Read())
                    {
                        txtUserName.Text = sdr["Username"].ToString();
                        TxtFullName.Text = sdr["Name"].ToString();
                        txtAddress.Text = sdr["Addres"].ToString();
                        txtMobile.Text = sdr["Mobile"].ToString();
                        txtEmail.Text = sdr["Email"].ToString();
                        ddlCountry.SelectedValue = sdr["Country"].ToString();
                        txtTenth.Text = sdr["TenthGrade"].ToString();
                        txtTwelfth.Text = sdr["TwefthGrade"].ToString();
                        txtGraduation.Text = sdr["GraduateGrade"].ToString();
                        txtPostGraduation.Text = sdr["PostGraduateGrade"].ToString();
                        txtPhd.Text = sdr["Phd"].ToString();
                        txtWork.Text = sdr["WorkOn"].ToString();
                        txtExperience.Text = sdr["Experience"].ToString();
                    }
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "User not found!";
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "An error occurred: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString["id"] != null)
                {
                    string concatQuery = string.Empty;
                    string filePath = string.Empty;

                    bool isValid = false;
                    con = new SqlConnection(str);
                    if (fuResume.HasFile)
                    {
                        if (Utils.IsValidExtension4Resume(fuResume.FileName))
                        {
                            concatQuery = "Resum=@resum,";
                            isValid = true;
                        }
                        else
                        {
                            concatQuery = string.Empty;
                            lblMsg.Visible = true;
                            lblMsg.Text = "Please select .doc, .docx, .pdf file for resume!";
                            lblMsg.CssClass = "alert alert-danger";
                        }
                    }
                    else
                    {
                        concatQuery = string.Empty;
                        isValid = true; // Allow update if no new file is uploaded
                    }

                    if (isValid)
                    {
                        query = @"UPDATE Useer SET Username=@Username, Name=@Name, Email=@Email, Mobile=@Mobile, 
                          TenthGrade=@TenthGrade, TwefthGrade=@TwelfthGrade, GraduationGrade=@GraduateGrade, 
                          PostGraduationGrade=@PostGraduateGrade, Phd=@Phd, WorkOn=@WorkOn, Experience=@Experience, 
                          " + concatQuery + @" Addres=@Address, Country=@Country WHERE UserId=@UserId";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Username", txtUserName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Name", TxtFullName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Mobile", txtMobile.Text.Trim());
                        cmd.Parameters.AddWithValue("@TenthGrade", txtTenth.Text.Trim());
                        cmd.Parameters.AddWithValue("@TwelfthGrade", txtTwelfth.Text.Trim());
                        cmd.Parameters.AddWithValue("@GraduateGrade", txtGraduation.Text.Trim());
                        cmd.Parameters.AddWithValue("@PostGraduateGrade", txtPostGraduation.Text.Trim());
                        cmd.Parameters.AddWithValue("@Phd", txtPhd.Text.Trim());
                        cmd.Parameters.AddWithValue("@WorkOn", txtWork.Text.Trim());
                        cmd.Parameters.AddWithValue("@Experience", txtExperience.Text.Trim());
                        cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("@Country", ddlCountry.SelectedValue);
                        cmd.Parameters.AddWithValue("@UserId", Request.QueryString["id"]);

                        if (fuResume.HasFile && Utils.IsValidExtension4Resume(fuResume.FileName))
                        {
                            Guid obj = Guid.NewGuid();
                            filePath = "Resumes/" + obj.ToString() + fuResume.FileName;
                            string serverPath = Server.MapPath("~/Resumes/") + obj.ToString() + fuResume.FileName;
                            fuResume.PostedFile.SaveAs(serverPath);
                            cmd.Parameters.AddWithValue("@resum", filePath);

                            // Log file path for debugging
                            System.Diagnostics.Debug.WriteLine("Resume saved to: " + serverPath);
                        }

                        con.Open();
                        int r = cmd.ExecuteNonQuery();
                        if (r > 0)
                        {
                            lblMsg.Visible = true;
                            lblMsg.Text = "Resume details updated successfully";
                            lblMsg.CssClass = "alert alert-success";
                        }
                        else
                        {
                            lblMsg.Visible = true;
                            lblMsg.Text = "Cannot update the records, please try after sometime..!";
                            lblMsg.CssClass = "alert alert-danger";
                        }
                    }
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Cannot update the records, please try <b>Relogin</b>!";
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Violation of Unique Key Constraint"))
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "<b>" + txtUserName.Text.Trim() + "</b> username already exists, try a new one..!";
                    lblMsg.CssClass = "alert alert-danger";
                }
                else
                {
                    Response.Write("<script>alert('" + ex.Message + "'); </script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "'); </script>");
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

    }
}

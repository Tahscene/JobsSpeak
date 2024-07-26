using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;

namespace JobSpeakJobPortal.User
{
    public partial class JobListing : System.Web.UI.Page
    {
        SqlDataAdapter sda;
        SqlConnection con;
        SqlCommand cmd;
        DataTable dt;
        string str = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                showJobList();
                RBSelectedColorChange();
            }
        }
        private void showJobList()
        {
            if (dt == null)
            {
                con = new SqlConnection(str);
                string query = @"SELECT JobId, Title, Salary, JobType, CompanyName, CompanyImage, Country, State, CreateDate FROM Jobs";
                cmd = new SqlCommand(query, con);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);
            }
            DataList1.DataSource = dt;
            DataList1.DataBind();
            lbljobCount.Text = JobCount(dt.Rows.Count);
        }
        void RBSelectedColorChange()
        {
            if (RadioButtonList1.SelectedItem.Selected == true)
            {
                RadioButtonList1.SelectedItem.Attributes.Add("class", "selectedradio");
            }
        }


        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCountry.SelectedValue != "0")
            {
                con = new SqlConnection(str);
                string query = "SELECT JobId, Title, Salary, JobType, CompanyName, CompanyImage, Country, State, CreateDate FROM Jobs WHERE Country='" + ddlCountry.SelectedValue + "'";
                cmd = new SqlCommand(query, con);
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);
                showJobList();
                RBSelectedColorChange();
            }
            else
            {
                showJobList();
                RBSelectedColorChange();
            }
        }



        string JobCount(int count)
        {
            if (count > 1)
            {
                return "Total <b>" + count + "</b> Jobs found";
            }
            else if (count == 1)
            {
                return "Total <b>" + count + "</b> Job found";
            }
            else
            {
                return "No job found";
            }
        }




        protected string GetImageUrl(Object url)
        {
            string url1 = "";
            if (string.IsNullOrEmpty(url.ToString()) || url == DBNull.Value)
            {
                url1 = "~/Images/No_image.png";
            }
            else
            {
                url1 = string.Format("~/{0}", url);
            }
            return ResolveUrl(url1);
        }
        public static string RelativeDate(DateTime theDate)

        {

            Dictionary<long, string> thresholds = new Dictionary<long, string>();

            int minute = 60;

            int hour = 60 * minute;

            int day = 24 * hour;

            thresholds.Add(60, "{0} seconds ago");

            thresholds.Add(minute * 2, "a minute ago");

            thresholds.Add(45 * minute, "{0} minutes ago");

            thresholds.Add(120 * minute, "an hour ago");

            thresholds.Add(day, "{0} hours ago");

            thresholds.Add(day * 2, "yesterday");

            thresholds.Add(day * 30, "{0} days ago");

            thresholds.Add(day * 365, "{0} months ago");

            thresholds.Add(long.MaxValue, "{0} years ago");

            long since = (DateTime.Now.Ticks - theDate.Ticks) / 10000000;

            foreach (long threshold in thresholds.Keys)

            {

                if (since < threshold)

                {

                    TimeSpan t = new TimeSpan((DateTime.Now.Ticks - theDate.Ticks));

                    return string.Format(thresholds[threshold], (t.Days > 365 ? t.Days / 365 : (t.Days > 0 ? t.Days : (t.Hours > 0 ? t.Hours : (t.Minutes > 0 ? t.Minutes : (t.Seconds > 0 ? t.Seconds : 0))))).ToString());

                }

            }

            return "";

        }


        protected void CheckBoxList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string jobType = selectedCheckBox();

            if (!string.IsNullOrEmpty(jobType))
            {
                // Split jobType into an array of job types
                string[] jobTypes = jobType.Split(',');

                // Create a list of parameter names
                List<string> paramNames = new List<string>();
                for (int i = 0; i < jobTypes.Length; i++)
                {
                    paramNames.Add("@jobType" + i);
                }

                // Join the parameter names with commas
                string inClause = string.Join(",", paramNames);

                using (SqlConnection con = new SqlConnection(str))
                {
                    string query = @"SELECT JobId, Title, Salary, JobType, CompanyName, CompanyImage, Country, State, CreateDate 
                             FROM Jobs 
                             WHERE JobType IN (" + inClause + ")";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameters to the command
                        for (int i = 0; i < jobTypes.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@jobType" + i, jobTypes[i]);
                        }

                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        showJobList();
                        RBSelectedColorChange();
                    }
                }
            }
            else
            {
                showJobList();
            }
        }





        protected void lbFilter_Click(object sender, EventArgs e)
        {
            try
            {
                bool isCondition = false;
                string subquery = string.Empty;
                string JobType = string.Empty;
                string postedDate = string.Empty;
                string addAnd = string.Empty;
                string query = string.Empty;
                List<string> queryList = new List<string>();
                SqlConnection con = new SqlConnection(str);

                if (ddlCountry.SelectedValue != "0")
                {
                    queryList.Add("Country ='" + ddlCountry.SelectedValue + "' ");
                    isCondition = true;
                }

                JobType = selectedCheckBox();

                if (JobType != " ")
                {
                    queryList.Add("JobType IN (" + JobType + ")");
                    isCondition = true;
                }

                if (RadioButtonList1.SelectedValue != "0")
                {
                    postedDate = selectedRadioButton();
                    queryList.Add("Convert(DATE, CreateDate) = " + postedDate);
                    isCondition = true;
                }

                if (isCondition)
                {
                    foreach (string a in queryList)
                    {
                        subquery += a + " and ";
                    }
                    subquery = subquery.Remove(subquery.LastIndexOf("and"), 3);
                    query = "SELECT JobId, Title, Salary, JobType, CompanyName, CompanyImage, Country, State, CreateDate FROM Jobs WHERE " + subquery;
                }
                else
                {
                    query = "SELECT JobId, Title, Salary, JobType, CompanyName, CompanyImage, Country, State, CreateDate FROM Jobs";
                }
                SqlDataAdapter sdr = new SqlDataAdapter(query, con);
                dt = new DataTable();
                sda.Fill(dt);
                showJobList();
                RBSelectedColorChange();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "');</script>");
            }

        }

        string selectedCheckBox()
        {
            string jobType = string.Empty;
            for (int i = 0; i < CheckBoxList1.Items.Count; i++)
            {
                if (CheckBoxList1.Items[i].Selected)
                {
                    jobType += "'" + CheckBoxList1.Items[i].Text + "',"; //Full Time, Remote
                }
            }
            return jobType = jobType.TrimEnd(',');
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RadioButtonList1.SelectedValue != "0")
            {
                string postedDate = string.Empty;
                postedDate = selectedRadioButton();
                con = new SqlConnection(str);
                string query = "@SELECT JobId, Title, Salary, JobType, CompanyName, CompanyImage, Country, State, CreateDate FROM Jobs WHERE convert(date,createDate)" + postedDate + "";
                cmd = new SqlCommand(query, con);
                sda = new SqlDataAdapter(cmd);
                RBSelectedColorChange();

            }
            else
            {
                showJobList();
                RBSelectedColorChange();

            }
        }


        string selectedRadioButton()
        {
            string postedDate = string.Empty;
            DateTime date = DateTime.Today;
            if (RadioButtonList1.SelectedValue == "1")
            {
                postedDate = " between Convert(DATE,'" + date.ToString("yyyy/MM/dd") + "') ";
            }
            else if (RadioButtonList1.SelectedValue == "2")
            {
                postedDate = " between Convert(DATE,'" + DateTime.Now.AddDays(-2).ToString("yyyy/MM/dd") + "') and Convert(DATE,'" + date.ToString("yyyy/MM/dd") + "') ";
            }
            else if (RadioButtonList1.SelectedValue == "3")
            {
                postedDate = " between Convert(DATE,'" + DateTime.Now.AddDays(-3).ToString("yyyy/MM/dd") + "') and Convert(DATE,'" + date.ToString("yyyy/MM/dd") + "') ";
            }
            else if (RadioButtonList1.SelectedValue == "4")
            {
                postedDate = " between Convert(DATE,'" + DateTime.Now.AddDays(-5).ToString("yyyy/MM/dd") + "') and Convert(DATE,'" + date.ToString("yyyy/MM/dd") + "') ";
            }
            else if (RadioButtonList1.SelectedValue == "5")
            {
                postedDate = " between Convert(DATE,'" + DateTime.Now.AddDays(-10).ToString("yyyy/MM/dd") + "') and Convert(DATE,'" + date.ToString("yyyy/MM/dd") + "') ";
            }
            return postedDate;
        }



        protected void lbReset_Click(object sender, EventArgs e)
        {
            ddlCountry.ClearSelection();
            CheckBoxList1.ClearSelection();
            RadioButtonList1.SelectedValue = "0";
            RBSelectedColorChange();
            showJobList();

        }



    }
}
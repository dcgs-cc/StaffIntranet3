﻿using System;
using System.Web.UI.WebControls;
using Cerval_Library;

namespace DCGS_Staff_Intranet2.content.NextTT
{
    public partial class RoomTimetables : System.Web.UI.Page
    {
        static int max_rows = 25;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RadioButtonList1.SelectedIndex = 0;
                SetupList();
            }
        }
        protected void SetupList()
        {
            if (RadioButtonList1.SelectedValue == "Room")
            {
                RoomList rl1 = new RoomList(); rl1.LoadList();

                ListBox_staff.Items.Clear();
                foreach (SimpleRoom r in rl1.m_roomlist)
                {
                    ListItem l = new ListItem(r.m_roomcode,  r.m_RoomID.ToString());
                    ListBox_staff.Items.Add(l);
                }

            }
            if (RadioButtonList1.SelectedValue == "Subjects")
            {
                ListBox_staff.Items.Clear();
                CourseList cl1 = new CourseList(4);
                foreach (Course c in cl1._courses)
                {
                    ListItem l = new ListItem(c.CourseCode, c._CourseID.ToString());
                    ListBox_staff.Items.Add(l);
                }
            }
        }
        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupList();
        }

        protected void ButtonGenerateTT_Click(object sender, EventArgs e)
        {
            string s = "";
            string file_name = Server.MapPath(@"~/App_Data/tt_data.ttd");
            int p1 = 0;
            string[,] data = new string[30, max_rows]; //has the display data
            string[] staff = new string[max_rows];
            int no_staff = 0;
            ListBox_staff.Visible = false;
            RadioButtonList1.Visible = false;
            HeaderDiv.Visible = false;
            Cerval_Configuration c;

            c = new Cerval_Configuration("StaffIntranet_TTD_TimetablePlanDate");
            DateTime date1 = new DateTime();
            try { date1 = System.Convert.ToDateTime(c.Value); }
            catch { date1 = DateTime.Now; }

            TTData tt1 = new TTData();
            c = new Cerval_Configuration("StaffIntranet_UseOldTTDfile");
            if (c.Value.ToUpper() == "TRUE")tt1.Load(file_name, date1, date1, false, false, ""); else tt1.Load_DB(date1);


            if (RadioButtonList1.SelectedValue == "Room")
            {
                foreach (ListItem l in ListBox_staff.Items)
                {
                    if (l.Selected)
                    {
                        s = l.Text;
                        staff[no_staff] = l.Text.Trim().ToUpper(); no_staff++;
                    }
                }
            }

            if (RadioButtonList1.SelectedValue == "Subjects")
            {
                bool found = false;
                foreach (TTData.TT_period t in tt1.periodlist1.m_list)
                {
                    foreach (ListItem l in ListBox_staff.Items)
                    {
                        if (t.SetName.ToUpper().Contains(l.Text.ToUpper().Trim()) && l.Selected)
                        {
                            found = false;
                            s = t.RoomCode.ToUpper().Trim();
                            for (int r = 0; r < no_staff; r++)
                            {
                                if (staff[r] == s)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                staff[no_staff] = s; no_staff++;
                            }
                        }
                    }
                }

            }

            foreach (TTData.TT_period t in tt1.periodlist1.m_list)
            {
                for (int r = 0; r < no_staff; r++)
                {
                    if (t.RoomCode.Trim().ToUpper() == staff[r])
                    {
                        try
                        {
                            p1 = System.Convert.ToInt16(t.PeriodCode);
                        }
                        catch
                        {
                            p1 = 0;//reg periods etc
                        }
                        if (p1 > 0)
                        {
                            data[t.DayNo * 5 + p1, r] = t.SetName + "<br>" + t.StaffCode.Trim();
                        }
                    }
                }
            }
            content0.InnerHtml = GenerateTimetable(data, staff, no_staff);
        }

        protected string GenerateTimetable(string[,] data, string[] staff, int no_staff)
        {
            string s = "<p><TABLE BORDER  class= \"TimetableTable\"   ><tr><TD>Room</td>";
            //only going to do mon-fri  period 1,2,3,4,5
            string[] days = new string[5]; days[0] = "MO "; days[1] = "TU "; days[2] = "WE "; days[3] = "TH "; days[4] = "FR ";
            for (int d = 0; d < 5; d++)
            {
                for (int p = 1; p < 6; p++)
                {
                    s += "<td>" + days[d] + p.ToString() + "</td>";
                }
            }
            s += "</tr>";
            for (int r = 0; r < no_staff; r++)
            {
                s += "<tr>";
                s += "<td>" + staff[r] + "</td>";
                for (int d = 0; d < 5; d++)
                {
                    for (int p = 1; p < 6; p++)
                    {
                        s += "<td>" + data[p + d * 5, r] + "</td>";
                    }
                }
                s += "</tr>";
            }
            s += "</table></p>";
            return s;
        }

    }
}

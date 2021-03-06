using System;
using System.Configuration;
using System.Drawing;
using System.Data.SqlClient;
using System.IO;
using Cerval_Library;

namespace StudentInformation
{
	/// <summary>
	/// Summary description for PersonImagePage.
	/// </summary>
	public partial class PersonImagePage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			Response.ContentType = "image/jpeg";
			int width = 110;
			int height = 140;
			if(Request.QueryString.Count>=3)
			{
				width= int.Parse(Request.QueryString["w"]);
				height=int.Parse(Request.QueryString["h"]);
			}
            string s = " SELECT * FROM dbo.tbl_Core_PeopleImages ";
            s += " WHERE (PersonId = '"+Request.QueryString["id"]+"' ) ";
            s += " ORDER BY ImageDate DESC";
            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            System.Data.SqlTypes.SqlBinary b1 = new System.Data.SqlTypes.SqlBinary();
                            b1 = dr.GetSqlBinary(2);
                            byte[] b3 = new byte[b1.Length];
                            b3 = (byte[])b1;
                            MemoryStream ms1 = new MemoryStream(b3);
                            Bitmap myImage = new Bitmap(ms1);
                            myImage.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        else
                        {
                            Bitmap b = new System.Drawing.Bitmap(width, height);
                            Graphics g = Graphics.FromImage(b);
                            g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), 0, 0, width, height);
                            b.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        dr.Close();
                    }
                }
            }
         

		}
		private bool GetThumbnailImageAbort()
		{
			return false;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}

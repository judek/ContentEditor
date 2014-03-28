using System;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Collections.Generic;



namespace ContentEditor
{
    public partial class _Default : System.Web.UI.Page
    {
        string sMainRootPath = ConfigurationSettings.AppSettings["ContentRootPath"];
        string sPreviewRootPath = ConfigurationSettings.AppSettings["PreviewRootPath"];

        const int MAX_AGE_BEFORE_RIVISION_SAVED_MINUTES = 20;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (false == IsPostBack)
            {
                PopulatePageNameList();
                DropDownListPages_SelectedIndexChanged(null,null);
            }
        }

        enum FileNameMode { Save, SaveRevision, Load };

        void PopulatePageNameList()
        {

            DropDownListPages.Items.Clear();
            
            DirectoryInfo directoryInfo;

            directoryInfo = new DirectoryInfo(Server.MapPath(sMainRootPath));
                FileInfo[] files = directoryInfo.GetFiles("content.*");

                Hashtable h = new Hashtable();
                foreach (FileInfo f in files)
                {
                    String sPageName = (f.Name.Split('.'))[1].ToLower();

                    if (false == h.ContainsKey(sPageName))
                    {
                        h.Add(sPageName, null);
                        DropDownListPages.Items.Add(new ListItem(sPageName));
                    }

                }


                TextBoxEdit.Text = "";

        }


        protected void DropDownListPages_SelectedIndexChanged(object sender, EventArgs e)
        {

            DropDownListSections.Items.Clear();
            
            String sPageName = DropDownListPages.SelectedItem.Text;

            DirectoryInfo directoryInfo;

            directoryInfo = new DirectoryInfo(Server.MapPath(sMainRootPath));
            FileInfo[] files = directoryInfo.GetFiles("content." + sPageName + "*");

            

            foreach (FileInfo f in files)
            {
                String sSectionName = (f.Name.Split('.'))[2].ToLower();
                DropDownListSections.Items.Add(new ListItem(sSectionName));

            }

            DropDownListSections_SelectedIndexChanged(null, null);

        }

        protected void DropDownListSections_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownListRevisions.Items.Clear();


            DirectoryInfo directoryInfo = new DirectoryInfo(Server.MapPath(sMainRootPath));
            string ttt = ("revision." + DropDownListPages.SelectedItem.Text + "." + DropDownListSections.SelectedValue + "*");
            DropDownListRevisions.Items.Add(new ListItem("-CURRENT-"));
            FileInfo[] files = directoryInfo.GetFiles(ttt);
            foreach (FileInfo f in files)
            {

                String sSectionName = (f.Name.Split('.'))[3].ToLower();
                ListItem l = new ListItem();
                DropDownListRevisions.Items.Add(new ListItem(sSectionName));

            }


            DropDownListRevisions_SelectedIndexChanged(null, null);



        }

        protected void DropDownListRevisions_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadContent();
        }


        private string GetFileName(FileNameMode Mode)
        {
            if ((DropDownListRevisions.SelectedIndex == 0) || (Mode == FileNameMode.Save))
                return (sMainRootPath + "/content." + DropDownListPages.SelectedItem.Text + "." + DropDownListSections.SelectedItem.Text + ".txt");
            else
                return (sMainRootPath + "/revision." + DropDownListPages.SelectedItem.Text + "." + DropDownListSections.SelectedItem.Text + "." + DropDownListRevisions.SelectedItem.Text + ".txt");

        }

        private string GetPerviewFileName()
        {
            return (sMainRootPath + "/preview." + DropDownListPages.SelectedItem.Text + "." + DropDownListSections.SelectedItem.Text + ".txt");
        }

        private string GetPerviewPageLink()
        {
            return (sMainRootPath + "/preview." + DropDownListPages.SelectedItem.Text + "." + DropDownListSections.SelectedItem.Text + ".txt");
        }


        void LoadContent()
        {
            string sFileName = GetFileName(FileNameMode.Load).Trim();

            TextBoxEdit.Text = "";

            if (sFileName.Length < 4)
                return; //more than likely bad filename


            StreamReader sr;

            try
            { sr = new StreamReader(Server.MapPath(sFileName)); }
            catch (FileNotFoundException fnf)
            {
                TextBoxEdit.Text = fnf.Message;
                return;
            }

            string line;


            line = sr.ReadLine();
            while (line != null)
            {

                TextBoxEdit.Text += (line + "\r\n");

                line = sr.ReadLine();
            }

            sr.Close();

            ButtonSave.Enabled = true;

        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            string sFileName = GetFileName(FileNameMode.Save);

            //Save last good copy as revision if the current file is at least "a bit" old.
            FileInfo fi = new FileInfo(Server.MapPath(sFileName));

            if (fi.LastWriteTime < DateTime.Now.AddMinutes(-MAX_AGE_BEFORE_RIVISION_SAVED_MINUTES))
            {
                string sRivisonNumber = string.Format(".{0:yyyy-MM-dd-HH-mm-ss}.txt", fi.LastWriteTime);
                string OldRevisionFileName = sFileName.Replace(".txt", (sRivisonNumber));
                OldRevisionFileName = OldRevisionFileName.Replace("content.", "revision.");
                File.Copy(Server.MapPath(sFileName), Server.MapPath(OldRevisionFileName));

            }

            //Save Main content file
            string sNewContent = TextBoxEdit.Text;
            StreamWriter sw = new StreamWriter(Server.MapPath(sFileName));
            sw.Write(sNewContent);
            sw.Close();


            string sUpdatedPage = "http://rivervalleycommunity.org/" + DropDownListPages.SelectedItem.Text + ".aspx";

            string tweet = DropDownListPages.SelectedItem.Text + " page updated " + sUpdatedPage;
            
            if(false == CheckBoxDontTweet.Checked)
                AddTweet(tweet);

                
            DropDownListRevisions.SelectedIndex = 0;


        }

        protected void ButtonPreview_Click(object sender, EventArgs e)
        {
            string sFileName = GetPerviewFileName();

            sFileName.Replace("content", "preview");

            StreamWriter sw = new StreamWriter(Server.MapPath(sFileName));
            FileInfo f = new FileInfo(Server.MapPath(sFileName));

            string[] fileParts = f.Name.Split('.');
            string sPreviewSection = "";
            if (fileParts.Length == 4)
            {
                if (fileParts[3].EndsWith("txt"))
                {
                    sPreviewSection = fileParts[2];
                }
            }
            
            
            sw.Write(TextBoxEdit.Text);

            sw.Close();


            string sPreviewPath = sPreviewRootPath + DropDownListPages.SelectedItem.Text +".aspx";

            StringBuilder sb = new StringBuilder();
            sb.Append("<script>");
            //sb.Append("window.open('" + sPreviewPath + "?preview=yes&section=', '', '');");
            sb.Append("window.open('" + sPreviewPath + "?preview=yes&section=" + sPreviewSection + "', '', '');");
            sb.Append("</script>");
            
            Page.RegisterStartupScript("test", sb.ToString());

        }

        protected void AddTweet(string tweet)
        {
            if (null == Session["Tweets"])
                Session["Tweets"] = new List<string>();

            List<string> tweets = Session["Tweets"] as List<string>;

            if (null != tweets.Find(delegate(string t) { return t.ToLower() == tweet.ToLower(); }))
                return;


            tweets.Add(tweet);

        }
        
   
    }
}

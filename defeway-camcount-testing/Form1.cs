using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace defeway_camcount_testing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string cookiePub;
        public string camfinal;
        public string pCamCount;
        public string exepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\img\";
        string inputSafeFileName;
        string inputFileName;
        string[] lines;

        public class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                var req = base.GetWebRequest(address);
                req.Timeout = 5000;
                return req;
            }
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(100);
        }

        public void TestThread(string[] uris)
        {
            foreach(string s in uris){
                Download(s);
            }
        }

        public async void Download(string uri)
        {
            string camcount;

            try
            {
                camcount = await GetCamCount2(new Uri(uri));
                int count = Int32.Parse(camcount);
                for(int i = 0; i == count; i++)
                {

                }
            }
            catch
            {
                //v bad uri
            }
        }

        public async Task<String> GetCamCount2(Uri uri)
        {
            //test if host is alive
            WebRequest request = WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                return ("BADURI");
            }

            WebBrowser w = new WebBrowser();
            string cookie = "null";
            w.Navigate(uri);
            while (cookie == "null")
            {
                try
                {
                    cookie = w.Document.Cookie;
                }
                catch
                {
                    //was unable to get cookie
                    cookie = "null";
                }
                cookie = w.Document.Cookie;
                if (string.IsNullOrEmpty(cookie))
                {
                    cookie = "null";
                }
                await PutTaskDelay();

                if(!(cookie == "null"))
                {
                    cookiePub = cookie;
                    string[] cookieAr = cookie.Split(';');
                    foreach(string s in cookieAr)
                    {
                        if (s.Contains("dvr_camcnt"))
                        {
                            pCamCount = s;
                            pCamCount = pCamCount.Replace("dvr_camcnt", "");
                            pCamCount = pCamCount.Replace("=", "");
                            return (pCamCount);
                        }
                    }
                    return ("idk");
                }
            }
            return ("idk");
        }
        
        public void Finalprint()
        {
            //Console.WriteLine("camfinal: " + cookiePub);
            Console.WriteLine("\n\n" + pCamCount);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread a = new Thread(() => Download(textBox1.Text));
            
            Console.WriteLine(a.ThreadState);
            a.IsBackground = true;
            a.Start();
            Console.WriteLine(a.ThreadState);

            //Console.WriteLine(cookiePub);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    exepath = fbd.SelectedPath + "/";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Input File";
                dlg.Filter = "Text Files | *.txt";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    inputSafeFileName = dlg.SafeFileName;
                    inputFileName = dlg.FileName;
                    lines = System.IO.File.ReadAllLines(inputFileName);
                }
            }
        }
    }
}

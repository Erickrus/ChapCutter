using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ChapCutter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.tbxOutputDirectory.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.tbxFilename.Text = openFileDialog1.FileName;
                String f = tbxFilename.Text.Substring(tbxFilename.Text.LastIndexOf(@"\") + 1);
                String name = f.Substring(0, f.LastIndexOf("."));
                String suffix = f.Substring(f.LastIndexOf("."));
                this.tbxOutputFilename.Text = name + "{0}" + suffix;

                this.tbxOutputDirectory.Text = tbxFilename.Text.Substring(0,tbxFilename.Text.LastIndexOf(@"\") );


                btnOutput.Enabled = false;
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            if (!tbxFilename.Text.Equals(""))
            {
                AnalyzeFile(tbxFilename.Text);
                btnOutput.Enabled = true;
            }
        }

        ArrayList fLines = new ArrayList();
        private void AnalyzeFile(String filename)
        {
            this.lblStatus.Text = "分析中...";
            this.lvPreview.Items.Clear();


            StreamReader sr = new StreamReader(filename, Encoding.GetEncoding("GB2312"));
            String line;
            int i = 0;
            fLines = new ArrayList();

            while ((line = sr.ReadLine()) != null)
            {

                Match m = Regex.Match(line, tbxRegularExpression.Text);
                if (m.Success)
                {
                    ListViewItem lvi = new ListViewItem(line);
                    lvi.Text = line;
                    lvi.Checked = true;
                    lvi.SubItems.Add(i + "");
                    lvPreview.Items.Add(lvi);
                }
                i++;
                fLines.Add(line);
            }
            sr.Close();
            this.lblStatus.Text = "分析已完成";
        }


        private String zeroleading(int i)
        {
            if (i < 10) return "0" + i;
            else return "" + i;
        }
        private void btnOutput_Click(object sender, EventArgs e)
        {
            

            DumpFile();
            this.lblStatus.Text = "输出已完成";
        }

        private void DumpFile()
        {
            ArrayList lines = new ArrayList();
            lines.Add(0);
            for (int i = 0; i < lvPreview.Items.Count; i++)
            {
                if (lvPreview.Items[i].Checked)
                {
                    int p = int.Parse(lvPreview.Items[i].SubItems[1].Text);

                    if (p!=0)
                    lines.Add(p);
                }
            }


            int EveryNChapter = int.Parse(tbxEveryNChapter.Text);
            if (EveryNChapter > 1)
            {
                int j = 0;
                while (j < lines.Count)
                {
                    try
                    {
                        for (int k=0;k<EveryNChapter-1;k++)
                        lines.RemoveAt(j + 1);
                    }
                    catch (Exception e) { }
                    j++;
                }
            }


            int linesCount = 0;
            String outputFilename = tbxOutputDirectory.Text + @"\" + string.Format(tbxOutputFilename.Text, zeroleading(1+linesCount));
            StreamWriter sw = new StreamWriter(outputFilename);

            for (int i = 0; i < fLines.Count; i++)
            {
                if (linesCount >= lines.Count) { linesCount = lines.Count - 1; }
                int CurrentLowerBoundary = int.Parse(lines[linesCount].ToString());
                if (i == CurrentLowerBoundary)
                {
                    sw.Close();
                    outputFilename = tbxOutputDirectory.Text + @"\" + string.Format(tbxOutputFilename.Text, zeroleading(1+linesCount++));
                    sw = new StreamWriter(outputFilename);
                }

                if (checkBox1.Checked && fLines[i].Equals(""))
                {
                }
                else
                {
                    sw.WriteLine(fLines[i]);
                }
            }

            sw.Close();


        }
    }
}

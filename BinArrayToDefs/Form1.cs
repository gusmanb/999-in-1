using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BinArrayToDefs
{
    public partial class Form1 : Form
    {
        Regex rxLine = new Regex("%([01]+)(b?)(,?)", RegexOptions.IgnoreCase);
        Regex intLine = new Regex("([0-9]+)", RegexOptions.IgnoreCase);
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isNewline = true;
            StringBuilder sb = new StringBuilder();
            int len = 0;

            string[] lines = checkBox1.Checked ? textBox1.Text.Substring(textBox1.Text.IndexOf("{")).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries) : textBox1.Lines;

            foreach (var line in lines)
            {
                var m = checkBox1.Checked ? intLine.Match(line) : rxLine.Match(line);
                if (m != null && m.Success)
                {
                    int itLen = m.Groups[1].Value.Length;

                    if (isNewline)
                    {
                        isNewline = false;
                        if(itLen <= 8)
                            sb.Append("DEFB ");
                        else
                            sb.Append("DEFW ");
                    }

                    int val = checkBox1.Checked ? int.Parse(m.Groups[1].Value) : Convert.ToInt32(m.Groups[1].Value, 2);

                    if (!checkBox1.Checked && string.IsNullOrWhiteSpace(m.Groups[3].Value))
                    {
                        sb.AppendLine("0" + val.ToString(itLen <= 8 ? "X2" : "X4") + "h");
                        isNewline = true;
                    }
                    else
                        sb.Append("0" + val.ToString(itLen <= 8 ? "X2" : "X4") + "h, ");
                    
                    len += itLen <= 8 ? 1 : 2;

                }     

            }

            if (checkBox1.Checked)
                sb.Remove(sb.Length - 2, 2).Append("\r\n");

            sb.AppendLine($";length: {len}");

            textBox2.Text = sb.ToString();
        }
    }
}

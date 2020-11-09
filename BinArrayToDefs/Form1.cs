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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isNewline = true;
            StringBuilder sb = new StringBuilder();
            int len = 0;

            foreach (var line in textBox1.Lines)
            {
                var m = rxLine.Match(line);
                if (m != null && m.Success)
                {
                    if (isNewline)
                    {
                        isNewline = false;
                        sb.Append("DEFW ");
                    }

                    int val = Convert.ToInt32(m.Groups[1].Value, 2);

                    if (string.IsNullOrWhiteSpace(m.Groups[3].Value))
                    {
                        sb.AppendLine(val.ToString("X4") + "h");
                        isNewline = true;
                    }
                    else
                        sb.Append(val.ToString("X4") + "h, ");
                    
                    len += 2;

                }     

            }

            sb.AppendLine($";length: {len}");

            textBox2.Text = sb.ToString();
        }
    }
}

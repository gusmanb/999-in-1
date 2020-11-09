using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeeperToArray
{
    public partial class Form1 : Form
    {
        double[] table = new double[]
        { 
             261.625565290,
             277.182631135,
             293.664768100,
             311.126983881,
             329.627557039,
             349.228231549,
             369.994422674,
             391.995436072,
             415.304697513,
             440.000000000,
             466.163761616,
             493.883301378
        };

        const string templateArray = "Dim {0} as uInteger({1}) => {{ {2} }}";

        Regex regBeep = new Regex("(BEEP)\\s*?([0-9\\.-]*),\\s*?([0-9\\.-]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        Regex regPause = new Regex("(PAUSE)\\s*?([0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        public Form1()
        {
            InitializeComponent();
        }

        private int[] GetValues(double duration, double pitch)
        {
            /* """Converts duration,pitch to a pair of unsigned 16 bit integers,
            to be loaded in DE,HL, following the ROM listing.
            Returns a t-uple with the DE, HL values.
            """
            intPitch = int(pitch)
            fractPitch = pitch - intPitch  # Gets fractional part
            tmp = 1 + 0.0577622606 * fractPitch
            if not -60 <= intPitch <= 127:
                raise BeepError('Pitch out of range: must be between [-60, 127]')

            if duration < 0 or duration > 10:
                raise BeepError('Invalid duration: must be between [0, 10]')

            A = intPitch + 60
            B = -5 + int(A / 12)  # -5 <= B <= 10
            A %= 0xC  # Semitones above C

            frec = TABLE[A]
            tmp2 = tmp * frec
            f = tmp2 * 2.0 ** B

            DE = int(0.5 + f * duration - 1)
            HL = int(0.5 + 437500.0 / f - 30.125)
            return DE, HL*/

            int intPitch = (int)pitch;
            double fractPitch = pitch - intPitch;
            double tmp = 1 + 0.0577622606 * fractPitch;

            int a = intPitch + 60;
            int b = -5 + (int)(a / 12.0);

            a = a % 0x0c;
            double frec = table[a];
            double tmp2 = tmp * frec;
            double f = tmp2 * Math.Pow(2, b);

            int de = (int)(0.5 + f * duration - 1);
            int hl = (int)(0.5 + 437500.0 / f - 30.125);

            return new int[] { de, hl };



        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox3.Text))
                return;

            List<ushort> data = new List<ushort>();

            foreach (var line in textBox1.Lines)
            {
                var match = regBeep.Match(line);

                if (match == null || !match.Success)
                {
                    match = regPause.Match(line);

                    if (match == null || !match.Success)
                        continue;

                    byte len = byte.Parse(match.Groups[2].Value, System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture);

                    ushort operationPause = (ushort)((2 << 8) | len);
                    data.Add(0);
                    data.Add(len);
                }
                else
                {
                    int pitch = int.Parse(match.Groups[3].Value, System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture);
                    double duration = double.Parse(match.Groups[2].Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);

                    var vals = GetValues(duration, pitch);
                    data.Add((ushort)vals[0]);
                    data.Add((ushort)vals[1]);
                }


            }

            StringBuilder sb = new StringBuilder();

            foreach (int v in data)
                sb.Append($"{v}, ");

            sb.Remove(sb.Length - 2, 2);

            textBox2.Text = string.Format("Dim {0}({1}) as uInteger => {{ {2} }}", textBox3.Text, data.Count, sb.ToString());
        }
    }
}

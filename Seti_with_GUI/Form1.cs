using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Numerics;

namespace Seti_with_GUI
{
    public partial class Form1 : Form
    {
        private float fl, fh, f0, fn, C1, C2, L, R, R0, Rn, Am0, Amn, dt, Time, dE0, dEn, k, MinX, MaxX, MinY, MaxY, w;

        private int Nc, Nd, Nt, FFTArraySize;
        const double pi = 3.14159265, TwoPi = 6.283185307179586;
        float[] V, U, dV, dU, FFTin, FFTout, FFTArray, TimeToSave;
        TextAnnotation annotationTime = new TextAnnotation();

        private void button3_Click(object sender, EventArgs e)
        {
            //ResetButton
            chart1.Series["V"].Points.Clear();
            textBox1.Text = Convert.ToString(10); //fh
            textBox2.Text = Convert.ToString(5); ; //f0
            textBox3.Text = Convert.ToString(0); ; //fn
            textBox4.Text = Convert.ToString(0.01F); //C1
            textBox5.Text = Convert.ToString(0.01F); //R
            textBox6.Text = Convert.ToString(1); //Am0
            textBox7.Text = Convert.ToString(0); //Amn
            textBox8.Text = Convert.ToString(3); //Time
            textBox9.Text = Convert.ToString(1); //fl
            textBox10.Text = Convert.ToString(5); //Nc
            textBox11.Text = Convert.ToString(20); //Nd
            textBox12.Text = Convert.ToString(1); //k
            textBox14.Text = Convert.ToString(0);
            textBox15.Text = Convert.ToString(1);
            textBox16.Text = Convert.ToString(0);
            textBox17.Text = Convert.ToString(1);
            comboBox1.SelectedIndex = 0;
            radioButton1.Select();
            checkBox1.Checked = false;
            checkBox2.Checked = true;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = true;
            checkBox6.Checked = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //BuildButton
            fl = float.Parse(textBox9.Text);
            fh = float.Parse(textBox1.Text);
            f0 = float.Parse(textBox2.Text);
            fn = float.Parse(textBox3.Text);
            C1 = float.Parse(textBox4.Text);
            R = float.Parse(textBox5.Text);
            Am0 = float.Parse(textBox6.Text);
            Amn = float.Parse(textBox7.Text);
            Time = float.Parse(textBox8.Text);
            k = float.Parse(textBox12.Text);
            Nc = Int32.Parse(textBox10.Text);
            Nd = Int32.Parse(textBox11.Text);

            C2 = (float)(C1 / 2 * ((Math.Pow(fh, 2) / Math.Pow(fl, 2)) - 1));
            L = (float)(1 / (Math.Pow(2 * pi * fl, 2) * C2));

            //Characteristic impedance
            w = (float)(2 * pi * f0);
            Complex Z1 = 2 / ((-1) * w * C1), Z2 = 1 / ((-1) * w * C2), Z3 = (-1) * w * L, Rw;
            Rw = Complex.Sqrt((Complex.Pow(Z1, 2) * (Z2 + Z3)) / (2 * Z1 + Z2 + Z3));

            Rn = R0 = (float)Rw.Real * k;
            dt = 1 / (fh * Nd);

            V = new float[Nc + 1];
            dV = new float[Nc + 1];
            U = new float[Nc];
            dU = new float[Nc];

            for (int i = 0; i < Nc; i++) { V[i] = dV[i] = U[i] = dU[i] = 0.0F; }
            V[Nc] = dV[Nc] = 0.0F;

            dt = 1.0F / fh / Nd;
            Nt = (int)(Time / dt);

            FFTin = new float[Nt];
            FFTout = new float[Nt];

            if (comboBox1.SelectedIndex == 4)
            {
                FFTArraySize = (int)(Math.Pow(2.0F, Math.Truncate(Math.Log(FFTin.Length) / Math.Log(2))));
                FFTArray = new float[FFTArraySize];
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                FFTArraySize = (int)(Math.Pow(2.0F, Math.Truncate(Math.Log(FFTout.Length) / Math.Log(2))));
                FFTArray = new float[FFTArraySize];
            }

            if (checkBox4.Checked)
            {
                string[] TemporaryTimeToSave = textBox13.Text.Split(';');

                TimeToSave = new float[TemporaryTimeToSave.Length];

                for (int i = 0; i < TemporaryTimeToSave.Length; i++)
                    TimeToSave[i] = float.Parse(TemporaryTimeToSave[i]);
            }

            chart1.Series["V"].Points.Clear();
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "F2";
            chart1.ChartAreas["ChartArea1"].AxisY.LabelStyle.Format = "F2";
            chart1.Series["V"].ChartType = SeriesChartType.Spline;

            if (!checkBox5.Checked)
            {
                MinX = float.Parse(textBox14.Text);
                MaxX = float.Parse(textBox15.Text);
                chart1.ChartAreas["ChartArea1"].AxisX.Minimum = MinX;
                chart1.ChartAreas["ChartArea1"].AxisX.Maximum = MaxX;
            }
            else if (checkBox5.Checked && radioButton1.Checked) chart1.ChartAreas["ChartArea1"].AxisX.Maximum = Time;
            else if (checkBox5.Checked && radioButton2.Checked) chart1.ChartAreas["ChartArea1"].AxisX.Maximum = Nc;
            else if (checkBox5.Checked && radioButton3.Checked) chart1.ChartAreas["ChartArea1"].AxisX.Maximum = fh * 2;

            if (!checkBox6.Checked)
            {
                MinY = float.Parse(textBox16.Text);
                MaxY = float.Parse(textBox17.Text);
                chart1.ChartAreas["ChartArea1"].AxisY.Minimum = MinY;
                chart1.ChartAreas["ChartArea1"].AxisY.Maximum = MaxY;
            }
            else {
                chart1.ChartAreas["ChartArea1"].AxisY.Maximum = chart1.ChartAreas["ChartArea2"].AxisY.Maximum;
                chart1.ChartAreas["ChartArea1"].AxisY.Minimum = chart1.ChartAreas["ChartArea2"].AxisY.Minimum;
            }

            if (radioButton2.Enabled && radioButton2.Checked)
            {
                chart1.ChartAreas["ChartArea1"].AxisX.Title = "Ячейки";
                annotationTime.Visible = true;
            }
            else annotationTime.Visible = false;

            if (radioButton1.Enabled && radioButton1.Checked) chart1.ChartAreas["ChartArea1"].AxisX.Title = "Время";

            for (int it = 0; it < Nt; it++) //for each cell
            {
                float time = it * dt;

                if (checkBox2.Checked && checkBox3.Checked)
                {
                    dE0 = (float)(Am0 * 2 * pi * f0 * Math.Sin(2 * pi * f0 * time) - Am0 * dPulse(time));
                    dEn = (float)(Amn * 2 * pi * fn * Math.Sin(2 * pi * fn * time) - Amn);
                }
                else if (checkBox2.Checked)
                {
                    //signal
                    dE0 = (float)(Am0 * 2 * pi * f0 * Math.Sin(2 * pi * f0 * time));
                    dEn = (float)(Amn * 2 * pi * fn * Math.Sin(2 * pi * fn * time));
                }
                else if (checkBox3.Checked)
                {
                    //pulse
                    dE0 = (float)(-Am0 * dPulse(time));
                    dEn = (float)(-Amn);
                }
                else break;

                dV[0] += 2.0F / (R0 * C1) * (dE0 - dV[0]) * dt + 2.0F / (L * C1) * (V[1] - V[0] + U[0]) * dt;
                dV[Nc] += 2.0F / (L * C1) * (V[Nc - 1] - V[Nc] - U[Nc - 1]) * dt - 2.0F / (Rn * C1) * (dV[Nc] - dEn) * dt;

                for (int i = 0; i < Nc; i++)
                {
                    dU[i] += 1.0F / (L * C2) * (V[i] - V[i + 1] - U[i]) * dt - 1.0F / (R * C2) * dU[i] * dt;
                    if (i == 0) continue;
                    dV[i] += 1.0F / (L * C1) * (V[i - 1] - 2 * V[i] + V[i + 1] + U[i] - U[i - 1]) * dt;
                } //i-cycle

                if (radioButton2.Enabled && radioButton2.Checked) { chart1.Series["V"].Points.Clear(); }

                for (int i = 0; i < Nc; i++)
                {
                    V[i] += dV[i] * dt;
                    U[i] += dU[i] * dt;

                    if (radioButton2.Checked && radioButton2.Enabled) { 
                        switch (comboBox1.SelectedIndex)
                        {
                            case 0: { chart1.Series["V"].Points.AddXY(i, V[i]); break; } //on С1
                            case 2: { chart1.Series["V"].Points.AddXY(i, U[i]); break; } //on C2
                        }
                    annotationTime.Text = "Current time: " + String.Format("{0:0.000}", (time + dt)) + " sec";
                    }
                } //i-cycle

                V[Nc] += dV[Nc] * dt;

                if (checkBox1.Checked) chart1.Update();

                if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 2)
                    chart1.Series["V"].Color = Color.Blue;
                else if (comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 3)
                    chart1.Series["V"].Color = Color.Red;
                else chart1.Series["V"].Color = Color.Green;

                if ((it % 1) == 0)
                {
                    if ((radioButton1.Checked && radioButton1.Enabled) || !radioButton1.Enabled)
                    {
                        switch (comboBox1.SelectedIndex)
                        {
                            case 0: { chart1.Series["V"].Points.AddXY(time, V[0]); break; }
                            case 1: { chart1.Series["V"].Points.AddXY(time, V[Nc]); break; }
                            case 2: { chart1.Series["V"].Points.AddXY(time, U[0]); break; }
                            case 3: { chart1.Series["V"].Points.AddXY(time, U[Nc - 1]); break; }
                            case 4: { FFTin[it] = V[0]; break; }
                            case 5: { FFTout[it] = V[Nc]; break; }
                        }
                    }
                } //time-cycle

                if (textBox13.Enabled)
                    for (int i = 0; i < TimeToSave.Length; i++)
                        if ((TimeToSave[i] - 2 * dt) < (time) && (TimeToSave[i] - 2 * dt) > (time - dt))
                            chart1.SaveImage("C:\\Users\\Egor\\Desktop\\time" + (TimeToSave[i]) + ".png", ChartImageFormat.Png);
            }

            if (comboBox1.SelectedIndex == 4 || comboBox1.SelectedIndex == 5)
            {
                FFTAnalysis();

                chart1.ChartAreas["ChartArea1"].AxisX.Title = "Частота";

                for (int i = 0; i < FFTArray.Length / 2; i++) //harmonics
                {
                    chart1.Series["V"].ChartType = SeriesChartType.Column;
                    chart1.Series["V"].Points.AddXY(i * (1 / (FFTArraySize * dt)), FFTArray[i]);
                }
            }
        }

        private float Pulse(float T)
        {
            float Start = 0, Stop = 3, fwFront = 1, bwFront = 1;
            float ret = (T >= Start && T <= Stop) ? 1 : 0;
            if (T >= Start && T <= (Start + fwFront))
                if (fwFront > 0.0)
                    ret *= (float)(0.5 * (1.0 - Math.Cos(pi * (T - Start) * fwFront)));
            if (T >= (Stop - bwFront) && T <= Stop)
                if (bwFront > 0.0)
                    ret *= (float)(0.5 * (1.0 - Math.Cos(pi * (Stop - T) * bwFront)));
            return ret;
        }

        private float dPulse(float T)
        {
            float Start = 0, Stop = 3, fwFront = 1, bwFront = 1;
            float ret = 0;
            if ((T >= Start) && (T <= (Start + fwFront)))
                if (fwFront > 0.0)
                    ret = (float)(-0.5 * Math.Sin(pi * (T - Start) / fwFront) * pi / fwFront);
            if ((T >= (Stop - bwFront)) && (T <= Stop))
                if (bwFront > 0.0)
                    ret = (float)(0.5 * Math.Sin(pi * (Stop - T) / bwFront) * pi / bwFront);
            return ret;
        }

        private void FFTAnalysis()
        {
            int i, j, n, m, Mmax, Istp;
            float Tmpr, Tmpi, Wtmp, Theta;
            float Wpr, Wpi, Wr, Wi;
            float[] Tmvl;

            n = FFTArraySize * 2;
            Tmvl = new float[n];

            if (comboBox1.SelectedIndex == 4)
                for (i = 0; i < n; i += 2)
                {
                    Tmvl[i] = 0;
                    Tmvl[i + 1] = FFTin[i / 2];
                }
            else if (comboBox1.SelectedIndex == 5)
                for (i = 0; i < n; i += 2)
                {
                    Tmvl[i] = 0;
                    Tmvl[i + 1] = FFTout[i / 2];
                };

            i = 1; j = 1;
            while (i < n)
            {
                if (j > i)
                {
                    Tmpr = Tmvl[i]; Tmvl[i] = Tmvl[j]; Tmvl[j] = Tmpr;
                    Tmpr = Tmvl[i + 1]; Tmvl[i + 1] = Tmvl[j + 1]; Tmvl[j + 1] = Tmpr;
                }
                i = i + 2; m = FFTArraySize;

                while ((m >= 2) && (j > m)) { j = j - m; m = m >> 1; }

                j = j + m;
            }

            Mmax = 2;
            while (n > Mmax)
            {
                Theta = (float)(-TwoPi / Mmax); Wpi = (float)(Math.Sin(Theta));
                Wtmp = (float)(Math.Sin(Theta / 2)); Wpr = Wtmp * Wtmp * 2;
                Istp = Mmax * 2; Wr = 1; Wi = 0; m = 1;

                while (m < Mmax)
                {
                    i = m; m = m + 2; Tmpr = Wr; Tmpi = Wi;
                    Wr = Wr - Tmpr * Wpr - Tmpi * Wpi;
                    Wi = Wi + Tmpr * Wpi - Tmpi * Wpr;

                    while (i < n)
                    {
                        j = i + Mmax;
                        Tmpr = Wr * Tmvl[j] - Wi * Tmvl[j - 1];
                        Tmpi = Wi * Tmvl[j] + Wr * Tmvl[j - 1];

                        Tmvl[j] = Tmvl[i] - Tmpr; Tmvl[j - 1] = Tmvl[i - 1] - Tmpi;
                        Tmvl[i] = Tmvl[i] + Tmpr; Tmvl[i - 1] = Tmvl[i - 1] + Tmpi;
                        i = i + Istp;
                    }
                }
                Mmax = Istp;
            }

            for (i = 0; i < FFTArraySize; i++)
            {
                j = i * 2;
                FFTArray[i] = (float)(2 * Math.Sqrt(Math.Pow(Tmvl[j], 2) + Math.Pow(Tmvl[j + 1], 2)) / FFTArraySize);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 1:
                case 3:
                    {
                        checkBox1.Enabled = true;
                        radioButton2.Enabled = false;
                        radioButton1.Enabled = true; radioButton1.Checked = true;
                        radioButton3.Enabled = false; radioButton3.Checked = false; break;
                    }
                case 4:
                case 5:
                    {
                        checkBox1.Checked = false; checkBox1.Enabled = false;
                        radioButton2.Enabled = false;
                        radioButton1.Enabled = false;
                        radioButton3.Enabled = true; radioButton3.Checked = true; break;
                    }
                default:
                    {
                        checkBox1.Enabled = true;
                        radioButton2.Enabled = true;
                        radioButton1.Enabled = true; radioButton1.Checked = true;
                        radioButton3.Enabled = false; radioButton3.Checked = false; break;
                    }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Convert.ToString(10); //fh
            textBox2.Text = Convert.ToString(5); ; //f0
            textBox3.Text = Convert.ToString(0); ; //fn
            textBox4.Text = Convert.ToString(0.01F); //C1
            textBox5.Text = Convert.ToString(0.01F); //R
            textBox6.Text = Convert.ToString(1); //Am0
            textBox7.Text = Convert.ToString(0); //Amn
            textBox8.Text = Convert.ToString(3); //Time
            textBox9.Text = Convert.ToString(1); //fl
            textBox10.Text = Convert.ToString(5); //Nc
            textBox11.Text = Convert.ToString(20); //Nd
            textBox12.Text = Convert.ToString(1); //k
            textBox14.Text = Convert.ToString(0);
            textBox15.Text = Convert.ToString(1);
            textBox16.Text = Convert.ToString(0);
            textBox17.Text = Convert.ToString(1);

            comboBox1.SelectedIndex = 0;
            radioButton1.Select();
            checkBox2.Checked = true;
            checkBox5.Checked = true;
            checkBox6.Checked = true;
            textBox13.Enabled = false;
            textBox14.Enabled = false;
            textBox15.Enabled = false;
            textBox16.Enabled = false;
            textBox17.Enabled = false;

            annotationTime.Visible = false;
            annotationTime.Name = "annotationTime";
            annotationTime.X = 69;
            annotationTime.Y = 0;
            annotationTime.Font = new Font("Arial", 10);
            chart1.Annotations.Add(annotationTime);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox13.Enabled) textBox13.Enabled = false;
            else textBox13.Enabled = true;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox14.Enabled && textBox15.Enabled) { textBox14.Enabled = false; textBox15.Enabled = false; }
            else { textBox14.Enabled = true; textBox15.Enabled = true; }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox16.Enabled && textBox17.Enabled) { textBox16.Enabled = false; textBox17.Enabled = false; }
            else { textBox16.Enabled = true; textBox17.Enabled = true; }
        }
    }
}

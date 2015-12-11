using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiFiSavedPassword
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = string.Empty;
            this.Text = Application.ProductName+ " (Version " + Application.ProductVersion + ") ";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                var ssid = GetSSID();
                toolStripProgressBar1.Maximum = ssid.Count;
                int i = 1;
                foreach (var item in GetSSID())
                {

                    listBox1.Items.Add(item + " | " + await GetInfomation(item, true));
                    toolStripStatusLabel1.Text = i.ToString() + "/" + ssid.Count;
                    toolStripProgressBar1.Value = i++;
                }
                SystemSounds.Beep.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
        }

        private ProcessStartInfo CMD(string arg)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            try
            {
                info.Arguments = arg;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.CreateNoWindow = true;
                info.FileName = "cmd.exe";
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return info;
        }

        private List<String> GetSSID()
        {
            List<String> ssid = new List<string>();
            try
            {
                using (Process process = Process.Start(CMD("/C netsh wlan show profiles")))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = string.Empty;
                        while ((result = reader.ReadLine()) != null)
                        {
                            if (result.Contains(": "))
                            {
                                ssid.Add(result.Split(':')[1].Trim());
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ssid;
        }

        private async Task<string> GetInfomation(string ssid, bool passOnly)
        {
            string info = string.Empty;
            try
            {
                using (Process process = Process.Start(CMD("/C netsh wlan show profile name=\"" + ssid + "\" key=clear")))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = string.Empty;
                            if (passOnly)
                            {
                                while ((result = await reader.ReadLineAsync()) != null)
                                {
                                    if (result.Contains("Key Content"))
                                    {
                                        info = result.Split(':')[1].Trim();
                                        break;
                                    }

                                }
                            }
                            else
                            {
                                while ((result = await reader.ReadLineAsync()) != null)
                                {
                                    info += result.Trim() + Environment.NewLine;
                                }
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return info;
        }

        private async  void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = await GetInfomation(listBox1.Items[listBox1.SelectedIndex].ToString().Split('|')[0].Trim(), false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.Items.Count == 0)
                {
                    return;
                }

                saveFileDialog1.FileName = "WiFi-Password-List-" + Guid.NewGuid();
                saveFileDialog1.Filter = "Text Documents|*.txt";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                StreamWriter file = new StreamWriter(saveFileDialog1.FileName);
                foreach (var item in listBox1.Items)
                {
                    await file.WriteLineAsync(item.ToString());
                }
                
                file.Close();
                SystemSounds.Beep.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.TextLength == 0)
                {
                    return;
                }

                saveFileDialog1.FileName = "WiFi-Infomation-" + Guid.NewGuid();
                saveFileDialog1.Filter = "Text Documents|*.txt";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                StreamWriter file = new StreamWriter(saveFileDialog1.FileName);
                await file.WriteAsync(textBox1.Text);
                file.Close();
                SystemSounds.Beep.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("www.cracker.in.th");
                SystemSounds.Beep.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}

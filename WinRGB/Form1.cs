using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;


namespace WinRGB
{
    public partial class Form1 : Form
    {
        public Form1() {
            InitializeComponent();
            this.KeyPreview = true;
        }

        public void RGBRect(int R, int G, int B) {
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, R, G, B));
            System.Drawing.Graphics formGraphics;
            formGraphics = this.CreateGraphics();
            formGraphics.FillRectangle(myBrush, new Rectangle(284, 24, 75, 75));
            myBrush.Dispose();
            formGraphics.Dispose();
        }

        void exportRegistry(string strKey, string filepath)
        {
            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "reg.exe";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.Arguments = "export \"" + strKey + "\" \"" + filepath + "\" /y";
                    proc.Start();
                    string stdout = proc.StandardOutput.ReadToEnd();
                    string stderr = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();
                    MessageBox.Show(this, String.Format("WinRGB Registry Backup: {0}", stdout), "WinRGB", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("Exception: {0}", ex), "WinRGB Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void writeRegistry() {
            try
            {
                RegistryKey rgbKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Colors", true);
                if (rgbKey != null)
                {
                    rgbKey.SetValue("Hilight", String.Format("{0} {1} {2}", textBox1.Text, textBox2.Text, textBox3.Text), RegistryValueKind.String);
                    rgbKey.SetValue("HotTrackingColor", String.Format("{0} {1} {2}", textBox1.Text, textBox2.Text, textBox3.Text), RegistryValueKind.String);
                    rgbKey.Close();
                    MessageBox.Show(this, "All done! Re-log or restart your computer to apply changes.", "WinRGB", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("Exception: {0}", ex), "WinRGB Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (int.TryParse(textBox1.Text, out int res1) && int.TryParse(textBox2.Text, out int res2) && int.TryParse(textBox3.Text, out int res3)) {
                if ((res1 >= 0 && res1 <= 255) && (res2 >= 0 && res2 <= 255) && (res3 >= 0 && res3 <= 255)) {
                    var msgBoxResult = MessageBox.Show(this, String.Format("Are you sure you want to use entered values?\nR:{0} G:{1} B:{2}", textBox1.Text, textBox2.Text, textBox3.Text), "WinRGB", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (msgBoxResult == DialogResult.Yes) {
                        var cwd = System.AppContext.BaseDirectory;
                        MessageBox.Show(this, String.Format("Registry will be backed up into:\n{0}reg_backup.reg", cwd), "WinRGB", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        exportRegistry(@"HKEY_CURRENT_USER\Control Panel\Colors", String.Format(@"{0}reg_backup.reg", cwd));
                        writeRegistry();
                    }
                }
                else {
                    MessageBox.Show(this, "Entered value is not a valid RGB value! 0-255", "Invalid Value", MessageBoxButtons.OK);
                }
            } else {
                MessageBox.Show(this, "Entered value is not a number!", "Invalid Value", MessageBoxButtons.OK);
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }
        public void fixInvalid() {
            int.TryParse(textBox1.Text, out int R);
            int.TryParse(textBox2.Text, out int G);
            int.TryParse(textBox3.Text, out int B);
            if (R > 255) { textBox1.Text = "255"; } else if (R < 0) { textBox1.Text = "0"; }
            if (G > 255) { textBox2.Text = "255"; } else if (G < 0) { textBox2.Text = "0"; }
            if (B > 255) { textBox3.Text = "255"; } else if (B < 0) { textBox3.Text = "0"; }
        }


        private void textBox1_KeyUp(object sender, KeyEventArgs e) {
            if (int.TryParse(textBox1.Text, out int R)) {
                fixInvalid();
                if (R >= 0 && R <= 255) {
                    RGBRect(R, int.Parse(textBox2.Text), int.Parse(textBox3.Text));
                } else {
                    fixInvalid();
                }
            }
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e) {
            if (int.TryParse(textBox2.Text, out int G)) {
                fixInvalid();
                if (G >= 0 && G <= 255) {
                    RGBRect(int.Parse(textBox1.Text), G, int.Parse(textBox3.Text));
                } else {
                    fixInvalid();
                }
            }
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e) {
            if (int.TryParse(textBox3.Text, out int B)) {
                fixInvalid();
                if (B >= 0 && B <= 255) {
                    RGBRect(int.Parse(textBox1.Text), int.Parse(textBox2.Text), B);
                } else {
                    fixInvalid();
                }
            }
        }
    }
}

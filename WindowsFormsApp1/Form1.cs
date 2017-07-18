using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        File_treatment file = null;
        private void button1_Click(object sender, EventArgs e)
        {
            string error_log;
            if(openFileDialog1.ShowDialog()!=DialogResult.Cancel)
            {
                if (openFileDialog1.FileName == "")
                    return;

                try
                {                    
                    FileStream stream = new FileStream(openFileDialog1.FileName, FileMode.Open);
                    file = new File_treatment(openFileDialog1.FileName, comboBox1.Text, comboBox2.Text, numericUpDown1.Value);
                    file.vWrite_MIF_File(file.vRead_File(ref stream, out file.file), file.file,out error_log);
                    if (error_log.Length > 14)
                    {
                        richTextBox1.Text += (error_log + "\r\n" + "Выполнено с ошибками");
                    }
                    else
                    {
                        richTextBox1.Text += "Выполнено без ошибок";
                    }
                    MessageBox.Show("Готово!");
                }
                catch
                {
                    MessageBox.Show("Ошибка при открытии файла.");
                    return;
                }
            }
            return;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked==false)
            {
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                numericUpDown1.Enabled = true;
            }
            else
            {
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                numericUpDown1.Value = 256;
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                numericUpDown1.Enabled = false;
            }
        }
    }
}

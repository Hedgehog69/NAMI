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
        struct file_info
        {
            public byte files;
            public byte data_width;
            public ushort words_in_file;
            public string almost_filename;
        };

        byte[] file;


        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog()!=DialogResult.Cancel)
            {
                if (openFileDialog1.FileName == "")
                    return;

                try
                {                    
                    FileStream stream = new FileStream(openFileDialog1.FileName, FileMode.Open);
                    vWrite_MIF_File(vRead_File(stream, out file), file);
                }
                catch
                {
                    return;
                }
            }
            return;
        }
        private void vRefresh_File_Info(ref file_info finf)
        {
            finf.almost_filename = openFileDialog1.FileName;
            finf.files = Convert.ToByte(comboBox1.Text);
            finf.data_width = Convert.ToByte(comboBox2.Text);
            finf.words_in_file = Convert.ToUInt16(numericUpDown1.Value);

            int index = finf.almost_filename.IndexOf('.');
            finf.almost_filename = finf.almost_filename.Substring(0, index);
        }
        private file_info vRead_File(FileStream stream, out byte[] file)
        {
            file_info finf = new file_info();
            BinaryReader br = new BinaryReader(stream);
            vRefresh_File_Info(ref finf);

            file = new byte[stream.Length];
            for (int i = 0; i < file.Length; i++)
                file[i] = br.ReadByte();

            return finf;
        }
        private void vWrite_MIF_File(file_info finf, byte[] file)
        {
            string end_file = "";
            string error_message = "";
            NumberFormatInfo ni = new NumberFormatInfo();
            if ((finf.words_in_file * finf.data_width * finf.files) < file.Length)
            {
                error_message = "Информация выведена не полностью";
            }

            end_file += ("Width = " + Convert.ToString(finf.data_width) + "\\r\n");
            end_file += ("Depth = " + Convert.ToString(finf.words_in_file) + "\\r\n"+ "\\r\n");

                           

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
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                numericUpDown1.Enabled = false;
            }
        }
    }
}

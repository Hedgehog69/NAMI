using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace WindowsFormsApp1
{
    class File_treatment
    {
        public struct file_info
        {
            public byte files;
            public byte data_width;
            public ushort words_in_file;
            public string almost_filename;
        };
        public File_treatment(OpenFileDialog openFileDialog, ComboBox comboBox01, ComboBox comboBox02, NumericUpDown numericUpDown01)
        {
            openFileDialog1 = openFileDialog;
            comboBox1 = comboBox01;
            comboBox2 = comboBox02;
            numericUpDown1 = numericUpDown01;
        }




        OpenFileDialog openFileDialog1;
        ComboBox comboBox1;
        ComboBox comboBox2;
        NumericUpDown numericUpDown1;
        public byte[] file;



        private void vRefresh_File_Info(ref file_info finf)
        {
            finf.almost_filename = openFileDialog1.FileName;
            finf.files = Convert.ToByte(comboBox1.Text);
            finf.data_width = Convert.ToByte(comboBox2.Text);
            finf.words_in_file = Convert.ToUInt16(numericUpDown1.Value);

            int index = finf.almost_filename.IndexOf('.');
            finf.almost_filename = finf.almost_filename.Substring(0, index);
        }
        public file_info vRead_File(FileStream stream, out byte[] file)
        {
            file_info finf = new file_info();
            BinaryReader br = new BinaryReader(stream);
            vRefresh_File_Info(ref finf);

            file = new byte[stream.Length];
            for (int i = 0; i < file.Length; i++)
                file[i] = br.ReadByte();

            return finf;
        }
        private void vHeader_Creater(file_info finf, ref string end_file)
        {
            end_file += ("Width = " + Convert.ToString(finf.data_width) + "\\r\n");
            end_file += ("Depth = " + Convert.ToString(finf.words_in_file) + "\\r\n" + "\\r\n");
            end_file += "Address_radix = HEX;\\r\n";
            end_file += "DATA_radix = HEX;\\r\n\\r\n";
            end_file += "Content_begin:";
        }
        private void vContent_Formatter(file_info finf, ref string content, ref string error_message)
        {

        }
        private void vEnd_Formatter(ref string end_file, string error_message)
        {
            end_file += "Content_end";
            end_file += "Сообщения:\\r\n";
            end_file += error_message;
        }
        public void vWrite_MIF_File(file_info finf, byte[] file)
        {
            string end_file = "";
            string error_message = "";
            string content = "";
            NumberFormatInfo ni = new NumberFormatInfo();
            if ((finf.words_in_file * finf.data_width * finf.files) < file.Length)
            {
                error_message = "Информация выведена не полностью";
            }

            vHeader_Creater(finf, ref end_file);
            vContent_Formatter(finf, ref content, ref error_message);
            vEnd_Formatter(ref end_file, error_message);
        }
    }
}

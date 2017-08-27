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
        public File_treatment(string filename, string files_amount, string width, decimal words)
        {
            sfile_name = filename;
            sfiles = files_amount;
            swidth = width;
            uwords = words;
        }

        string sfile_name = "";
        string sfiles = "";
        string swidth = "";
        decimal uwords = 0;
        public byte[] file;

        const string Too_Much_Data = "Информация будет выведена не полностью, исходный файл слишком велик.\r\n";
        const string No_Data_For_All_Files = "Размер исходного файла недостаточен для заполнения всех файлов";

        private void vRefresh_File_Info(ref file_info finf)
        {
            finf.almost_filename = sfile_name;
            finf.files = Convert.ToByte(sfiles);
            finf.data_width = Convert.ToByte(swidth);
            finf.words_in_file = Convert.ToUInt16(uwords);

            int index = finf.almost_filename.IndexOf('.');
            finf.almost_filename = finf.almost_filename.Substring(0, index);
        }

        public file_info vRead_File(ref FileStream stream, out byte[] file)
        {
            file_info finf = new file_info();
            BinaryReader br = new BinaryReader(stream);
            vRefresh_File_Info(ref finf);

            file = new byte[stream.Length];
            for (int i = 0; i < file.Length; i++)
                file[i] = br.ReadByte();
            // file = br.ReadBytes((int)stream.Length);
            br.Close();
            stream.Close();
            return finf;
        }

        private void vHeader_Creater(file_info finf, ref string end_file)
        {
            end_file += ("WIDTH = " + Convert.ToString(finf.data_width) + "\r\n");
            end_file += ("DEPTH = " + Convert.ToString(finf.words_in_file) + "\r\n" + "\r\n");
            end_file += "ADDRESS_RADIX = HEX;\r\n";
            end_file += "DATA_RADIX = HEX;\r\n \r\n";
            end_file += "CONTENT_BEGIN\r\n";
        }        

        private void vName_Generator(file_info finf, out string[] filenames)
        {
            filenames = new string[finf.files];

            for (byte i = 0; i < filenames.Length; i++)
            {
                filenames[i] = finf.almost_filename;
            }

            for (byte i = 0; i < filenames.Length; i++)
            {
                filenames[i] += (i.ToString() + ".mif");
            }
        }

        private void vContent_Formatter(file_info finf, ref string content, ref string error_message, int file_number)
        {
            UInt32[] ui32_content = new UInt32[finf.words_in_file];
         
            for (ushort i = 0; i < ui32_content.Length; i++)
            {
                ui32_content[i] = 0xFFFFFFFF;
            }

            uint counter = (uint)(file_number * finf.words_in_file * (finf.data_width / 8));

            switch (finf.data_width)
            {
                case (16):
                    {
                        try
                        {
                            for (ushort i = 0; i < ui32_content.Length; i++)
                            {
                                ui32_content[i] = (UInt32)((file[counter] << 8) | (file[counter + 1]));
                                counter += 2;
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            error_message += No_Data_For_All_Files;
                        }

                        for (int i = 0; i < finf.words_in_file; i++)
                        {
                            content += " " + String.Format("{0:X}", i) + ": "
                                + String.Format("{0:X4}", (ushort)ui32_content[i]) + "\r\n";
                        }

                        break;
                    }
                case (8):
                    {
                        try
                        {
                            for (ushort i = 0; i < ui32_content.Length; i++)
                            {
                                ui32_content[i] = (UInt32)(file[counter]);
                                counter++;
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            error_message += No_Data_For_All_Files;
                        }

                        for (int i = 0; i < finf.words_in_file; i++)
                        {
                            content += " " + String.Format("{0:X}", i) + ": " + String.Format("{0:X2}", (byte)ui32_content[i]) + "\r\n";
                        }                       
                        break;
                    }

                case (32):
                    {
                        try
                        {
                            for (ushort i = 0; i < ui32_content.Length; i++)
                            {
                                ui32_content[i] = (UInt32)((file[counter] << 24) | (file[counter + 1] << 16) | (file[counter + 2] << 8) | (file[counter + 3]));
                                counter += 4;
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            error_message += No_Data_For_All_Files;
                        }

                        for (int i = 0; i < finf.words_in_file; i++)
                        {
                            content += " " + String.Format("{0:X}", i) + ": " + String.Format("{0:X8}", (UInt32)ui32_content[i]) + "\r\n";
                        }

                        break;
                    }
                default:
                    break;
            }
            
        }

        private void vEnd_Formatter(ref string end_file, string error_message)
        {
            end_file += "END;\r\n";
        }

        private void vFile_Writer(string filename, string file)
        {
            FileStream fls = new FileStream(filename, FileMode.Create);
            StreamWriter str = new StreamWriter(fls);
            str.Write(file);
            str.Close();
            fls.Close();
        }

        public void vWrite_MIF_File(file_info finf, byte[] file, out string output_error_message)
        {
            string end_file = "";
            string error_message = "";
            string content = "";
            string end_of_cur_file = "";
            string almost_file = "";
            output_error_message = "";

            if ((finf.words_in_file * finf.data_width * finf.files) < file.Length)
            {
                error_message = Too_Much_Data;
            }
            string[] filenames;
            vName_Generator(finf, out filenames);

            for (int i = 0; i < finf.files; i++)
            {
                vHeader_Creater(finf, ref end_file);
                vContent_Formatter(finf, ref content, ref error_message, i);
                vEnd_Formatter(ref end_of_cur_file, error_message);
                almost_file = end_file + content + end_of_cur_file;

                if (almost_file != null)
                {
                    vFile_Writer(filenames[i], almost_file);
                }

                almost_file = "";
                end_file = "";
                output_error_message += ("\r\n file "+ i.ToString()+") "+error_message + "\r\n");
                if (error_message != Too_Much_Data)
                {
                    error_message = "";
                }
                content = "";
                end_of_cur_file = "";
            }
        }
    }
}

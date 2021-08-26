using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SevenZip;

namespace WorkWithArchiveBF
{
    
    public partial class Form1 : Form
    {
        public string SafeFileName = "";
        public string SafeFileDirect = "";
        public string TempFolder = "";
        public string TrustTempFolder = "";
        public string NotAllowSimbol = "/:*?<>\\|%!@";
        public string curFolderName = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox7.Text.Intersect(NotAllowSimbol).Any())
            {
                MessageBox.Show("Строка не может содержать запрещенные символы");
                textBox7.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                SafeFileName = openFileDialog1.SafeFileName;
                SafeFileDirect = textBox1.Text.Replace(SafeFileName, "");
                SafeFileName = SafeFileName.Replace(Path.GetExtension(textBox1.Text), "");
                textBox8.Text = SafeFileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Выберите путь к архиву");
            }
            if (textBox8.Text == "")
            {
                MessageBox.Show("Введите имя будущего архива");
            }
            if ((textBox8.Text + ".zip") == openFileDialog1.SafeFileName)
            {
                MessageBox.Show("Имя будущего архива не может совпадать с изначальным архивом");
            }
            else
            {
                TempFolder = Path.GetTempPath();
                TempFolder = TempFolder + @"TestFolder";
                TrustTempFolder = TempFolder;

                try
                {
                    Directory.Delete(TrustTempFolder, true);
                }
                catch (Exception ex)
                {}

                SevenZipExtractor.SetLibraryPath(@"C:\Program Files\7-Zip\7z.dll");
                SevenZipExtractor extr = new SevenZipExtractor(textBox1.Text);
                extr.ExtractArchive(TempFolder);
                DirectoryInfo drInfo = new DirectoryInfo(TempFolder);
                List<string> arr = new List<string>();

                GetNewArchive(TempFolder, arr, drInfo);

                SevenZipCompressor szc = new SevenZipCompressor();
                szc.ArchiveFormat = OutArchiveFormat.Zip;
                szc.CompressionMode = SevenZip.CompressionMode.Create;
                szc.CompressFiles(SafeFileDirect + textBox8.Text + ".zip", arr.ToArray());

                Directory.Delete(TrustTempFolder, true);
                MessageBox.Show("Архив успешно сформирован");
            }
        }

        private void GetNewArchive(string TempFolder, List<string> arr, DirectoryInfo drInfo)
        {
            try
            {
                if (drInfo.Exists)
                {
                    
                    DirectoryInfo[] di = drInfo.GetDirectories();
                    foreach (var dir in di)
                    {
                        if (dir.Parent.FullName.ToString() == TrustTempFolder)
                        {
                            bool NeedMove = false;
                            curFolderName = dir.Name;
                            if (textBox2.Text != "" && textBox3.Text != "")
                            {
                                if (curFolderName.Intersect(textBox2.Text).Any())
                                {
                                    curFolderName = curFolderName.Replace(textBox2.Text, textBox3.Text);
                                    NeedMove = true;
                                }
                                
                            }
                            if (checkBox1.Checked == true && textBox4.Text != "")
                            {
                                curFolderName = textBox4.Text + curFolderName;
                                NeedMove = true;
                            }
                            if (checkBox2.Checked == true && textBox5.Text != "")
                            {
                                curFolderName = curFolderName + textBox5.Text;
                                NeedMove = true;
                            }
                            if (checkBox3.Checked == true && textBox6.Text != "")
                            {
                                if (!Directory.Exists(TrustTempFolder + "\\" + textBox6.Text))
                                {
                                    Directory.CreateDirectory(TrustTempFolder + "\\" + textBox6.Text);
                                    TempFolder = TrustTempFolder + "\\" + textBox6.Text;
                                }
                                
                                arr.Add((TempFolder).ToString());
                                NeedMove = true;
                            }
                            string FutureDirectForRecur = TempFolder + "\\" + curFolderName;
                            string DirectForRecur = FutureDirectForRecur;
                            string FutureDirect = TempFolder + "\\" + curFolderName;
                            if (NeedMove == true)
                            {
                                dir.MoveTo(FutureDirect);
                                arr.Add(FutureDirect.ToString());
                            }
                            arr.Add(FutureDirect.ToString());
                            FileInfo[] dirFiles = dir.GetFiles();
                            if (checkBox4.Checked == true && textBox7.Text != "")
                            {
                                if (!Directory.Exists(TempFolder + "\\" + curFolderName + "\\" + textBox7.Text))
                                {
                                    arr.Add((TempFolder + "\\" + curFolderName).ToString());
                                    Directory.CreateDirectory(TempFolder + "\\" + curFolderName + "\\" + textBox7.Text);
                                    FutureDirectForRecur = TempFolder + "\\" + curFolderName + "\\" + textBox7.Text;
                                    arr.Add(FutureDirectForRecur.ToString());
                                }

                                foreach (var dirFile in dirFiles)
                                {
                                    string FutureDirectFile = TempFolder + "\\" + curFolderName + "\\" + textBox7.Text + "\\" + dirFile.Name;
                                    dirFile.MoveTo(FutureDirectFile);
                                    arr.Add(FutureDirectFile.ToString());
                                }
                            }
                            else
                            {
                                foreach (var dirFile in dirFiles)
                                {
                                    arr.Add(dirFile.FullName.ToString());
                                }
                            }
                            GetNewArchiveArr(TempFolder, arr, dir, FutureDirectForRecur, DirectForRecur);
                        }
                    }

  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void GetNewArchiveArr(string TempFolder, List<string> arr, DirectoryInfo drInfo, string FutureDirect, string DirectForRecur)
        {
            try
            {
                if (drInfo.Exists)
                {

                    DirectoryInfo[] di = drInfo.GetDirectories();
                    foreach (var dir in di)
                    {
                        if (dir.FullName != FutureDirect)
                        {
                            if (checkBox4.Checked == true && textBox7.Text != "")
                            {
                                string FutDirectDir = dir.FullName.Replace(DirectForRecur, FutureDirect);
                                dir.MoveTo(FutDirectDir);
                                arr.Add(FutDirectDir.ToString());
                                FileInfo[] dirFiles = dir.GetFiles();
                                foreach (var dirFile in dirFiles)
                                {
                                    arr.Add(dirFile.FullName.ToString());
                                }
                            }
                            else
                            {
                                arr.Add(dir.FullName.ToString());
                                FileInfo[] dirFiles = dir.GetFiles();
                                foreach (var dirFile in dirFiles)
                                {
                                    arr.Add(dirFile.FullName.ToString());
                                }
                            }
                        }
                        
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.ReadOnly = (checkBox1.Checked == false);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text.Intersect(NotAllowSimbol).Any())
            {
                MessageBox.Show("Строка не может содержать запрещенные символы");
                textBox4.Text = "";
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.ReadOnly = (checkBox2.Checked == false);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            textBox6.ReadOnly = (checkBox3.Checked == false);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            textBox7.ReadOnly = (checkBox4.Checked == false);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Intersect(NotAllowSimbol).Any())
            {
                MessageBox.Show("Строка не может содержать запрещенные символы");
                textBox3.Text = "";
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Text.Intersect(NotAllowSimbol).Any())
            {
                MessageBox.Show("Строка не может содержать запрещенные символы");
                textBox5.Text = "";
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox6.Text.Intersect(NotAllowSimbol).Any())
            {
                MessageBox.Show("Строка не может содержать запрещенные символы");
                textBox6.Text = "";
            }
        }

    }
}

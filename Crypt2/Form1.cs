using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Crypt2
{
    public partial class Form1 : Form
    {
        private string fileContent = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileStream = dialog.OpenFile();
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
                if (fileContent.Count() == 10000)
                    setPicture();
                else
                {
                    MessageBox.Show($"Неправильная длина строки {fileContent.Count()}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Ошибка чтения файла", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void setPicture()
        {
            var bitmap = new Bitmap(100, 100);
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    if (fileContent[x * 100 + y] == '1')
                        bitmap.SetPixel(x, y, Color.Black);
                    else if (fileContent[x * 100 + y] == '0')
                        bitmap.SetPixel(x, y, Color.White);
                    else
                        MessageBox.Show("Обнаружен символ отличный от 0 и 1", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            pictureBox1.Image = bitmap;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string compressedFile = compresseFile();
            File.WriteAllText("2.txt", compressedFile);
        }

        private string compresseFile()
        {
            string toReturn = "";
            int numberOfMatches = 0;
            for (int i = 0; i < 10000 / 8;i++)
            {
                toReturn += "11";
                int amount = 0;
                string toCode = fileContent.Substring(i * 8, 8);
                while (amount <= 63 && i+1 < 10000 / 8)
                {
                    string nextCode = fileContent.Substring((i + 1) * 8, 8);
                    if (toCode == nextCode)
                    {
                        i += 1;
                        amount++;
                        
                    }
                    else
                    {
                        numberOfMatches+=amount;
                        break;
                    }
                }
                string binary = SetZeroes(Convert.ToString(amount, 2));
                toReturn += binary;
                toReturn += toCode;
            }
            MessageBox.Show($"Количество совпадений = {numberOfMatches}");
            return toReturn;
        }
        private string SetZeroes(string value)
        {
            string newStr = "";
            for (int i = 0; i < 6 - value.Length; i++)
            {
                newStr += "0";
            }
            newStr += value;
            return newStr;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string compressedFile = string.Empty;
            var dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileStream = dialog.OpenFile();
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    compressedFile = reader.ReadToEnd();
                }
                for (int x = 0; x < compressedFile.Length; x++)
                    if (compressedFile[x] != '1' && compressedFile[x] != '0')
                    {
                        MessageBox.Show("Обнаружен символ отличный от 0 и 1", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                string decompressFile = decompresseFile(compressedFile);
                File.WriteAllText("3.txt", decompressFile);
            }
            else
            {
                MessageBox.Show("Ошибка чтения файла", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private string decompresseFile(string file)
        {
            var toReturn = new StringBuilder();
            int numberOfMatches = 0;
            for(int i=0;i< file.Length / 16; i++)
            {
                //string с = file.Substring(i * 16, 2);
                string counter = file.Substring(i * 16 + 2, 6);
                string code = file.Substring(i * 16 + 8, 8);
                byte b = (byte)(byte.Parse(counter)+1);
                var sb = new StringBuilder();
                for(byte j = 0; j < b; j++)
                {
                    sb.Append(code);
                }
                toReturn.Append(sb.ToString());
                numberOfMatches += (b - 1);
            }
            MessageBox.Show($"Количество совпадений = {numberOfMatches}");
            return toReturn.ToString();
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Convert_Degrees_to_Decimal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text += " : v" + Assembly.GetExecutingAssembly().GetName().Version; // put in the version number
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_openFile_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); 
                openFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Read file into datagridview
                    foreach (var srLine in File.ReadAllLines(openFileDialog.FileName).Skip(0))
                    {
                        dgv_data.Rows.Add(srLine.Split(','));
                    }
                }
            }
        }

        private void btn_convert_Click(object sender, EventArgs e)
        {
            /*
             * Degrees is 38, minutes is 53, and seconds is 21.948.
               Decimal Degrees = 38 + (53/60) + (21.948/3600) = 38.88943
             */
           
            string data = "";

            for (int i = 0; i < dgv_data.RowCount; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    data = dgv_data[j, i].Value.ToString();

                    

                    if (data != "")
                    {
                        dgv_data[j + 2, i].Value = 
                           Math.Round(decimal.Parse(data.Split('°', '\'')[0]) +
                                      (decimal.Parse(data.Split('°', '\'')[1]) / 60) +
                                      (decimal.Parse(data.Split('\'', '"')[1]) / 3600), 5).ToString();

                        if (data.Split('\'', '"')[2] == "W") //west of meridian
                        {
                            dgv_data[j + 2, i].Value = (decimal.Parse(dgv_data[j + 2, i].Value.ToString()) *(- 1)).ToString();
                        }
                    }
                }
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Assembly.GetEntryAssembly().Location;
            saveFileDialog1.Title = "Save csv Files";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "csv";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] output = new string[dgv_data.RowCount];

                for (int i = 0; i < dgv_data.RowCount; i++)
                {
                    for (int j = 0; j < dgv_data.ColumnCount; j++)
                    {
                        dgv_data.CurrentCell = dgv_data[0, 0]; //Move cursor back to cell[0,0] incase it is resting in an updating cell.
                        output[i] += dgv_data.Rows[i].Cells[j].Value + ",";
                    }
                }
                File.WriteAllLines(saveFileDialog1.FileName, output, Encoding.UTF8);
            }
        }
    }
}

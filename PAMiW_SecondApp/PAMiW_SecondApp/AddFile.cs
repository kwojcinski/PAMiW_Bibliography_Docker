using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace PAMiW_SecondApp
{
    public partial class AddFile : Form
    {
        HttpClient client = new HttpClient();
        byte[] bytes = default(byte[]);
        string name;
        public AddFile()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Pdf Files|*.pdf";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                String path = dialog.FileName;
                name = dialog.SafeFileName;
                label3.Text = name;
                using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open), new UTF8Encoding()))
                {
                    using (var memstream = new MemoryStream())
                    {
                        reader.BaseStream.CopyTo(memstream);
                        bytes = memstream.ToArray();
                    }
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_Name.Text))
            {
                MessageBox.Show("Podaj nazwę pliku!");
            }
            else if (bytes == null || bytes.Length == 0)
            {
                MessageBox.Show("Wprowadź plik!");
            }
            else
            {
                string bytestring = Convert.ToBase64String(bytes);
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(UserId.Id), "UserGuid");
                form.Add(new StringContent(txt_Name.Text), "FileName");
                form.Add(new StringContent(bytestring), "PDFbytes");
                HttpResponseMessage response = await client.PostAsync("http://localhost:8081/rest/AddFile", form);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("Pomyślnie dodano!");
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Coś poszło nie tak, spróbuj ponownie!");
                }
            }
        }

        private void AddFile_Load(object sender, EventArgs e)
        {

        }
    }
}

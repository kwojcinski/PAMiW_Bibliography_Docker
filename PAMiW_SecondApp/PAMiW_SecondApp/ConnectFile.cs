using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PAMiW_SecondApp
{
    public partial class ConnectFile : Form
    {
        HttpClient client = new HttpClient();
        public ConnectFile()
        {
            InitializeComponent();
        }

        private async void ConnectFile_Load(object sender, EventArgs e)
        {
            var response = await client.GetAsync(GetFileUrl.Url);
            var data = await response.Content.ReadAsStringAsync();
            List<File> files = JsonConvert.DeserializeObject<List<File>>(data);

            TableLayoutPanel tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.Location = new System.Drawing.Point(219, 50);
            tableLayoutPanel1.Name = "TableLayoutPanel1";
            tableLayoutPanel1.Size = new System.Drawing.Size(366, 300);
            tableLayoutPanel1.TabIndex = 0;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.RowCount = 0;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            tableLayoutPanel1.AutoScroll = true;
            Controls.Add(tableLayoutPanel1);

            var i = 0;
            if (files.Count() == 0)
                Controls.Add(new Label { Text = "Brak plików", AutoSize = false, TextAlign = ContentAlignment.MiddleCenter });
            foreach (File pos in files)
            {
                tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
                Button btn = new Button { Text = "Podłącz", Anchor = AnchorStyles.Left, AutoSize = false };
                tableLayoutPanel1.Controls.Add(new Label { Text = pos.FileName, Anchor = AnchorStyles.Left, AutoSize = false }, 0, i);
                tableLayoutPanel1.Controls.Add(btn, 1, i);
                i++;
                btn.Click += new EventHandler((s, e1) => Mouse_Click(sender, e, GetPositionUrl.Url + pos.Guid));
            }
        }
        private async void Mouse_Click(object sender, EventArgs e, string getLink)
        {
            var confirmResult = MessageBox.Show("Czy jesteś pewny, że chcesz podłączyć ten plik?",
                                     "Tak, podłącz",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, new Uri(getLink));
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Pomyślnie podłączono plik.");
                    this.Hide();
                    ShowPosition fm = new ShowPosition();
                    fm.Show();
                }
                else
                {
                    MessageBox.Show("Coś poszło nie tak. Być może plik jest już podłączony do twojej pozycji.");
                }
            }
            else
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            ShowPosition fm = new ShowPosition();
            fm.Show();
        }
    }
}

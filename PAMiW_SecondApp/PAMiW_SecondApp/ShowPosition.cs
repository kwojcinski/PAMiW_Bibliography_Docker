using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PAMiW_SecondApp
{
    public partial class ShowPosition : Form
    {
        HttpClient client = new HttpClient();
        public ShowPosition()
        {
            InitializeComponent();
        }

        private async void ShowPosition_Load(object sender, EventArgs e)
        {
            TableLayoutPanel tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.Location = new System.Drawing.Point(169, 50);
            tableLayoutPanel1.Name = "TableLayoutPanel1";
            tableLayoutPanel1.Size = new System.Drawing.Size(466, 300);
            tableLayoutPanel1.TabIndex = 0;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.RowCount = 0;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.AutoScroll = true;
            Controls.Add(tableLayoutPanel1);
            var response = await client.GetAsync(GetPositionUrl.PositionUrl);
            var data = await response.Content.ReadAsStringAsync();
            BibliographicPosition position = JsonConvert.DeserializeObject<BibliographicPosition>(data);
            tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
            Button btnC = new Button { Text = "Podłącz plik", Anchor = AnchorStyles.Left, AutoSize = false };
            tableLayoutPanel1.Controls.Add(btnC, 2, 0);
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F)); ;
            tableLayoutPanel1.Controls.Add(new Label { Text = "Tytuł:", Anchor = AnchorStyles.Left, AutoSize = false }, 0, 0);
            tableLayoutPanel1.Controls.Add(new Label { Text = position.Title, Anchor = AnchorStyles.Left, AutoSize = false }, 1, 0);
            tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            tableLayoutPanel1.Controls.Add(new Label { Text = "Autor:", Anchor = AnchorStyles.Left, AutoSize = false }, 0, 1);
            tableLayoutPanel1.Controls.Add(new Label { Text = position.Author, Anchor = AnchorStyles.Left, AutoSize = false }, 1, 1);
            tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            tableLayoutPanel1.Controls.Add(new Label { Text = "Data publikacji:", Anchor = AnchorStyles.Left, AutoSize = false }, 0, 2);
            tableLayoutPanel1.Controls.Add(new Label { Text = position.PublicationDate.Date.ToString(), Anchor = AnchorStyles.Left, AutoSize = false }, 1, 2);
            tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.Controls.Add(new Label { Text = "Pliki:", Anchor = AnchorStyles.Left, AutoSize = false }, 0, 3);
            string urlFile = "";
            string urlConnect = "";
            foreach (Link link in position.Links)
            {
                if (link.Rel == "get-files")
                {
                    urlFile = link.Href;
                }
                if (link.Rel == "connect-file")
                {
                    urlConnect = link.Href;
                }
            }
            btnC.Click += new EventHandler((s, e1) => ConnectFile(sender, e, urlConnect, urlFile));
            if (position.Files.Count() != 0)
            {
                var i = 3;
                string url = "";
                foreach (Link link in position.Links)
                {
                    if (link.Rel == "disconnect-file")
                    {
                        url = link.Href;
                    }
                }
                foreach (File file in position.Files)
                { 
                    tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute,30F));
                    Label label = new Label { Text = file.FileName, ForeColor = Color.Blue, Anchor = AnchorStyles.Left, AutoSize = true };

                    label.Font = new Font(label.Font, FontStyle.Bold);
                    label.Font = new Font(label.Font, FontStyle.Italic);
                    Button btn = new Button { Text = "Odłącz", Anchor = AnchorStyles.Left, AutoSize = false };
                    tableLayoutPanel1.Controls.Add(label, 1, i);
                    tableLayoutPanel1.Controls.Add(btn, 2, i);
                    btn.Click += new EventHandler((s, e1) => DisconnectFile(sender, e, url, file.Guid));
                    foreach (Link link in file.Links)
                    {
                        if (link.Rel == "get-file")
                        {
                            label.Click += new EventHandler((s, e1) => DownloadFile(sender, e, link.Href));
                        }
                    }
                    i++;
                }
            }
            else
            {
                tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 15F));
                tableLayoutPanel1.Controls.Add(new Label { Text = "Brak plików", Anchor = AnchorStyles.Left, AutoSize = true }, 1, 3);
            }
        }
        private void DownloadFile(object sender, EventArgs e, string getLink)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(getLink);
            Process.Start(sInfo);
        }
        private async void DisconnectFile(object sender, EventArgs e, string getLink, string guid)
        {
            var confirmResult = MessageBox.Show("Czy jesteś pewny, że chcesz odłączyć ten plik?",
                         "Tak, odłącz",
                         MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, new Uri(getLink + guid));
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Pomyślnie odłączono plik.");
                    this.Hide();
                    ShowPosition fm = new ShowPosition();
                    fm.Show();
                }
            }
            else
            {

            }
        }
        private void ConnectFile(object sender, EventArgs e, string getLink, string getFiles)
        {
            GetPositionUrl.Url = getLink;
            GetFileUrl.Url = getFiles;
            this.Hide();
            ConnectFile fm = new ConnectFile();
            fm.Show();
        }
    }
}

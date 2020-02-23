using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace PAMiW_SecondApp
{
    public partial class ShowPositions : Form
    {
        HttpClient client = new HttpClient();

        public ShowPositions()
        {
            InitializeComponent();
        }

        private async void ShowPositions_Load(object sender, EventArgs e)
        {
            var response = await client.GetAsync("http://localhost:8081/rest/GetPositions/" + UserId.Id);
            var data = await response.Content.ReadAsStringAsync();
            List<BibliographicPosition> positions = JsonConvert.DeserializeObject<List<BibliographicPosition>>(data);

            TableLayoutPanel tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.Location = new System.Drawing.Point(169, 50);
            tableLayoutPanel1.Name = "TableLayoutPanel1";
            tableLayoutPanel1.Size = new System.Drawing.Size(466, 300);
            tableLayoutPanel1.TabIndex = 0;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.RowCount = 0;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel1.AutoScroll = true;
            Controls.Add(tableLayoutPanel1);

            var i = 0;
            if (positions.Count() == 0)
                Controls.Add(new Label { Text = "Brak pozycji" });
            foreach (BibliographicPosition pos in positions)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
                tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
                Button btn = new Button { Text = "Detale", Anchor = AnchorStyles.Left, AutoSize = false };
                Button btnD = new Button { Text = "Usuń", Anchor = AnchorStyles.Left, AutoSize = false };
                tableLayoutPanel1.Controls.Add(new Label { Text = pos.Title, Anchor = AnchorStyles.Left, AutoSize = false }, 0, i);
                tableLayoutPanel1.Controls.Add(btn, 1, i);
                tableLayoutPanel1.Controls.Add(btnD, 2, i);
                i++;
                foreach(Link link in pos.Links)
                {
                    if (link.Rel == "get-position")
                    {
                        btn.Click += new EventHandler((s, e1) => GetPosition(sender, e, link.Href));
                    }
                    if (link.Rel == "delete-position")
                    {
                        btnD.Click += new EventHandler((s, e1) => DeletePosition(sender, e, link.Href));
                    }
                }
            }
        }
        private void GetPosition(object sender, EventArgs e, string getLink)
        {
            GetPositionUrl.PositionUrl = getLink;
            ShowPosition fm = new ShowPosition();
            fm.Show();
        }
        private async void DeletePosition(object sender, EventArgs e, string getLink)
        {
            var confirmResult = MessageBox.Show("Czy jesteś pewny, że chcesz usunąć tę pozycję?",
             "Tak, usuń",
             MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                var response = await client.DeleteAsync(getLink);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Pomyślnie usunięto pozycję.");
                    this.Hide();
                    ShowPositions fm = new ShowPositions();
                    fm.Show();
                }
            }
            else
            {

            }
        }
    }
}

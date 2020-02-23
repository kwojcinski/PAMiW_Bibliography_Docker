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
    public partial class AddPosition : Form
    {
        HttpClient client = new HttpClient();
        public AddPosition()
        {
            InitializeComponent();
        }

        private async void Add_Click(object sender, EventArgs e)
        {
            string title = txt_Title.Text;
            string author = txt_Author.Text;
            DateTime publicationDate = date_PublicationDate.Value;

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Title", title),
                new KeyValuePair<string, string>("UserGuid", UserId.Id),
                new KeyValuePair<string, string>("Author", author),
                new KeyValuePair<string, string>("PublicationDate", publicationDate.ToString())
            });
            var response = await client.PostAsync("http://localhost:8081/rest/PostPosition", formContent);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Pomyślnie dodano!");
                this.Hide();
            }
            else
            {
                MessageBox.Show("Podano błędne dane!");
            }

        }

        private void AddPosition_Load(object sender, EventArgs e)
        {

        }
    }
}

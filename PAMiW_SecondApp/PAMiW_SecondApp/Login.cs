using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PAMiW_SecondApp
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_Submit_Click(object sender, EventArgs e)
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var db = redis.GetDatabase();
            if (txt_Login.Text == "" || txt_Password.Text == "")
            {
                MessageBox.Show("Podaj login i hasło");
                return;
            }
            if(txt_Login.Text == "admin" && txt_Password.Text == "admin")
            {
                User user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Login = "admin",
                    Email = "admin@admin.pl",
                    Passwd = "admin",
                    Positions = new List<string>(),
                    Files = new List<string>(),
                    Logged = true
                };
                UserId.Id = user.Id;
                var serialized = JsonConvert.SerializeObject(user);
                db.StringSet("user_" + user.Id, serialized);
                MessageBox.Show("Pomyślnie zalogowano!");
                this.Hide();
                Main fm = new Main();
                fm.Show();
            }
            else
            {
                var keys = redis.GetServer("localhost", 6379).Keys();
                foreach (var key in keys)
                {
                    if (key.ToString().StartsWith("user_"))
                    {
                        var x = db.StringGet(key.ToString());
                        var u = JsonConvert.DeserializeObject<User>(x.ToString());
                        if (u.Login.Equals(txt_Login.Text))
                        {
                            if (u.Passwd.Equals(txt_Password.Text))
                            {
                                UserId.Id = u.Id;
                                MessageBox.Show("Pomyślnie zalogowano!");
                                this.Hide();
                                Main fm = new Main();
                                fm.Show();
                                return;
                            }
                            else
                            {
                                MessageBox.Show("Błędny login lub hasło!");
                                return;
                            }

                        }
                    }
                }
                MessageBox.Show("Błędny login lub hasło!");
                return;
            }
        }
    }
}

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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void AddPosition_Click(object sender, EventArgs e)
        {
            AddPosition fm = new AddPosition();
            fm.Show();
        }

        private void ShowPosition_Click(object sender, EventArgs e)
        {
            ShowPositions fm = new ShowPositions();
            fm.Show();
        }

        private void AddFile_Click(object sender, EventArgs e)
        {
            AddFile fm = new AddFile();
            fm.Show();
        }

        private void ShowFiles_Click(object sender, EventArgs e)
        {
            ShowFiles fm = new ShowFiles();
            fm.Show();
        }

    }
}

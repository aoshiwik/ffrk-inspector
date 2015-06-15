using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRKScan.Features.Friends
{
    public partial class FriendListUserControl : UserControl
    {
        public FriendListUserControl()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            autoDataGrid1.SaveToFile();
        }

        internal void OnDataChanged(DataRelations data)
        {
            data.UpdateControl(autoDataGrid1);
            this.toolStripStatusLabel1.Text = string.Format("{0:0.0%} Loaded", data.PercentLoaded());
        }
    }
}

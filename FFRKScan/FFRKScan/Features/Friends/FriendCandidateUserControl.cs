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
    public partial class FriendCandidateUserControl : UserControl
    {
        public FriendCandidateUserControl()
        {
            InitializeComponent();
        }

        internal void OnDataChanged(DataRelations data, IFriendFinder friendFinder)
        {
            if (data != null)
            {
                data.UpdateControl(autoDataGrid1);
                label2.Text = string.Format("Following Similar: ({0:0.0%} of friends loaded)", friendFinder.PercentOfFriendsLoaded());

                var first = data.GetLoadedFriends().FirstOrDefault();
                if (first != null)
                {
                    var similar = friendFinder.GetLoadedFriends().Where(each => each.lead_buddy_soul_strike_name == first.lead_buddy_soul_strike_name);
                    similar.UpdateControl(autoDataGrid2);             
                }
                else
                {
                    autoDataGrid2.Clear();
                }
            }
            else
            {
                autoDataGrid1.Clear();
                autoDataGrid2.Clear();
                label2.Text = "";
            }

        }
    }
}

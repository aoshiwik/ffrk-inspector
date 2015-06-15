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
    public partial class FriendsUserControl : UserControl
    {
        public FriendsUserControl(FriendsFeature feature)
        {
            InitializeComponent();

            feature.PropertyChanged += feature_PropertyChanged;
        }

        private void feature_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var feature = sender as FriendsFeature;

            if (e.PropertyName == FriendsFeature.FriendsDataProperty)
            {
                if (feature.FriendsData != null)
                {
                    following.OnDataChanged(feature.FriendsData.Followees);
                    followers.OnDataChanged(feature.FriendsData.Followers);
                }
            }
            else if (e.PropertyName == FriendsFeature.FoundByFriendIDProperty)
            {
                friendIDControl.OnDataChanged(feature.FoundByFriendID, feature);
            }
        }
    }
}

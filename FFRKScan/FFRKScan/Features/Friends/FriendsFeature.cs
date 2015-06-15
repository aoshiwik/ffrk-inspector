
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRKScan.Features.Friends
{
    // last login 

    interface IFriendFinder
    {
        float PercentOfFriendsLoaded();
        DataTargetProfiles[] GetLoadedFriends();
    }

    public class FriendsFeature : Feature, IFriendFinder
    {
        public const string FriendsDataProperty = "FriendsData";
        public const string FoundByFriendIDProperty = "FoundByFriendID";

        public FollowersAndFollowees FriendsData { get; private set; }


        public DataRelations FoundByFriendID { get; private set; }

        public FriendsFeature()
            : base("Friends")
        {

        }

        [Tamper("/dff/relation/followee_and_follower_list")]
        protected void followee_and_follower_list(Response response)
        {
            FriendsData = response.DeserializeBody<FollowersAndFollowees>();

            NotifyPropertyChanged(FriendsDataProperty);
        }

        [Tamper("/dff/relation/find_by_user_ids")]
        protected void find_by_user_ids(Response response)
        {
            if (FriendsData == null)
            {
                return;
            }

            var obj = response.DeserializeBody<DataRelations>();

            FriendsData.UpdateTargetProfiles(obj);

            NotifyPropertyChanged(FriendsDataProperty);
        }

        [Tamper("/dff/relation/unfollow")]
        protected void unfollow(Response response)
        {
            if (FriendsData == null)
            {
                return;
            }

            var res = response.DeserializeBody<SimpleSuccessData>();

            if (res.IsSuccess)
            {
                var req = response.Request.DeserializeBody<SimpleIdData>();
                FriendsData.Unfollow(req.Id);
                NotifyPropertyChanged(FriendsDataProperty);
            }
        }

        [Tamper("/dff/relation/follow")]
        protected void follow(Response response)
        {
            if (FriendsData == null)
            {
                return;
            }

            var res = response.DeserializeBody<FriendsFollowData>();

            if (res.IsSuccess && !res.HasErrors())
            {
                var req = response.Request.DeserializeBody<SimpleIdData>();
                FriendsData.Follow(req.Id, new[] { FoundByFriendID });
                NotifyPropertyChanged(FriendsDataProperty);
            }
        }

        [Tamper("/dff/relation/find_by_invite_id")]
        protected void find_by_invite_id(Response response)
        {
            var res = response.DeserializeBody<DataRelations>();

            if (res.IsSuccess)
            {
                FoundByFriendID = res;
                NotifyPropertyChanged(FoundByFriendIDProperty);
            }
        }

        public override System.Windows.Forms.Control CreateControl()
        {
            return new FriendsUserControl(this);
        }

        public float PercentOfFriendsLoaded()
        {
            if (FriendsData == null || FriendsData.Followees == null)
            {
                return 0;
            }

            return FriendsData.Followees.PercentLoaded();
        }

        public DataTargetProfiles[] GetLoadedFriends()
        {
            if (FriendsData == null || FriendsData.Followees == null)
            {
                return new DataTargetProfiles[] { };
            }

            return FriendsData.Followees.GetLoadedFriends();
        }
    }
}


using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRKScan.Features.Friends
{
    public enum RelationStatus : byte
    {
        None = 0,
        Follower = 1,
        Following,
        Mutual
    }

    public static class RelationStatusExt
    {
        public static RelationStatus AddFollower(this RelationStatus self)
        {
            return self == RelationStatus.Following ? RelationStatus.Mutual : RelationStatus.Follower;
        }
        public static RelationStatus AddFollowing(this RelationStatus self)
        {
            return self == RelationStatus.Follower ? RelationStatus.Mutual : RelationStatus.Following;
        }
    }

    public class FriendsFollowData : SimpleSuccessData
    {

        [JsonProperty("user_relation_errors")]
        public string[] user_relation_errors;

        public bool HasErrors()
        {
            return user_relation_errors != null && user_relation_errors.Length != 0;
        }
    }

    public class DataUserRelations
    {
        [JsonProperty("target_user_id")]
        public ulong target_user_id;
        [JsonProperty("user_id")]
        public ulong user_id;
        [JsonProperty("is_new_relation")]
        public bool is_new_relation;
        [JsonProperty("relation_status")]
        public RelationStatus relation_status;

        public DataRow ToRow(DataTable dt, short index)
        {
            DataRow result = dt.NewRow();

            result["id"] = target_user_id;
            result["#"] = index;
            result["Nickname"] = "";
            result["Job"] = "";
            result["Soul Break"] = "";
            result["Lv"] = (short)0;
            result["ATK"] = (short)0;
            result["MAG"] = (short)0;
            result["MND"] = (short)0;
            result["Soul Break Description"] = "";
            result["Message"] = "";
            result["Relation"] = relation_status;

            return result;
        }

    }

    public class DataTargetProfiles
    {
        [JsonProperty("user_id")]
        public ulong user_id;
        [JsonProperty("lead_buddy_soul_strike_name")]
        public string lead_buddy_soul_strike_name;
        [JsonProperty("lead_buddy_soul_strike_description")]
        public string lead_buddy_soul_strike_description;
        [JsonProperty("profile_message")]
        public string profile_message;
        [JsonProperty("lead_buddy_atk")]
        public short lead_buddy_atk;
        [JsonProperty("lead_buddy_level")]
        public short lead_buddy_level;
        [JsonProperty("lead_buddy_mnd")]
        public short lead_buddy_mnd;
        [JsonProperty("lead_buddy_matk")]
        public short lead_buddy_matk;
        [JsonProperty("nickname")]
        public string nickname;
        [JsonProperty("lead_buddy_job_name")]
        public string lead_buddy_job_name;

        [JsonProperty("relation_status")]
        public RelationStatus relation_status;

        public DataRow ToRow(DataTable dt, short index)
        {
            DataRow result = dt.NewRow();

            result["id"] = user_id;
            result["#"] = index;
            result["Nickname"] = nickname;
            result["Job"] = lead_buddy_job_name;
            result["Soul Break"] = lead_buddy_soul_strike_name;
            result["Lv"] = lead_buddy_level;
            result["ATK"] = lead_buddy_atk;
            result["MAG"] = lead_buddy_matk;
            result["MND"] = lead_buddy_mnd;
            result["Soul Break Description"] = lead_buddy_soul_strike_description;
            result["Message"] = profile_message;
            result["Relation"] = relation_status;
            return result;
        }
    }

    public class DataRelations
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }

        [JsonProperty("SERVER_TIME")]
        public ulong SERVER_TIME;

        [JsonProperty("user_relations")]
        public List<DataUserRelations> UserRelations;

        [JsonProperty("target_profiles")]
        public List<DataTargetProfiles> TargetProfiles;

        public static string GetPrimaryKey()
        {
            return "id";
        }

        public static DataColumn[] GetColumns()
        {
            return new[]
            {  
                new DataColumn("id"){ColumnMapping= MappingType.Hidden},
                new DataColumn("#",typeof(short)),
                new DataColumn("Nickname"),
                new DataColumn("Job"),
                new DataColumn("Soul Break"),
                new DataColumn("Lv",typeof(short)),   
                new DataColumn("ATK",typeof(short)),                
                new DataColumn("MAG",typeof(short)),                
                new DataColumn("MND",typeof(short)),      
                new DataColumn("Soul Break Description"),  
                new DataColumn("Message"),
                new DataColumn("Relation"),
            };
        }

        internal void UpdateRows(DataTable dt)
        {
            DataTargetProfiles[] myRows = null;
            DataUserRelations[] myRelations = null;

            lock (this)
            {
                myRows = TargetProfiles.ToArray();
                myRelations = UserRelations.ToArray();
            }

            List<DataRow> rows = new List<DataRow>();

            for (int i = 0; i < myRelations.Length; i++)
            {
                var found = myRows.FirstOrDefault(each => each.user_id == myRelations[i].target_user_id);

                if (found != null)
                {
                    rows.Add(found.ToRow(dt, (short)(i + 1)));
                }
                else
                {
                    rows.Add(myRelations[i].ToRow(dt, (short)(i + 1)));
                }
            }


            foreach (var item in rows)
            {
                var found = dt.Rows.Find(item["id"]);

                if (found == null)
                {
                    dt.Rows.Add(item);
                }
                else
                {
                    short index = (short)(((short)found["#"]) - 1);
                    dt.Rows.RemoveAt(index);
                    dt.Rows.InsertAt(item, index);
                }
            }
        }

        public void UpdateTargetProfiles(DataRelations other)
        {
            lock (this)
            {
                foreach (var item in other.TargetProfiles)
                {
                    var foundRelation = UserRelations.FirstOrDefault(each => each.target_user_id == item.user_id);

                    if (foundRelation == null)
                    {
                        continue;
                    }

                    var found = TargetProfiles.FirstOrDefault(each => each.user_id == item.user_id);


                    if (found != null)
                    {
                        var i = TargetProfiles.IndexOf(found);
                        TargetProfiles[i] = found;
                    }
                    else
                    {
                        TargetProfiles.Add(item);
                    }
                }
            }
        }

        internal void Remove(ulong id)
        {
            UserRelations.RemoveAll(each => each.target_user_id == id);
            TargetProfiles.RemoveAll(each => each.user_id == id);
        }

        internal void TrySetRelation(ulong id, RelationStatus relationStatus)
        {
            var found1 = UserRelations.FirstOrDefault(each => each.target_user_id == id);

            if (found1 != null)
            {
                found1.relation_status = relationStatus;
            }

            var found2 = TargetProfiles.FirstOrDefault(each => each.user_id == id);

            if (found2 != null)
            {
                found2.relation_status = relationStatus;
            }
        }

        internal void InsertFrontFrom(ulong id, bool addFollowingStatus, params DataRelations[] sources)
        {
            var found1 = UserRelations.FirstOrDefault(each => each.target_user_id == id);

            if (found1 != null)
            {
                return;
            }

            foreach (var item in sources)
            {
                if (item == null)
                {
                    continue;
                }

                found1 = item.UserRelations.FirstOrDefault(each => each.target_user_id == id);

                if (found1 == null)
                {
                    continue;
                }

                if (addFollowingStatus)
                {
                    found1.relation_status = found1.relation_status.AddFollowing();
                }

                UserRelations.Insert(0, found1);

                var found2 = item.TargetProfiles.FirstOrDefault(each => each.user_id == id);

                if (found2 != null)
                {

                    if (addFollowingStatus)
                    {
                        found2.relation_status = found2.relation_status.AddFollowing();
                    }

                    TargetProfiles.Insert(0, found2);
                }

                return;

            }
        }

        internal float PercentLoaded()
        {
          if(UserRelations==null || TargetProfiles==null || UserRelations.Count==0 || TargetProfiles.Count==0)
          {
              return 0;
          }

          return TargetProfiles.Count / (float)UserRelations.Count;
        }

        public DataTargetProfiles[] GetLoadedFriends()
        {
            if (TargetProfiles == null)
            {
                return new DataTargetProfiles[] { };
            }

            return TargetProfiles.ToArray();
        }
    }

    public class FollowersAndFollowees
    {
        [JsonProperty("followees")]
        public DataRelations Followees;

        [JsonProperty("followers")]
        public DataRelations Followers;

        public void UpdateTargetProfiles(DataRelations other)
        {
            Followees.UpdateTargetProfiles(other);
            Followers.UpdateTargetProfiles(other);
        }

        internal void Unfollow(ulong id)
        {
            Followees.Remove(id);
            Followers.TrySetRelation(id, RelationStatus.Follower);
        }

        internal void Follow(ulong id, DataRelations[] sources)
        {
            Followees.InsertFrontFrom(id, true, sources.Concat(new[] { Followers }).ToArray());
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using Fiddler;
using FFRKScan.Features.Friends;


namespace FFRKScan.UnitTestProject
{
    [TestClass]
    public class FriendsUnitTest
    {
        //[TestMethod]
        //public void Standalone()
        //{
        //    var uiPolicy = new StandAloneUIPolicy();
        //    FFRKScanAutoTamper proxy = new FFRKScanAutoTamper(uiPolicy);
        //    proxy.OnLoad();

        //    UnitTestUtils.SimulateResponse(proxy, "Responses\\relation.followee_and_follower_list.json", "/dff/relation/followee_and_follower_list");
        //    UnitTestUtils.SimulateResponse(proxy, "Responses\\relation.find_by_user_ids.json", "/dff/relation/find_by_user_ids");

        //    uiPolicy.Run();
        //}

        //[TestMethod]
        //public void TestFolloweeAndFollowers()
        //{
        //    FFRKScanAutoTamper proxy = new FFRKScanAutoTamper(new NoUIPolicy());
        //    proxy.OnLoad();

        //    var length = -1;
        //    proxy.GetFeature<FriendsFeature>().PropertyChanged += (s,e) =>
        //    {
        //        var followeeAndFollowers = (s as FriendsFeature).FriendsData;
        //        length = followeeAndFollowers.Followees.TargetProfiles.Count;
        //    };

        //    Assert.AreEqual(-1, length);

        //    UnitTestUtils.SimulateResponse(proxy, "Responses\\relation.followee_and_follower_list.json", "/dff/relation/followee_and_follower_list");

        //    Assert.AreEqual(5, length);
        //}     
    }
}

﻿using Dapper;
using HSS.SharedServices.Contacts;
using HSS.SharedServices.Contacts.Contracts;
using HSS.SharedServices.Contacts.Services;
using Microsoft.Data.SqlClient;

namespace HSS.SharedServices.Sql.Contact
{
    public class ContactService : IContactService
    {
        private readonly string connectionString;

        public ContactService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public GetContactResponse GetUserContact(string userId)
        {
            var groups = GetUserGroups(userId);
            var friends = GetUserFriends(userId);

            return new GetContactResponse()
            {
                UserId = userId,

                Contact = new UserContactModel
                {
                    Groups = groups,
                    Friends = friends
                }
            };
        }

        public GetColleagueResponse GetColleagues(string userId, string keywords)
        {

            var friends = GetUserFriends(userId);
            keywords = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(keywords);
            var colleagues = GetUserColleagues(userId, keywords);

            foreach (var item in colleagues)
            {
                item.IsFriend = friends.Any(y => y.UserId == item.UserId);
            }

            var results = new GetColleagueResponse()
            {
                UserId = userId,
                Colleagues = colleagues
            };

            return results;
        }

        private IList<FriendModel> GetUserFriends(string userId)
        {
            using (var connnection = new SqlConnection(connectionString))
            {
                var friendIds = connnection.Query<string>("SELECT FriendId FROM hss.Friends WHERE UserId=@userId", new { userId });
                var results = connnection.Query<FriendModel>("SELECT Id as UserId, Email, NickName, AvatarUrl, TenantId FROM dbo.AspNetUsers WHERE Id IN @ids", new { ids = friendIds });
                return results.ToList();
            }
        }

        private IList<GroupModel> GetUserGroups(string userId)
        {
            using (var connnection = new SqlConnection(connectionString))
            {
                var groupIds = connnection.Query<string>("SELECT GroupId FROM hss.GroupMembers WHERE UserId=@userId", new { userId = userId });
                var results = connnection.Query<GroupModel>("SELECT * FROM hss.Groups WHERE GroupId IN @ids", new { ids = groupIds });

                return results.ToList();
            }
        }

        private IList<ColleagueModel> GetUserColleagues(string userId, string keywords)
        {
            using (var connnection = new SqlConnection(connectionString))
            {
                var currentUserTenantId = connnection.Query<string>("SELECT TenantId FROM dbo.AspNetUsers WHERE Id=@userId", new { userId = userId });
                var results = connnection.Query<ColleagueModel>("SELECT Id as UserId, Email, NickName, AvatarUrl, TenantId FROM dbo.AspNetUsers WHERE TenantId = @tenantId", new { tenantId = currentUserTenantId.First() });
                if (!string.IsNullOrWhiteSpace(keywords)) {
                    results = results.Where(x=>x.Email.Contains(keywords) || x.NickName.Contains(keywords));
                }

                return results.ToList();
            }
        }
    }
}

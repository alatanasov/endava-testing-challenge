using Newtonsoft.Json;
using FluentAssertions;
using RestSharp;
using System.Net;
using ReqResTests.Models;

namespace ReqResTests.Tests
{
    [TestFixture]
    public class UserApiTests : BaseApiTest
    {
        private const string UsersEndpoint = "/api/users";
        private const string DefaultJobTitle = "Senior Test Automation Engineer";

        [Test]
        [Category("Users.List")]
        public void VerifyAllUsersCanBeRetrievedSuccessfully()
        {
            var users = FetchAllUsers();

            users.Should().OnlyContain(u =>
                u.Id > 0 &&
                !string.IsNullOrEmpty(u.Email) &&
                !string.IsNullOrEmpty(u.FirstName) &&
                !string.IsNullOrEmpty(u.LastName));

            var firstUser = users.First();
            TestContext.Out.WriteLine($"Extracted user from list: ID={firstUser.Id}, Email={firstUser.Email}");

            var sortedUsers = users.OrderBy(u => u.FirstName).ToList();
            TestContext.Out.WriteLine("Users sorted by first name:");
            sortedUsers.ForEach(u => TestContext.Out.WriteLine($"{u.FirstName} {u.LastName}"));
        }

        [Test]
        [Category("Users.Single")]
        public void VerifySingleUserDetailsAreReturnedCorrectly()
        {
            var users = FetchAllUsers();
            var userId = users.First().Id;

            var request = new RestRequest($"{UsersEndpoint}/{userId}");
            var response = Client.Execute(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var userResponse = JsonConvert.DeserializeObject<SingleUserResponse>(response.Content!);
            userResponse.Should().NotBeNull();
            userResponse!.Data.Should().NotBeNull();

            var user = userResponse.Data;
            user.Id.Should().Be(userId);
            user.Email.Should().NotBeNullOrEmpty();
            user.FirstName.Should().NotBeNullOrEmpty();
            user.LastName.Should().NotBeNullOrEmpty();
            user.Avatar.Should().NotBeNullOrEmpty();

            TestContext.Out.WriteLine($"Details for user with ID:{user.Id} ");
            TestContext.Out.WriteLine($"Name: {user.FirstName} {user.LastName}");
            TestContext.Out.WriteLine($"Email: {user.Email}");
            TestContext.Out.WriteLine($"Avatar: {user.Avatar}");
        }

        [Test]
        [Category("Users.NonExisting")]
        public void VerifyRequestForNonExistingUserReturnsNotFound()
        {
            const int nonExistingUserId = 9999;
            var request = new RestRequest($"{UsersEndpoint}/{nonExistingUserId}");
            var response = Client.Execute(request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Content.Should().Be("{}");

            TestContext.Out.WriteLine($"Attempted to retrieve user with ID {nonExistingUserId}");
            TestContext.Out.WriteLine($"Response Status: {response.StatusCode}");
            TestContext.Out.WriteLine($"Response Content: {response.Content}");
        }

        [Test]
        [Category("Users.Create")]
        public void VerifyNewUserIsCreatedSuccessfully()
        {
            var uniqueUserName = $"Alex_{Guid.NewGuid():N}".Substring(0, 12);

            var request = new RestRequest(UsersEndpoint, Method.Post);
            request.AddJsonBody(new { name = uniqueUserName, job = DefaultJobTitle });

            var response = Client.Execute(request);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content!);
            responseData.Should().NotBeNull();
            responseData.Should().ContainKey("name").WhoseValue.Should().Be(uniqueUserName);
            responseData.Should().ContainKey("job").WhoseValue.Should().Be(DefaultJobTitle);
            responseData.Should().ContainKey("id");
            responseData.Should().ContainKey("createdAt");

            TestContext.Out.WriteLine("User successfully created! ");
            TestContext.Out.WriteLine($"ID: {responseData["id"]}");
            TestContext.Out.WriteLine($"Name: {responseData["name"]}");
            TestContext.Out.WriteLine($"Job: {responseData["job"]}");
            TestContext.Out.WriteLine($"Created At: {responseData["createdAt"]}");
        }

        [Test]
        [Category("Users.Delete")]
        public void VerifyUserIsDeletedSuccessfully()
        {
            var uniqueUserName = $"Gosho{Guid.NewGuid():N}".Substring(0, 12);
            var userId = CreateUser(uniqueUserName, DefaultJobTitle);

            var deleteRequest = new RestRequest($"{UsersEndpoint}/{userId}", Method.Delete);
            var deleteResponse = Client.Execute(deleteRequest);

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            deleteResponse.Content.Should().BeEmpty();

            TestContext.Out.WriteLine($"Deleted user with ID: {userId}");
            TestContext.Out.WriteLine($"Response Status: {deleteResponse.StatusCode}");
            TestContext.Out.WriteLine($"Response Content: '{deleteResponse.Content}'");


            var verifyRequest = new RestRequest($"{UsersEndpoint}/{userId}");
            var verifyResponse = Client.Execute(verifyRequest);

            verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            TestContext.Out.WriteLine($"Get user with ID:{userId} — Response Status: {verifyResponse.StatusCode}");
        }

        private List<User> FetchAllUsers()
        {
            var request = new RestRequest($"{UsersEndpoint}?page=1");
            var response = Client.Execute(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var users = JsonConvert.DeserializeObject<UserListResponse>(response.Content!);
            users.Should().NotBeNull();
            users!.Data.Should().NotBeEmpty();

            return users.Data;
        }

        private string CreateUser(string name, string job)
        {
            var request = new RestRequest(UsersEndpoint, Method.Post);
            request.AddJsonBody(new { name, job });

            var response = Client.Execute(request);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content!);
            responseData.Should().NotBeNull();

            responseData.Should().ContainKey("id");

            return responseData["id"];
        }
    }
}
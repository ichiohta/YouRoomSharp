using AsyncOAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using YouRoomSharp.Data;
using YouRoomSharp.Serialization;

namespace YouRoomSharp
{
    public class YouRoomClient
    {
        #region Helper Types

        public delegate byte[] ComputeHash(byte[] key, byte[] buffer);
        public delegate Task<string> RetrievePinCode(string authorizeUri);

        #endregion

        #region Constants / Pseudo Constants

        private const string requestTokenUri = "https://www.youroom.in/oauth/request_token";
        private const string authorizeUri = "https://www.youroom.in/oauth/authorize";
        private const string accessTokenUri = "https://www.youroom.in/oauth/access_token";

        private static readonly IEnumerable<KeyValuePair<string, string>> requestTokenParameters =
            new Dictionary<string, string>()
            {
                { "oauth_callback", "oob" }
            };

        private static readonly HttpContent NoContent = null;

        #endregion  

        #region Internal mutable fields

        private readonly string consumerKey;
        private readonly string consumerSecret;
        private readonly RetrievePinCode retrievePin;

        private SemaphoreSlim authorizationLock = new SemaphoreSlim(1, 1);
        private AccessToken accessToken;

        #endregion

        #region .ctor

        public YouRoomClient(string consumerKey, string consumerSecret, ComputeHash hashFunction, RetrievePinCode retrievePin)
        {
            Assert.IsNotNullOrWhiteSpace(consumerKey, nameof(consumerKey));
            Assert.IsNotNullOrWhiteSpace(consumerSecret, nameof(consumerSecret));
            Assert.IsNotNull(hashFunction, nameof(hashFunction));
            Assert.IsNotNull(retrievePin, nameof(retrievePin));

            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.retrievePin = retrievePin;
            this.accessToken = null;

            OAuthUtility.ComputeHash = (key, buffer) => hashFunction(key, buffer);
        }

        #endregion

        #region Properties

        public bool IsAuthorized
        {
            get
            {
                return accessToken != null;
            }
        }

        #endregion

        #region Authorization

        public async Task AuthorizeAsync()
        {
            if (IsAuthorized)
                return;

            await authorizationLock.WaitAsync();

            if (IsAuthorized)
                return;

            try
            {
                var authorizer = new OAuthAuthorizer(consumerKey, consumerSecret);
                var requestTokenResponse = await authorizer.GetRequestToken(requestTokenUri, requestTokenParameters);
                var requestToekn = requestTokenResponse.Token;
                var pinRequestUri = authorizer.BuildAuthorizeUrl(authorizeUri, requestToekn);
                var pinCode = await retrievePin(pinRequestUri);
                var accessTokenResponse = await authorizer.GetAccessToken(accessTokenUri, requestToekn, pinCode);

                accessToken = accessTokenResponse.Token;
            }
            finally
            {
                authorizationLock.Release();
            }
        }

        #endregion

        #region Attachment

        public Task<Stream> ShowAttachmentAsync(int groupParam, int entryId, bool? original = null)
        {
            AssertAuthorized();

            string queryString =
                new Dictionary<string, string>()
                    .AddOrUpdateWhen(original.HasValue && original.Value, "original", original.ToString())
                    .ToQueryString();

            return
                CreateHttpClient()
                    .GetStreamAsync(
                        string.Join(
                            "?",
                            $"https://www.youroom.in/r/{groupParam}/entries/{entryId}/attachment",
                            queryString));
        }

        #endregion

        #region Entry

        public async Task<Entry> CreateEntryAsync(string content, int groupParam, int? parentId = null)
        {
            // Empty string can be a valid content
            Assert.IsNotNull(content, nameof(content));

            HttpContent parameters =
                NewParamSet()
                    .AddOrUpdate("entry[content]", content)
                    .AddOrUpdateWhen(parentId.HasValue, "entry[parent_id]", parentId.ToString())
                    .ToFormUrlEncodedContent();

            return
                await SendRequest(
                    HttpMethod.Post,
                    $"https://www.youroom.in/r/{groupParam}/entries/",
                    parameters,
                    (document) =>
                        new Entry());
        }

        public async Task<Entry> ShowEntryAsync(int groupParam, int entryId)
        {
            AssertAuthorized();

            return
                await SendRequest(
                    HttpMethod.Get,
                    $"https://www.youroom.in/r/{groupParam}/entries/{entryId}/?format=xml",
                    NoContent,
                    (document) =>
                        document.Root
                            .ToEntry());
        }

        public async Task<Entry> DestroyEntryAsync(int groupParam, int entryId)
        {
            AssertAuthorized();

            return
                await SendRequest(
                    HttpMethod.Delete,
                    $"https://www.youroom.in/r/{groupParam}/entries/{entryId}/?format=xml",
                    NoContent,
                    (document) =>
                        document.Root
                            .ToEntry());
        }

        #endregion

        #region Group

        public async Task<IEnumerable<Group>> GetMyGroups()
        {
            AssertAuthorized();

            return
                await SendRequest(
                    HttpMethod.Get,
                    "https://www.youroom.in/groups/my/?format=xml",
                    NoContent,
                    (document) =>
                        document.Root
                            .Elements("group")
                            .Select(element => element.ToGroup()));
        }

        #endregion Group

        #region Timeline

        public async Task<IEnumerable<Entry>> GetHomeTimeline(DateTimeOffset? since = null, bool? flat = false, bool? unreadOnly = null, int? page = null)
        {
            AssertAuthorized();

            string queryString =
                NewParamSet()
                    .AddOrUpdateWhen(since.HasValue, "since", since?.ToRfc3339DateFormat())
                    .AddOrUpdateWhen(flat.HasValue && flat.Value, "flat", flat.ToString())
                    .AddOrUpdateWhen(unreadOnly.HasValue && unreadOnly.Value, "read_state", "unread")
                    .AddOrUpdateWhen(page.HasValue, "page", page.ToString())
                    .ToQueryString();

            return
                await SendRequest(
                    HttpMethod.Get,
                    "https://www.youroom.in/?" + queryString,
                    NoContent,
                    (document) =>
                        document.Root
                            .Elements("entry")
                            .Select(element => element.ToEntry()));
        }

        public async Task<IEnumerable<Entry>> GetRoomTimeline(int groupParam, DateTimeOffset? since = null, string searchQuery = null, bool? flat = false, bool? unreadOnly = null, int? page = null)
        {
            AssertAuthorized();

            string queryString =
                NewParamSet()
                    .AddOrUpdateWhen(since.HasValue, "since", since?.ToRfc3339DateFormat())
                    .AddOrUpdateWhen(!string.IsNullOrEmpty(searchQuery), "search_query", searchQuery)
                    .AddOrUpdateWhen(flat.HasValue && flat.Value, "flat", flat.ToString())
                    .AddOrUpdateWhen(unreadOnly.HasValue && unreadOnly.Value, "read_state", "unread")
                    .AddOrUpdateWhen(page.HasValue, "page", page.ToString())
                    .ToQueryString();

            return
                await SendRequest(
                    HttpMethod.Get,
                    $"https://www.youroom.in/r/{groupParam}/?" + queryString,
                    NoContent,
                    (document) =>
                        document.Root
                            .Elements("entry")
                            .Select(element => element.ToEntry()));
        }

        #endregion

        #region Others

        #endregion

        #region Helper methods

        private void AssertAuthorized()
        {
            Assert.IsTrue(IsAuthorized, () => new InvalidOperationException("Not authorized yet"));
        }

        private HttpClient CreateHttpClient()
        {
            return OAuthUtility.CreateOAuthClient(consumerKey, consumerSecret, accessToken);
        }

        private async Task<T> SendRequest<T>(HttpMethod method, string uri, HttpContent content, Func<XDocument, T> action)
        {
            Func<HttpClient, Task<HttpResponseMessage>> send;

            if (method == HttpMethod.Get)
                send = (cl) => cl.GetAsync(uri);
            else if (method == HttpMethod.Post)
                send = (cl) => cl.PostAsync(uri, content);
            else if (method == HttpMethod.Put)
                send = (cl) => cl.PutAsync(uri, content);
            else if (method == HttpMethod.Delete)
                send = (cl) => cl.DeleteAsync(uri);
            else
                throw new ArgumentException(nameof(method));

            using (HttpClient client = CreateHttpClient())
            using (HttpResponseMessage response = await send(client))
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument document = XDocument.Load(reader);
                return action(document);
            }
        }

        private IDictionary<string, string> NewParamSet()
        {
            return
                new Dictionary<string, string>()
                {
                    { "format", "xml" }
                };
        }

        #endregion
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;

namespace DotNetOpenAuth.AspNet.Clients
{
    public class VkClient : OAuth2Client
    {
        /// <summary>
        /// Authorization End Point
        /// </summary>
        private const string AuthorizationEndpoint = "https://oauth.vk.com/authorize";
        /// <summary>
        /// Access Token End Point
        /// </summary>
        private const string TokenEndPoint = "https://oauth.vk.com/access_token";
        /// <summary>
        /// Consumer Key
        /// </summary>
        private string ClientId;
        /// <summary>
        /// Consumer Secret
        /// </summary>
        private string ClientSecret;
        /// <summary>
        /// Scopes
        /// https://vk.com/dev/permissions  scope=friends,video,offline
        /// </summary>
        private string[] scopes;

        public VkClient(string clientId, string clientSecret,params string[] scpes):base("Vk")
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            scopes = scpes;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("client_id", ClientId);
            query.Add("redirect_uri", returnUrl.ToString());
            if (scopes != null && scopes.Length > 0)
                query.Add("scope", String.Join(",", scopes));
            query.Add("response_type", "code");
            return new UriBuilder(AuthorizationEndpoint) { Query = query.ToString() }.Uri;
        }
        private Dictionary<string, string> userData = new Dictionary<string, string>();
        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            //external data via accessToken
            return userData;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("client_id", ClientId);
            query.Add("client_secret", ClientSecret);
            query.Add("code", authorizationCode);
            query.Add("grant_type", "authorization_code");
            query.Add("redirect_uri", returnUrl.ToString());
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(TokenEndPoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (Stream rstream = request.GetRequestStream())
            using (StreamWriter sw = new StreamWriter(rstream))
            {
                sw.Write(query.ToString());
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(stream))
            {
                //{"access_token":"533bacf01e11f55b536a565b57531ac114461ae8736d6506a3", "expires_in":43200, "user_id":6492}
                //https://vk.com/dev/auth_sites
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());
                userData.Add("access_token", Convert.ToString(obj.access_token));
                userData.Add("expires_in", Convert.ToString(obj.expires_in));
                userData.Add("user_id", Convert.ToString(obj.user_id));
            }
            return userData["access_token"];
        }
    }
}
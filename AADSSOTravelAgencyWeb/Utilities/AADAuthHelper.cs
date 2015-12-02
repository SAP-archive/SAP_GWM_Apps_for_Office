using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SAP.AppsForOffice.Workflow.Utilities
{
    internal static class AADAuthHelper
    {
        private static readonly string _authority = ConfigurationManager.AppSettings["Authority"];
        private static readonly string _appHostName = ConfigurationManager.AppSettings["AppHostName"];
        private static readonly string _resourceUrl = ConfigurationManager.AppSettings["ResourceUrl"];
        private static readonly string _clientId = ConfigurationManager.AppSettings["ClientID"];
        private static readonly ClientCredential _clientCredential = new ClientCredential(
        ConfigurationManager.AppSettings["ClientID"],
        ConfigurationManager.AppSettings["ClientKey"]);
        private static readonly AuthenticationContext _authenticationContext = new AuthenticationContext("https://login.windows.net/" + _authority);

        /// <summary>
        /// Gets/sets the current host type
        /// </summary>
        internal static string CurrentHostType
        {
            get
            {
                var hostInfo = HttpContext.Current.Session["_host_Info"] as string;
                return string.IsNullOrEmpty(hostInfo) ? "client" : hostInfo;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    HttpContext.Current.Session["_host_Info"] = value;
                }
            }
        }        
        
        /// <summary>
        /// Gets redirect url based on host type
        /// </summary>
        private static string AppRedirectUrl
        {
            get
            {
                if (string.Compare(CurrentHostType, "client", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return _appHostName + "Pages/Common/Authenticate.aspx";
                }
                return _appHostName + "Pages/AzurePages/Default.aspx";
            }
        }
        
        /// <summary>
        /// Gets/sets state_guid of current context
        /// </summary>
        private static string CurrentStateGuid
        {
            get
            {
                return HttpContext.Current.Session["state_guid"] as string;
            }
            set
            {
                if (!string.IsNullOrEmpty(value as string))
                {
                    HttpContext.Current.Session["state_guid"] = value;
                }
            }
        }
        
        /// <summary>
        /// Stores authorization code from http request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static bool StoreAuthorizationCodeFromRequest(HttpRequest request)
        {
            var stateGuid = request.QueryString["state"];
            if (!string.IsNullOrEmpty(stateGuid))
            {
                var authCode = request.QueryString["code"];
                if (!string.IsNullOrEmpty(authCode))
                {
                    HttpContext.Current.Cache[stateGuid] = authCode;
                }
            }
            return IsAuthorizationCodeNotEmpty;
        }
        
        /// <summary>
        /// Gets autorization code from the httpcontext
        /// </summary>
        internal static string AuthorizationCode
        {
            get
            {
                if (!string.IsNullOrEmpty(CurrentStateGuid))
                {
                    return HttpContext.Current.Cache[CurrentStateGuid] as string;
                }
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Gets authorize url for authentication with Azure AD
        /// </summary>
        internal static string AuthorizeUrl
        {
            get
            {
                CurrentStateGuid = Guid.NewGuid().ToString();
                return string.Format("https://login.windows.net/{0}/oauth2/authorize?response_type=code&redirect_uri={1}&client_id={2}&state={3}",
                    _authority,
                    AppRedirectUrl,
                    _clientId,
                    CurrentStateGuid);
            }
        }
        
        /// <summary>
        /// Gets/sets refresh token
        /// </summary>
        private static string RefreshToken
        {
            get
            {
                return HttpContext.Current.Session["RefreshToken" + _resourceUrl] as string;
            }
            set
            {
                HttpContext.Current.Session["RefreshToken-" + _resourceUrl] = value;
            }
        }
        
        /// <summary>
        /// Gets if the refresh token is empty or not
        /// </summary>
        private static bool IsRefreshTokenNotEmpty
        {
            get
            {
                return !string.IsNullOrEmpty(RefreshToken);
            }
        }
        
        /// <summary>
        /// Checks if already authorized (have valid access token)
        /// </summary>
        internal static bool IsAuthorized
        {
            get
            {
                return IsAccessTokenValid || IsRefreshTokenNotEmpty || IsAuthorizationCodeNotEmpty;
            }
        }
        
        /// <summary>
        /// Gets Azure access token using authorization code
        /// </summary>
        /// <param name="authCode"></param>
        /// <returns></returns>
        private static Tuple<Tuple<string, DateTimeOffset>, string> AcquireTokensUsingAuthCode(string authCode)
        {
            try
            {
                var authResult = _authenticationContext.AcquireTokenByAuthorizationCode(
                    authCode,
                    new Uri(AppRedirectUrl),
                    _clientCredential,
                    _resourceUrl);
                return new Tuple<Tuple<string, DateTimeOffset>, string>(
                new Tuple<string, DateTimeOffset>(authResult.AccessToken, authResult.ExpiresOn),
                    authResult.RefreshToken);
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
            return null;
        }
        
        /// <summary>
        /// Renews the access token using refresh token
        /// </summary>
        /// <returns></returns>
        private static Tuple<string, DateTimeOffset> RenewAccessTokenUsingRefreshToken()
        {
            var authResult = _authenticationContext.AcquireTokenByRefreshToken(
                RefreshToken,
               _clientCredential,
               _resourceUrl);
            return new Tuple<string, DateTimeOffset>(authResult.AccessToken, authResult.ExpiresOn);
        }

        /// <summary>
        /// Returns valid access token after verification of the token
        /// </summary>
        /// <param name="currentContext"></param>
        /// <returns></returns>
        internal static string EnsureValidAccessToken(HttpContext currentContext)
        {
            if (IsAccessTokenValid)
            {
                return AccessToken.Item1;
            }
            else if (IsRefreshTokenNotEmpty)
            {
                AccessToken = RenewAccessTokenUsingRefreshToken();
                return AccessToken.Item1;
            }
            else if (StoreAuthorizationCodeFromRequest(currentContext.Request))
            {
                Tuple<Tuple<string, DateTimeOffset>, string> tokens = AcquireTokensUsingAuthCode(AuthorizationCode);
                AccessToken = tokens.Item1;
                RefreshToken = tokens.Item2;
                return AccessToken.Item1;
            }
            else
            {
                throw new InvalidOperationException("Please sign in first.");
            }
        }
        
        /// <summary>
        /// Checks if autorization code is null/empty
        /// </summary>
        private static bool IsAuthorizationCodeNotEmpty
        {
            get
            {
                return !string.IsNullOrEmpty(AuthorizationCode);
            }
        }
        
        /// <summary>
        /// Gets/sets access token from/to httpcontext
        /// </summary>
        internal static Tuple<string, DateTimeOffset> AccessToken
        {
            get
            {
                return HttpContext.Current.Session["AccessTokenWithExpireTime-" + _resourceUrl] as Tuple<string, DateTimeOffset>;
            }
            set
            {
                HttpContext.Current.Session["AccessTokenWithExpireTime-" + _resourceUrl] = value;
            }
        }
        
        /// <summary>
        /// Checks if the access token is valid or not
        /// </summary>
        private static bool IsAccessTokenValid
        {
            get
            {
                return AccessToken != null &&
                    !string.IsNullOrEmpty(AccessToken.Item1) &&
                    AccessToken.Item2 > DateTimeOffset.UtcNow;
            }
        }
    }
}
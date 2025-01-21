using CleanArchitecture.Core.Application;
using CleanArchitecture.Core.Domain.Api;
using CleanArchitecture.Core.Domain.EventArguments;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CleanArchitecture.Core.Infrastructure
{
    /// <summary>
    /// Implements OAuth 2.0 flows for an external API, using the Singleton design pattern.
    /// Use the Instance property to access.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OAuthApi<T> : ApiBase<T>, IOAuthService where T : new()
    {
        #region Properties
        /// <summary>
        /// The authorization endpoint.
        /// </summary>
        private string OAuthEndPoint { get; set; }

        private string OAuthTokenEndpoint { get; set; }

        private string OAuthRedirectUri { get; set; }

        private string OAuthTokenRevocationEndpoint { get; set; }

        /// <summary>
        /// A cache of the app credentials for the API.
        /// </summary>
        private ApiCredential? apiCredentials { get; set; }

        /// <summary>
        /// A cache of the token data for the API.
        /// </summary>
        private Dictionary<string, OAuthToken> tokenDataCollection { get; set; }

        /// <summary>
        /// The name of the API.
        /// </summary>
        public string Name { get; set; }

        private bool isInitialized;
        /// <summary>
        /// Returns whether the API is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get => this.isInitialized;
            set
            {
                this.isInitialized = value;

                // Only raise the initialized event if this is set to true.
                if (value)
                {
                    this.RaiseInitialized(this.Name);
                    System.Diagnostics.Debug.WriteLine("API Initialized!");
                }
            }
        }
        #endregion

        #region Properties - Loopback
        /// <summary>
        /// The background worker that runs the listener on the loopback IP address.
        /// </summary>
        private BackgroundWorker backgroundWorker;

        /// <summary>
        /// The ID of the account pending authorization.
        /// </summary>
        private string accountIdAuthorizing { get; set; }
        #endregion

        #region Delegate Functions
        private Func<ApiCredential, string> GetOAuthQueryString;
        private Func<string, string, ApiCredential, IList<KeyValuePair<string, string>>> GetTokenExchangeParams;
        private Func<string, OAuthToken> ConvertResponseToToken;
        private Func<string, ApiCredential, IList<KeyValuePair<string, string>>> GetTokenRefreshParams;
        #endregion

        #region Events
        public delegate void InitializedHandler(object sender, ApiEventArgs e);
        /// <summary>
        /// Raised when the API is initialized.
        /// </summary>
        public event InitializedHandler Initialized;
        private void RaiseInitialized(string apiName)
        {
            // Create the args and call the listening event handlers, if there are any.
            ApiEventArgs args = new ApiEventArgs(apiName);
            this.Initialized?.Invoke(this, args);
        }

        public delegate void AuthorizedHandler(object sender, ApiAuthorizedEventArgs e);
        /// <summary>
        /// Raised when the API is authorized.
        /// </summary>
        public event AuthorizedHandler Authorized;
        private void RaiseAuthorized(string apiName, string accountId, bool success)
        {
            // Create the args and call the listening event handlers, if there are any.
            ApiAuthorizedEventArgs args = new ApiAuthorizedEventArgs(apiName, accountId, success);
            this.Authorized?.Invoke(this, args);
        }

        internal delegate void OAuthResponseHandler(object sender, string e);
        /// <summary>
        /// Raised when an OAuth response has been received.
        /// </summary>
        internal event OAuthResponseHandler OAuthResponseReceived;
        private void RaiseOAuthResponseReceived(string response)
        {
            // Create the args and call the listening event handlers, if there are any.
            this.OAuthResponseReceived?.Invoke(this, response);
        }
        #endregion

        /// <summary>
        /// Constructs a new instance of the OAuthApi class.
        /// </summary>
        /// <param name="oauthEndPoint">The url for the OAuth 2.0 endpoint.</param>
        /// <param name="oauthTokenEndpoint">The url for the OAuth 2.0 endpoint to exchange a code for a token.</param>
        /// <param name="oauthRedirectUri">The redirect url for the OAuth 2.0 to redirect to on completion.</param>
        /// <param name="getOAuthQueryString">The function that provides the query string for the request to the OAuth 2.0 authorization endpoint.</param>
        /// <param name="getTokenExchangeParams">The function that gets the parameters for the request to the OAuth 2.0 token exchange endpoint.</param>
        /// <param name="convertResponseToToken">The function that parses the response from the OAuth 2.0 token exchange endpoint.</param>
        /// <param name="getTokenRefreshParams">The function that gets the parameters for the request to the OAuth 2.0 token exchange endpoint to refresh the token.</param>
        public OAuthApi(string oauthEndPoint, string oauthTokenEndpoint, string oauthRedirectUri, string oauthRevocationEndpoint,
                        Func<ApiCredential, string> getOAuthQueryString,
                        Func<string, string, ApiCredential, IList<KeyValuePair<string, string>>> getTokenExchangeParams,
                        Func<string, OAuthToken> convertResponseToToken,
                        Func<string, ApiCredential, IList<KeyValuePair<string, string>>> getTokenRefreshParams)
        {
            this.IsInitialized = false;

            // Initialize the OAuth endpoints and redirect uri.
            this.OAuthEndPoint = oauthEndPoint;
            this.OAuthTokenEndpoint = oauthTokenEndpoint;
            this.OAuthRedirectUri = oauthRedirectUri;
            this.OAuthTokenRevocationEndpoint = oauthRevocationEndpoint;

            // Initialize the API credentials to empty.
            this.apiCredentials = null;

            // Initialize the delegate functions.
            this.GetOAuthQueryString = getOAuthQueryString;
            this.GetTokenExchangeParams = getTokenExchangeParams;
            this.ConvertResponseToToken = convertResponseToToken;
            this.GetTokenRefreshParams = getTokenRefreshParams;

            // Initialize the token data collection.
            this.tokenDataCollection = new Dictionary<string, OAuthToken>();

            // Initialize Background worker.
            this.backgroundWorker = new BackgroundWorker();
            this.backgroundWorker.WorkerReportsProgress = false;
            this.backgroundWorker.DoWork += BackgroundWorker_DoWork;
            this.backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        #region Event Handlers
        /// <summary>
        /// Handles when the background worker listening for the redirect URI has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Raise the response received event.
            this.RaiseOAuthResponseReceived(e.Result.ToString());

            // Parse the token from the query string, and continue the OAuth flow by exchanging the code for a token.
            if (e.Result is string resultString)
            {
                string parsedUrl = resultString.Split('?')[1];
                NameValueCollection parsedQueryString = HttpUtility.ParseQueryString(parsedUrl);
                if (!string.IsNullOrWhiteSpace(parsedQueryString["code"]))
                {
                    // Exchange the code for a token.
                    this.GetOauthTokenAsync(this.accountIdAuthorizing, parsedQueryString["code"]);
                }
            }
        }

        /// <summary>
        /// Handles when the background worker is called to start.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = this.ListenForOauthResponse();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the app's credentials to access the API.
        /// </summary>
        /// <param name="credentials">The app's credentials with the API.</param>
        public void Initialize(ApiCredential credentials)
        {
            this.apiCredentials = credentials;
        }

        /// <summary>
        /// Gets the access token associated with the given account.
        /// </summary>
        /// <param name="accountId">The account ID to get the access token of.</param>
        /// <exception cref="Exception">The given accountId has no data associated with it.</exception>
        /// <returns></returns>
        public string GetToken(string accountId)
        {
            // Verify that the account has token data associated with it.
            if (!this.IsAuthorized(accountId))
            {
                throw new Exception("No data for the given account ID");
            }

            return this.tokenDataCollection[accountId].AccessToken;
        }

        /// <summary>
        /// Returns whether the user has authorized the app with the API on the given account.
        /// </summary>
        /// <param name="accountId">The account ID to check the authorization on.</param>
        /// <returns></returns>
        public bool IsAuthorized(string accountId)
        {
            // The token data for the given account contains a value if the app has previously been authorized on that account.
            return this.tokenDataCollection != null && this.tokenDataCollection.ContainsKey(accountId);
        }

        /// <summary>
        /// Returns whether the token exists and has expired, or exists but is missing an expiration limit.
        /// </summary>
        /// <param name="accountId">The account ID to check the access token of.</param>
        /// <exception cref="Exception">The given accountId has no data associated with it.</exception>
        /// <returns></returns>
        public bool IsTokenExpired(string accountId)
        {
            // Verify that the account has token data associated with it.
            if (!this.IsAuthorized(accountId))
            {
                throw new Exception("No data for the given account ID");
            }

            // There has to be a token data, AND EITHER
            //      there is no expiration time OR
            //      there is an expiration time and it has passed.
            return this.IsAuthorized(accountId) &&
                (this.tokenDataCollection[accountId].ExpiresInSeconds.HasValue == false ||
                    DateTime.Compare(DateTime.UtcNow, this.tokenDataCollection[accountId].IssuedUtc.AddSeconds(this.tokenDataCollection[accountId].ExpiresInSeconds.Value)) >= 0);
        }
        #endregion

        #region Methods - OAuth Flow
        /// <summary>
        /// Gets the URI to start the OAuth 2.0 authoization flow.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this method is called before the API credentials are initialized. Use Initialize() to set the credentials.</exception>
        /// <returns>The URI to start the OAuth 2.0 authoization flow</returns>
        public Uri GetOAuthUri()
        {
            // Throw an exception if the API credentials are not set.
            if (this.apiCredentials == null)
            {
                throw new InvalidOperationException("API credentials must be initialized before making a call to GetOAuthUri(). Use Initialize() to set the credentials.");
            }

            string oauthUri = this.OAuthEndPoint + this.GetOAuthQueryString(this.apiCredentials);

            return new Uri(oauthUri);
        }

        /// <summary>
        /// Starts the OAuthListener for the given account ID. Note that only a single OAuthListener can run at a time. Calling this method when an existing listener is running will cancel the existing listener and replace it with a new listener.
        /// </summary>
        /// <param name="accountId">The account ID to associate with the listener.</param>
        public void StartOAuthListener(string accountId)
        {
            // Cancel the existing background worker, if one is running.
            if (this.backgroundWorker.IsBusy == true) { this.backgroundWorker.CancelAsync(); }

            // Start the background worker and store the accountId.
            this.accountIdAuthorizing = accountId;
            this.backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Cancels the OAuthListener, if one is running.
        /// </summary>
        public void CancelOAuthListener()
        {
            // Cancel the background worker.
            this.backgroundWorker.CancelAsync();
        }

        /// <summary>
        /// Completes the OAuth flow by exchanging the given authorization code for a token.
        /// </summary>
        /// <param name="accountId">The account ID to check the access token of.</param>
        /// <param name="authorizationCode">The authorization code to exchange for the token.</param>
        /// <exception cref="InvalidOperationException">Thrown if this method is called before the API credentials are initialized. Use Initialize() to set the credentials.</exception>
        /// <exception cref="Exception">The given accountId has no data associated with it.</exception>
        public async Task GetOauthTokenAsync(string accountId, string authorizationCode)
        {
            // Throw an exception if the API credentials are not set.
            if (this.apiCredentials == null)
            {
                throw new InvalidOperationException("API credentials must be initialized before making a call to GetOauthTokenAsync(). Use Initialize() to set the credentials.");
            }

            System.Diagnostics.Debug.WriteLine("Getting token using code: " + authorizationCode);

            IList<KeyValuePair<string, string>> content = this.GetTokenExchangeParams(authorizationCode, this.OAuthRedirectUri, this.apiCredentials);

            string responseContent = "";
            bool success = true;
            try
            {
                responseContent = await this.PostAsync(this.OAuthTokenEndpoint, content);

                // No exceptions were thrown, so parse the response message.
                this.tokenDataCollection.Add(accountId, this.ConvertResponseToToken(responseContent));
            }
            catch (Exception ex)
            {
                // An exception was thrown. The authorization was not a success.
                success = false;
            }

            // Raise the authorized event to signal the completion of the get token.
            this.RaiseAuthorized(this.Name, accountId, success);
        }
        #endregion

        #region Methods - HTTP Requests
        /// <summary>
        /// Executes an HTTP POST to the given URI with the given parameters, adding the stored authorization headers to the request.
        /// </summary>
        /// <param name="accountId">The account ID to check the access token of.</param>
        /// <param name="uri">The uri to make an HTTP POST to.</param>
        /// <param name="parameters">The HTTP content parameters for the POST.</param>
        /// <exception cref="Exception">The given accountId has no data associated with it.</exception>
        /// <returns>A string containing the HTTP Response received.</returns>
        public async Task<string> PostAsync(string accountId, string uri, IList<KeyValuePair<string, string>> parameters)
        {
            // Verify that the account has token data associated with it.
            if (!this.IsAuthorized(accountId))
            {
                throw new Exception("No data for the given account ID");
            }

            // Refresh the token if needed.
            await this.RefreshTokenIfNeededAsync(accountId);

            // Set the authorization header.
            this.Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(this.tokenDataCollection[accountId].TokenType, this.tokenDataCollection[accountId].AccessToken);

            return await base.PostAsync(uri, parameters);
        }

        /// <summary>
        /// Executes an HTTP GET to the given URI, adding the stored authorization headers to the request.
        /// </summary>
        /// <param name="accountId">The account ID to check the access token of.</param>
        /// <param name="uri">The uri to request an HTTP GET from.</param>
        /// <exception cref="Exception">The given accountId has no data associated with it.</exception>
        /// <returns>A string containing the HTTP Response received.</returns>
        public async Task<string> GetAsync(string accountId, string uri)
        {
            // Verify that the account has token data associated with it.
            if (!this.IsAuthorized(accountId))
            {
                throw new Exception("No data for the given account ID");
            }

            // Refresh the token if needed.
            await this.RefreshTokenIfNeededAsync(accountId);

            // Set the authorization header.
            this.Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(this.tokenDataCollection[accountId].TokenType, this.tokenDataCollection[accountId].AccessToken);

            return await base.GetAsync(uri);
        }
        #endregion

        #region Helper Methods - OAuth Flow
        private string ListenForOauthResponse()
        {
            using (HttpListener httpListener = new HttpListener())
            {
                httpListener.Prefixes.Add(this.OAuthRedirectUri + "/");
                httpListener.Start();

                // Blocking call - this is why this method is run in a background worker.
                HttpListenerContext context = httpListener.GetContext();
                HttpListenerRequest request = context.Request;

                HttpListenerResponse response = context.Response;

                System.Diagnostics.Debug.WriteLine("Response url received: " + request.Url);

                //response.Redirect("");
                response.Close();

                // Pause to ensure the response has been sent.
                Thread.Sleep(1000);
                httpListener.Stop();

                return request.Url.ToString();
            }
        }
        #endregion

        #region Helper Methods - Add/Remove Token
        /// <summary>
        /// Initializes the token data and refreshes each token if needed.
        /// </summary>
        /// <param name="tokens">A ditionary of OAuthToken values keyed by account ID.</param>
        public async Task InitializeTokenDataAsync(Dictionary<string, OAuthToken> tokens)
        {
            // If there are tokens to initialize...
            if (tokens != null)
            {
                // Set the token data collection.
                this.tokenDataCollection = tokens;

                // For each token to initialize...
                foreach (string accountId in this.tokenDataCollection.Keys)
                {
                    // Refresh the token if needed.
                    await this.RefreshTokenIfNeededAsync(accountId);
                }
            }

            // Mark initialization as complete.
            this.IsInitialized = true;
        }

        /// <summary>
        /// Gets the cached token data if there is any.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, OAuthToken> GetCachedTokenData()
        {
            // Return a new instance (copy) of the token data collection.
            return new Dictionary<string, OAuthToken>(this.tokenDataCollection);
        }

        /// <summary>
        /// Removes the account by revoking the API access token and deleting any locally cached token data.
        /// </summary>
        /// <param name="accountId">The ID for the account assigned by the app.</param>
        /// <returns></returns>
        public async Task<bool> RemoveConnectionAsync(string accountId)
        {
            // Revoke the connection token.
            // Post a request to the token revocation endpoint.
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("token", this.GetToken(accountId))
            };
            await this.PostAsync(this.OAuthTokenRevocationEndpoint, args);

            // Clear the cached token data in memory.
            return this.tokenDataCollection.Remove(accountId);
        }
        #endregion

        #region Helper Methods - Token Refresh
        /// <summary>
        /// Refreshes the token, if the token needs to be refreshed.
        /// </summary>
        /// <param name="accountId">The account ID to check the access token of.</param>
        /// <exception cref="Exception">The given accountId has no data associated with it.</exception>
        /// <returns>Whether the token was refreshed.</returns>
        private async Task<bool> RefreshTokenIfNeededAsync(string accountId)
        {
            // Verify that the account has token data associated with it.
            if (this.tokenDataCollection == null || this.tokenDataCollection.ContainsKey(accountId) == false)
            {
                throw new Exception("No data for the given account ID");
            }

            // Refresh the token if needed.
            if (this.IsTokenExpired(accountId))
            {
                System.Diagnostics.Debug.WriteLine("Token expired!");
                await this.RefreshTokenAsync(accountId);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Refreshes the access token using the refresh token.
        /// </summary>
        /// <param name="accountId">The account ID to check the access token of.</param>
        /// <exception cref="Exception">The given accountId has no data associated with it.</exception>
        /// <exception cref="Exception">The given accountId has no data associated with it.</exception>
        /// <returns></returns>
        private async Task RefreshTokenAsync(string accountId)
        {
            // Verify that the account has token data associated with it.
            if (this.tokenDataCollection == null || this.tokenDataCollection.ContainsKey(accountId) == false)
            {
                throw new Exception("No data for the given account ID");
            }

            // Throw an exception if the API credentials are not set.
            if (this.apiCredentials == null)
            {
                throw new InvalidOperationException("API credentials must be initialized before making a call to RefreshTokenAsync(). Use Initialize() to set the credentials.");
            }

            IList<KeyValuePair<string, string>> content = this.GetTokenRefreshParams(this.tokenDataCollection[accountId].RefreshToken, this.apiCredentials);

            string responseContent = await this.PostAsync(this.OAuthTokenEndpoint, content);

            // No exceptions were thrown, so parse the response message.
            OAuthToken updatedToken = this.ConvertResponseToToken(responseContent);

            // Update the access token and expiration.
            this.tokenDataCollection[accountId].AccessToken = updatedToken.AccessToken;
            this.tokenDataCollection[accountId].ExpiresInSeconds = updatedToken.ExpiresInSeconds;
            this.tokenDataCollection[accountId].IssuedUtc = DateTime.UtcNow;

            System.Diagnostics.Debug.WriteLine("Done refreshing token!");
        }
        #endregion
    }
}

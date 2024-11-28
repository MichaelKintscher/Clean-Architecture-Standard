using CleanArchitecture.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Core.Infrastructure
{
    /// <summary>
    /// Represents an external API, using the Singleton design pattern.
    /// Use the Instance property to access.
    /// </summary>
    /// <typeparam name="T">The type of ApiBase, which must implement a public parameterless constructor.</typeparam>
    public class ApiBase<T> : Singleton<T> where T : new()
    {
        #region Types
        /// <summary>
        /// An enumeration of the supported HTTP methods.
        /// </summary>
        private enum HttpMethod
        {
            Get,
            Post,
            Put,
            Delete
        }
        #endregion

        #region Properties
        /// <summary>
        /// The instance of the HTTP Client for sending requests to the API.
        /// </summary>
        protected HttpClient Client { get; set; }
        #endregion

        #region Constructors
        public ApiBase()
        {
            this.Client = new HttpClient();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a response from a GET request to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to GET from.</param>
        /// <exception cref="Exception">Thrown if an unknown exception occurs when making the HTTP request.</exception>
        /// <exception cref="HttpRequestException">Thrown if an HTTP request exception occurs when making the HTTP request.</exception>
        /// <returns>The content of the HTTP response.</returns>
        public virtual async Task<string> GetAsync(string endpoint)
        {
            // Try to GET at the given endpoint.
            return await this.ExecuteHttpMethod(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Makes a POST to the given URI with the given parameters.
        /// </summary>
        /// <param name="endpoint">The endpoint to POST to.</param>
        /// <param name="parameters">The parameters for the POST in the form of a list of key-value pairs.</param>
        /// <exception cref="Exception">Thrown if an unknown exception occurs when making the HTTP request.</exception>
        /// <exception cref="HttpRequestException">Thrown if an HTTP request exception occurs when making the HTTP request.</exception>
        /// <returns>The content of the HTTP response.</returns>
        public virtual async Task<string> PostAsync(string endpoint, IList<KeyValuePair<string, string>> parameters)
        {
            // Encode the parameters and then try to POST the resulting payload at the given endpoint.
            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);
            return await this.ExecuteHttpMethod(HttpMethod.Post, endpoint, content);
        }

        /// <summary>
        /// Posts the given payload at the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to POST to.</param>
        /// <param name="payload">The payload for the POST.</param>
        /// <exception cref="Exception">Thrown if an unknown exception occurs when making the HTTP request.</exception>
        /// <exception cref="HttpRequestException">Thrown if an HTTP request exception occurs when making the HTTP request.</exception>
        /// <returns>The content of the HTTP response.</returns>
        public virtual async Task<string> PostAsync(string endpoint, StringContent payload)
        {
            // Try to POST the given payload at the given endpoint.
            return await this.ExecuteHttpMethod(HttpMethod.Post, endpoint, payload);
        }

        /// <summary>
        /// Puts the given payload at the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to PUT at.</param>
        /// <param name="payload">The payload for the PUT.</param>
        /// <exception cref="Exception">Thrown if an unknown exception occurs when making the HTTP request.</exception>
        /// <exception cref="HttpRequestException">Thrown if an HTTP request exception occurs when making the HTTP request.</exception>
        /// <returns>The content of the HTTP response.</returns>
        public virtual async Task<string> PutAsync(string endpoint, StringContent payload)
        {
            // Try to PUT the given payload at the given endpoint.
            return await this.ExecuteHttpMethod(HttpMethod.Put, endpoint, payload);
        }

        /// <summary>
        /// Sends a DELETE request to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to DELETE at.</param>
        /// <exception cref="Exception">Thrown if an unknown exception occurs when making the HTTP request.</exception>
        /// <exception cref="HttpRequestException">Thrown if an HTTP request exception occurs when making the HTTP request.</exception>
        /// <returns>The content of the HTTP response.</returns>
        public virtual async Task<string> DeleteAsync(string endpoint)
        {
            // Try to DELETE at the given endpoint.
            return await this.ExecuteHttpMethod(HttpMethod.Delete, endpoint);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Executes the given HTTP method at the given endpoint.
        /// </summary>
        /// <param name="method">The HTTP method to execute.</param>
        /// <param name="endpoint">The endpoint to execute the given HTTP method at.</param>
        /// <param name="payload">[optional] The payload for the given HTTP method.</param>
        /// <exception cref="NotImplementedException">Thrown if the given HTTP method is not implemented.</exception>
        /// <exception cref="Exception">Thrown if an unknown exception occurs when making the HTTP request.</exception>
        /// <exception cref="HttpRequestException">Thrown if an HTTP request exception occurs when making the HTTP request.</exception>
        /// <returns>The content of the HTTP response.</returns>
        private async Task<string> ExecuteHttpMethod(HttpMethod method, string endpoint, HttpContent? payload = null)
        {
            // Try to execute the given method at the given endpoint.
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                switch (method)
                {
                    case HttpMethod.Get:
                        response = await this.Client.GetAsync(endpoint);
                        break;
                    case HttpMethod.Post:
                        response = await this.Client.PostAsync(endpoint, payload);
                        break;
                    case HttpMethod.Put:
                        response = await this.Client.PutAsync(endpoint, payload);
                        break;
                    case HttpMethod.Delete:
                        response = await this.Client.DeleteAsync(endpoint);
                        break;
                    default:
                        // This should be unreachable, given the useage of an enum for the switch.
                        throw new NotImplementedException("Http method is invalid.");
                }

                // Throws an exception if the response status code is not OK.
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Error in HTTP response for " + method.ToString() + " at \"" + endpoint + "\": " + ex.StatusCode?.ToString() + " \"" + ex.Message + "\"", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unknown error when attempting to " + method.ToString() + " at \"" + endpoint + "\": " + ex.HResult + " \"" + ex.Message + "\"", ex);
            }

            // No exceptions were thrown, so parse the response message.
            return await response.Content.ReadAsStringAsync();
        }
        #endregion
    }
}

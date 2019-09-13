using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using POCGraphAPICreateCalendar.Enums;
using POCGraphAPICreateCalendar.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace POCGraphAPICreateCalendar
{
    public class Program
    {
        /// <summary>
        /// JSON serializer settings for http calls.
        /// </summary>
        private static readonly JsonSerializerSettings jsonSerializerSettings;

        public static void Main(string[] args)
        {
            RunPOC().Wait();
        }

        public static async Task RunPOC()
        {
            string userAccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImllX3FXQ1hoWHh0MXpJRXN1NGM3YWNRVkduNCIsImtpZCI6ImllX3FXQ1hoWHh0MXpJRXN1NGM3YWNRVkduNCJ9.eyJhdWQiOiI1N2ViMWIxNy1jN2YxLTQ0MzMtYjExOS04Y2YzYzY3NDUxZWYiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zZDY5NzU3NC0xNjdjLTRmZjItYmRhOC05ODlmOWFmYzg2N2YvIiwiaWF0IjoxNTY4MzcxMzkzLCJuYmYiOjE1NjgzNzEzOTMsImV4cCI6MTU2ODM3NTI5MywiYWlvIjoiNDJGZ1lMaGJ2cXB0VS9wM0dmc29RYk03M2pWdGg4dFM0bWUvenA0MmhaUDk4NGVIeVpJQSIsImFtciI6WyJwd2QiXSwiZmFtaWx5X25hbWUiOiJrIiwiZ2l2ZW5fbmFtZSI6Im5pcmFqIiwiaXBhZGRyIjoiMTY3LjIyMC4yMzguMTU0IiwibmFtZSI6Im5pcmFqIGsiLCJub25jZSI6ImQzOTE5NGY5LTZhNTAtNDFlNC05NzhhLTI2ZTU2MTAxMGYxOSIsIm9pZCI6IjdiZDUzODJmLTMyZmMtNGUxZS1hZjYwLTMwYTlhMzMxNGViOCIsInN1YiI6ImdZazhBZ09LTDBXUWJ2YkFoLXN1SDhQZDRQcmFfTEFKYmdvVzl3UDF1aHciLCJ0aWQiOiIzZDY5NzU3NC0xNjdjLTRmZjItYmRhOC05ODlmOWFmYzg2N2YiLCJ1bmlxdWVfbmFtZSI6Imd0YXRlc3RAZ3Rhc2NoLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6Imd0YXRlc3RAZ3Rhc2NoLm9ubWljcm9zb2Z0LmNvbSIsInV0aSI6Im5ZY3BYcmJuUDBpVkZsOGhQQUdaQUEiLCJ2ZXIiOiIxLjAifQ.LfjTyx2Bma9WKYGvjno9S9m4qQ9mz2I-X_3dsU3SGxr-jS00I5WiJepNs61gvoi8NoO-wjCTEjphlsiuo1e-pT_HhAxkHGZJTqBgTB9ng-d0h4sOWxC5eAlQXmGYk2GB9QqqnMZafN27cWkSvIutWWT1ZqkAU5AKQcWYoeVkYCAk_l8AaSqwpWhtGHoLcTSjIIYvdcoIf_fgNxQCrFJIRHkEhyyxXR8R4Z5VkRfUOcibJu_NJIp3lu7NdpkaM500aNEKPOq6DnuCulO6dJT2gFWrEHsOKvbTKz3vtExbI2GWkwqTsGnhaQV0qU8Jjvhoq2Y-Evk5sBRhgKAkzV0qgg";
            var calendarEvent = await GetCalendarEvent(userAccessToken, "");
            if (calendarEvent != null)
            {
                
            }
        }

        /// <summary>
        /// Get calendar event request.
        /// </summary>
        /// <param name="userAccessToken">User Id token.</param>
        /// <param name="calendarEventId">calendar event id.</param>
        /// <returns>Event that was retrieved</returns>
        public async static Task<CalendarEvent> GetCalendarEvent(string userAccessToken, string calendarEventId)
        {
            if (calendarEventId == null)
            {
                //  "Invalid input parameter. CalendarEventId must not be null.";
            }

            var eventResponse = new CalendarEvent();

            using (var httpClient = new HttpClient())
            {
                // Force refreshing cache so that the cached resource token from the user principle isn't returned
                var schedulerGraphToken = await GetBearerTokenFromUserToken(userAccessToken);
                httpClient.DefaultRequestHeaders.Authorization = schedulerGraphToken;

                var userEmail = GetUserEmailFromToken(userAccessToken);

                var url = $"https://graph.microsoft.com/v1.0/users/{userEmail}/events/{calendarEventId}{SchedulerConstants.ExpandExtensionFilter}";


                var exceptions = new List<Exception>();
                for (int i = 1; i <= 5; i++)
                {
                    var getRequest = new HttpRequestMessage(HttpMethod.Get, url);
                    getRequest.Headers.Authorization = schedulerGraphToken;

                    using (var response = await httpClient.SendAsync(getRequest))
                    {
                        // this.Trace.TraceInformation($"Get calendar event call made with account {userEmail}");
                        var content = await response.Content.ReadAsStringAsync();

                        var responseHeaders = response.Headers.ToString();
                        // this.Trace.TraceInformation($"Response headers for Get calendar event are {responseHeaders}");
                        if (response.IsSuccessStatusCode)
                        {
                            return JsonConvert.DeserializeObject<CalendarEvent>(content); //, eventResponse);
                        }
                        else
                        {
                            //var exception = new GraphException(HttpMethod.Get.Method, url, response.StatusCode, content);

                            //if (ShouldRetryOnGraphException(response.StatusCode))
                            //{
                            //    // this.Trace.TraceWarning($"GetCalendarEvent: Get attempt #{i} failed, Status code: {response.StatusCode}, content: {content}. Will try again");
                            //    exceptions.Add(exception);

                            //    await ExponentialDelay(response, i);
                            //}
                            //else
                            //{
                            //    throw exception.EnsureTraced(this.Trace);
                            //}
                        }
                    }
                }
            }

            return eventResponse;
        }

        public async static Task<AuthenticationHeaderValue> GetBearerTokenFromUserToken(string userAccessToken)
        {
            UserAssertion userAssertion = new UserAssertion(userAccessToken, "urn:ietf:params:oauth:grant-type:jwt-bearer");
            string aadInstance = "https://login.microsoftonline.com/{0}";
            string tenant = "3d697574-167c-4ff2-bda8-989f9afc867f";
            string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
            AuthenticationContext authContext = new AuthenticationContext(authority);
            var clientValue = @"";
            ClientCredential clientCredential = new ClientCredential("57eb1b17-c7f1-4433-b119-8cf3c67451ef", clientValue);
            var result = await authContext.AcquireTokenAsync("https://graph.microsoft.com", clientCredential, userAssertion);
            return new AuthenticationHeaderValue("Bearer", result.AccessToken);
        }

        /// <summary>
        /// Get User Email From Token
        /// </summary>
        /// <param name="accessToken">Access Token</param>
        /// <returns>service account email</returns>
        public static string GetUserEmailFromToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var user = handler.ReadToken(accessToken) as JwtSecurityToken;
            return user.Claims.FirstOrDefault(c => c.Type == "unique_name").Value;
        }

        /// <summary>
        /// Exponential Delay
        /// </summary>
        /// <param name="response">Http response</param>
        /// <param name="retryAttempt">Retry count</param>
        /// <returns>Task</returns>
        public static async Task ExponentialDelay(HttpResponseMessage response, int retryAttempt)
        {
            var delayInSeconds = (1d / 2d) * (Math.Pow(2d, retryAttempt) - 1d);

            var waitTimeSpan = response?.Headers?.RetryAfter?.Delta;
            var defaultTimeSpan = TimeSpan.FromSeconds(delayInSeconds);
            if (waitTimeSpan == null || waitTimeSpan <= TimeSpan.FromSeconds(0))
            {
                waitTimeSpan = defaultTimeSpan;
            }

            // Trace.TraceInformation($"Delay thread with {waitTimeSpan ?? defaultTimeSpan} seconds before retry");

            await Task.Delay(waitTimeSpan ?? defaultTimeSpan);

            // Trace.TraceInformation($"Processing retry  after {waitTimeSpan ?? defaultTimeSpan} delay");
        }

        /// <summary>
        /// Check for retry on graph exception
        /// </summary>
        /// <param name="statusCode">Http status Code</param>
        /// <returns>Is retry required</returns>
        public static bool ShouldRetryOnGraphException(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.ServiceUnavailable
                        || (int)statusCode == 429
                        || statusCode == HttpStatusCode.GatewayTimeout
                        || statusCode == HttpStatusCode.PreconditionFailed;
        }
    }
}

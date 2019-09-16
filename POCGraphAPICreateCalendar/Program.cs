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
            string userAccessToken = "eyJeredfiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImllX3FXQ1hoWHh0MXpJRXN1NGM3YWNRVkduNCIsImtpZCI6ImllX3FXQ1hoWHh0MXpJRXN1NGM3YWNRVkduNCJ9.eyJhdWQiOiI1N2ViMWIxNy1jN2YxLTQ0MzMtYjExOS04Y2YzYzY3NDUxZWYiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zZDY5NzU3NC0xNjdjLTRmZjItYmRhOC05ODlmOWFmYzg2N2YvIiwiaWF0IjoxNTY4NjIxMzU1LCJuYmYiOjE1Njg2MjEzNTUsImV4cCI6MTU2ODYyNTI1NSwiYWlvIjoiQVNRQTIvOE1BQUFBaStOYThEYThQTW0wVloyVmhPL3g1bHRMK21YMGg4WXdQLzQ1b2hJSmFzST0iLCJhbXIiOlsicHdkIl0sImZhbWlseV9uYW1lIjoiayIsImdpdmVuX25hbWUiOiJuaXJhaiIsImlwYWRkciI6IjE2Ny4yMjAuMjM4LjIxOCIsIm5hbWUiOiJuaXJhaiBrIiwibm9uY2UiOiJiNTliZjI2MS0zMTEyLTQ0NTQtODY3My1hMTY0NzgwZjdkMjIiLCJvaWQiOiI3YmQ1MzgyZi0zMmZjLTRlMWUtYWY2MC0zMGE5YTMzMTRlYjgiLCJzdWIiOiJnWWs4QWdPS0wwV1FidmJBaC1zdUg4UGQ0UHJhX0xBSmJnb1c5d1AxdWh3IiwidGlkIjoiM2Q2OTc1NzQtMTY3Yy00ZmYyLWJkYTgtOTg5ZjlhZmM4NjdmIiwidW5pcXVlX25hbWUiOiJndGF0ZXN0QGd0YXNjaC5vbm1pY3Jvc29mdC5jb20iLCJ1cG4iOiJndGF0ZXN0QGd0YXNjaC5vbm1pY3Jvc29mdC5jb20iLCJ1dGkiOiJqOXo3emJnOWRFS3pmeHRTRlZObkFBIiwidmVyIjoiMS4wIn0.ureBd0ruQPqwTqtrDCYOB6J8N3GAULBYvmtLujmkSxEkQUh2yQHJZGOgN76ntNRmq_csHAiVfYvl6RNAk9kPLfz-o8VTYgo0F8B6pvWOaB9qUsVs-BFNDUlPrkuphANwPV3nCSYvPMv8LNPQchwB3eAp9X6ezzZTrCaIR1cfzDcyQvauYqu5eNb4o7OsfyfDaxzxcnhCIDI2xJkFgJdvy9qSz7GCMNFC7JLOQ9yuPbpK68y1mPg37xoqRP_czauVu509Lmw4cG1qgrYwusluO_NjJfDoLTu3i7NVYcgWpcWSxOT7Ox6ufi_k5Kp6jxYHlcZL68j1c7Wo0nMJurzayQ";
            var calendarEvent = await GetCalendarEvent(userAccessToken, "");
            calendarEvent.Start = new MeetingDateTime
            {
                DateTime = DateTime.Now.ToString(),
                TimeZone = "UTC"
            };

            calendarEvent.End = new MeetingDateTime
            {
                DateTime = DateTime.Now.AddMinutes(30).ToString(),
                TimeZone = "UTC"
            };

            await SendPostEvent(userAccessToken, calendarEvent);
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
            calendarEventId = "AAMkABkLTYxODktNGM1Mi05NDkzLTU0YzdiNmQ1MjFhMABGAAAAAADrC3xKWVoKTL0UT-1mGp4UBwBV4CfwFLncSIBFMicfZzlEAAAAAAENAABV4CfwFLncSIBFMicfZzlEAAABuvnFAAA=";
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,

            };
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
                            return JsonConvert.DeserializeObject<CalendarEvent>(content, jsonSerializerSettings);
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

        /// <summary>
        /// Sends update calendar event request.
        /// </summary>
        /// <param name="userAccessToken">User Id token.</param>
        /// <param name="eventRequest">Request body.</param>
        /// <param name="expand"> bool to controll response expansion</param>
        /// <returns>Event that was updated</returns>
        public async static Task<CalendarEvent> SendPatchEvent(string userAccessToken, CalendarEvent eventRequest, bool expand = true)
        {
            if (eventRequest == null)
            {
                // , "Invalid input parameter. EventRequest must not be null.");
            }

            var eventResponse = new CalendarEvent();

            try
            {
                var userEmail = GetUserEmailFromToken(userAccessToken);
                //this.Trace.TraceInformation($"Sending patch update using email {userEmail}");

                var relativePath = $"/users/{userEmail}/events/{eventRequest?.Id}";

                var url = $"https://graph.microsoft.com/v1.0/{relativePath}";

                // Force refreshing cache so that the cached resource token from the user principle isn't returned
                var schedulerGraphToken = await GetBearerTokenFromUserToken(userAccessToken);
                var exceptions = new List<Exception>();

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = schedulerGraphToken;
                    var requestData = JsonConvert.SerializeObject(eventRequest);

                    for (int i = 1; i <= 5; i++)
                    {
                        var message = new HttpRequestMessage(new HttpMethod("PATCH"), url);
                        message.Headers.Authorization = schedulerGraphToken;
                        message.Content = new StringContent(
                                           requestData,
                                           Encoding.UTF8,
                                           "application/json");

                        using (var response = await httpClient.SendAsync(message))
                        {
                            var responseHeaders = response.Headers.ToString();
                            // this.Trace.TraceInformation($"Response headers for patch calendar event are {responseHeaders}");

                            if (response.IsSuccessStatusCode)
                            {
                                if (expand)
                                {
                                    for (int j = 1; j <= 5; j++)
                                    {
                                        // Do another GET so that we can return the Extensions
                                        var getMessage = new HttpRequestMessage(HttpMethod.Get, $"{url}{SchedulerConstants.ExpandExtensionFilter}");
                                        getMessage.Headers.Authorization = schedulerGraphToken;

                                        using (var extentionResponse = await httpClient.SendAsync(getMessage))
                                        {
                                            if (extentionResponse.IsSuccessStatusCode)
                                            {
                                                var content = await extentionResponse.Content.ReadAsStringAsync();

                                                return JsonConvert.DeserializeObject<CalendarEvent>(content);
                                            }
                                            else
                                            {
                                                //string content = await extentionResponse.Content.ReadAsStringAsync();
                                                //var exception = new GraphException(HttpMethod.Get.Method, $"{url}{SchedulerConstants.ExpandExtensionFilter}", extentionResponse.StatusCode, content);
                                                //if (ShouldRetryOnGraphException(extentionResponse.StatusCode))
                                                //{
                                                //    this.Trace.TraceWarning($"Retry # - {j}. Get extension for caledar event failed with StatusCode: {extentionResponse.StatusCode}, error: {content}");
                                                //    exceptions.Add(exception);

                                                //    await ExponentialDelay(extentionResponse, j);
                                                //}
                                                //else
                                                //{
                                                //    throw exception.EnsureTraced(this.Trace);
                                                //}
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    return eventRequest;
                                }

                                throw new AggregateException(exceptions);
                            }
                            else
                            {
                                string content = await response.Content.ReadAsStringAsync();
                                //var exception = new GraphException("PATCH", url, response.StatusCode, content);

                                //if (EmailUtils.ShouldRetryOnGraphException(response.StatusCode))
                                //{
                                //    this.Trace.TraceWarning($"Retry # - {i}. Updating calendar event failed with StatusCode: {response.StatusCode}, error: {content}");
                                //    exceptions.Add(exception);

                                //    await EmailUtils.ExponentialDelay(response, i);
                                //}
                                //else
                                //{
                                //    throw exception.EnsureTraced(this.Trace);
                                //}
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                //this.Trace.TraceError($"stackTrace {e.StackTrace} InnerException: {e.InnerException}");
                //throw new SchedulerUpdateCalendarException(e).EnsureTraced(this.Trace);
            }

            return eventResponse;
        }

        /// <summary>
        /// Sends create event request.
        /// </summary>
        /// <param name="userAccessToken">User Id token.</param>
        /// <param name="eventRequest">Request body.</param>
        /// <returns>Event that was created</returns>
        public static async Task<CalendarEvent> SendPostEvent(string userAccessToken, CalendarEvent eventRequest)
        {

            var eventResponse = new CalendarEvent();
            var extensionsToPatch = eventRequest.Extensions;

            using (var httpClient = new HttpClient())
            {
                // Force refreshing cache so that the cached resource token from the user principle isn't returned
                httpClient.DefaultRequestHeaders.Authorization = await GetBearerTokenFromUserToken(userAccessToken);

                // Serialize the request object.
                var requestData = JsonConvert.SerializeObject(eventRequest, jsonSerializerSettings);
                var userEmail = GetUserEmailFromToken(userAccessToken);

                var url = $"https://graph.microsoft.com/v1.0/users/{userEmail}/events";


                List<Exception> exceptions = new List<Exception>();
                for (int i = 1; i <= 5; i++)
                {
                    using (var response = await httpClient.PostAsync(url, new StringContent(requestData, Encoding.UTF8, "application/json")))
                    {
                        var responseHeaders = response.Headers.ToString();

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var postEventResult = JsonConvert.DeserializeObject<CalendarEvent>(content, jsonSerializerSettings);

                        }
                        else
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            //var exception = new GraphException(HttpMethod.Post.Method, url, response.StatusCode, content);

                            //if (EmailUtils.ShouldRetryOnGraphException(response.StatusCode))
                            //{
                            //    this.Trace.TraceWarning($"PostCalendarEvent: Post attempt #{i} failed, for user {userEmail}, status code {response.StatusCode}, response content: {content}");
                            //    exceptions.Add(exception);
                            //    await EmailUtils.ExponentialDelay(response, i);
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
            string tenant = "TenantId";
            string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
            AuthenticationContext authContext = new AuthenticationContext(authority);
            var clientValue = @"ClientSecret";
            ClientCredential clientCredential = new ClientCredential("ClientId", clientValue);
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

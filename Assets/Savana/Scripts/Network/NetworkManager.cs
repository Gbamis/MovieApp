using System;
using System.Net.Http;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Savana.Movie
{
    public class NetworkManager : MonoBehaviour
    {
        private static string KEY;
        private static HttpClient Client = new();
        private static string uri_now_playing = "https://api.themoviedb.org/3/movie/now_playing?language=en-US";
        private static readonly string uri_imageFilePath = "https://image.tmdb.org/t/p/";
        private static readonly string uri_movie_details = "https://api.themoviedb.org/3/movie/";
        private static readonly string uri_search_keyword = "https://api.themoviedb.org/3/search/keyword?query=";


        public static List<Response_MovieDetail> response_NowPlaying = new();

        public static async void Request_Get_NowPlaying(string key, Action OnResults = null, Action OnError = null, int page = 1)
        {
            string bearer = "Bearer " + key;
            string pageToken = "&page=" + page.ToString();
            uri_now_playing += pageToken;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri_now_playing),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", bearer},
                }
            };

            try
            {
                HttpResponseMessage response = await Client.SendAsync(request);
                int statusCode = (int)response.StatusCode;
                string responseBody = await response.Content.ReadAsStringAsync();
                KEY = key;

                // Debug.Log($"Status Code: {statusCode}");
                // Debug.Log($"Response Body: {responseBody}");

                if (statusCode == 200)
                {
                    Response_NowPlaying_List data = JsonUtility.FromJson<Response_NowPlaying_List>(responseBody);

                    foreach (Model_Result res in data.results)
                    {
                        Response_MovieDetail detail = await Request_Get_MovieDetail_by_ID(res.id);
                        await DownloadPosterForMovie(detail);
                        response_NowPlaying.Add(detail);
                    }

                    OnResults?.Invoke();
                }
                else
                {
                    OnError?.Invoke();
                }



            }
            catch (HttpRequestException e)
            {
                OnError?.Invoke();
            }
        }



        public static async void Request_SearchforMovie_ByKeyword(string keyword,
        Action<List<Response_MovieDetail>> OnResults = null, Action OnError = null, int page = 1)
        {
            List<Response_MovieDetail> queryResults = new();

            string bearer = "Bearer " + KEY;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri_search_keyword + keyword + "&page=1"),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", bearer},
                }
            };

            try
            {
                HttpResponseMessage response = await Client.SendAsync(request);
                int statusCode = (int)response.StatusCode;
                string responseBody = await response.Content.ReadAsStringAsync();

                //Debug.Log($"Status Code: {statusCode}");
                //Debug.Log($"Response Body: {responseBody}");

                if (statusCode == 200)
                {
                    Response_SearchResult data = JsonUtility.FromJson<Response_SearchResult>(responseBody);
                    foreach (Model_SearchResult res in data.results)
                    {
                        Response_MovieDetail detail = await Request_Get_MovieDetail_by_ID(res.id);
                        if (detail.CanParse())
                        {
                            await DownloadPosterForMovie(detail);

                            if (detail.imageData != null)
                            {
                                queryResults.Add(detail);
                            }

                        }

                    }
                    OnResults(queryResults);
                }
                else
                {
                    OnError?.Invoke();
                }



            }
            catch (HttpRequestException e)
            {
                OnError?.Invoke();
            }
        }

        public static async Task<Response_MovieDetail> Request_Get_MovieDetail_by_ID(string mmovieID)
        {
            Response_MovieDetail details = new();

            string bearer = "Bearer " + KEY;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri_movie_details + mmovieID),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", bearer},
                }
            };

            try
            {
                HttpResponseMessage response = await Client.SendAsync(request);
                int statusCode = (int)response.StatusCode;
                string responseBody = await response.Content.ReadAsStringAsync();

                details = JsonUtility.FromJson<Response_MovieDetail>(responseBody);

            }
            catch (HttpRequestException e)
            {

            }
            return details;
        }

        private static async Task DownloadPosterForMovie(Response_MovieDetail movie)
        {
            string bearer = "Bearer " + KEY;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri_imageFilePath + "w500" + movie.poster_path),
                Headers =
                    {
                        { "accept", "application/json" },
                        { "Authorization", bearer},
                    }
            };

            try
            {
                HttpResponseMessage response = await Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                movie.SetImageData(bytes);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
            }
        }


    }

}
using Azure;
using System;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;



namespace TeamSpecs.RideAlong.CarNewsCenter
{
    public class CarNewsCenterViewVehicleNewsArticleService : ICarNewsCenterViewVehicleNewsArticleServices
    {
        private readonly ICarNewsCenterTarget _vehicleTarget;
        private readonly ILogService _logService;
        public CarNewsCenterViewVehicleNewsArticleService(ICarNewsCenterTarget sqlDbCarNewsCenterTarget, ILogService logService)
        {
            _vehicleTarget = sqlDbCarNewsCenterTarget;
            _logService = logService;
        }
        public async Task<IResponse> GetNewsForAllVehicles(IAccountUserModel userAccount)
        {
            #region Validate Parameters
            if (userAccount is null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }
            if (string.IsNullOrWhiteSpace(userAccount.UserHash))
            {
                throw new ArgumentNullException(nameof(userAccount.UserHash));
            }
            #endregion

            var search = new List<object>()
            {
            new KeyValuePair<string, long>("Owner_UID", userAccount.UserId)
            };

            //getting all veihicles from db
            var response = _vehicleTarget.GetsAllVehicles(search);



            #region API REQUEST 
            //Extracting all Make and Model from response 
            //SettingUpAPI
            var test = new Model.Response {
                ReturnValue = new List<object>()
            };
            HttpClient client = new HttpClient();
            var keyword = "";
            const string API_KEY = "871284c554b7e91b19a642b97c7f816f";
            string URL = "";
            if (response.ReturnValue is null)
            {
                throw new ArgumentNullException(nameof(response.ReturnValue));
            }
            foreach (VehicleProfileModel vehicle in response.ReturnValue)
            {
                //Constructing keyword for making API Request 
                keyword = vehicle.Make + "&" + vehicle.Model;
                URL = $"https://gnews.io/api/v4/search?q={keyword}&lang=en&country=us&max=3&apikey={API_KEY}";//3 at a time should be enoughv
                HttpResponseMessage result = await client.GetAsync(URL);
                result.EnsureSuccessStatusCode();
                string responseBody = await result.Content.ReadAsStringAsync();
                if (responseBody is not null)
                {
                    test.ReturnValue.Add(responseBody);

                }
                //var data = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                /*var data = JsonSerializer.Deserialize<ApiResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                List<Articles> articles = data.Articles;*/
                //Looping through number of articles to construct a string and add to ReturnValue
                /*
                for (int i = 0; i < articles.Count; i++)
                {
                    // articles[i].title
                    test.ReturnValue.Add(($"Title: {articles[i].Title}\n") + ($"Description: {articles[i].Description}\n") + ($"URL: {articles[i].Url}\n") + ($"Source: {articles[i].Source}\n"));
                    // articles[i].description
                    /* Console.WriteLine($"Description: {articles[i].Description}\n");
                     Console.WriteLine($"URL: {articles[i].Url}\n");
                     Console.WriteLine($"Source: {articles[i].Source}\n");
                     Console.WriteLine($"Date: {articles[i].PublishedAt}\n");
                }*/

            }
            #endregion

            #region Log the action to the database
            if (response.HasError)
            {
                response.ErrorMessage = "Could not retrieve vehicles. " + response.ErrorMessage;
            }
            else
            {
                response.ErrorMessage = "Successful retrieval of vehicle profile. ";
            }
            await _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
            #endregion
            return test;
        }
    }
}

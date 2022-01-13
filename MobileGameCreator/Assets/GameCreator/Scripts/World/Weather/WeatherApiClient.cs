using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GraphQlClient.Core;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace GameCreator.Weather
{

    public class WeatherApiClient : MonoBehaviour
    {
        public class WeatherData
        {
            public uint cityId;
            public bool isLoaded;

            /// <summary>
            /// Timestamp in GMT
            /// </summary>
            public uint timestamp;

            /// <summary>
            /// Temperature in Kelvin
            /// </summary>
            public float temperature;
            public float windSpeed;
            public float windDirection;
            public float clouds;

            public string summaryTitle;
            public string summaryDescription;
            public string summaryIcon;
        }

        [SerializeField] private GraphApi weatherApi;

        private Dictionary<uint, WeatherData> cache = new Dictionary<uint, WeatherData>();

        public WeatherData GetCityWeather(uint cityId)
        {
            if (!cache.TryGetValue(cityId, out WeatherData weather))
            {
                weather = new WeatherData()
                {
                    cityId = cityId
                };

                cache[cityId] = weather;
            }

            return weather;
        }


        public async Task<WeatherData> LoadWeatherForCity(uint cityId)
        {
            WeatherData weatherData = GetCityWeather(cityId);

            try
            {
                GraphApi.Query query = weatherApi.GetQueryByName(
                    "GetCityWeatherById",
                    GraphApi.Query.Type.Query
                );
                query.SetArgs(new { id = cityId.ToString() });

                UnityWebRequest request = await weatherApi.Post(query);

                string json = request.downloadHandler.text;
                JObject response = JObject.Parse(json);
                JArray entries = (response["data"]["getCityById"] as JArray);

                bool success = false;
                if (entries != null && entries.Count > 0)
                {
                    success = true;

                    JObject entry = (JObject)entries[0];
                    JObject weather = (JObject)entry["weather"];
                    JObject wind = (JObject)weather["wind"];
                    JObject clouds = (JObject)weather["clouds"];
                    JObject summary = (JObject)weather["summary"];

                    weatherData.isLoaded = true;
                    weatherData.timestamp = (uint)weather["timestamp"];
                    weatherData.temperature = (float)weather["temperature"]["actual"];
                    weatherData.windSpeed = (float)wind["speed"];
                    weatherData.windDirection = (float)wind["deg"];
                    weatherData.clouds = (float)clouds["all"];
                    weatherData.summaryTitle = (string)summary["title"];
                    weatherData.summaryDescription = (string)summary["description"];
                    weatherData.summaryIcon = (string)summary["icon"];
                }

                if (!success)
                    Debug.LogError("Failed retrieving weather info: " + response["error"]);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            return weatherData;
        }

        public static WeatherConditionType GetConditionByIcon(string icon)
        {
            Debug.Assert(icon.Length >= 2);

            string id = icon.Substring(0, 2);

            if (id == "09" || id == "10")
                return WeatherConditionType.Rain;

            if (id == "11")
                return WeatherConditionType.Thunderstorm;

            if (id == "13")
                return WeatherConditionType.Snow;

            return WeatherConditionType.Clear;
        }
    }
}
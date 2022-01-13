
namespace GameCreator.Weather
{
    public class WeatherState
    {
        public bool isLoaded;

        public WeatherConditionType condition;
        public string description;

        /// <summary>
        /// Temperature in Celsius
        /// </summary>
        public float temperature;
        public float clouds;

        public float windSpeed;
        public float windDirection;
    }
}

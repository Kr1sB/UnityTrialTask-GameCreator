using UnityEngine;

namespace GameCreator.Weather
{
    //TODO(cb): Separate time-related code

    public static class WeatherUtil
    {
        public const int nightStartHour = 23;
        public const int eveningStartHour = 18;
        public const int dayStartHour = 11;
        public const int morningStartHour = 5;

        public static TimeOfDay HourToTimeOfDay(int hour)
        {
            hour = Mathf.Clamp(hour, 0, 23);

            if (hour >= nightStartHour)
                return TimeOfDay.Night;

            if (hour >= eveningStartHour)
                return TimeOfDay.Evening;

            if (hour >= dayStartHour)
                return TimeOfDay.Day;

            if (hour >= morningStartHour)
                return TimeOfDay.Morning;

            return TimeOfDay.Night;
        }


        public static float KelvinToCelsius(float kelvin) =>
            kelvin - 273.15f;

        public static float KelvinToFahrenheit(float kelvin) =>
            kelvin * (9f / 5f) - 459.67f;

        public static float CelsiusToFahrenheit(float celsius) =>
            celsius * (9f / 5f) + 32f;

        public static string CelsiusToString(float celsius) =>
            string.Format("{0:0.0} °C", celsius);

        public static string FahrenheitToString(float fahrenheit) =>
            string.Format("{0:0.0} °F", fahrenheit);

        public static string CelsiusToFahrenheitString(float celsius) =>
            FahrenheitToString(CelsiusToFahrenheit(celsius));
    }
}

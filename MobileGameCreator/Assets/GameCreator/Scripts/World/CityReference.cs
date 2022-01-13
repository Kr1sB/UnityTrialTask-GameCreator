using UnityEngine;

[System.Serializable]
public class CityReference
{
    public string label;

    [Tooltip("City ID provided by OpenWeather API")]
    /// <summary>
    /// City ID provided by OpenWeather API
    /// </summary>
    /// <see cref="https://openweathermap.org/current#cityid"/>
    public uint id;
    public string timezoneId;
}
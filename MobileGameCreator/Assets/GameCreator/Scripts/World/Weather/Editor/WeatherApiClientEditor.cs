using UnityEngine;
using UnityEditor;

namespace GameCreator.Weather
{

    [CustomEditor(typeof(WeatherApiClient))]
    public class WeatherApiClientEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            /*
            WeatherApiClient client = (WeatherApiClient)target;
            var lib = client.cityReferences;

            if(lib != null)
            {
                for(int i=0; i < lib.cities.Length; ++i)
                {
                    CityReference city = lib.cities[i];

                    if(GUILayout.Button("Test " + city.label))
                    {
                        client.LoadWeatherForCity(city.id, city.timezoneId);
                    }
                }
            }
            */
            DrawDefaultInspector();
        }
    }
}
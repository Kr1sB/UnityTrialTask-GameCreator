using UnityEngine;

namespace GameCreator.Weather
{
    public class LightingController : MonoBehaviour
    {
        [System.Serializable]
        public class TimeSettings
        {
            public LightingPreset lightingPreset;
        }

        [System.Serializable]
        public class TimeOfDaySettings
        {
            public TimeSettings morning;
            public TimeSettings day;
            public TimeSettings evening;
            public TimeSettings night;
        }


        [SerializeField] private TimeOfDaySettings timeOfDaySettings;

        public Light sun;
        private TimeSettings currentTimeSettings;


        private TimeSettings GetTimeSettings(TimeOfDay timeOfDay)
        {
            switch(timeOfDay)
            {
                case TimeOfDay.Night: return timeOfDaySettings.night;
                case TimeOfDay.Evening: return timeOfDaySettings.evening;
                case TimeOfDay.Morning: return timeOfDaySettings.morning;

                case TimeOfDay.Day:
                default:
                    return timeOfDaySettings.day;
            };
        }

        public void SetTimeOfDay(TimeOfDay timeOfDay)
        {
            TimeSettings settings = GetTimeSettings(timeOfDay);
            ApplyTimeSettings(settings);
        }

        private void ApplyTimeSettings(TimeSettings settings)
        {
            currentTimeSettings = settings;
            UpdateLighting();
        }

        private void UpdateLighting()
        {
            LightingPreset p = currentTimeSettings.lightingPreset;

            Light sun = RenderSettings.sun;
            sun.transform.rotation = Quaternion.Euler(p.sun.rotation);
            sun.intensity = p.sun.intensity;
            sun.color = p.sun.color;

            RenderSettings.fog = p.fog.enabled;
            RenderSettings.fogColor = p.fog.color;
            RenderSettings.fogDensity = p.fog.density;
            RenderSettings.skybox = p.skybox.material;
        }
    }
}
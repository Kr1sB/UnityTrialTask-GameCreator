using UnityEngine;

namespace GameCreator.Weather
{
    public class WeatherEffectBox : MonoBehaviour
    {
        [System.Serializable]
        public class Effects
        {
            public WeatherEffect clear;
            public WeatherEffect drizzle;
            public WeatherEffect rain;
            public WeatherEffect thunderstorm;
            public WeatherEffect snow;
        }


        [SerializeField] private Effects effects;

        public Transform target;

        private WeatherEffect currentEffect;
        private WeatherConditionType currentCondition;

        public void SetWeatherCondition(WeatherConditionType condition)
        {
            if (currentEffect != null)
            {
                if (condition == currentCondition)
                    return;

                currentEffect.StopEffect();
            }

            currentCondition = condition;
            currentEffect = GetEffect(condition);
            currentEffect.StartEffect();
        }

        private WeatherEffect GetEffect(WeatherConditionType condition)
        {
            //TODO(cb): Make the lookup more efficient/flexible

            switch(condition)
            {
                case WeatherConditionType.Drizzle: return effects.drizzle;
                case WeatherConditionType.Rain: return effects.rain;
                case WeatherConditionType.Thunderstorm: return effects.thunderstorm;
                case WeatherConditionType.Snow: return effects.snow;

                case WeatherConditionType.Clear:
                default:
                    return effects.clear;
            }
        }

        private void Update()
        {
            if(target)
            {
                transform.position = target.position;
                transform.rotation = Quaternion.Euler(0, target.rotation.eulerAngles.y, 0);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeZoneInfo=System.TimeZoneInfo;
using DateTime = System.DateTime;
using UnityEngine;


namespace GameCreator.Weather
{
    public class WeatherController : MonoBehaviour
    {
        public delegate void WeatherEvent(WeatherState weather);
        public event WeatherEvent onWeatherUpdated;

        public static WeatherController instance { get; private set; }

        [SerializeField] private CityReferenceLibrary _cityReferences;
        public CityReferenceLibrary cityReferences => _cityReferences;

        [SerializeField] private WeatherEffectBox effectBox;
        [SerializeField] private WeatherApiClient weatherApiClient;

        [SerializeField] private LightingController _lighting;

        [Tooltip("Update interval in seconds")]
        [SerializeField] private float cityWeatherUpdateInterval = 10;

        [SerializeField] private TimeOfDay _defaultTimeOfDay;
        [SerializeField] private WeatherConditionType _defaultWeatherCondition;

        private WeatherState _currentWeather;
        public WeatherState currentWeather => _currentWeather;

        private TimeOfDay _currentTimeOfDay;
        public TimeOfDay currentTimeOfDay => _currentTimeOfDay;

        private WeatherState _customWeather;
        public WeatherState customWeather => _customWeather;

        private TimeOfDay _customTimeOfDay;
        public TimeOfDay customTimeOfDay => _customTimeOfDay;

        public TimeOfDay cityTimeOfDay
        {
            get
            {
                if (_currentCity == null)
                    return TimeOfDay.Day;

                return WeatherUtil.HourToTimeOfDay(currentTime.Hour);
            }
        }

        public bool isUsingCityWeather =>
            _currentCity != null;

        public DateTime currentTime =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _currentTimeZone);

        public DateTime currentSystemTime =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);

        public TimeOfDay systemTimeOfDay =>
            WeatherUtil.HourToTimeOfDay(currentSystemTime.Hour);


        private TimeZoneInfo _currentTimeZone;
        public TimeZoneInfo currentTimeZone =>
            _currentTimeZone;

        private CityReference _currentCity;
        public CityReference currentCity => _currentCity;

        public uint currentCityId
        {
            get
            {
                if (_currentCity == null)
                    return 0;

                return _currentCity.id;
            }
        }


        private Dictionary<uint, WeatherState> cityWeatherMap = new Dictionary<uint, WeatherState>();
        private Coroutine updateCityWeatherRoutine;

        private void Awake()
        {
            if (instance != null)
                throw new System.Exception("Multiple instances of " + GetType());

            instance = this;

            //
            // Init weather
            //

            _customWeather = MakeDefaultWeather();
            _customWeather.condition = _defaultWeatherCondition;

            _customWeather.isLoaded = true;
            _currentTimeZone = TimeZoneInfo.Local;
            _customTimeOfDay = _defaultTimeOfDay;

            UseCustomWeather();
        }

        public CityReference GetCityByIndex(int index) =>
            cityReferences.GetByIndex(index);

        public void SetCustomTimeOfDay(TimeOfDay timeOfDay)
        {
            _customTimeOfDay = timeOfDay;
        }

        public void SetTimeOfDay(TimeOfDay timeOfDay)
        {
            _currentTimeOfDay = timeOfDay;
            _lighting.SetTimeOfDay(timeOfDay);
        }

        public void SetWeatherCondition(WeatherConditionType condition)
        {
            _currentWeather.condition = condition;
            effectBox.SetWeatherCondition(condition);
        }

        public void BindEffectBox(Transform target)
        {
            effectBox.target = target;
        }

        private WeatherState MakeDefaultWeather()
        {
            var weather = new WeatherState()
            {
                condition = WeatherConditionType.Clear,
                temperature = 25
            };

            return weather;
        }


        private void ApplyWeather(WeatherState weather)
        {
            _currentWeather = weather;
            SetWeatherCondition(_currentWeather.condition);
        }


        public void UseCustomWeather()
        {
            _currentCity = null;
            _currentTimeZone = TimeZoneInfo.Local;

            StopCityWeatherUpdate();

            ApplyWeather(_customWeather);
            SetTimeOfDay(_customTimeOfDay);
        }


        public void UseCityWeatherByIndex(int cityIndex) =>
            UseCityWeather(GetCityByIndex(cityIndex));


        public void UseCityWeather(CityReference reference)
        {
            _currentCity = reference;
            _currentTimeZone = SystemHelper.GetTimeZoneInfoById(reference.timezoneId);

            UpdateCurrentCityWeather();

            ApplyWeather(GetCityWeather(reference.id));
            SetTimeOfDay(cityTimeOfDay);
        }

        private void UpdateCurrentCityWeather()
        {
            StopCityWeatherUpdate();
            updateCityWeatherRoutine = StartCoroutine(UpdateCityWeatherRoutine());
        }


        private void StopCityWeatherUpdate()
        {
            if (updateCityWeatherRoutine == null)
                return;

            StopCoroutine(updateCityWeatherRoutine);
            updateCityWeatherRoutine = null;
        }


        private IEnumerator UpdateCityWeatherRoutine()
        {
            for(;;)
            {
                if (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    Task<WeatherApiClient.WeatherData> task = weatherApiClient.LoadWeatherForCity(_currentCity.id);
                    yield return new WaitUntil(() => task.IsCompleted);

                    if(task.Status == TaskStatus.RanToCompletion)
                    {
                        ReadWeatherData(task.Result, _currentWeather);
                        SetTimeOfDay(cityTimeOfDay);
                        SetWeatherCondition(_currentWeather.condition);

                        onWeatherUpdated?.Invoke(_currentWeather);
                    }
                }

                yield return new WaitForSecondsRealtime(cityWeatherUpdateInterval);
            }
        }


        private void ReadWeatherData(WeatherApiClient.WeatherData data, WeatherState weather)
        {
            weather.isLoaded = data.isLoaded;
            weather.temperature = WeatherUtil.KelvinToCelsius(data.temperature);
            weather.clouds = data.clouds;

            weather.windDirection = data.windDirection;
            weather.windSpeed = data.windSpeed;
            weather.condition = WeatherApiClient.GetConditionByIcon(data.summaryIcon);
            weather.description = data.summaryDescription;
        }


        public WeatherState GetCityWeather(uint cityId)
        {
            if(!cityWeatherMap.TryGetValue(cityId, out WeatherState weather))
            {
                weather = MakeDefaultWeather();
                cityWeatherMap[cityId] = weather;
            }

            return weather;
        }
    }
}

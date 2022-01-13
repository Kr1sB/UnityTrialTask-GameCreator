using UnityEngine;
using GameCreator.UI;
using GameCreator.Weather;
using System.Collections.Generic;


namespace GameCreator.Creator.UI
{
    public class CustomSkySettingsPanel : UIView
    {
        [SerializeField] private OptionGroup timeOfDayOptions;
        [SerializeField] private OptionGroup weatherOptions;

        private WeatherController controller;

        private void Awake()
        {
            //
            // Options for time of day 
            //

            timeOfDayOptions.onSelectionChanged += TimeSelected;
            timeOfDayOptions.AddOptions(
                new List<string>(System.Enum.GetNames(typeof(TimeOfDay)))
            );

            //
            // Options for weather condition
            //

            weatherOptions.onSelectionChanged += WeatherSelected;
            weatherOptions.AddOptions(
                new List<string>(System.Enum.GetNames(typeof(WeatherConditionType)))
            );
            weatherOptions.SelectWithoutNotify((int)WeatherConditionType.Clear);
        }


        override protected void OnShow()
        {
            base.OnShow();

            controller = WeatherController.instance;

            TimeOfDay time = controller.customTimeOfDay;
            timeOfDayOptions.SelectWithoutNotify((int)time);
        }


        private void TimeSelected(int index)
        {
            var timeOfDay = (TimeOfDay)index;

            controller.SetCustomTimeOfDay(timeOfDay);
            controller.SetTimeOfDay(timeOfDay);
        }

        private void WeatherSelected(int index)
        {
            var condition = (WeatherConditionType)index;
            controller.SetWeatherCondition(condition);
        }
    }
}

using System.Collections;
using GameCreator.UI;
using GameCreator.Weather;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Creator.UI
{
    public class CitySkySettingsPanel : UIView
    {
        [SerializeField] private TMP_Text cityLabel;
        [SerializeField] private TMP_Text timeLabel;
        [SerializeField] private TMP_Text descriptionLabel;
        [SerializeField] private TMP_Text systemTimeLabel;
        [SerializeField] private TMP_Text temperatureLabel;

        private WeatherController controller;

        private CityReference city;

        override protected void OnShow()
        {
            base.OnShow();

            controller = WeatherController.instance;
            controller.onWeatherUpdated += WeatherUpdated;

            StartCoroutine(TimeUpdateRoutine());
        }


        override protected void OnHide()
        {
            base.OnHide();

            StopAllCoroutines();
            controller.onWeatherUpdated -= WeatherUpdated;
        }

        private IEnumerator TimeUpdateRoutine()
        {
            for(;;)
            {
                yield return new WaitForSecondsRealtime(1);
                UpdateTimeDisplay();
            }
        }


        private void WeatherUpdated(WeatherState weather)
        {
            UpdateDisplay();
        }

        public void SetCity(CityReference city)
        {
            this.city = city;
            UpdateDisplay();
        }


        private void UpdateDisplay()
        {
            cityLabel.text = city.label;

            WeatherState weather = controller.currentWeather;

            if (weather.isLoaded)
            {
                descriptionLabel.text = string.Concat(
                    weather.condition,
                    " (", weather.description, ")"
                );

                float temperature = weather.temperature;
                temperatureLabel.text = string.Concat(
                    WeatherUtil.CelsiusToString(temperature),
                    " / ",
                    WeatherUtil.CelsiusToFahrenheitString(temperature)
                );
            }
            else
            {
                descriptionLabel.text = "--";
                temperatureLabel.text = "-- °C / -- °F";
            }

            UpdateTimeDisplay();
        }


        private void UpdateTimeDisplay()
        {
            //TODO(cb): Support different culture formats
            System.DateTime now = controller.currentTime;

            timeLabel.text = string.Concat(
                now.ToString("HH:mm:ss"),
                " (", controller.currentTimeOfDay, ")"
            );

            System.DateTime systemNow = controller.currentSystemTime;
            systemTimeLabel.text = string.Concat(
                systemNow.ToString("HH:mm:ss"),
                " (", controller.systemTimeOfDay, ")"
            );
        }
    }
}
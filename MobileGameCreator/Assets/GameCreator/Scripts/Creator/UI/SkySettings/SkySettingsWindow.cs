using UnityEngine;
using GameCreator.UI;
using GameCreator.Weather;


namespace GameCreator.Creator.UI
{
    public class SkySettingsWindow : UIView
    {
        [SerializeField] private OptionGroup cityOptions;

        [SerializeField] private CustomSkySettingsPanel customSettingsPanel;
        [SerializeField] private CitySkySettingsPanel citySettingsPanel;

        private WeatherController controller;

        private void Awake()
        {
            cityOptions.onSelectionChanged += CitySelected;
        }

        private void OnEnable()
        {
            if (controller == null)
                Initialize();
        }

        private void Initialize()
        {
            controller = WeatherController.instance;

            cityOptions.AddOption("Custom");

            CityReferenceLibrary lib = controller.cityReferences;
            for (int i = 0; i < lib.cities.Length; ++i)
                cityOptions.AddOption(lib.cities[i].label);

            cityOptions.Select(0);
        }


        private void CitySelected(int index)
        {
            if (index == 0)
            {
                //
                // Custom
                //

                customSettingsPanel.Show();
                citySettingsPanel.Hide();

                controller.UseCustomWeather();
            }
            else
            {
                customSettingsPanel.Hide();
                citySettingsPanel.Show();

                CityReference city = controller.GetCityByIndex(index - 1);
                controller.UseCityWeather(city);
                citySettingsPanel.SetCity(city);
            }
        }
    }
}
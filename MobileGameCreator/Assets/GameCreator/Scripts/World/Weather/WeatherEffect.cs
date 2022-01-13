using UnityEngine;


namespace GameCreator.Weather
{
    public class WeatherEffect : MonoBehaviour
    {
        public void StartEffect()
        {
            gameObject.SetActive(true);
        }

        public void StopEffect()
        {
            gameObject.SetActive(false);
        }
    }
}

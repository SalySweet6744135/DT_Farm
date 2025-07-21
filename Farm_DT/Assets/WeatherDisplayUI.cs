using UnityEngine;
using TMPro;
using System;

public class WeatherDisplayUI : MonoBehaviour
{
    public TextMeshProUGUI weatherText;
    public SkyboxWeatherController weatherController;

    void Update()
    {
        if (weatherController != null && weatherText != null && weatherController.isWeatherReady)
        {
            string timeNow = DateTime.Now.ToString("HH:mm");
            string temp = weatherController.GetCurrentTemperature();
            string weather = weatherController.GetCurrentWeatherName();

            weatherText.text = $"Time: {timeNow} Temp: {temp}°C Weather: {weather}";
        }
    }
}

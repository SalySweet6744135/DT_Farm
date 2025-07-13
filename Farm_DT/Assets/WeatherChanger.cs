using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class WeatherInfo
{
    public string main;
    public string description;
}

[System.Serializable]
public class WeatherArray
{
    public WeatherInfo[] weather;
    public Clouds clouds;
}

[System.Serializable]
public class Clouds
{
    public int all;
}

public class WeatherChanger : MonoBehaviour
{
    public string apiKey = "0b162a8bfadefd25f210878f1f172e69"; // Your API Key
    public string city = "Jeddah";

    public GameObject cloud;
    public GameObject rain;

    void Start()
    {
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            WeatherArray weatherData = JsonUtility.FromJson<WeatherArray>(json);

            string mainWeather = weatherData.weather[0].main;
            int cloudCoverage = weatherData.clouds.all;

            Debug.Log("Main Weather: " + mainWeather);
            Debug.Log("☁️ Cloud Coverage: " + cloudCoverage + "%");

            // Logic to enable or disable particles based on weather
            if (mainWeather == "Rain" || cloudCoverage > 60)
            {
                cloud.SetActive(true);
                rain.SetActive(true);
                Debug.Log("🌧️ Rainy sky applied.");
            }
            else if (cloudCoverage > 20)
            {
                cloud.SetActive(true);
                rain.SetActive(false);
                Debug.Log("⛅ Cloudy sky applied.");
            }
            else
            {
                cloud.SetActive(false);
                rain.SetActive(false);
                Debug.Log("🌞 Sunny sky applied.");
            }
        }
        else
        {
            Debug.LogError("Error fetching weather: " + www.error);
        }
    }
}

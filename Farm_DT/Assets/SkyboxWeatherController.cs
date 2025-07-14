using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SkyboxWeatherController : MonoBehaviour
{
    public string city = "Abha";
    public string apiKey = "7aea9b145adb6f2c936293a5e7dc3f1c";

    public Material[] clearSkyboxes;
    public Material[] cloudySkyboxes;
    public Material[] rainySkyboxes;
    public Material[] nightSkyboxes;
    public Material[] fogSkyboxes;

    public GameObject rainEffect;
    public GameObject cloudEffect;

    private string currentWeather = ""; // ✅ الآن متغير عام

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
            string weatherJson = www.downloadHandler.text;
            WeatherResponse weather = JsonUtility.FromJson<WeatherResponse>(weatherJson);

            currentWeather = weather.weather[0].main.ToLower();
            string icon = weather.weather[0].icon;

            Debug.Log("☁️ Weather Condition: " + currentWeather);
            Debug.Log("🕒 Icon Code: " + icon);

            // Skybox
            if (icon.EndsWith("n") && nightSkyboxes.Length > 0)
                RenderSettings.skybox = nightSkyboxes[Random.Range(0, nightSkyboxes.Length)];
            else if (currentWeather.Contains("clear") && clearSkyboxes.Length > 0)
                RenderSettings.skybox = clearSkyboxes[Random.Range(0, clearSkyboxes.Length)];
            else if ((currentWeather.Contains("cloud") || currentWeather.Contains("overcast")) && cloudySkyboxes.Length > 0)
                RenderSettings.skybox = cloudySkyboxes[Random.Range(0, cloudySkyboxes.Length)];
            else if (currentWeather.Contains("rain") && rainySkyboxes.Length > 0)
                RenderSettings.skybox = rainySkyboxes[Random.Range(0, rainySkyboxes.Length)];
            else if ((currentWeather.Contains("fog") || currentWeather.Contains("mist")) && fogSkyboxes.Length > 0)
                RenderSettings.skybox = fogSkyboxes[Random.Range(0, fogSkyboxes.Length)];

            // Effects
            if (rainEffect != null) rainEffect.SetActive(currentWeather.Contains("rain"));
            if (cloudEffect != null) cloudEffect.SetActive(currentWeather.Contains("cloud") || currentWeather.Contains("overcast"));

            // ✅ نحدث النباتات الآن
            UpdatePlantsWithWeather(currentWeather);
        }
        else
        {
            Debug.LogError("API Error: " + www.error);
        }
    }

    // ✅ تحديث النباتات بناءً على الطقس
    void UpdatePlantsWithWeather(string weather)
    {
        PlantWeatherReaction[] allPlants = Object.FindObjectsByType<PlantWeatherReaction>(FindObjectsSortMode.None);
        foreach (var plant in allPlants)
        {
            plant.ApplyWeatherEffect(weather);
        }
    }

    [System.Serializable]
    public class Weather
    {
        public string main;
        public string icon;
    }

    [System.Serializable]
    public class WeatherResponse
    {
        public Weather[] weather;
    }
}

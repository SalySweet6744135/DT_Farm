using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SkyboxWeatherController : MonoBehaviour
{
    public string city = "Abha";
    public string apiKey = "7aea9b145adb6f2c936293a5e7dc3f1c";

    public Material[] clearSkyboxes;
    public Material[] cloudySkyboxes;
    public Material[] nightSkyboxes;

    public GameObject rainEffect;
    public GameObject cloudEffect;
    public GameObject fogEffect;

    public AudioSource rainSound;
    public AudioSource nightSound;
    public AudioSource clearSound;

    private string currentWeather = "";

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

            // إيقاف التأثيرات
            if (rainEffect) rainEffect.SetActive(false);
            if (cloudEffect) cloudEffect.SetActive(false);
            if (fogEffect) fogEffect.SetActive(false);

            // إيقاف جميع الأصوات
            if (rainSound) rainSound.Stop();
            if (nightSound) nightSound.Stop();
            if (clearSound) clearSound.Stop();

            // سكايبوكس وصوت بناءً على الحالة
            if (icon.EndsWith("n") && nightSkyboxes.Length > 0)
            {
                RenderSettings.skybox = nightSkyboxes[Random.Range(0, nightSkyboxes.Length)];
                if (nightSound) nightSound.Play();
            }
            else if (currentWeather.Contains("clear") && clearSkyboxes.Length > 0)
            {
                RenderSettings.skybox = clearSkyboxes[Random.Range(0, clearSkyboxes.Length)];
                if (clearSound) clearSound.Play();
            }
            else if ((currentWeather.Contains("cloud") || currentWeather.Contains("overcast")) && cloudySkyboxes.Length > 0)
            {
                RenderSettings.skybox = cloudySkyboxes[Random.Range(0, cloudySkyboxes.Length)];
                if (cloudEffect) cloudEffect.SetActive(true);
                if (clearSound) clearSound.Play();
            }
            else if (currentWeather.Contains("rain"))
            {
                if (cloudySkyboxes.Length > 0)
                    RenderSettings.skybox = cloudySkyboxes[Random.Range(0, cloudySkyboxes.Length)];

                if (rainEffect) rainEffect.SetActive(true);
                if (rainSound) rainSound.Play();
            }
            else if (currentWeather.Contains("fog") || currentWeather.Contains("mist"))
            {
                if (cloudySkyboxes.Length > 0)
                    RenderSettings.skybox = cloudySkyboxes[Random.Range(0, cloudySkyboxes.Length)];

                if (fogEffect) fogEffect.SetActive(true);
                if (clearSound) clearSound.Play();
            }

            UpdatePlantsWithWeather(currentWeather);
        }
        else
        {
            Debug.LogError("API Error: " + www.error);
        }
    }

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

using System.Collections;
using TMPro;
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

    public Light sceneLight;
    public Color dayLightColor = Color.white;
    public Color nightLightColor = new Color(0.2f, 0.2f, 0.4f);
    public float dayIntensity = 1.2f;
    public float nightIntensity = 0.3f;
    public float cloudyIntensity = 0.8f;
    public float rainyIntensity = 0.5f;
    public float foggyIntensity = 0.6f;

    private string currentWeather = "unknown";
    private string currentTemp = "N/A";

    public bool isWeatherReady = false;

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
            currentTemp = weather.main.temp.ToString("F1");

            Debug.Log("☁️ Weather Condition: " + currentWeather);
            Debug.Log("🕒 Icon Code: " + icon);
            Debug.Log("🌡️ Temperature: " + currentTemp + "°C");

            if (rainEffect) rainEffect.SetActive(false);
            if (cloudEffect) cloudEffect.SetActive(false);
            if (fogEffect) fogEffect.SetActive(false);

            if (rainSound) rainSound.Stop();
            if (nightSound) nightSound.Stop();
            if (clearSound) clearSound.Stop();

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

            UpdateLighting(currentWeather, icon);
            UpdatePlantsWithWeather(currentWeather);

            isWeatherReady = true;
        }
        else
        {
            Debug.LogError("API Error: " + www.error);
        }
    }

    void UpdateLighting(string weather, string icon)
    {
        if (sceneLight == null) return;

        if (icon.EndsWith("n"))
        {
            sceneLight.color = nightLightColor;
            sceneLight.intensity = nightIntensity;
            sceneLight.transform.rotation = Quaternion.Euler(340f, 0f, 0f);
        }
        else if (weather.Contains("clear"))
        {
            sceneLight.color = dayLightColor;
            sceneLight.intensity = dayIntensity;
            sceneLight.transform.rotation = Quaternion.Euler(50f, 30f, 0f);
        }
        else if (weather.Contains("cloud") || weather.Contains("overcast"))
        {
            sceneLight.color = dayLightColor * 0.8f;
            sceneLight.intensity = cloudyIntensity;
            sceneLight.transform.rotation = Quaternion.Euler(50f, 30f, 0f);
        }
        else if (weather.Contains("rain"))
        {
            sceneLight.color = new Color(0.6f, 0.6f, 0.7f);
            sceneLight.intensity = rainyIntensity;
            sceneLight.transform.rotation = Quaternion.Euler(50f, 30f, 0f);
        }
        else if (weather.Contains("fog") || weather.Contains("mist"))
        {
            sceneLight.color = new Color(0.7f, 0.7f, 0.7f);
            sceneLight.intensity = foggyIntensity;
            sceneLight.transform.rotation = Quaternion.Euler(50f, 30f, 0f);
        }

        RenderSettings.ambientLight = sceneLight.color * 0.6f;
    }

    void UpdatePlantsWithWeather(string weather)
    {
        PlantWeatherReaction[] allPlants = Object.FindObjectsByType<PlantWeatherReaction>(FindObjectsSortMode.None);
        foreach (var plant in allPlants)
        {
            plant.ApplyWeatherEffect(weather);
        }
    }

    public string GetCurrentTemperature()
    {
        return currentTemp;
    }

    public string GetCurrentWeatherName()
    {
        return currentWeather;
    }

    [System.Serializable]
    public class Weather
    {
        public string main;
        public string icon;
    }

    [System.Serializable]
    public class Main
    {
        public float temp;
    }

    [System.Serializable]
    public class WeatherResponse
    {
        public Weather[] weather;
        public Main main;
    }
}

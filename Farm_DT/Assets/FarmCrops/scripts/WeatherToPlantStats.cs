using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeatherToPlantStats : MonoBehaviour
{
    public string city = "Abha";
    public string apiKey = "7aea9b145adb6f2c936293a5e7dc3f1c"; // ✅ Your API key

    void Start()
    {
        StartCoroutine(GetWeatherAndApply());
    }

    IEnumerator GetWeatherAndApply()
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            WeatherData data = JsonUtility.FromJson<WeatherData>(www.downloadHandler.text);

            float humidity = data.main.humidity;        // 🟢 Soil Moisture
            float temperature = data.main.temp;         // 🟡 Sunlight approximation

            float soilMoisture = Mathf.Clamp(humidity, 0, 100); // Use humidity directly
            float sunlight = Mathf.Clamp((temperature / 40f) * 100f, 0, 100); // 40°C = full sunlight

            float healthIndex = (soilMoisture + sunlight) / 2f;

            Debug.Log($"🌱 Soil Moisture: {soilMoisture}%");
            Debug.Log($"☀️ Sunlight: {sunlight}%");
            Debug.Log($"🌿 Plant Health Index: {healthIndex}%");

            ApplyToAllPlants(soilMoisture, sunlight, healthIndex);
        }
        else
        {
            Debug.LogError("API Error: " + www.error);
        }
    }

    void ApplyToAllPlants(float soilMoisture, float sunlight, float healthIndex)
    {
        var plants = Object.FindObjectsByType<PlantWeatherReaction>(FindObjectsSortMode.None);

        foreach (var plant in plants)
        {
            plant.soilMoisture = soilMoisture;
            plant.sunlightExposure = sunlight;
            plant.plantHealth = healthIndex;
            plant.UpdateGrowthStage();
        }
    }

    [System.Serializable]
    public class WeatherMain
    {
        public float temp;
        public float humidity;
    }

    [System.Serializable]
    public class WeatherData
    {
        public WeatherMain main;
    }
}

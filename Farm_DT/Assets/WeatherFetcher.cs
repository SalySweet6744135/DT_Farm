using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq; // تأكد أنك أضفت Newtonsoft من Package Manager

public class WeatherFetcher : MonoBehaviour
{
    public Material sunnySky;
    public Material cloudySky;
    public Material rainySky;

    string apiKey = "0b162a8bfadefd25f210878f1f172e69";
    string city = "Jeddah";

    void Start()
    {
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
        Debug.Log("Weather URL: " + url);

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            Debug.Log("Weather: " + json);

            JObject weatherJson = JObject.Parse(json);
            string weatherMain = weatherJson["weather"][0]["main"].ToString();
            Debug.Log("Main Weather: " + weatherMain);

            // تغيير السماء حسب الحالة
            if (weatherMain == "Clear")
                RenderSettings.skybox = sunnySky;
            else if (weatherMain == "Clouds")
                RenderSettings.skybox = cloudySky;
            else if (weatherMain == "Rain" || weatherMain == "Drizzle")
                RenderSettings.skybox = rainySky;
            else
                Debug.Log("Unknown weather type: " + weatherMain);
        }
        else
        {
            Debug.LogError("Error fetching weather: " + www.error);
        }
    }
}

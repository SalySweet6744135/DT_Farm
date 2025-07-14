using UnityEngine;

public class PlantWeatherReaction : MonoBehaviour
{
    [Header("🪴 Plant Name (Enter Manually)")]
    public string plantName;

    [Header("🎭 Growth Stages")]
    public GameObject seedStage;
    public GameObject growingStage;
    public GameObject matureStage;
    public GameObject rottenStage;

    [Header("📊 Plant Stats")]
    [Range(0, 100)] public float soilMoisture = 50;
    [Range(0, 100)] public float sunlightExposure = 50;
    [Range(0, 100)] public float plantHealth = 80;

    void Start()
    {
        UpdateGrowthStage();
    }

    public void UpdateGrowthStage()
    {
        if (seedStage != null) seedStage.SetActive(false);
        if (growingStage != null) growingStage.SetActive(false);
        if (matureStage != null) matureStage.SetActive(false);
        if (rottenStage != null) rottenStage.SetActive(false);

        if (plantHealth <= 25)
        {
            if (rottenStage != null) rottenStage.SetActive(true);
        }
        else if (plantHealth <= 50)
        {
            if (growingStage != null) growingStage.SetActive(true);
        }
        else if (plantHealth <= 80)
        {
            if (matureStage != null) matureStage.SetActive(true);
        }
        else
        {
            if (seedStage != null) seedStage.SetActive(true);
        }
    }

    public void ApplyWeatherEffect(string weather)
    {
        if (weather == "rain") soilMoisture += 10;
        if (weather == "clear") sunlightExposure += 10;
        if (weather == "fog" || weather == "cloud") sunlightExposure -= 5;

        plantHealth = Mathf.Clamp(plantHealth + (soilMoisture > 70 ? -10 : 5), 0, 100);
        UpdateGrowthStage();
    }
}

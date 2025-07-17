using UnityEngine;
using System.Collections;

public class PlantGrowth : MonoBehaviour
{
    [Header("Stages of Growth (0 to N)")]
    public GameObject[] growthStages;

    private float growthTimer = 0f;
    private int currentStage = 0;

    private const float timeToGrowEachStage = 5f; // ËÇÈÊ ÏÇÆãðÇ 5 ËæÇäí

    void Start()
    {
        ShowOnlyCurrentStage();
    }

    void Update()
    {
        growthTimer += Time.deltaTime;

        if (growthTimer >= timeToGrowEachStage && currentStage < growthStages.Length - 1)
        {
            growthTimer = 0f;
            currentStage++;
            ShowOnlyCurrentStage();
        }
    }

    void ShowOnlyCurrentStage()
    {
        for (int i = 0; i < growthStages.Length; i++)
        {
            growthStages[i].SetActive(i == currentStage);
        }
    }
}


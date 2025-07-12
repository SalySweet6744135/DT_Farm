using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DroneDataCollector : MonoBehaviour
{
    public static DroneDataCollector Instance;

    private List<string> collectedData = new List<string>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void AddData(string plantName, float moisture, float health)
    {
        string line = $"{plantName},{moisture},{health}";
        collectedData.Add(line);
    }

    public void ExportToCSV()
    {
        string path = Application.dataPath + "/DroneReport.csv";
        List<string> output = new List<string>
        {
            "Plant Name,Soil Moisture,Health"
        };
        output.AddRange(collectedData);
        File.WriteAllLines(path, output.ToArray());

        Debug.Log("✅ Report exported to: " + path);
    }
}

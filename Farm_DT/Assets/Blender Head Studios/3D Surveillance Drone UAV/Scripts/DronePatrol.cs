using UnityEngine;

public class DronePatrol : MonoBehaviour
{
    public float speed = 5f;
    private Transform[] waypoints;
    private int currentIndex = 0;
    private bool[] visitedPoints;
    private bool isFinished = false;

    void Start()
    {
        GameObject pathParent = GameObject.Find("DronePath");

        if (pathParent == null)
        {
            Debug.LogError("❌ لم يتم العثور على DronePath في المشهد!");
            return;
        }

        int count = pathParent.transform.childCount;
        waypoints = new Transform[count];
        visitedPoints = new bool[count];

        for (int i = 0; i < count; i++)
        {
            waypoints[i] = pathParent.transform.GetChild(i);
        }

        System.Array.Sort(waypoints, (a, b) => a.name.CompareTo(b.name));
    }

    void Update()
    {
        if (isFinished || waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, target.position);

        if (!visitedPoints[currentIndex] && distance < 0.5f)
        {
            PlantWeatherReaction crop = target.GetComponent<PlantWeatherReaction>();
            if (crop != null)
            {
                DroneDataCollector.Instance.AddData(
                    target.name,
                    crop.soilMoisture,
                    crop.sunlightExposure,
                    crop.plantHealth
                );
                Debug.Log($"📦 Collected from {target.name}: moisture={crop.soilMoisture}, sun={crop.sunlightExposure}, health={crop.plantHealth}");
            }
            else
            {
                Debug.LogWarning($"⚠️ No PlantWeatherReaction script found on: {target.name}");
            }

            visitedPoints[currentIndex] = true;
        }

        if (distance < 0.2f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
            {
                isFinished = true;
                DroneDataCollector.Instance.ExportToCSV();
                Debug.Log("✅ Drone finished patrol and exported data.");
            }
        }
    }
}

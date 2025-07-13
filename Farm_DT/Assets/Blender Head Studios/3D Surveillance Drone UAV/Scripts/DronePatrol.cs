using UnityEngine;

public class DronePatrol : MonoBehaviour
{
    public float speed = 5f; // سرعة الطيران
    private Transform[] waypoints;
    private int currentIndex = 0;
    private bool[] visitedPoints;
    private bool isFinished = false;

    //public Animator DroneController;

    void Start()
    {
        //if (DroneController != null)
        //{
        //    DroneController.SetBool("isSpinning", true);
        //}

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

        // جمع البيانات إذا قرب من النبات
        if (!visitedPoints[currentIndex] && distance < 0.5f)
        {
            CropHealth crop = target.GetComponent<CropHealth>();
            if (crop != null)
            {
                DroneDataCollector.Instance.AddData(target.name, crop.soilMoisture, crop.health);
                Debug.Log($"📦 Collected from {target.name}: moisture={crop.soilMoisture}, health={crop.health}");
            }
            else
            {
                Debug.LogWarning($"⚠️ No CropHealth script found on: {target.name}");
            }

            visitedPoints[currentIndex] = true;
        }

        // التنقل للنقطة التالية
        if (distance < 0.2f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
            {
                isFinished = true;
                DroneDataCollector.Instance.ExportToCSV();
                Debug.Log("✅ Drone finished patrol and exported data.");

                // 🛑 إيقاف أنميشن المروحة
                //if (DroneController != null)
                //{
                //    DroneController.SetBool("isSpinning", false);
                //}
            }
        }
    }
}

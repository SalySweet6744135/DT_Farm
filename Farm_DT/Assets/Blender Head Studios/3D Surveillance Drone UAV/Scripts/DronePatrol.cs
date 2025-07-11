using UnityEngine;

public class DronePatrol : MonoBehaviour
{
    public float speed = 5f;                      // سرعة الطيران
    private Transform[] waypoints;               // قائمة النقاط
    private int currentIndex = 0;                // المؤشر الحالي
    private bool dataCollected = false;          // عشان ما يجمع مرتين

    void Start()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("Waypoint");
        waypoints = new Transform[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            waypoints[i] = points[i].transform;
        }

        System.Array.Sort(waypoints, (a, b) => a.name.CompareTo(b.name));
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // إذا قرب من الهدف: اجمع البيانات مره وحده
        if (!dataCollected && Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            CropHealth crop = target.GetComponent<CropHealth>();
            if (crop != null)
            {
                DroneDataCollector.Instance.AddData(target.name, crop.soilMoisture, crop.health);
                dataCollected = true;
            }
        }

        // إذا وصل للنقطة، ينتقل للي بعدها
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = 0;
            }

            dataCollected = false; // نسمح بجمع بيانات للنقطة الجديدة
        }
    }
}

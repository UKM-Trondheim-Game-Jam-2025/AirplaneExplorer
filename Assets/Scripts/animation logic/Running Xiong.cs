using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RunningXiong : MonoBehaviour
{
#region Variables
    public List<Transform> RunningPoints = new List<Transform>();
    public float MoveDuration = 1.0f; // Total duration to move between start and end points in seconds
    public GameObject Xiong;
                                      #endregion

    public void RunCoroutine()
    {
        if (RunningPoints.Count > 0)
        {
            StartCoroutine(MoveXiong());
        }
    }

    IEnumerator MoveXiong()
    {
        int currentPointIndex = 0;
        float totalDistance = 0;

        // Calculate total distance
        for (int i = 0; i < RunningPoints.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(RunningPoints[i].position, RunningPoints[i + 1].position);
        }

        while (currentPointIndex < RunningPoints.Count - 1)
        {
            var startPoint = RunningPoints[currentPointIndex];
            var endPoint = RunningPoints[currentPointIndex + 1];
            float segmentDistance = Vector3.Distance(startPoint.position, endPoint.position);
            float segmentDuration = segmentDistance / totalDistance * MoveDuration;
            var startPosition = Xiong.transform.position;
            float elapsedTime = 0;

            while (elapsedTime < segmentDuration)
            {
                Xiong.transform.position = Vector3.Lerp(startPosition, endPoint.position, elapsedTime / segmentDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Xiong.transform.position = endPoint.position;
            currentPointIndex++;
        }

        Destroy(Xiong);
    }
}

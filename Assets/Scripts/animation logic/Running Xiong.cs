using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;
        
        public class RunningXiong : MonoBehaviour
        {
            #region Variables
            public List<Transform> runningPoints = new List<Transform>();
            public GameObject Xiong;
            public float moveDuration = 1.0f; // Total duration to move between start and end points in seconds
            #endregion
        
            private void Start()
            {
                if (runningPoints.Count > 0)
                {
                    StartCoroutine(MoveXiong());
                }
            }
        
            private IEnumerator MoveXiong()
            {
                int currentPointIndex = 0;
                float totalDistance = 0;
        
                // Calculate total distance
                for (int i = 0; i < runningPoints.Count - 1; i++)
                {
                    totalDistance += Vector3.Distance(runningPoints[i].position, runningPoints[i + 1].position);
                }
        
                while (currentPointIndex < runningPoints.Count - 1)
                {
                    Transform startPoint = runningPoints[currentPointIndex];
                    Transform endPoint = runningPoints[currentPointIndex + 1];
                    float segmentDistance = Vector3.Distance(startPoint.position, endPoint.position);
                    float segmentDuration = (segmentDistance / totalDistance) * moveDuration;
                    Vector3 startPosition = Xiong.transform.position;
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
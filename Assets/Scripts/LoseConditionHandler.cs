using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseConditionHandler : MonoBehaviour
{
    [Header("Timer and Losing")]
    private float timeElapsed = 0;
    [SerializeField] private float timeToStay = 10;

    private void Update()
    {
        if (GameManager.instance.hasLost)
        {
            if (timeElapsed > timeToStay)
            {
                GameManager.instance.RestartGame();
            }
            timeElapsed += Time.deltaTime;
        }
    }
}

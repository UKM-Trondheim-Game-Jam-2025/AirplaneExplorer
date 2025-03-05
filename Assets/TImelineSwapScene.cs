using UnityEngine;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;

public class TimelineSwapScene : MonoBehaviour
{
    [SerializeField] SceneReference nextScene;
    
    public void SwapScene()
    {
        SceneManager.LoadScene(nextScene.Name);
    }
}

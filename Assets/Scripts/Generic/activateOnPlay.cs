using UnityEngine;
using TMPro;

public class activateOnPlay : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().enabled = true;
    }
}
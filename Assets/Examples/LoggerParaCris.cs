using UnityEngine;
using UnityEngine.PlayerLoop;

public class LoggerParaCris : MonoBehaviour
{
    void EarlyUpdate()
    {
        Debug.Log($"Early Update at frame {Time.frameCount}");
    }

    void Update()
    {
        Debug.Log($"Update at frame {Time.frameCount}");
    }

    void LateUpdate()
    {
        Debug.Log($"Late Update at frame {Time.frameCount}");
    }
}
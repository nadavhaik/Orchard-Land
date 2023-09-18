
using UnityEngine;
using UnityEngine.Events;

public class HeightSensor : MonoBehaviour
{
    public float targetHeight;
    public UnityEvent reachedHeightEvent = new();

    void Update()
    {
        if (transform.position.y >= targetHeight)
        {
            reachedHeightEvent.Invoke();
            Destroy(gameObject);
        }
    }
}

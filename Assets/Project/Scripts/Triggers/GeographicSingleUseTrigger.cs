
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class GeographicSingleUseTrigger : MonoBehaviour
{
    public UnityEvent onTrigger = new();

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        onTrigger.Invoke();
        Destroy(gameObject);
    }
}

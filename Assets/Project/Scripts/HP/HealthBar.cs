
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class HealthBar : MonoBehaviour
{
    public Image hbImage;
    public float CurrentHealth { private set; get; }
    public float maxHealth;
    

    public void Reduce(float reduceBy)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - reduceBy); 
        Redraw();
    }

    public void Add(float addBy)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + addBy);
        Redraw();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        CurrentHealth = maxHealth;
    }
    

    protected abstract void Redraw();

    // Update is called once per frame
    protected void UpdateBar()
    {
        hbImage.fillAmount = Mathf.Clamp(CurrentHealth / maxHealth, 0, 1f);
    }
}

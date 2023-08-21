using UnityEngine;

public class EnemyHealthBar : HealthBar
{
    public float hideAfterNotHitFor = 10f;
    private bool _shown;

    void Hide()
    {
        hbImage.fillAmount = 0f;
    }
    

    protected override void Redraw()
    {
        CancelInvoke(nameof(Hide));
        UpdateBar();
        Invoke(nameof(Hide), hideAfterNotHitFor);
    }
    
    protected override void Start()
    {
        base.Start();
        Hide();
    }

}

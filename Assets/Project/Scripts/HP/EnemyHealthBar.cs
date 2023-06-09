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
    

    // Update is called once per frame
    void Update()
    {
        foreach (var camera in Camera.allCameras)
        {
            if (camera.enabled)
            {
                transform.LookAt(camera.transform.position);
                transform.Rotate(Vector3.up, 180);
                break;
            }
        }

        
    }

    protected override void Start()
    {
        base.Start();
        Hide();
    }

}

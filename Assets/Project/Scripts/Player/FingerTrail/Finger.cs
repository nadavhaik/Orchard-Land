using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Finger : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform rectTransform;
    public Canvas canvas;
    public LineRenderer lineRenderer;
    public float decayPerSecond = 0.5f;
    private List<Vector3> _pointsRelToCamera = new();
    private CameraManager _cameraManager;
    private bool _decaying = false;

    private CameraManager CameraManagerPtr
    {
        get
        {
            if (_cameraManager == null)
                _cameraManager = FindObjectOfType<CameraManager>();
            return _cameraManager;
        }
    }
    void Start()
    {
        lineRenderer.transform.parent = canvas.transform;
        if (_cameraManager == null)
        {
            _cameraManager = FindObjectOfType<CameraManager>();    
        }
    }
    
    Vector3 PositionRelativeToCamera
    {
        get => transform.position - CameraManagerPtr.MainCamera.transform.position;
    }


    void RedrawLine()
    {
        if(_pointsRelToCamera.Count < 2) return;
        var pointsForLine = _pointsRelToCamera.ConvertAll(p => p + CameraManagerPtr.MainCamera.transform.position);
        lineRenderer.positionCount = pointsForLine.Count;
        lineRenderer.SetPositions(pointsForLine.ToArray());
    }

    Vector3 ScreenToWorldPoint(Vector2 worldPoint) => 
        CameraManagerPtr.MainCamera.ScreenToWorldPoint(new Vector3(worldPoint.x, worldPoint.y, 2f));
    
    public void MoveTo(Vector2 newPosition)
    {
        transform.position = ScreenToWorldPoint(new Vector3(newPosition.x, newPosition.y, 2f));
        _pointsRelToCamera.Add(PositionRelativeToCamera);
    }
    

    public void Decay()
    {
        if (_pointsRelToCamera.Count > 1)
        {
            // Vector2 firstPoint2d = CameraManagerPtr.MainCamera.WorldToScreenPoint(_pointsRelToCamera.First()) ;
            // Vector2 lastPoint2d = CameraManagerPtr.MainCamera.WorldToScreenPoint(_pointsRelToCamera.Last());
            // float distance = Vector2.Distance(firstPoint2d, lastPoint2d);
            // float angle = Vector2.SignedAngle(firstPoint2d, lastPoint2d);
            // float interpolatedAngle = (Mathf.Round((angle / 360f) * 8) * 45) % 360; // nearest 45° integer multiplier in [-180, 180)
            //
            // float radians = interpolatedAngle * Mathf.Deg2Rad; 
            // Vector2 fixedStart = lastPoint2d - distance * new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
            //
            // Vector3 fixedStart3d =  CameraManagerPtr.MainCamera.ScreenToWorldPoint(new Vector3(fixedStart.x, fixedStart.y, 0f));
            // Vector3 realStart = _pointsRelToCamera.First();

            _pointsRelToCamera = new[] { _pointsRelToCamera.First(), _pointsRelToCamera.Last() }.ToList();
            _decaying = true;

            RedrawLine();
        }
       
    }


    void HandleDecay()
    {
        lineRenderer.startColor =
            new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b,
                lineRenderer.startColor.a - decayPerSecond * Time.deltaTime);
        
        lineRenderer.endColor =
            new Color(lineRenderer.endColor.r, lineRenderer.startColor.g, lineRenderer.endColor.b,
                lineRenderer.endColor.a - decayPerSecond * Time.deltaTime);

        if (lineRenderer.startColor.a <= 0)
        {
            Destroy(lineRenderer.gameObject);
            Destroy(gameObject);
        }
        
    }

    void Update()
    {
        if (_decaying)
        {
            HandleDecay();
        }
        RedrawLine();
    }
}


using UnityEngine;

public struct PositionRotation
{
    public Vector3 Position { get; }
    public Quaternion Rotation { get; }

    public PositionRotation(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    public static PositionRotation FromTransform(Transform transform) =>
        new(transform.position, transform.rotation);
    
    public static PositionRotation LocalFromTransform(Transform transform) =>
        new(transform.localPosition, transform.localRotation);

}

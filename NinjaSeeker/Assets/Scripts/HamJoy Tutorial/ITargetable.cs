using UnityEngine;


public interface ITargetable 
{
    public bool Targetable { get; }
    public Transform TargetTransform { get; }
}


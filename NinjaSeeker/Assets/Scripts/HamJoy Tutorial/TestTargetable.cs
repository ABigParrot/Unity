using UnityEngine;


public class TestTargetable : MonoBehaviour, ITargetable
    {
        [Header("Targetable")] 
        [SerializeField] private bool _targetable = true;
        [SerializeField] private Transform _targetTransform;

        bool ITargetable.Targetable { get => _targetable; }
        Transform ITargetable.TargetTransform { get => _targetTransform; }
    }


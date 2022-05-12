using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_HitResponder : MonoBehaviour, IHitResponder
{
    [SerializeField] private bool m_attack;
    [SerializeField] private int m_damage = 10;
    [SerializeField] private Comp_HitBox _hitBox;
    
    int IHitResponder.Damage {get => m_damage;}
    // Start is called before the first frame update
    void Start()
    {
        _hitBox.HitResponder = this;
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_attack)
            _hitBox.CheckHit();
    }

    bool IHitResponder.CheckHit(HitData data)
    {
        return true;
    }

    void IHitResponder.Response(HitData data)
    {
        
    }
}

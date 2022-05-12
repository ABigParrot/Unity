using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comp_Target : MonoBehaviour, ITargetable, IHurtResponder
{
    [SerializeField] private bool m_targetable = true;
    [SerializeField] private Transform m_targetTransform;
    [SerializeField] private Rigidbody m_rbTarget;

    private List<Comp_HurtBox> m_hurtBoxes = new List<Comp_HurtBox>();

    bool ITargetable.Targetable { get => m_targetable; }
    Transform ITargetable.TargetTransform {get => m_targetTransform;}
    
    // Start is called before the first frame update
    void Start()
    {
        m_hurtBoxes = new List<Comp_HurtBox>(GetComponentsInChildren<Comp_HurtBox>());
        foreach (Comp_HurtBox _hurtBox in m_hurtBoxes)
            _hurtBox.HurtResponder = this;
    }
    bool IHurtResponder.CheckHit(HitData data)
    {
        return true;
    }
    void IHurtResponder.Response(HitData data)
    {
       Debug.Log("Hurt response");
    }
}

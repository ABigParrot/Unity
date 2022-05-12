using UnityEngine;


public class Comp_HurtBox : MonoBehaviour, IHurtBox
{
    [SerializeField] private bool m_active = true;
    [SerializeField] private GameObject m_owner = null;
    [SerializeField] private HurtBoxType m_hurtBoxType = HurtBoxType.Enemy;
    private IHurtResponder m_hurtResponder;

    public bool Active { get => m_active; }
    public GameObject Owner { get => m_owner; }
    public Transform Transform { get => transform; }
    public HurtBoxType Type { get => m_hurtBoxType; }
    public IHurtResponder HurtResponder { get => m_hurtResponder; set => m_hurtResponder = value; }
    public bool CheckHit(HitData data)
    {
        if (m_hurtResponder == null)
            Debug.Log("No Responder");
        return true;
    }
}


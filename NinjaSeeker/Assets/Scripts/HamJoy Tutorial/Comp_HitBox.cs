using UnityEngine;


public class Comp_HitBox : MonoBehaviour, IHitDetector
{
    [SerializeField] private BoxCollider m_collider;
    [SerializeField] private LayerMask m_layerMask;
    [SerializeField] private HurtBoxMask m_hurtBoxMask = HurtBoxMask.Enemy;

    private float m_thickness = 0.025f;
    private IHitResponder m_hitResponder;

    public IHitResponder HitResponder { get => m_hitResponder; set => m_hitResponder = value; }

    public void CheckHit()
    {
        //Capture the colliders size and scale on x, y, and z axies
        Vector3 _scaledSize = new Vector3(
            m_collider.size.x * transform.lossyScale.x,
            m_collider.size.y * transform.lossyScale.y,
            m_collider.size.z * transform.lossyScale.z);

        float _distance = _scaledSize.y - m_thickness;
        Vector3 _direction = transform.up;
        Vector3 _center = transform.TransformPoint(m_collider.center);
        Vector3 _start = _center - _direction * (_distance / 2);
        Vector3 _halfExtents = new Vector3(_scaledSize.x, m_thickness, _scaledSize.z);
        Quaternion _orientation = transform.rotation;

        HitData _hitData = null;
        IHurtBox _hurtBox = null;
        RaycastHit[] _hits = Physics.BoxCastAll(_start, _halfExtents, _direction, _orientation, _distance, m_layerMask);
        foreach (RaycastHit _hit in _hits)
        {
            //If there is a hurtbox and the hurtbox is active, take in the new data.
            _hurtBox = _hit.collider.GetComponent<IHurtBox>();
            if (_hurtBox != null)
                if (_hurtBox.Active)
                    if(m_hurtBoxMask.HasFlag((HurtBoxMask)_hurtBox.Type))
                    {
                        //Genetate hit data
                        /*Damage defaults to the hitresponder being null.
                         If it is null, damage is zero. 
                         If it is not null, take the damage value 
                         from the hit responder.*/
                        /*
                        * The hit point is a vector3 with zero
                        * on all axis values at the center.
                        * If not the center, at the area of the hit
                        */
                        _hitData = new HitData
                        {
                            damage = m_hitResponder == null ? 0 : m_hitResponder.Damage,
                            hitPoint = _hit.point == Vector3.zero ? _center : _hit.point,
                            hitNormal = _hit.normal,
                            hurtBox = _hurtBox,
                            hitDetector = this
                        };

                        if (_hitData.Validate())
                        {
                            _hitData.hitDetector.HitResponder?.Response(_hitData);
                            _hitData.hurtBox.HurtResponder?.Response(_hitData);
                        }
                    }
        }
    }
}


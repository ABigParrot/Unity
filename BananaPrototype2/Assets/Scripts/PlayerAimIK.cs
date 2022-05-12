
using UnityEngine;


public class PlayerAimIK : MonoBehaviour
{
    public Transform targetTransform;
    public Transform aimTransform;
    public Transform bone;
    
    [Range(0.0f, 1.0f)] 
    public float weight = 1.0f;

    public int iterations = 10;


    void LateUpdate()
    {
        Vector3 targetPosition = targetTransform.position;
        for (int i = 0; i < iterations; i++)
        {
            AimAtTarget(bone, targetPosition, weight);
            
        }
    }

    //rotate bone
    private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
    {
        //1st Aim IK
        //Calculate the angle between the aim direction and the target position
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        
        //Calculate a delta rotation
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        
        //Calculate how much influence your weight is going to have
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        
        //Apply aimTowards to the bone rotation
        bone.rotation = blendedRotation * bone.rotation;
        
    }
    
}

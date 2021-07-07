using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum FollowCamsModel
{
    FollowPerfect,
    FollowWithDecal,
    FollowWithDecalMax,
    FollowWithLerp,
    FollowingWithSmoothDamp
};

public class FollowTarget : MonoBehaviour
{

    [SerializeField] private Transform target;

    [Space(10)]
    [SerializeField] FollowCamsModel camsModel;

    [Space(10)]
    [SerializeField] private Vector3 offset;

    [SerializeField] private Vector3 maxDecal;

    [SerializeField] private float speed;
    [SerializeField] private float smoothSpeed;

    [SerializeField] private float smoothTime;

    [Space(10)]
    [SerializeField] private CameraHelicopterInfo runTimeInfo;

    private Vector3 camTargetPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void FixedUpdate()
    {
        //FollowWithLerp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {

        camTargetPosition = target.position + offset;
        runTimeInfo.targetPosition = camTargetPosition;

        switch(camsModel)
        {
            case FollowCamsModel.FollowPerfect:
                FollowPerfect();
                break;
            case FollowCamsModel.FollowWithDecal:
                FollowWithDecal();
                break;
            case FollowCamsModel.FollowWithDecalMax:
                FollowWithDecalMax();
                break;
            case FollowCamsModel.FollowWithLerp:
                FollowWithLerp();
                break;
            case FollowCamsModel.FollowingWithSmoothDamp:
                FollowingWithSmoothDamp();
                break;
        }

        UpdateRunTimeInfo();
    }

    private void FollowPerfect()
    {
        transform.position = camTargetPosition;
        transform.LookAt(target);
    }

    private void FollowWithDecal()
    {
        Vector3 camDirection = camTargetPosition - transform.position;
        Vector3 camSpeed = camDirection.normalized * speed;
        Vector3 camNextPosition = transform.position + camSpeed;

        float dist = camDirection.magnitude;

        if (dist <= speed)
        {
            //Debug.Log($"{dist} <= {speed}: TargetPosition");
            transform.position = camTargetPosition;
        }
        else
        {
            //Debug.Log($"{dist} <= {speed}: NextPosition");
            transform.position = camNextPosition;
        }

        transform.LookAt(target);
    }
    
    private void FollowWithLerp()
    {
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position, 
            camTargetPosition, 
            smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        transform.LookAt(target);
    }

    private void FollowingWithSmoothDamp()
    {
        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            camTargetPosition, 
            ref runTimeInfo.velocity, 
            smoothTime);

        transform.LookAt(target);
    }

    private void FollowWithDecalMax()
    {
        Vector3 camDirection = camTargetPosition - transform.position;
        Vector3 camSpeed = camDirection.normalized * speed;
        Vector3 camNextPosition = transform.position + camSpeed;
        
        runTimeInfo.nextPosition = camNextPosition;

        float distToTargetPosition = camDirection.magnitude;

        camNextPosition = ApplyMaxDecal(camNextPosition, camTargetPosition);

        if (distToTargetPosition <= speed)
        {
            //Debug.Log($"{distToTargetPosition} <= {speed}: TargetPosition: {camTargetPosition}");
            transform.position = camTargetPosition;
        }
        else
        {
            //Debug.Log($"{distToTargetPosition} <= {speed}: NextPosition: {camNextPosition}");
            transform.position = camNextPosition;
        }

        transform.LookAt(target);
    }

    private Vector3 ApplyMaxDecal(Vector3 nextPosition, Vector3 targetPosition)
    {
        Vector3 correctedPosition = nextPosition;

        // x:
        float xDistFromNextToTarget = targetPosition.x - nextPosition.x;
        if (Mathf.Abs(xDistFromNextToTarget) > maxDecal.x)
        {
            correctedPosition.x = targetPosition.x - Mathf.Sign(xDistFromNextToTarget) * maxDecal.x;
        }

        // y:
        float yDistFromNextToTarget = targetPosition.y - nextPosition.y;
        if (Mathf.Abs(yDistFromNextToTarget) > maxDecal.y)
        {
            correctedPosition.y = targetPosition.y - Mathf.Sign(yDistFromNextToTarget) * maxDecal.y;
        }

        // z:
        float zDistFromNextToTarget = targetPosition.z - nextPosition.z;
        if (Mathf.Abs(zDistFromNextToTarget) > maxDecal.z)
        {
            correctedPosition.z = targetPosition.z - Mathf.Sign(zDistFromNextToTarget) * maxDecal.z;
        }

        return correctedPosition;
    }

    private void UpdateRunTimeInfo()
    {
        runTimeInfo.correctedPosition = transform.position;
        runTimeInfo.decalFromTarget = runTimeInfo.targetPosition - runTimeInfo.correctedPosition;
    }
}

[System.Serializable] 
class CameraHelicopterInfo
{
    [ReadOnly] public Vector3 targetPosition;
    [ReadOnly] public Vector3 nextPosition;
    [ReadOnly] public Vector3 correctedPosition;
    [ReadOnly] public Vector3 decalFromTarget;
    [ReadOnly] public Vector3 velocity;
}

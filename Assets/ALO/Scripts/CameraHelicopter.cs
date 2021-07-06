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

public class CameraHelicopter : MonoBehaviour
{

    [SerializeField] private GameObject helicopter;

    [Space(10)]
    [SerializeField] FollowCamsModel camsModel;

    [Space(10)]
    [SerializeField] private Vector3 offset;

    [SerializeField] private float xDecal;
    [SerializeField] private float yDecal;
    [SerializeField] private float zDecal;

    [SerializeField] private float speed;
    [SerializeField] private float smoothSpeed = 0.125f;

    [Space(10)]
    [SerializeField] private CameraHelicopterInfo runTimeInfo;

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
    }

    private void FollowPerfect()
    {
        Vector3 camPosition = helicopter.transform.position + offset;

        transform.position = camPosition;
        transform.LookAt(helicopter.transform);
    }

    private void FollowWithDecal()
    {
        Vector3 camTargetPosition = helicopter.transform.position + offset;

        Vector3 camDirection = camTargetPosition - transform.position;
        Vector3 camSpeed = camDirection.normalized * speed;
        Vector3 camNextPosition = transform.position + camSpeed;

        float dist = camDirection.magnitude;

        if (dist <= speed)
        {
            Debug.Log($"{dist} <= {speed}: TargetPosition");
            transform.position = camTargetPosition;
        }
        else
        {
            Debug.Log($"{dist} <= {speed}: NextPosition");
            transform.position = camNextPosition;
        }

        transform.LookAt(helicopter.transform);
    }
    
    private void FollowWithLerp()
    {
        Vector3 desiredPosition = helicopter.transform.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position, 
            desiredPosition, 
            smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        transform.LookAt(helicopter.transform);
    }

    private Vector3 velocity = Vector3.zero;

    [SerializeField] private float smoothTime;
    private void FollowingWithSmoothDamp()
    {
        

        Vector3 desiredPosition = helicopter.transform.position + offset;

        // Define a target position above and behind the target transform
        //Vector3 targetPosition = target.TransformPoint(new Vector3(0, 5, -10));

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            desiredPosition, 
            ref velocity, 
            smoothTime);

        transform.LookAt(helicopter.transform);
    }

    private void FollowWithDecalMax()
    {
        Vector3 camTargetPosition = helicopter.transform.position + offset;

        Vector3 camDirection = camTargetPosition - transform.position;
        Vector3 camSpeed = camDirection.normalized * speed;
        Vector3 camNextPosition = transform.position + camSpeed;

        float distToTargetPosition = camDirection.magnitude;

        camNextPosition = ApplyMaxDecal(camNextPosition, camTargetPosition);

        if (distToTargetPosition <= speed)
        {
            Debug.Log($"{distToTargetPosition} <= {speed}: TargetPosition: {camTargetPosition}");
            transform.position = camTargetPosition;
        }
        else
        {
            Debug.Log($"{distToTargetPosition} <= {speed}: NextPosition: {camNextPosition}");
            transform.position = camNextPosition;
        }

        transform.LookAt(helicopter.transform);
    }

    private Vector3 ApplyMaxDecal(Vector3 nextPosition, Vector3 targetPosition)
    {
        Vector3 correctedPosition = nextPosition;
        
        // RuntimeInfo Update:
        runTimeInfo.targetPosition = targetPosition;
        runTimeInfo.nextPosition = nextPosition;

        // x:
        float xDistFromNextToTarget = targetPosition.x - nextPosition.x;
        if (Mathf.Abs(xDistFromNextToTarget) > xDecal)
        {
            correctedPosition.x = targetPosition.x - Mathf.Sign(xDistFromNextToTarget) * xDecal;
        }

        // y:
        float yDistFromNextToTarget = targetPosition.y - nextPosition.y;
        if (Mathf.Abs(yDistFromNextToTarget) > yDecal)
        {
            correctedPosition.y = targetPosition.y - Mathf.Sign(yDistFromNextToTarget) * yDecal;
        }

        // z:
        float zDistFromNextToTarget = targetPosition.z - nextPosition.z;
        if (Mathf.Abs(zDistFromNextToTarget) > zDecal)
        {
            correctedPosition.z = targetPosition.z - Mathf.Sign(zDistFromNextToTarget) * zDecal;
        }

        // RuntimeInfo Update:
        runTimeInfo.correctedPosition = correctedPosition;
        runTimeInfo.xdistNextPos = xDistFromNextToTarget;
        runTimeInfo.xdistCorrPos = Mathf.Abs(targetPosition.x - correctedPosition.x);
        runTimeInfo.ydistNextPos = yDistFromNextToTarget;
        runTimeInfo.ydistCorrPos = Mathf.Abs(targetPosition.y - correctedPosition.y);
        runTimeInfo.zdistNextPos = zDistFromNextToTarget;
        runTimeInfo.zdistCorrPos = Mathf.Abs(targetPosition.z - correctedPosition.z);

        return correctedPosition;
    }
}

[System.Serializable] 
class CameraHelicopterInfo
{
    [ReadOnly] public Vector3 targetPosition;
    [ReadOnly] public Vector3 nextPosition;
    [ReadOnly] public Vector3 correctedPosition;
    [ReadOnly] public float xdistNextPos;
    [ReadOnly] public float xdistCorrPos;
    [ReadOnly] public float ydistNextPos;
    [ReadOnly] public float ydistCorrPos;
    [ReadOnly] public float zdistNextPos;
    [ReadOnly] public float zdistCorrPos;
}
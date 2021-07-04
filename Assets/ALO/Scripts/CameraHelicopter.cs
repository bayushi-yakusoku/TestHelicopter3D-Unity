using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHelicopter : MonoBehaviour
{

    [SerializeField] private GameObject helicopter;
    [SerializeField] private float xFar;
    [SerializeField] private float yFar;
    [SerializeField] private float zFar;

    [SerializeField] private float xDecal;
    [SerializeField] private float yDecal;
    [SerializeField] private float zDecal;

    [SerializeField] private float speed;

    [Space(10)]
    [SerializeField] private CameraHelicopterInfo runTimeInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        //FollowPerfect();
        //FollowWithDecal();
        FollowWithDecalMax();
    }

    private void FollowPerfect()
    {
        Vector3 camPosition = helicopter.transform.position;
        camPosition.x += xFar;
        camPosition.y += yFar;
        camPosition.z += zFar;

        transform.position = camPosition;
        transform.LookAt(helicopter.transform);
    }

    private void FollowWithDecal()
    {
        Vector3 camTargetPosition = new Vector3(
            helicopter.transform.position.x + xFar,
            helicopter.transform.position.y + yFar,
            helicopter.transform.position.z + zFar);

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

    private void FollowWithDecalMax()
    {
        Vector3 camTargetPosition = new Vector3(
            helicopter.transform.position.x + xFar,
            helicopter.transform.position.y + yFar,
            helicopter.transform.position.z + zFar);

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
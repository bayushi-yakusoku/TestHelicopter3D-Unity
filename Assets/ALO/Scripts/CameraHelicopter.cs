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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //FollowPerfect();
        FollowWithDecal();
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
}

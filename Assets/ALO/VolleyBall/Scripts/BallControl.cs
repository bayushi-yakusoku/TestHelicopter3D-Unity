using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    private Vector3 respawnLocation;
    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        respawnLocation = transform.position;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {
        transform.position = respawnLocation;
        transform.rotation = Quaternion.identity;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }
}

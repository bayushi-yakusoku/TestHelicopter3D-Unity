using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    private Vector3 respawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        respawnLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {
        transform.position = respawnLocation;
        transform.rotation = Quaternion.identity;
    }
}

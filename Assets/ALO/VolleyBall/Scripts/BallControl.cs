using UnityEngine;

public class BallControl : MonoBehaviour {
    private Vector3 respawnLocation;
    private Rigidbody body;

    private void Awake() {
        respawnLocation = transform.position;
        body = GetComponent<Rigidbody>();
    }

    public void Respawn() {
        transform.SetPositionAndRotation(respawnLocation, Quaternion.identity);

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }
}

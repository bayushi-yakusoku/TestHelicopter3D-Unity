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

    // On collision with the ground play bounce sound:
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            Debug.Log(this + ": Bounce on the ground");

            SfxController.Singleton.PlayBounceFloorSfx(transform.position);
        }
    }
}

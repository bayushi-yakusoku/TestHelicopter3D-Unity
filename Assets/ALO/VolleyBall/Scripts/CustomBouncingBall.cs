using UnityEngine;

public class CustomBouncingBall : MonoBehaviour {
    float gravity = 9.8f; // Gravity force
    float bounceForce = 0.8f; // Bounce force multiplier
    float initialVerticalVelocity = 10f; // Initial upward velocity
    float initialHorizontalVelocity = 1f; // Initial horizontal velocity
    
    [SerializeField] Transform groundPlane; // Reference to the ground plane

    private Vector3 velocity;
    private bool isGrounded = false;

    void Start() {
        // Apply initial velocities
        velocity = new Vector3(initialHorizontalVelocity, initialVerticalVelocity, 0);
    }

    void Update() {
        // Apply gravity
        velocity.y -= gravity * Time.deltaTime;

        // Move the ball
        transform.Translate(velocity * Time.deltaTime);

        // Check for collision with the ground plane
        if (IsGrounded() && !isGrounded) {
            isGrounded = true;
            Bounce();
        }
        else if (!IsGrounded()) {
            isGrounded = false;
        }
    }

    bool IsGrounded() {
        // Check if the ball is below or at the ground plane
        return Vector3.Dot(groundPlane.up, (transform.position - groundPlane.position)) <= 0.5f;
    }

    void Bounce() {
        // Calculate the normal of the ground plane
        Vector3 planeNormal = groundPlane.up;

        // Reflect the velocity off the plane normal
        velocity = Vector3.Reflect(velocity, planeNormal) * bounceForce;

        // Optionally, reduce horizontal velocity on bounce to simulate friction
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
        velocity = horizontalVelocity * 0.9f + Vector3.up * velocity.y;
    }

    void OnDrawGizmos() {
        // Draw a line to visualize the ground plane
        if (groundPlane != null) {
            Gizmos.color = Color.red;
            Vector3 planeSize = new Vector3(10, 0, 10);
            Vector3 planeCenter = groundPlane.position;
            Gizmos.DrawWireCube(planeCenter, planeSize);
        }
    }
}

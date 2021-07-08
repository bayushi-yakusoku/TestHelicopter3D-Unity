using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private float hitForce;
    [SerializeField] private GameObject arrow;

    private Rigidbody ballRigidBody;
    private BallControl ballControl;

    private VolleyBall inputActions;

    private GameObject clone;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        inputActions = new VolleyBall();

        ballRigidBody = ball.GetComponent<Rigidbody>();
        ballControl = ball.GetComponent<BallControl>();

        inputActions.TestWithBall.Respawn.performed += InputRespawnPerformed;

        inputActions.TestWithBall.Hit.performed += InputHitPerformed;

        inputActions.TestWithBall.Direction.performed += InputDirectionPerformed;
        inputActions.TestWithBall.Direction.canceled += InputDirectionCanceled;
    }

    private void InputDirectionCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Direction: Canceled");
        Destroy(clone);
        direction = Vector3.zero;
    }

    private Vector3 direction;

    private void InputDirectionPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        direction = obj.ReadValue<Vector2>();
        direction.x *= -1;

        Debug.Log($"Direction: {direction}");

        if (clone)
        {
            //clone.transform.Rotate(direction);

            Vector3 target = transform.position + (Vector3) (direction);
            clone.transform.LookAt(target);
            Debug.Log($"Target: {target}");
        }
        else
        {
            clone = Instantiate(arrow, transform);
        }
    }

    private void InputHitPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Hit!");
        ballRigidBody.AddForce(direction * hitForce);
    }

    private void InputRespawnPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Respawn");
        ballControl.Respawn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }


}

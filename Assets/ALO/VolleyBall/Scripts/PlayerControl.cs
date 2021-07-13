using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private float hitForce;
    [SerializeField] private GameObject arrow;

    [SerializeField] private Transform target;
    [SerializeField] private float h;

    [SerializeField] private Vector3 myGravity;

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

        InitInputCallBack();

        Physics.gravity = Vector3.zero;
    }

    private void InitInputCallBack()
    {
        inputActions.TestWithBall.Respawn.performed += InputRespawnPerformed;

        inputActions.TestWithBall.Hit.performed += InputHitPerformed;

        inputActions.TestWithBall.Direction.performed += InputDirectionPerformed;
        inputActions.TestWithBall.Direction.canceled += InputDirectionCanceled;

        inputActions.TestWithBall.Shoot.performed += InputShootPerformed;
    }

    private void InputShootPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Shoot");
        Physics.gravity = myGravity;

        ballRigidBody.velocity = GetShootVelocity();
    }

    private Vector3 GetShootVelocity()
    {
        float Py = target.position.y - ball.transform.position.y;
        float Sx = target.position.x - ball.transform.position.x;
        float Th = target.position.y + h;
        float g = myGravity.y;

        float Uy = Mathf.Sqrt(-2 * g * Th);

        float Ux = Sx / ( Mathf.Sqrt(-2 * Th / g) + Mathf.Sqrt(2 * (Py - Th) / g) );

        Vector3 velocity = new Vector3(Ux, Uy, 0);

        Debug.Log($"Velocity: {velocity}");

        return velocity;

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
        Physics.gravity = myGravity;

        ballRigidBody.AddForce(direction * hitForce);
    }

    private void InputRespawnPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Respawn");

        Physics.gravity = Vector3.zero;
        ballControl.Respawn();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(ball.transform.position, target.position);
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

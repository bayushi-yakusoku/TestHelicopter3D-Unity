using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] GameObject ball;
    [SerializeField] float hitForce;
    [SerializeField] GameObject arrow;

    [SerializeField] Transform target;
    [SerializeField] float h;

    [SerializeField] float g;

    [SerializeField] bool drawTrajectory;
    [SerializeField] int nbDots;

    Vector3 myGravity;

    Rigidbody ballRigidBody;
    BallControl ballControl;

    VolleyBall inputActions;

    GameObject clone;

    Vector3 refTarget;
    Vector3 refBall;

    Trajectory refTrajectory = new Trajectory(Vector3.zero, Vector3.zero);


    // Start is called before the first frame update
    void Start()
    {
        refTarget = target.position;
        refBall = ballRigidBody.transform.position;

        refTrajectory = new Trajectory(
            ballRigidBody.position,
            target.position,
            myGravity,
            h);
    }

    void Awake()
    {
        inputActions = new VolleyBall();

        ballRigidBody = ball.GetComponent<Rigidbody>();
        ballControl = ball.GetComponent<BallControl>();

        InitInputCallBack();

        myGravity = Vector3.up * g;

        Physics.gravity = myGravity;
        ballRigidBody.useGravity = false;
    }

    void InitInputCallBack()
    {
        inputActions.TestWithBall.Respawn.performed += InputRespawnPerformed;

        inputActions.TestWithBall.Hit.performed += InputHitPerformed;

        inputActions.TestWithBall.Direction.performed += InputDirectionPerformed;
        inputActions.TestWithBall.Direction.canceled += InputDirectionCanceled;

        inputActions.TestWithBall.Shoot.performed += InputShootPerformed;
    }

    void InputShootPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Shoot");
        ballRigidBody.useGravity = true;

        ballRigidBody.velocity = refTrajectory.Velocity;
    }

    void InputDirectionCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Direction: Canceled");
        Destroy(clone);
        direction = Vector3.zero;
    }

    Vector3 direction;

    void InputDirectionPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
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

    void InputHitPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Hit!");
        ballRigidBody.useGravity = true;

        ballRigidBody.AddForce(direction * hitForce);
    }

    void InputRespawnPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Respawn");

        ballRigidBody.useGravity = false;
        ballControl.Respawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (refTarget != target.position)
        {
            Debug.Log($"Target moved!");
            refTarget = target.position;
            refTrajectory.Target = refTarget;
        }

        if (drawTrajectory)
            refTrajectory.DrawTrajectory();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnValidate()
    {
        Debug.Log($"Validate event!");
        refTrajectory.Hight = h;
        refTrajectory.Gravity = Vector3.up * g;
        Physics.gravity = refTrajectory.Gravity;
        refTrajectory.NbDots = nbDots;
    }
}

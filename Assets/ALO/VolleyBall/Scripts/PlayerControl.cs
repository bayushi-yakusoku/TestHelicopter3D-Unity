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
    Vector3 myGravity;

    Rigidbody ballRigidBody;
    BallControl ballControl;

    VolleyBall inputActions;

    GameObject clone;

    Vector3 refTarget;
    Vector3 refBall;

    TrajectoryData shootToTargetData;


    // Start is called before the first frame update
    void Start()
    {
        refTarget = target.position;
        refBall = ballRigidBody.transform.position;

        shootToTargetData = GetShootVelocity();
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

        ballRigidBody.velocity = GetShootVelocity().velocity;
    }

    TrajectoryData GetShootVelocity()
    {
        float Py = target.position.y - refBall.y;
        float Sx = target.position.x - refBall.x;

        Vector3 planXZ = new Vector3(
            target.position.x - refBall.x,
            0,
            target.position.z - refBall.z);

        float Th = target.position.y + h;
        float g = myGravity.y;

        float Uy = Mathf.Sqrt(-2 * g * Th);

        float timeToTarget = Mathf.Sqrt(-2 * Th / g) + Mathf.Sqrt(2 * (Py - Th) / g);

        float Ux = Sx / timeToTarget;
        planXZ = planXZ / timeToTarget;

        //Vector3 velocity = new Vector3(Ux, Uy, 0);
        Vector3 velocity = new Vector3(planXZ.x, Uy, planXZ.z);

        Debug.Log($"Velocity: {velocity} Time: {timeToTarget}");

        return new TrajectoryData(velocity, timeToTarget);

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
        Debug.DrawLine(ball.transform.position, target.position);

        if (refTarget != target.position)
        {
            Debug.Log($"Target moved!");
            refTarget = target.position;
            shootToTargetData = GetShootVelocity();
        }

        DrawTrajectory();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    [SerializeField] int nbDots;

    void DrawTrajectory()
    {
        float delta = shootToTargetData.time / (float) nbDots;

        Vector3 previousDot = ballRigidBody.transform.position;

        for (int i = 1; i <= nbDots; i++)
        {
            float t = i * delta;

            Vector3 nextDot = refBall + (shootToTargetData.velocity * t +
                (myGravity * t * t) / 2);

            Debug.DrawLine(previousDot, nextDot);

            previousDot = nextDot;
        }
    }
}

public readonly struct TrajectoryData
{
    public TrajectoryData(Vector3 _velocity, float _time)
    {
        velocity = _velocity;
        time = _time;
    }

    public Vector3 velocity { get; }
    public float time { get; }

    public override string ToString() => $"Velocity: {velocity} Time: {time})";
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    enum Target
    {
        Libero,
        Setter,
        Hitter,
        Enemy
    }

    Target currentTarget = Target.Libero;

    [SerializeField] GameObject ball;
    [SerializeField] float hitForce;
    [SerializeField] GameObject arrow;

    [SerializeField] Transform targetLibero;
    [SerializeField] Transform targetSetter;
    [SerializeField] Transform targetHitter;
    [SerializeField] Transform targetEnemy;
    [SerializeField] float h;

    [SerializeField] float g;

    [SerializeField] bool drawTrajectory;
    [SerializeField] int nbDots;

    Vector3 myGravity;

    Rigidbody ballRigidBody;
    BallControl ballControl;

    VolleyBall inputActions;

    GameObject hitDirectionArrow;
    Vector3 direction;

    Vector3 refTarget;

    Trajectory trajToTarget = new Trajectory(Vector3.zero, Vector3.zero);

    PhysicsScene realPhysicScene;

    PrediktPhysic prediktPhysic;

    List<Vector3> predikTraj = new List<Vector3>();

    bool hitPressed = false;



    // Start is called before the first frame update
    void Start()
    {
        refTarget = targetLibero.position;

        currentTarget = Target.Libero;

        trajToTarget = new Trajectory(
            ballRigidBody.position,
            targetLibero.position,
            myGravity,
            h);

        //PreparePhysicScene();
        prediktPhysic = new PrediktPhysic(SceneManager.GetActiveScene());
        realPhysicScene = SceneManager.GetActiveScene().GetPhysicsScene();
        prediktPhysic.AddMobile(ball, ball.transform.position);
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

        Physics.autoSimulation = false;
    }

    void InitInputCallBack()
    {
        inputActions.TestWithBall.Respawn.performed += InputRespawnPerformed;

        inputActions.TestWithBall.Hit.performed += InputHitPerformed;
        inputActions.TestWithBall.Hit.canceled += InputHitCanceled;

        inputActions.TestWithBall.Direction.performed += InputDirectionPerformed;
        inputActions.TestWithBall.Direction.canceled += InputDirectionCanceled;

        inputActions.TestWithBall.Shoot.performed += InputShootPerformed;
    }


    List<List<Vector3>> listTrajectories = new List<List<Vector3>>();

    void InputShootPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Shoot");
        ballRigidBody.useGravity = true;

        ballRigidBody.velocity = trajToTarget.Velocity;

        listTrajectories.Add(trajToTarget.ListDots);

        // Switch Target:
        switch(currentTarget)
        {
            case Target.Libero:
                currentTarget = Target.Setter;
                trajToTarget.Target = targetSetter.position;
                break;

            case Target.Setter:
                currentTarget = Target.Hitter;
                trajToTarget.Target = targetHitter.position;
                break;

            case Target.Hitter:
                currentTarget = Target.Enemy;
                trajToTarget.Target = targetEnemy.position;
                break;

            default:
                Debug.LogError("Target for calculate trajectory is not set");
                break;

        }
    }

    void InputDirectionCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Direction: Canceled");
        Destroy(hitDirectionArrow);
        direction = Vector3.zero;
    }

    void InputDirectionPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        direction = obj.ReadValue<Vector2>();
        direction.x *= -1;

        Debug.Log($"Direction: {direction}");

        if (hitDirectionArrow)
        {
            Vector3 target = transform.position + (Vector3) (direction);
            hitDirectionArrow.transform.LookAt(target);
            Debug.Log($"Target: {target}");
        }
        else
        {
            hitDirectionArrow = Instantiate(arrow, transform);
        }

        if (hitPressed)
        {
            predikTraj = prediktPhysic.Predikt(direction * hitForce, nbDots);
        }
    }

    private void InputHitCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        hitPressed = false;
        Debug.Log("Hit Canceled!");

        ballRigidBody.useGravity = true;
        ballRigidBody.AddForce(direction * hitForce, ForceMode.Impulse);
    }

    void InputHitPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        hitPressed = true;

        Debug.Log("Hit performed!");
    }

    void InputRespawnPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Respawn");

        ballRigidBody.useGravity = false;
        ballControl.Respawn();

        listTrajectories.Clear();

        currentTarget = Target.Libero;
        trajToTarget.Target = targetLibero.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (refTarget != targetLibero.position)
        {
            Debug.Log($"Target moved!");
            refTarget = targetLibero.position;
            trajToTarget.Target = refTarget;
        }

        if (drawTrajectory)
            DrawTrajectories();

        if (predikTraj.Count > 0)
        {
            Vector3 previous = predikTraj[0];

            foreach (Vector3 pos in predikTraj)
            {
                Debug.DrawLine(previous, pos);
                previous = pos;
            }
        }

    }

    void DrawTrajectories()
    {
        trajToTarget.DrawTrajectory();

        foreach (List<Vector3> trajectory in listTrajectories)
        {
            DrawDots(trajectory);
        }
    }

    private void DrawDots(List<Vector3> listDots)
    {
        if (listDots.Count == 0)
            return;

        Vector3 previousDot = listDots[0];

        foreach (Vector3 nextDot in listDots)
        {
            Debug.DrawLine(previousDot, nextDot);
            previousDot = nextDot;
        }
    }

    void FixedUpdate()
    {
        if (realPhysicScene.IsValid())
        {
            realPhysicScene.Simulate(Time.fixedDeltaTime);

            trajToTarget.Origin = ballRigidBody.position;
        }
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
        trajToTarget.Hight = h;
        trajToTarget.Gravity = Vector3.up * g;
        Physics.gravity = trajToTarget.Gravity;
        trajToTarget.NbDots = nbDots;
    }
}

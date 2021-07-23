using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    Scene currentScene;
    Scene predictionScene;

    PhysicsScene currentPhysicsScene;
    PhysicsScene predictionPhysicsScene;

    Rigidbody dummyBall;
    LineRenderer lineRenderer;

    PrediktPhysic prediktPhysic;

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

        //PreparePhysicScene();
        prediktPhysic = new PrediktPhysic(currentScene);
        prediktPhysic.AddMobile(ball, ball.transform.position);

        //predikTraj = prediktPhysic.Predikt(new Vector3(100, 100, 0));
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

        currentScene = SceneManager.GetActiveScene();
        currentPhysicsScene = currentScene.GetPhysicsScene();

        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        predictionScene = SceneManager.CreateScene("Prediction", parameters);
        predictionPhysicsScene = predictionScene.GetPhysicsScene();
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

    List<Vector3> predikTraj = new List<Vector3>();

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

        predikTraj = prediktPhysic.Predikt(new Vector3(10, -10, 0), 50);
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

    void FixedUpdate()
    {
        if (currentPhysicsScene.IsValid())
        {
            currentPhysicsScene.Simulate(Time.fixedDeltaTime);
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
        refTrajectory.Hight = h;
        refTrajectory.Gravity = Vector3.up * g;
        Physics.gravity = refTrajectory.Gravity;
        refTrajectory.NbDots = nbDots;
    }

    GameObject[] rootObjects;
    GameObject[] staticObjects;
    void PreparePhysicScene()
    {
        rootObjects = currentScene.GetRootGameObjects();

        foreach (GameObject item in rootObjects)
        {
            if (item.isStatic)
            {
                Debug.Log($"Name: {item.name} is static");
                
                if (item.GetComponentInChildren<Collider>())
                {
                    Debug.Log($"Name: {item.name} has a collider");

                    GameObject tmp = Instantiate(item);

                    foreach (Renderer rend in tmp.GetComponentsInChildren<Renderer>())
                    {
                        rend.enabled = false;
                    }
                }
            }
        }
    }
}

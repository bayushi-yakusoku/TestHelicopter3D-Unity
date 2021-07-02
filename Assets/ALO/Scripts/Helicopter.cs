using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Helicopter : MonoBehaviour
{
    [SerializeField] private float collectiveModifier;
    [SerializeField] private float cyclicModifier;
    [SerializeField] private float paddleModifier;
    [SerializeField] private float backTurnSpeed;
    [SerializeField] private RotorSpeed rotorSpeedModifier;

    [Space(10)]
    [SerializeField] private HelicopterInfo runTimeInfo;

    private float collective;
    private Vector3 cyclic;
    private float paddle;

    private HelicopterActions inputActions;
    private Rigidbody rigidBody;
    private Animator animator;

    private bool inputAccepted = true;
    private bool backTurnInProgress = false;
    private float backTurnAngle = 180;
    private int countdownFrameBackTurn;

    private bool engineOn = false;
    private float rotorSpeed;
    private float rotorSpeedIncrement;

    private Vector3 respawnPoint;
    private Quaternion respawnRotation;

    private void Awake()
    {
        inputActions = new HelicopterActions();
        rigidBody = GetComponent<Rigidbody>();

        animator = GetComponentInChildren<Animator>();

        respawnPoint = transform.position;
        respawnRotation = transform.rotation;

        //playerInput = GetComponent<PlayerInput>();

        inputActions.Default.Collective.performed += InputCollectivePerformed;
        inputActions.Default.Collective.canceled += InputCollectiveCanceled;

        inputActions.Default.Cyclic.performed += InputCyclicPerformed;

        inputActions.Default.Paddle.performed += InputPaddlePerformed;
        inputActions.Default.Paddle.canceled += InputPaddleCanceled;

        inputActions.Default.BackTurn.performed += InputBackTurnPerformed;

        inputActions.Default.EnginePowerSwitch.performed += InputEnginePowerSwitchPerformed;

        inputActions.Default.RotorSpeed.performed += InputRotorSpeedPerformed;
        inputActions.Default.RotorSpeed.canceled += InputRotorSpeedCanceled;

        inputActions.Default.Respawn.performed += InputRespawnPerformed;
    }

    private void InputRespawnPerformed(InputAction.CallbackContext obj)
    {
        Respawn();
    }

    private void InputRotorSpeedCanceled(InputAction.CallbackContext obj)
    {
        rotorSpeedIncrement = 0;
    }

    private void InputRotorSpeedPerformed(InputAction.CallbackContext obj)
    {
        float input = obj.ReadValue<float>();

        rotorSpeedIncrement = rotorSpeedModifier.multiplicator * input;
    }

    private void InputEnginePowerSwitchPerformed(InputAction.CallbackContext obj)
    {
        engineOn = (engineOn == false);

        animator.SetBool("EngineOn", engineOn);
    }

    private void InputBackTurnPerformed(InputAction.CallbackContext obj)
    {
        if (!inputAccepted) return;

        aimForBackTurn = (transform.localRotation.eulerAngles.y + backTurnAngle) % 360;
        backTurnInProgress = true;
        countdownFrameBackTurn = (int) (backTurnAngle / backTurnSpeed) + 2;
        inputAccepted = false;
    }

    private void InputPaddleCanceled(InputAction.CallbackContext obj)
    {
        paddle = 0f;
    }

    private void InputPaddlePerformed(InputAction.CallbackContext obj)
    {
        if (!inputAccepted) return;

        paddle = paddleModifier * obj.ReadValue<float>();
    }

    private void InputCyclicPerformed(InputAction.CallbackContext obj)
    {
        if (!inputAccepted) return;

        Vector2 wasd = cyclicModifier * obj.ReadValue<Vector2>();

        cyclic = new Vector3(wasd.y, 0, - wasd.x);
    }

    private void InputCollectiveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        collective = 0f;
    }

    private void InputCollectivePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (! inputAccepted ||
            ! engineOn) return;

        float force = obj.ReadValue<float>();

        collective = force * collectiveModifier;
    }

    // Start is called before the first frame update
    void Start()
    {
        rotorSpeed = animator.GetFloat("rotorSpeed");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInfo();
    }

    private void FixedUpdate()
    {
        rotorSpeed += rotorSpeedIncrement;
        if (rotorSpeed < rotorSpeedModifier.min)
            rotorSpeed = rotorSpeedModifier.min;
        else if (rotorSpeed > rotorSpeedModifier.max)
            rotorSpeed = rotorSpeedModifier.max;

        animator.SetFloat("rotorSpeed", rotorSpeed);
        
        BackTurn();

        rigidBody.AddRelativeForce(new Vector3(0, collective, 0));

        Vector3 direction = new Vector3(cyclic.x, paddle, cyclic.z);

        transform.Rotate(direction);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }


    private float aimForBackTurn;

    private void BackTurn()
    {
        if (!backTurnInProgress) return;        

        float currentAngle = transform.eulerAngles.y;
        float deltaAngle = aimForBackTurn - currentAngle;

        if (countdownFrameBackTurn < 0 || 
            Mathf.Approximately(currentAngle, aimForBackTurn))
        {
            Debug.Log($"Backturn Halted: CountDown: {countdownFrameBackTurn}");
            backTurnInProgress = false;
            inputAccepted = true;
            paddle = 0;

            return;
        }

        countdownFrameBackTurn--;

        if (Mathf.Abs(deltaAngle) < backTurnSpeed)
        {
            paddle = deltaAngle;
            Debug.Log($"Delta: {deltaAngle}");
        }
        else
            paddle = backTurnSpeed;

    }

    public void Respawn()
    {
        Debug.Log("Respawn Helicopter!");
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

        transform.position = respawnPoint;
        transform.rotation = respawnRotation;
    }

    private void UpdateInfo()
    {
        runTimeInfo.cyclic = cyclic;
        runTimeInfo.collective = collective;
        runTimeInfo.paddle = paddle;
        runTimeInfo.backTurnInProgress = backTurnInProgress;
        runTimeInfo.aimForBackTurn = aimForBackTurn;
        runTimeInfo.inputAccepted = inputAccepted;
        runTimeInfo.engineOn = engineOn;
        runTimeInfo.rotorSpeed = rotorSpeed;
    }
}

[System.Serializable]
class HelicopterInfo
{
    [ReadOnly] public float collective;
    [ReadOnly] public Vector3 cyclic;
    [ReadOnly] public float paddle;

    [ReadOnly] public bool engineOn;
    [ReadOnly] public float rotorSpeed;

    [ReadOnly] public bool backTurnInProgress;
    [ReadOnly] public float aimForBackTurn;
    [ReadOnly] public bool inputAccepted;
}

[System.Serializable] 
class RotorSpeed
{
    public float multiplicator;
    public int min;
    public int max;
}

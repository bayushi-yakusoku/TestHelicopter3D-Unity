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

    private float collective;
    private Vector3 cyclic;
    private float paddle;

    private HelicopterActions inputActions;
    private Rigidbody rigidBody;

    private bool inputAccepted = true;
    private bool backTurnInProgress = false;

    private void Awake()
    {
        inputActions = new HelicopterActions();
        rigidBody = GetComponent<Rigidbody>();

        //playerInput = GetComponent<PlayerInput>();

        inputActions.Default.Collective.performed += CollectivePerformed;
        inputActions.Default.Collective.canceled += CollectiveCanceled;

        inputActions.Default.Cyclic.performed += CyclicPerformed;

        inputActions.Default.Paddle.performed += PaddlePerformed;
        inputActions.Default.Paddle.canceled += PaddleCanceled;

        inputActions.Default.BackTurn.performed += BackTurnPerformed;
    }

    private void BackTurnPerformed(InputAction.CallbackContext obj)
    {
        if (!inputAccepted) return;

        aimForBackTurn = (transform.localRotation.eulerAngles.y + 180) % 360;
        backTurnInProgress = true;
        inputAccepted = false;

        Debug.Log($"Bacturn Target: {aimForBackTurn}");
    }

    private void PaddleCanceled(InputAction.CallbackContext obj)
    {
        paddle = 0f;
    }

    private void PaddlePerformed(InputAction.CallbackContext obj)
    {
        if (!inputAccepted) return;

        paddle = paddleModifier * obj.ReadValue<float>();

        Debug.Log($"Paddle: {paddle}");
    }

    private void CyclicPerformed(InputAction.CallbackContext obj)
    {
        if (!inputAccepted) return;

        Vector2 wasd = cyclicModifier * obj.ReadValue<Vector2>();

        cyclic = new Vector3(wasd.y, 0, - wasd.x);

        Debug.Log($"Direction: {cyclic}");
    }

    private void CollectiveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        collective = 0f;
    }

    private void CollectivePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (! inputAccepted) return;

        float force = obj.ReadValue<float>();

        collective = force * collectiveModifier;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
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
        if (! backTurnInProgress) return;

        float currentAngle = transform.eulerAngles.y;
        float deltaAngle = aimForBackTurn - currentAngle;

        if (Mathf.Approximately(currentAngle, aimForBackTurn))
        {
            backTurnInProgress = false;
            inputAccepted = true;
            paddle = 0;

            return;
        }

        if (deltaAngle < backTurnSpeed)
        {
            paddle = deltaAngle;
            Debug.Log($"Delta: {deltaAngle}");
        }
        else
            paddle = backTurnSpeed;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Helicopter : MonoBehaviour
{
    [SerializeField] private float collective;
    [SerializeField] private float cyclic;

    private float collectiveForce;
    private Vector3 direction;

    private HelicopterActions inputActions;
    private Rigidbody rigidBody;

    private void Awake()
    {
        inputActions = new HelicopterActions();
        rigidBody = GetComponent<Rigidbody>();

        //playerInput = GetComponent<PlayerInput>();

        inputActions.Default.Collective.performed += CollectivePerformed;
        inputActions.Default.Collective.canceled += CollectiveCanceled;

        inputActions.Default.Cyclic.performed += CyclicPerformed;
    }

    private void CyclicPerformed(InputAction.CallbackContext obj)
    {
        Vector2 wasd = obj.ReadValue<Vector2>();

        direction = new Vector3(wasd.y, 0, - wasd.x);

        Debug.Log($"Direction: {direction}");
    }

    private void CollectiveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        collectiveForce = 0f;
    }

    private void CollectivePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        float force = obj.ReadValue<float>();

        collectiveForce = force * collective;
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
        rigidBody.AddRelativeForce(new Vector3(0, collectiveForce, 0));

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
}

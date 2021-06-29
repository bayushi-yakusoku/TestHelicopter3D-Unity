// GENERATED AUTOMATICALLY FROM 'Assets/ALO/HelicopterActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @HelicopterActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @HelicopterActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""HelicopterActions"",
    ""maps"": [
        {
            ""name"": ""Default"",
            ""id"": ""fdac9370-b74a-4da1-9ffa-c287b415d236"",
            ""actions"": [
                {
                    ""name"": ""Collective"",
                    ""type"": ""Value"",
                    ""id"": ""cc30e10c-e6ba-45ac-83f9-7ac5f214af89"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cyclic"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9ef3effd-9fb2-4fbd-8f3e-2e506f532ba3"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b217546e-51d1-4450-bb86-686bd58dbf5e"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Collective"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""624753e7-f652-4498-94e1-e1f346fd87cc"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cyclic"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1b41b86f-eca0-4d27-bf9d-ddd3d25140d4"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cyclic"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""20ff961c-f482-4f26-89ee-25b6ab4358af"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cyclic"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e5b7a5ea-db19-4018-9906-7b0081c1de0d"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cyclic"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""95747951-6514-4330-a7a8-a8ceb5d39858"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cyclic"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Default
        m_Default = asset.FindActionMap("Default", throwIfNotFound: true);
        m_Default_Collective = m_Default.FindAction("Collective", throwIfNotFound: true);
        m_Default_Cyclic = m_Default.FindAction("Cyclic", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Default
    private readonly InputActionMap m_Default;
    private IDefaultActions m_DefaultActionsCallbackInterface;
    private readonly InputAction m_Default_Collective;
    private readonly InputAction m_Default_Cyclic;
    public struct DefaultActions
    {
        private @HelicopterActions m_Wrapper;
        public DefaultActions(@HelicopterActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Collective => m_Wrapper.m_Default_Collective;
        public InputAction @Cyclic => m_Wrapper.m_Default_Cyclic;
        public InputActionMap Get() { return m_Wrapper.m_Default; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
        public void SetCallbacks(IDefaultActions instance)
        {
            if (m_Wrapper.m_DefaultActionsCallbackInterface != null)
            {
                @Collective.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCollective;
                @Collective.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCollective;
                @Collective.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCollective;
                @Cyclic.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCyclic;
                @Cyclic.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCyclic;
                @Cyclic.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnCyclic;
            }
            m_Wrapper.m_DefaultActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Collective.started += instance.OnCollective;
                @Collective.performed += instance.OnCollective;
                @Collective.canceled += instance.OnCollective;
                @Cyclic.started += instance.OnCyclic;
                @Cyclic.performed += instance.OnCyclic;
                @Cyclic.canceled += instance.OnCyclic;
            }
        }
    }
    public DefaultActions @Default => new DefaultActions(this);
    public interface IDefaultActions
    {
        void OnCollective(InputAction.CallbackContext context);
        void OnCyclic(InputAction.CallbackContext context);
    }
}

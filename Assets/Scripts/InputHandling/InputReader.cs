using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace InputHandling
{
    using static InputSystem_Actions;

    public interface IInputReader
    {
        Vector2 MoveInput { get; }
        void EnablePlayerActions();
        void DisablePlayerActions();
        bool IsPaused { get; }
        void RequestUIMode();
        void RequestPlayerMode();
        event Action<bool> PauseEvent;
        void AssignPlayerInput(PlayerInput playerInput);
    }

    [CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
    public class InputReader : ScriptableObject, IPlayerActions, IUIActions, IInputReader
    {
        public Vector2 MoveInput => m_InputActions.Player.Move.ReadValue<Vector2>();
        public bool IsPaused { get; private set; }
        
        #region Player Action Events
        public event Action<Vector2> MoveEvent = delegate { };
        public event Action<bool> JumpEvent = delegate { };
        public event Action<bool> SprintEvent = delegate { }; 
        public event Action<bool> CrouchEvent = delegate { }; 
        public event Action<bool> ResetEvent = delegate { };
        public event Action<bool> PauseEvent = delegate { };

        // Added event for input mode changes (true = UI mode, false = Player mode)
        #endregion
        #region UI Events
        public event Action<Vector2> NavigationEvent = delegate { };
        public event Action<bool> GoBackEvent = delegate { };
        #endregion
        
        InputSystem_Actions m_InputActions;
        PlayerInput m_AssignedPlayerInput;
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Assigns a PlayerInput component to this InputReader. This allows input routing for specific player instances.
        /// </summary>
        /// <param name="playerInput">The PlayerInput component to associate with this InputReader</param>
        public void AssignPlayerInput(PlayerInput playerInput)
        {
            m_AssignedPlayerInput = playerInput;
            
            // If we already had input actions, unsubscribe first
            if (m_InputActions is not null)
            {
                m_InputActions.Player.Disable();
                m_InputActions.UI.Disable();
            }
            
            // Create new input actions for this specific player
            m_InputActions = new InputSystem_Actions();
            m_InputActions.Player.SetCallbacks(this);
            m_InputActions.UI.SetCallbacks(this);
            
            // Set the devices to match the PlayerInput component
            if (playerInput is not null && playerInput.devices.Count > 0)
            {
                m_InputActions.devices = playerInput.devices;
            }
        }
        
        public void EnablePlayerActions()
        {
            if (m_InputActions is null)
            {
                m_InputActions = new InputSystem_Actions();
                m_InputActions.Player.SetCallbacks(this);
                m_InputActions.UI.SetCallbacks(this);
            }

            m_InputActions.Player.Enable();
        }

        public void DisablePlayerActions()
        {
            if (m_InputActions is null)
                return;
            m_InputActions.Player.Disable();
            m_InputActions.UI.Disable();
            m_InputActions = null;
        }

        // Public methods for external components to request input mode changes
        public void RequestUIMode()
        {
            if (!IsPaused)
                SwitchToUIControls();
        }

        public void RequestPlayerMode()
        {
            if (IsPaused)
                SwitchToPlayerControls();
        }

        void SwitchToUIControls()
        {
            IsPaused = true;
            m_InputActions.Player.Disable();
            m_InputActions.UI.Enable();
            PauseEvent?.Invoke(true);
        }

        void SwitchToPlayerControls()
        {
            IsPaused = false;
            m_InputActions.Player.Enable();
            m_InputActions.UI.Disable();
            PauseEvent?.Invoke(false);
        }

        #region InputSystem_Actions.IPlayerActions Members

        //*************IMPLEMENTED METHODS*************
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            CrouchEvent?.Invoke(context.ReadValueAsButton());
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            JumpEvent?.Invoke(context.action.WasPressedThisFrame());
        }
        public void OnPrevious(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
        public void OnNext(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnPunch(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (!IsPaused)
                SwitchToUIControls();
            else
                SwitchToPlayerControls();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            SprintEvent?.Invoke(context.ReadValueAsButton());
        }

        //********************************************

        //*************NOT IMPLEMENTED METHODS*************
        public void OnLeap(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            //AimPosition = context.ReadValue<Vector2>();
        }
        public void OnAttack(InputAction.CallbackContext context)
        {
            //noop
        }
        public void OnInteract(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            //noop
        }

        //********************************************

        #endregion

        #region InputSystem_Actions.IUIActions Members

        // Implementing UI interface methods
        public void OnNavigate(InputAction.CallbackContext context)
        {
            NavigationEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.performed && EventSystem.current is not null && EventSystem.current.currentSelectedGameObject is not null)
                // Simulate a click on the currently selected object
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject,
                    new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }

        public void OnBack(InputAction.CallbackContext context)
        {
            GoBackEvent?.Invoke(context.ReadValue<bool>());
        }
        
        //********************************************NOT IMPLEMENTED METHODS
        public void OnPoint(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
            //noop
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
            //noop
        }
        //********************************************

        #endregion

    }
}
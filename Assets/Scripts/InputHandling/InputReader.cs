using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace InputHandling
{
    using static InputSystem_Actions;

    public interface IInputReader
    {
        Vector2 moveInput { get; }
        void EnablePlayerActions();
        void DisablePlayerActions();
        bool isPaused { get; }

        void RequestUIMode();
        void RequestPlayerMode();
        event Action<bool> PauseEvent;
        void AssignPlayerInput(PlayerInput playerInput);
    }

    [CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
    public class InputReader : ScriptableObject, IPlayerActions, IUIActions, IInputReader
    {
        public Vector2 moveInput => _inputActions.Player.Move.ReadValue<Vector2>();
        public bool isPaused { get; private set; }
        
        #region Player Action Events
        public event Action<Vector2> MoveEvent = delegate { };
        public event Action<bool> JumpEvent = delegate { };
        public event Action<bool> ResetEvent = delegate { };
        public event Action<bool> PauseEvent = delegate { };

        // Added event for input mode changes (true = UI mode, false = Player mode)
        #endregion
        #region UI Events
        public event Action<Vector2> NavigationEvent = delegate { };
        public event Action<bool> GoBackEvent = delegate { };
        #endregion
        
        InputSystem_Actions _inputActions;
        PlayerInput _assignedPlayerInput;
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Assigns a PlayerInput component to this InputReader. This allows input routing for specific player instances.
        /// </summary>
        /// <param name="playerInput">The PlayerInput component to associate with this InputReader</param>
        public void AssignPlayerInput(PlayerInput playerInput)
        {
            _assignedPlayerInput = playerInput;
            
            // If we already had input actions, unsubscribe first
            if (_inputActions != null)
            {
                _inputActions.Player.Disable();
                _inputActions.UI.Disable();
            }
            
            // Create new input actions for this specific player
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.SetCallbacks(this);
            _inputActions.UI.SetCallbacks(this);
            
            // Set the devices to match the PlayerInput component
            if (playerInput != null && playerInput.devices.Count > 0)
            {
                _inputActions.devices = playerInput.devices;
            }
        }
        
        public void EnablePlayerActions()
        {
            if (_inputActions is null)
            {
                _inputActions = new InputSystem_Actions();
                _inputActions.Player.SetCallbacks(this);
                _inputActions.UI.SetCallbacks(this);
            }

            _inputActions.Player.Enable();
        }

        public void DisablePlayerActions()
        {
            if (_inputActions is null)
                return;
            _inputActions.Player.Disable();
            _inputActions.UI.Disable();
            _inputActions = null;
        }

        // Public methods for external components to request input mode changes
        public void RequestUIMode()
        {
            if (!isPaused)
                SwitchToUIControls();
        }

        public void RequestPlayerMode()
        {
            if (isPaused)
                SwitchToPlayerControls();
        }

        void SwitchToUIControls()
        {
            isPaused = true;
            _inputActions.Player.Disable();
            _inputActions.UI.Enable();
            PauseEvent?.Invoke(true);
        }

        void SwitchToPlayerControls()
        {
            isPaused = false;
            _inputActions.Player.Enable();
            _inputActions.UI.Disable();
            PauseEvent?.Invoke(false);
        }

        #region InputSystem_Actions.IPlayerActions Members

        //*************IMPLEMENTED METHODS*************
        public void OnMove(InputAction.CallbackContext context)
        {
            //MoveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            //JumpEvent?.Invoke(context.action.WasPressedThisFrame());
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
            if (!isPaused)
                SwitchToUIControls();
            else
                SwitchToPlayerControls();
        }

        //********************************************

        //*************NOT IMPLEMENTED METHODS*************
        public void OnSprint(InputAction.CallbackContext context)
        {
            //noop
        }

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
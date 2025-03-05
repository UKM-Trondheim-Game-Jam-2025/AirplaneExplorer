using System.Collections.Generic;
using Eflatun.SceneReference;
using InputHandling;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    //The state machine, which keeps track of everything
    public class MenuController : MonoBehaviour
    {
        [Header("Main Menu Parameters")]
        public SceneReference GameScene;

        [Header("Input")]
        [SerializeField] InputReader inputReader;

        //Drags = the different menus we have
        public State.MenuState[] AllMenus;

        //The states we can choose from
        public enum MenuState
        {
            Game, Main, Settings, Help, GameUIPanel, ConfirmPanel, ControlPanel, AudioPanel
        }

        //State-object dictionary to make it easier to activate a menu 
        readonly Dictionary<MenuState, State.MenuState> m_MenuDictionary = new Dictionary<MenuState, State.MenuState>();

        //The current active menu
        State.MenuState m_ActiveState;

        //To easier jump back one step, we can use a stack
        //This was also suggested in the Game Programming Patterns book
        //If so we don't have to hard-code in each state what happens when we jump back one step
        readonly Stack<MenuState> m_StateHistory = new Stack<MenuState>();

        #region InputReader Event Handler

        // Handle the GoBackEvent from InputReader
        void HandleGoBackEvent(bool value)
        {
            if (value)
                //Jump back one menu step when we press escape or a back button
                m_ActiveState?.SwitchBack();
        }

        #endregion

        #region Unity Lifecycle Methods
        void Start()
        {
            //Put all menus into a dictionary
            foreach (var menu in AllMenus)
            {
                if (menu is null) continue;

                //Inject a reference to this script into all menus
                menu.InitState(this);

                inputReader.GoBackEvent += HandleGoBackEvent;

                //Check if this key already exists, because it means we have forgotten to give a menu its unique key
                if (!m_MenuDictionary.TryAdd(menu.state, menu))
                    Debug.LogWarning($"The key <b>{menu.state}</b> already exists in the menu dictionary!");
            }

            //Deactivate all menus
            foreach (var state in m_MenuDictionary.Keys) m_MenuDictionary[state].gameObject.SetActive(false);

            //Activate the default menu
            SetActiveState(MenuState.Game);
        }
        
        void OnDestroy()
        {
            // Unsubscribe from events when destroyed
            if (inputReader is not null) inputReader.GoBackEvent -= HandleGoBackEvent;
        }
        
        #endregion

        #region MenuController Methods
        //Jump back one step = what happens when we press escape or one of the back buttons
        public void SwitchBack()
        {
            switch (m_StateHistory.Count)
            {
                //If we have just one item in the stack then, it means we are at the state we set at start, so we have to jump forward
                case <= 1:
                    SetActiveState(MenuState.Main);
                    break;
                default:
                    //Remove one from the stack
                    m_StateHistory.Pop();

                    //Activate the menu that's on the top of the stack
                    SetActiveState(m_StateHistory.Peek(), true);
                    break;
            }
        }

        //Activate a menu
        public void SetActiveState(MenuState newState, bool isSwitchingBack = false)
        {
            //First check if this menu exists
            if (!m_MenuDictionary.ContainsKey(newState))
            {
                Debug.LogWarning($"The key <b>{newState}</b> doesn't exist so you can't activate the menu!");

                return;
            }

            //Deactivate the old state
            m_ActiveState?.gameObject.SetActive(false);

            //Activate the new state
            m_ActiveState = m_MenuDictionary[newState];

            m_ActiveState.gameObject.SetActive(true);

            //If we are jumping back we shouldn't add to history because then we will get doubles
            if (!isSwitchingBack) m_StateHistory.Push(newState);
        }
        #endregion
        
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ::::::::NEEDS TO BE CHANGED:::::
        /// </summary>
        //Play game
        public void PlayGame()
        {
            SceneManager.LoadScene(GameScene.Name);
        }
        //Quit game
        public static void QuitGame()
        {
            Debug.Log("You quit da game!");

            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
        //------------------------------------------------------------------------------------------------------------------
        
    }
}
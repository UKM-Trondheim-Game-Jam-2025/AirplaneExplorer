using System.Collections.Generic;
using DG.Tweening;
using InputHandling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//https://youtu.be/0EsrYNAAEEY?si=IgVJhcgMx_TUdHk8
public class MenuNavigationLogic : MonoBehaviour
{
    #region Serialized Fields
    [Header("References")]
    public List<Selectable> Selectables = new List<Selectable>();
    [SerializeField] protected Selectable firstSelected;

    [Header("Controls")]
    [SerializeField] protected InputReader inputReader;

    [Header("Animations")]
    [SerializeField] protected float selectionAnimationScale = 1.1f;
    [SerializeField] protected float scaleDuration = 0.25f;
    [SerializeField] protected List<GameObject> animationExclusions = new List<GameObject>();

    [Header("Menu Behavior")]
    [SerializeField] protected bool activateOnEnable = true;
    [SerializeField] protected bool deactivateOnDisable = true;

    [Header("Audio")]
    [SerializeField] protected UnityEvent soundEvent;
    #endregion

    #region Private/Protected Fields
    protected Dictionary<Selectable, Vector3> Scales = new Dictionary<Selectable, Vector3>();
    protected Selectable LastSelection;
    protected Tween ScaleUpTween;
    protected Tween ScaleDownTween;
    #endregion

    #region Unity Lifecycle Methods
    public virtual void Awake()
    {
        foreach (var selectable in Selectables)
        {
            AddSelectionListeners(selectable);
            Scales.Add(selectable, selectable.transform.localScale);
        }
    }

    public virtual void OnEnable()
    {
        //Ensure that all selectables are reset to their original scale
        foreach (var selectable in Selectables) selectable.transform.localScale = Scales[selectable];

        inputReader.NavigationEvent += HandleNavigation;
        inputReader.PauseEvent += HandleInputModeChanged;

        // Request UI mode if this menu should activate input on enable
        if (activateOnEnable) inputReader.RequestUIMode();

        // Select the first element if specified
        if (firstSelected && EventSystem.current)
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
    }

    public virtual void OnDisable()
    {
        inputReader.NavigationEvent -= HandleNavigation;
        inputReader.PauseEvent -= HandleInputModeChanged;

        // Request player mode if this menu should deactivate input on disable
        if (deactivateOnDisable) inputReader.RequestPlayerMode();

        ScaleUpTween?.Kill(true);
        ScaleDownTween?.Kill(true);
    }
    #endregion

    #region Navigation Setup
    protected virtual void SetupNavigation()
    {
        // Option 2: Only set up navigation if not already configured
        for (int i = 0; i < Selectables.Count; i++)
        {
            var selectable = Selectables[i];
            var navigation = selectable.navigation;

            // Only set up automatic navigation if not already in explicit mode
            if (navigation.mode != Navigation.Mode.Explicit)
            {
                navigation.mode = Navigation.Mode.Explicit;

                // Set up basic cyclic navigation (up/down only)
                navigation.selectOnDown = Selectables[(i + 1) % Selectables.Count];
                navigation.selectOnUp = Selectables[(i - 1 + Selectables.Count) % Selectables.Count];

                selectable.navigation = navigation;
            }
        }
    }
    #endregion

    #region Input Handling
    protected virtual void HandleInputModeChanged(bool isUIMode)
    {
        if (isUIMode && EventSystem.current && EventSystem.current.currentSelectedGameObject == null)
        {
            // UI mode activated - select the first element if nothing is selected
            if (firstSelected)
                EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
            else if (Selectables.Count > 0) 
                EventSystem.current.SetSelectedGameObject(Selectables[0].gameObject);
        }
        // Player mode activated - could add custom behavior here
    }
    protected virtual void HandleNavigation(Vector2 direction)
    {
        // Only handle navigation when in UI mode
        if (!inputReader.IsPaused)
            return;

        // Small deadzone to prevent accidental navigation
        if (Mathf.Abs(direction.x) < 0.3f && Mathf.Abs(direction.y) < 0.3f)
            return;

        if (EventSystem.current && Selectables.Count > 0 && EventSystem.current.currentSelectedGameObject == null)
        {
            // If nothing is selected, select the first element
            EventSystem.current.SetSelectedGameObject(firstSelected ? firstSelected.gameObject : Selectables[0].gameObject);
        }
        //Unity's navigation system handles the rest. (Set up explicit navigation for buttons in the editor)
    }
    #endregion

    #region Selection Logic
    protected virtual void AddSelectionListeners(Selectable selectable)
    {
        //add LISTENERS
        var trigger = selectable.gameObject.GetComponent<EventTrigger>() ?? selectable.gameObject.AddComponent<EventTrigger>();

        //add SELECT event
        var selectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Select };
        selectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(selectEntry);

        //add DESELECT event
        var deselectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Deselect };
        deselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(deselectEntry);

        //add POINTER ENTER event
        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(pointerEnter);

        //add POINTER EXIT event
        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(pointerExit);
    }

    public void OnSelect(BaseEventData eventData)
    {
        soundEvent?.Invoke();
        LastSelection = eventData.selectedObject.GetComponent<Selectable>();

        if (animationExclusions.Contains(eventData.selectedObject))
            return;

        var newScale = eventData.selectedObject.transform.localScale * selectionAnimationScale;
        ScaleUpTween = eventData.selectedObject.transform.DOScale(newScale, scaleDuration);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (animationExclusions.Contains(eventData.selectedObject))
            return;

        var selectable = eventData.selectedObject.GetComponent<Selectable>();
        ScaleDownTween = eventData.selectedObject.transform.DOScale(Scales[selectable], scaleDuration);
    }

    public void OnPointerEnter(BaseEventData eventData)
    {
        var pointerEventData = (PointerEventData)eventData;
        if (pointerEventData == null)
            return;

        //Make so that the text does not block the selection of the button
        var selectable = pointerEventData.pointerEnter.GetComponentInParent<Selectable>() ??
                         pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
        pointerEventData.selectedObject = selectable.gameObject;
    }

    public void OnPointerExit(BaseEventData eventData)
    {
        var pointerEventData = (PointerEventData)eventData;
        if (pointerEventData == null)
            return;

        _ = pointerEventData.pointerEnter.GetComponentInParent<Selectable>() ??
            pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
        pointerEventData.selectedObject = null;
    }
    #endregion

    #region Public API
    // Public method to manually show this menu and activate UI mode
    public virtual void ShowMenu()
    {
        gameObject.SetActive(true);
        inputReader?.RequestUIMode();
    }

    // Public method to manually hide this menu and return to player mode
    public virtual void HideMenu()
    {
        if (deactivateOnDisable) inputReader.RequestPlayerMode();
        gameObject.SetActive(false);
    }
    #endregion
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Skills window controller implementing IUIMenu interface.
/// Manage all the values for the open and close menu and change tab animations.
/// Contains references to all skills buttons and skills panels
/// </summary>
public class SkillsController : MonoBehaviour, IUIMenu
{
    private RectTransform _transform;
    private Button _button;
    
    
    [Header("Skill Tabs")] 
    [SerializeField] private int SelectTabIndex= 2;

    [Header("General Lerp Values")] 
    [SerializeField] private float animationTime= 0.3f;
    
    /// <summary>
    /// Allows to ignore standardize movement of buttons if a specific tab was selected
    /// </summary>
    [SerializeField] private SkillTabSelection IgnoreForceRotationWhenSelected;
    
    [Header("Button Lerp Values")] 
    [SerializeField] private Vector2 OpenButtonSize;
    [SerializeField] private Vector2 OpenButtonPosition;
    [SerializeField] private Vector2 CloseButtonSize;
    [SerializeField] private Vector2 CloseButtonPosition;

    [Header("Tabs Lerp Values")]
    [Space]
    [Header("Open Skills Window Values")]
    [Space]
    [SerializeField] private float OpenButtonCircleRadius;
    [SerializeField] private Vector2 OpenButtonCircleCenter;
    [SerializeField] private Vector2[] OpenButtonTabPositions;
    [SerializeField] private Vector2[] OpenButtonTabSizes;
    [Space]
    [Header("Close Skills Window Values")]
    [Space]
    [SerializeField] private float ClosedButtonCircleRadius;
    [SerializeField] private Vector2 ClosedButtonCircleCenter;
    [SerializeField] private Vector2[] ClosedButtonTabPositions;
    [SerializeField] private Vector2 ClosedButtonTabSizes;

    [SerializeField]private SkillTabSelection[] SkillsTabsButtons;
    private SkillTabSelection[] OpenWindowSkillTabOrder;
    
   
    void Awake()
    {
        OpenWindowSkillTabOrder = new SkillTabSelection[SkillsTabsButtons.Length];
        SkillsTabsButtons.CopyTo(OpenWindowSkillTabOrder, 0);

        if (_transform == null) _transform = GetComponent<RectTransform>();
        if (_button == null) _button = GetComponent<Button>();

        _transform.anchoredPosition = CloseButtonPosition;
        _transform.sizeDelta = CloseButtonSize;

        GetCloseButtonTabPositions();
        GetOpenButtonTabPositions();
        
        for (int i = 0; i < SkillsTabsButtons.Length; i++)
        {
            SkillsTabsButtons[i].SetUp(this, ClosedButtonTabPositions[i], ClosedButtonTabSizes);
        }
    }

    
    /// <summary>
    /// Calculate the position for all the buttons when the menu is closed based on a pentagon shape
    /// </summary>
    void GetCloseButtonTabPositions()
    {
        float startAngleDeg = 90f;
        int sides = SkillsTabsButtons.Length;

        for (int i = 0; i < sides; i++)
        {
            float angleDeg = startAngleDeg + i * 360f / sides;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float x = Mathf.Cos(angleRad) * ClosedButtonCircleRadius + ClosedButtonCircleCenter.x;
            float y = Mathf.Sin(angleRad) * ClosedButtonCircleRadius + ClosedButtonCircleCenter.y;

            ClosedButtonTabPositions[i]=(new Vector2(x, y));
        }
    }

    /// <summary>
    /// Calculate the position for al the buttons when the menu is open considering an equal distribution en the left side 
    /// </summary>
    void GetOpenButtonTabPositions()
    {
        float[] anglesDeg = { 90, -225, 180, 225, -90 };
        for (int i = 0; i < anglesDeg.Length; i++)
        {
            float angleRad = anglesDeg[i] * Mathf.Deg2Rad;
            float x = Mathf.Cos(angleRad) * OpenButtonCircleRadius + OpenButtonCircleCenter.x;
            float y = Mathf.Sin(angleRad) * OpenButtonCircleRadius + OpenButtonCircleCenter.y;

            OpenButtonTabPositions[i] = new Vector2(x, y);
        }
    }
    


    /// <summary>
    /// Manage the movements of the buttons when a new tab is selected.
    /// Creates a referential array of the current position, gets the index of the tab to be selected and shift the position
    /// of every tab based on their previous position, triggering a Lerp animation.
    /// </summary>
    /// <param name="tab"> the button being selected</param>
    public void ChangeTab(SkillTabSelection tab)
    {
        int temp = Array.IndexOf(OpenWindowSkillTabOrder,tab);
        
        OpenWindowSkillTabOrder[SelectTabIndex].DeselectTab();

        int moveSlots = SelectTabIndex - temp;

        bool clockwiseMovement = moveSlots < 0;
        
        SkillTabSelection[] tempArray=new SkillTabSelection[5];
        OpenWindowSkillTabOrder.CopyTo(tempArray, 0);

        for (int i = 0; i < OpenWindowSkillTabOrder.Length; i++)
        {

            int newSlotPos = i + moveSlots;
            if (newSlotPos < 0) newSlotPos = OpenWindowSkillTabOrder.Length + newSlotPos;
            else if (newSlotPos > 4) newSlotPos = -OpenWindowSkillTabOrder.Length + newSlotPos;

            

            OpenWindowSkillTabOrder[newSlotPos] = tempArray[i];
            OpenWindowSkillTabOrder[newSlotPos].MoveButton(OpenButtonTabPositions[newSlotPos], OpenButtonTabSizes[newSlotPos],OpenButtonCircleCenter, animationTime * 2,null, clockwiseMovement);
            
        }
        


    }

    
    /// <summary>
    /// Manage the behaviour when the menu opens
    /// Animate the size and position of the holder with lerps, and moves all the buttons based on the previous selected tab.
    /// </summary>
    public void OpenMenu()
    {
        _button.enabled = false;
        StartCoroutine(LerpSize(_transform.sizeDelta, OpenButtonSize, animationTime));
        StartCoroutine(LerpPosition(_transform.anchoredPosition, OpenButtonPosition, animationTime));

        bool OpenRotationDirection = 0 > OpenWindowSkillTabOrder[SelectTabIndex].transform.localPosition.y - OpenButtonTabPositions[SelectTabIndex].y;
        
        for (int i = 0; i < OpenWindowSkillTabOrder.Length; i++)
        {
            if(OpenWindowSkillTabOrder[SelectTabIndex]==IgnoreForceRotationWhenSelected)
                OpenWindowSkillTabOrder[i].MoveButton(OpenButtonTabPositions[i], OpenButtonTabSizes[i],OpenButtonCircleCenter, animationTime, OpenButtonCircleRadius);
            else
                OpenWindowSkillTabOrder[i].MoveButton(OpenButtonTabPositions[i], OpenButtonTabSizes[i],OpenButtonCircleCenter, animationTime, OpenButtonCircleRadius, OpenRotationDirection);
            OpenWindowSkillTabOrder[i].ActiveButton(true);
        }
        OpenWindowSkillTabOrder[SelectTabIndex].SelectTab();
    }

    /// <summary>
    /// Manage the behaviour when the menu closes
    /// Animate the size and position of the holder with lerps, and moves all the buttons based on final position of the selected tab.
    /// </summary>
    public void CloseMenu()
    {
        _button.enabled = true;
        StartCoroutine(LerpSize(_transform.sizeDelta, CloseButtonSize, animationTime));
        StartCoroutine(LerpPosition(_transform.anchoredPosition, CloseButtonPosition, animationTime));

        int temp = Array.IndexOf(SkillsTabsButtons,OpenWindowSkillTabOrder[SelectTabIndex]);
        
        bool clockwise = 0 > OpenWindowSkillTabOrder[SelectTabIndex].transform.localPosition.y - ClosedButtonTabPositions[temp].y;
        
        OpenWindowSkillTabOrder[SelectTabIndex].DeselectTab();
        for (int i = 0; i < SkillsTabsButtons.Length; i++)
        {
            if(OpenWindowSkillTabOrder[SelectTabIndex]==IgnoreForceRotationWhenSelected)
                SkillsTabsButtons[i].MoveButton(ClosedButtonTabPositions[i], ClosedButtonTabSizes,ClosedButtonCircleCenter,animationTime,ClosedButtonCircleRadius);
            else
                SkillsTabsButtons[i].MoveButton(ClosedButtonTabPositions[i], ClosedButtonTabSizes,ClosedButtonCircleCenter,animationTime,ClosedButtonCircleRadius,clockwise);
            SkillsTabsButtons[i].ActiveButton(false);
        }
    }
    
    /// <summary>
    /// Simple lerp to animate the holder size
    /// </summary>
    /// <param name="curSize">Current size of the holder</param>
    /// <param name="newSize">Target size of the holder</param>
    /// <param name="duration">Duration of the lerp animation</param>
    /// <returns></returns>
    IEnumerator LerpSize(Vector2 curSize,Vector2 newSize, float duration)
    {
        float t = 0;

        while (t < duration)
        {
            
            _transform.sizeDelta = Vector2.Lerp(curSize, newSize, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        _transform.sizeDelta = newSize;

    }

    /// <summary>
    /// Simple lerp to animate the holder position
    /// </summary>
    /// <param name="curPosition">Current position of the holder</param>
    /// <param name="newPosition">Target position of the holder</param>
    /// <param name="duration">Duration of the lerp animation</param>
    /// <returns></returns>
    IEnumerator LerpPosition(Vector2 curPosition, Vector2 newPosition, float duration)
    {
        float t = 0;

        while (t < duration)
        {
            _transform.anchoredPosition = Vector2.Lerp(curPosition, newPosition, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        _transform.anchoredPosition = newPosition;
    }
}

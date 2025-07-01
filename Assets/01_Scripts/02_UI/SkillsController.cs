using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillsController : MonoBehaviour, IUIMenu
{
    private RectTransform _transform;
    private Button _button;
    
    
    [Header("Skill Canvas")] 
    [SerializeField] private GameObject SkillsCanvas;


    [Header("General Lerp Values")] 
    [SerializeField] private float animationTime= 0.3f;
    [SerializeField] private int SelectTabIndex= 2;
    [Header("Button Lerp Values")] 
    [SerializeField] private Vector2 OpenButtonSize;
    [SerializeField] private Vector2 OpenButtonPosition;
    [SerializeField] private Vector2 CloseButtonSize;
    [SerializeField] private Vector2 CloseButtonPosition;

    [Header("Tabs Lerp Values")] 
    [SerializeField] private Vector2[] OpenButtonTabPositions;
    [SerializeField] private Vector2[] OpenButtonTabSizes;

    [SerializeField]private SkillTabSelection[] Tabs;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        

        if (_transform == null) _transform = GetComponent<RectTransform>();
        if (_button == null) _button = GetComponent<Button>();

        _transform.anchoredPosition = CloseButtonPosition;
        _transform.sizeDelta = CloseButtonSize;

        foreach (var tab in Tabs)
        {
            tab.SetUp(this);
        }
    }


    public void ChangeTab(SkillTabSelection tab)
    {
        int temp = Array.IndexOf(Tabs,tab);
        
        Tabs[SelectTabIndex].DeselectTab();

        int moveSlots = SelectTabIndex - temp;

        bool clockwiseMovement = moveSlots < 0;
        
        SkillTabSelection[] tempArray=new SkillTabSelection[5];
        Tabs.CopyTo(tempArray, 0);

        for (int i = 0; i < Tabs.Length; i++)
        {

            int newSlotPos = i + moveSlots;
            if (newSlotPos < 0) newSlotPos = Tabs.Length + newSlotPos;
            else if (newSlotPos > 4) newSlotPos = -Tabs.Length + newSlotPos;

            

            Tabs[newSlotPos] = tempArray[i];
            Tabs[newSlotPos].RotateButton(OpenButtonTabPositions[newSlotPos], OpenButtonTabSizes[newSlotPos], animationTime * 2, clockwiseMovement);
            
        }
        


    }

    public void OpenMenu()
    {
        _button.enabled = false;
        StartCoroutine(LerpSize(_transform.sizeDelta, OpenButtonSize, animationTime));
        StartCoroutine(LerpPosition(_transform.anchoredPosition, OpenButtonPosition, animationTime));

        for (int i = 0; i < Tabs.Length; i++)
        {
            Tabs[i].MoveButton(OpenButtonTabPositions[i], OpenButtonTabSizes[i], animationTime);
        }
        Tabs[SelectTabIndex].SelectTab();
    }

    public void CloseMenu()
    {
        _button.enabled = true;
        StartCoroutine(LerpSize(_transform.sizeDelta, CloseButtonSize, animationTime));
        StartCoroutine(LerpPosition(_transform.anchoredPosition, CloseButtonPosition, animationTime));

        for (int i = 0; i < Tabs.Length; i++)
        {
            Tabs[i].OnCloseButton(animationTime);
        }
        Tabs[SelectTabIndex].DeselectTab();
    }
    
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

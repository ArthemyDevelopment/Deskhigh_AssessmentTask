using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Controller for the Select Tab buttons
/// Manage the button icons, behaviour when selected and lerp animations.
/// </summary>
public class SkillTabSelection : MonoBehaviour
{
    private SkillsController _controller;
    private RectTransform _transform;
    private Image _iconImage;
    private Button _button;

    [Header("Skill Panel")] 
    [SerializeField] private GameObject SkillPanel;
    
    [Header("Icon values")]
    [SerializeField] private Sprite IdleSprite;
    [SerializeField] private Sprite SelectedSprite;
    
    

    /// <summary>
    /// Initial configuration of the buttons, sets all the starting values and parameters
    /// </summary>
    /// <param name="controller">Assign the Controller the button belongs to</param>
    /// <param name="startingPosition">Set the starting local position of the button</param>
    /// <param name="startingSize">Set the starting Delta Size of the button</param>
    public void SetUp(SkillsController controller, Vector2 startingPosition, Vector2 startingSize)
    {
        if (_iconImage == null) _iconImage = GetComponent<Image>();
        if (_transform == null) _transform = GetComponent<RectTransform>();
        if (_button == null) _button = GetComponent<Button>();
        _button.enabled = false;
        _controller = controller;
        _transform.localPosition = startingPosition;
        _transform.sizeDelta = startingSize;
         DeselectTab();
    }



    public void ChangeTab()
    {
        _controller.ChangeTab(this);
        SelectTab();
    }

    public void SelectTab()
    {
        _iconImage.sprite = SelectedSprite;
        SkillPanel.SetActive(true);
    }
    
    public void DeselectTab()
    {
        _iconImage.sprite = IdleSprite;
        SkillPanel.SetActive(false);
    }

    public void ActiveButton(bool state)
    {
        if(_button.enabled!=state)_button.enabled = state;
    }

    
    /// <summary>
    /// Starts the lerp animation
    /// </summary>
    /// <param name="newButtonPosition">Target position of the animation</param>
    /// <param name="NewButtonSize">Target size of the animation</param>
    /// <param name="CenterOfRotation">The center of the circumference to which the button will rotate</param>
    /// <param name="animationTime">The duration of the animation</param>
    /// <param name="targetRadius">Optional parameter: The radius of the circumference to which the button will rotate</param>
    /// <param name="clockwise">Optional parameter: Force if the rotation should follow a clockwise direction or no</param>
    public void MoveButton(Vector2 newButtonPosition, Vector2 NewButtonSize,Vector2 CenterOfRotation, float animationTime, float? targetRadius=null, bool? clockwise=null)
    {
        BeginLerpPosition(newButtonPosition, CenterOfRotation,animationTime,targetRadius, clockwise);
        BeginLerpSize(NewButtonSize, animationTime);
    }


    /// <summary>
    /// Init the Lerp size animation coroutine
    /// </summary>
    /// <param name="newSize">Target size of the animation</param>
    /// <param name="duration">Duration of the animation</param>
    private void BeginLerpSize(Vector2 newSize, float duration)
    {
        StartCoroutine(LerpSize(_transform.sizeDelta, newSize, duration));
    }

    
    /// <summary>
    /// Init the Lerp movement animation coroutine
    /// </summary>
    /// <param name="newPosition">Target position of the animation</param>
    /// <param name="CenterOfRotation">The center of the circumference to which the button will rotate</param>
    /// <param name="duration">Duration of the animation</param>
    /// <param name="targetRadius">Optional parameter: The radius of the circumference to which the button will rotate</param>
    /// <param name="clockwise">Optional parameter: Force if the rotation should follow a clockwise direction or no</param>
    private void BeginLerpPosition(Vector2 newPosition, Vector2 CenterOfRotation,float duration, float? targetRadius=null,bool? clockwise=null)
    {
        StartCoroutine(LerpCircularPosition(_transform.localPosition, newPosition, CenterOfRotation,duration, targetRadius,clockwise));
    }
    

    /// <summary>
    /// Simple Lerp to animate the button size
    /// </summary>
    /// <param name="curSize">Current button size</param>
    /// <param name="newSize">Target button size</param>
    /// <param name="duration">Duration of the lerp</param>
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
    /// Custom Slerp to animate the button position using a custom center, dynamic circumference radius and forcing movement direction, clockwise or counter clockwise. 
    /// </summary>
    /// <param name="curPosition">Current button position</param>
    /// <param name="newPosition">Target button position</param>
    /// <param name="CenterOfRotation">The center of the circumference to which the button will rotate</param>
    /// <param name="duration">Duration of the animation</param>
    /// <param name="targetRadiusOverride">Optional parameter: The radius of the circumference to which the button will rotate</param>
    /// <param name="clockwise">Optional parameter: Force if the rotation should follow a clockwise direction or no</param>
    /// <returns></returns>
    IEnumerator LerpCircularPosition(Vector2 curPosition, Vector2 newPosition, Vector2 CenterOfRotation,float duration, float? targetRadiusOverride = null, bool? clockwise = null)
    {
        float t = 0f;

        Vector2 from = curPosition - CenterOfRotation;
        Vector2 to = newPosition - CenterOfRotation;

        float startAngle = Mathf.Atan2(from.y, from.x);
        float endAngle = Mathf.Atan2(to.y, to.x);

        float angleDifference = Mathf.DeltaAngle(startAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;

        if (clockwise.HasValue)
        {
            if (clockwise.Value && angleDifference > 0)
                angleDifference -= 2 * Mathf.PI;
            else if (!clockwise.Value && angleDifference < 0)
                angleDifference += 2 * Mathf.PI;
        }

        float startRadius = from.magnitude;
        float endRadius = targetRadiusOverride ?? to.magnitude;

        while (t < duration)
        {
            float interp = t / duration;
            float currentAngle = startAngle + angleDifference * interp;
            
            float currentRadius = Mathf.Lerp(startRadius, endRadius, interp);

            Vector2 offset = new Vector2(
                Mathf.Cos(currentAngle),
                Mathf.Sin(currentAngle)
            ) * currentRadius;

            _transform.localPosition = CenterOfRotation + offset;

            t += Time.deltaTime;
            yield return null;
        }

        _transform.localPosition = newPosition;
    }
    
}

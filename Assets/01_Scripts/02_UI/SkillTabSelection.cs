using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillTabSelection : MonoBehaviour
{
    private SkillsController _controller;
    private RectTransform _transform;
    private Image _iconImage;
    private Button _button;
    [SerializeField] private Vector2 CenterOfRotation;
    [SerializeField] private Vector2 CloseButtonPosition;
    [SerializeField] private Vector2 CloseButtonSize= new Vector2(95,95);
    [SerializeField] private Sprite IdleSprite;
    [SerializeField] private Sprite SelectedSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private void Awake()
    {
        if (_iconImage == null) _iconImage = GetComponent<Image>();
        if (_transform == null) _transform = GetComponent<RectTransform>();
        if (_button == null) _button = GetComponent<Button>();
        _transform.localPosition = CloseButtonPosition;
        _transform.sizeDelta = CloseButtonSize;
        _button.enabled = false;
        DeselectTab();
    }

    public void SetUp(SkillsController controller)
    {
        _controller = controller;
    }

    private void OnEnable()
    {
        _iconImage.sprite = IdleSprite;
    }

    public void ChangeTab()
    {
        SelectTab();
        _controller.ChangeTab(this);
    }

    public void SelectTab()
    {
        _iconImage.sprite = SelectedSprite;
    }
    
    public void DeselectTab()
    {
        _iconImage.sprite = IdleSprite;
    }

    public void MoveButton(Vector2 OpenButtonPosition, Vector2 OpenButtonSize, float animationTime)
    {
        BeginLerpPosition(OpenButtonPosition, animationTime);
        BeginLerpSize(OpenButtonSize, animationTime);
        if (!_button.enabled) _button.enabled = true;
    }

    public void RotateButton(Vector2 OpenButtonPosition, Vector2 OpenButtonSize, float animationTime, bool clockwise = true)
    {
        BeginLerpCircularPosition(OpenButtonPosition, animationTime, clockwise);
        BeginLerpSize(OpenButtonSize, animationTime);
    }

    public void OnCloseButton(float animationTime)
    {
        BeginLerpSize(CloseButtonSize, animationTime);
        BeginLerpPosition(CloseButtonPosition, animationTime);
        _button.enabled = false;
    }


    public void BeginLerpSize(Vector2 newSize, float duration)
    {
        StartCoroutine(LerpSize(_transform.sizeDelta, newSize, duration));
    }

    public void BeginLerpPosition(Vector2 newPosition, float duration)
    {
        StartCoroutine(LerpPosition(_transform.localPosition, newPosition, duration));
    }
    
    public void BeginLerpCircularPosition(Vector2 newPosition, float duration, bool clockwise=true)
    {
        StartCoroutine(LerpCircularPosition(_transform.localPosition, newPosition, duration, clockwise));
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
            _transform.localPosition = Vector2.Lerp(curPosition, newPosition, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        _transform.localPosition = newPosition;
    }
    
    IEnumerator LerpCircularPosition(Vector2 curPosition, Vector2 newPosition, float duration, bool clockwise = true)
    {
        float t = 0f;

        Vector2 from = curPosition - CenterOfRotation;
        Vector2 to = newPosition - CenterOfRotation;

        float startAngle = Mathf.Atan2(from.y, from.x);
        float endAngle = Mathf.Atan2(to.y, to.x);

        float angleDifference = Mathf.DeltaAngle(startAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;
        
        if (clockwise && angleDifference > 0)
            angleDifference -= 2 * Mathf.PI;
        else if (!clockwise && angleDifference < 0)
            angleDifference += 2 * Mathf.PI;

        float radius = from.magnitude;

        while (t < duration)
        {
            float interp = t / duration;
            float currentAngle = startAngle + angleDifference * interp;

            Vector2 offset = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * radius;
            _transform.localPosition = CenterOfRotation + offset;

            t += Time.deltaTime;
            yield return null;
        }

        _transform.localPosition = newPosition;
    }
    
}

using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    public RectTransform joystickBG;
    public RectTransform joystickHandle;

    [Header("Settings")]
    public float handleRange = 100f;

    private Vector2 inputVector;
    private Vector2 joystickCenter;

    public Vector2 Direction => inputVector;

    void Start()
    {
        joystickCenter = joystickBG.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBG, eventData.position, eventData.pressEventCamera, out pos);

        inputVector = pos / (joystickBG.sizeDelta / 2f);
        inputVector = Vector2.ClampMagnitude(inputVector, 1f);

        // Move handle
        joystickHandle.anchoredPosition = inputVector * handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }
}

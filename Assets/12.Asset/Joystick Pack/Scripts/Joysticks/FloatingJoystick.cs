using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FloatingJoystick : NSY.Joystick
{
    public UnityEvent<Vector2, bool> OnMovementInput; // 이벤트 정의
    public Vector3 joystickMoveVec;
    public bool isClick;

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
        
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);

        joystickMoveVec = Direction;

        isClick = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
        joystickMoveVec = new Vector3(Horizontal, 0, Vertical);
        // 이벤트 발생
        OnMovementInput.Invoke(joystickMoveVec, false);

        isClick = false;
    }
}
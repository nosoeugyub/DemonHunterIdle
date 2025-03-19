using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 캐릭터 인포 팝업 내 헌터 회전 관리                  
/// </summary>
public class HunterControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("영웅 회전 속도")]
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] GameObject[] heroObj = null;

    bool isClick;
    Vector3 originPos = Vector3.zero;
    Vector3 movePos, rotation;

    public void FixedUpdate()
    {
        if (isClick) //커서가 헌터위에 존재할떄
        {
            var mousePosition = Input.mousePosition;
            movePos = mousePosition - originPos;
            movePos.Normalize();

            //  드래그되지 않은 상태에서 회전되지 않도록 예외처리
            if (movePos.x == 0)
                return;

            rotation = heroObj[GameDataTable.Instance.User.currentHunter].transform.localEulerAngles;

            if (movePos.x < 0)
                rotation.y += rotationSpeed;
            else if (movePos.x > 0)
                rotation.y -= rotationSpeed;

            heroObj[GameDataTable.Instance.User.currentHunter].transform.localEulerAngles = rotation;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isClick = true;
        originPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClick = false;
    }

    public void OnStopRotation()
    {
        isClick = false;
    }

}

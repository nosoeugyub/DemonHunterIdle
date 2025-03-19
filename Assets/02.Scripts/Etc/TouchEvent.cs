using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class TouchEvent : MonoBehaviour
{
    public Canvas canvas;  // 터치 이펙트를 표시할 캔버스
    public GameObject pos;
    public int size;
    void Update()
    {
        // 터치 또는 마우스 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPosition = Input.mousePosition;  // 터치된 화면 위치 가져오기

            // 화면 좌표를 월드 좌표로 변환
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
            worldPosition.z = 0;  // 이펙트를 2D 공간에 배치 (필요에 따라 z 값을 조정)

            // 화면 좌표를 UI 캔버스 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                touchPosition,
                canvas.worldCamera,
                out Vector2 canvasPosition
            );

            // ObjectPooler에서 이펙트를 생성 (캔버스 좌표로)
            GameObject gg = ObjectPooler.SpawnFromPool("SparkExplosion2D", Vector3.zero);
            gg.transform.SetParent(pos.transform, false); // 이펙트를 UI 캔버스의 자식으로 설정
            gg.transform.localPosition = canvasPosition; // 이펙트 위치를 캔버스 좌표로 설정
            gg.transform.localScale = Vector3.one * size; // 이펙트 크기 설정
            gg.SetActive(true);  // 이펙트 활성화
        }
    }
}

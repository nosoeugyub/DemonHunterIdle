using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resolution_Controller : MonoBehaviour
{
    #region Variables
    public Vector2 targetAspectRatio = new Vector2(16, 9);  // 원하는 비율 (16:9)
    public Camera mainCamera;  // 메인 카메라 참조
    public Canvas canvas;  // UI 캔버스 참조
    private int screenSizeX = 0;
    private int screenSizeY = 0;
    #endregion

    #region Methods

    // 카메라의 뷰포트를 비율에 맞춰 조정하는 함수
    private void RescaleCamera()
    {
        return;
        // 화면 크기가 변하지 않았으면 계산하지 않음
        if (Screen.width == screenSizeX && Screen.height == screenSizeY) return;

        float targetAspect = targetAspectRatio.x / targetAspectRatio.y;  // 목표 비율
        float windowAspect = (float)Screen.width / (float)Screen.height;  // 현재 창의 비율
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = mainCamera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            mainCamera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = mainCamera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            mainCamera.rect = rect;
        }

        // 현재 화면 크기 저장
        screenSizeX = Screen.width;
        screenSizeY = Screen.height;
    }

    // UI 캔버스의 스케일을 맞춰 조정하는 함수
    private void RescaleCanvas()
    {
        if (canvas != null)
        {
            CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();

            if (canvasScaler != null)
            {
                // 목표 비율로 화면 비율 고정
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

                // 현재 창의 비율과 목표 비율을 비교하여, 맞춰야 할 비율을 결정
                float targetAspect = targetAspectRatio.x / targetAspectRatio.y;
                float windowAspect = (float)Screen.width / (float)Screen.height;
                canvasScaler.matchWidthOrHeight = (windowAspect >= targetAspect) ? 1 : 0;
            }
        }
    }

    // 카메라와 캔버스의 비율을 조정하는 메서드
    public void AdjustResolution()
    {
        RescaleCamera();
        RescaleCanvas();
    }

    #endregion

    #region Unity Methods

    private void Start()
    {
        AdjustResolution();  // 시작할 때 해상도 조정
    }

    private void Update()
    {
        //AdjustResolution();  // 화면 크기가 변할 때마다 비율 조정
    }

    #endregion
}

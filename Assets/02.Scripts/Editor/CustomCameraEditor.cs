using UnityEditor;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-09-03
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 유니티 씬 내 카메라의 세팅을 변경하는 단축키 제작
/// CameraEditor을 사용해야하여 단축키 스크립트를 따로 분리함
/// </summary>
public class CustomCameraEditor : CameraEditor
{
    static bool isUIMode = false;

    [MenuItem("CustomShortcuts/Set Scene Camera Setting %e")]
    public static void ResetSceneCameraSetting() // UI확인용 씬 카메라 설정 토글
    {
        if(isUIMode)
        {
            Tools.visibleLayers = Layer.UIlayerMask; 
            
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null && !sceneView.isRotationLocked)
            {
                Vector3 pivot = GameObject.Find("Canvas").transform.position;
                Vector3 direction = Vector3.forward;

                // LookAt 함수를 사용하여 카메라 설정
                sceneView.LookAt(pivot, Quaternion.LookRotation(direction),4.2f); //줌 값을 임의의 값으로 넣음

                // SceneView 업데이트
                sceneView.Repaint();
            }
        }
        else
        {
            Tools.visibleLayers = ~0; //모든 레이어 표시

            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null && !sceneView.isRotationLocked)
            {
                GameObject setObj = (GameManager.Instance != null ) ? GameManager.Instance.MainCam.gameObject : GameObject.FindGameObjectWithTag(Tag.MainCamera);
                Vector3 pivot = setObj.transform.position;
                Vector3 direction = Vector3.down;
                // LookAt 함수를 사용하여 카메라 설정
                sceneView.LookAt(pivot, Quaternion.LookRotation(direction), 11f); //줌 값을 임의의 값으로 넣음

                Selection.activeGameObject = setObj;
                // SceneView 업데이트
                sceneView.Repaint();
            }
        }
        isUIMode = !isUIMode;
    }

}

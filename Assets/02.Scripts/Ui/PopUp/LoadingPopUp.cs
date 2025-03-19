using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 로딩 팝업 UI 관리 (화면 중앙 동그라미 회전)
/// </summary>
public class LoadingPopUp : MonoBehaviour
{
    public Image bufferImage = null;
    bool isLoading = false;

    public void Show(bool _isLoading)
    {
        gameObject.SetActive(true);
        StartCoroutine(Loading());
        isLoading = _isLoading;
    }
    public void Close(bool _isLoading)
    {
        if (_isLoading)
            isLoading = false;

        gameObject.SetActive(false);

    }
    IEnumerator Loading()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        while (true)
        {
            bufferImage.rectTransform.Rotate(new Vector3(0, 0, 12f));
            if (bufferImage.rectTransform.rotation.z > 1800)
            {
                bufferImage.rectTransform.rotation = new Quaternion(0, 0, 0, 0);
            }
            yield return wait;
            if (!this.gameObject.activeSelf)
                break;
        }
    }
}

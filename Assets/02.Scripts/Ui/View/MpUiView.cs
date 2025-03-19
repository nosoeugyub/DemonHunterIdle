using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-06-23
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : MP 관련 보여지는 컨틀롤러 
/// /// </summary>
public class MpUiView : MonoBehaviour
{
    public Image MpBar;

    public void Init_Hpbar()
    {
        MpBar.fillAmount = 0;
    }
    public void ActiveObject(bool isactive)
    {
        gameObject.SetActive(isactive);
    }
    public void UpdateHpBar(float maxmp, float currentmp)
    {
        float fill = Mathf.Clamp01(currentmp/ maxmp);
        MpBar.fillAmount = fill;
    }
}

using DuloGames.UI.Tweens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 :  HP 바 관련 스크립트
/// /// </summary>

public class HpUiView : MonoBehaviour
{
    public Transform Target;
    public Image HpBar;
    public Image BossTimeBar;
    private Quaternion initialRotation;

    public float bossTimeAtack;
    public void Init_Hpbar()
    {
        HpBar.fillAmount = 0;
    }

    void Start()
    {
        // 시작 시 초기 회전값을 저장
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // 초기 회전값으로 회전 고정
        transform.rotation = initialRotation;
        transform.position = Target.position;
    }

    public void Init_Timebar(float time)
    {
        BossTimeBar.fillAmount = 1;
        bossTimeAtack = time;
    }

    public void ActiveObject(bool isactive)
    {
        
        if (isactive) 
        {
            this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            this.gameObject.transform.localPosition = new Vector3(9999, 9999, 9999);
        }
       
    }
    public void UpdateHpBar(float maxhp  , float currenthp)
    {
        float TotalHp = Mathf.Clamp01(currenthp / maxhp);

        HpBar.fillAmount = TotalHp;
    }      

    //보스 타임어택 슬라이드 바
    public void UpdateBossTimeBar()
    {
        StopCoroutine(UpdateBossTimeBarcrutine());
        StartCoroutine(UpdateBossTimeBarcrutine());
    }

    private IEnumerator UpdateBossTimeBarcrutine()
    {
        float times = 0;
        times = bossTimeAtack;
        BossTimeBar.fillAmount = 1;
        while (times > 0)
        {
            yield return new WaitForSeconds(1f);  // 1초 대기

            times -= 1;  // 1초 감소
            float amount = times / bossTimeAtack;
            BossTimeBar.fillAmount = amount;  // 슬라이더 바 업데이트

            if (times <= 0)
            {
                TimeBarOver();
            }
        }
    }

    public void TimeBarOver()
    {
        //유저 다시시작
        GameEventSystem.GameHunterDie_SendGameEventHandler_Event();


    }
}

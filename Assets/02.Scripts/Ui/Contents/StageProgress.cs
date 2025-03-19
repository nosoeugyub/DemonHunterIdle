using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// </summary>
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스테이지 바 관련 스크립트
/// </summary>
public class StageProgress : MonoBehaviour
{
    [SerializeField] private GameObject[] StageProgressImgArray;
    [SerializeField] private TextMeshProUGUI StageTimer;
    public int StageTimerValue;
    public bool isEndBossTimer;
    public void StageTimer_Set()
    {
        StageTimer.gameObject.SetActive(true);
        StageTimerValue = 600;
        isEndBossTimer = false;

        StageTimer.text = StageTimerValue.ToString();
    }

    public void StartTimer()
    {
        // 타이머 시작
        StartCoroutine(StartBossTimer());
    }

    public void StopTimer()
    {
        // 타이머 중지
        StopCoroutine(StartBossTimer());
        StageTimer.gameObject.SetActive(false);
    }

    IEnumerator StartBossTimer()
    {
        while (StageTimerValue > 0)
        {
            // 1초씩 감소
            yield return new WaitForSeconds(1f);
            StageTimerValue--;

            // 타이머 UI 업데이트
            StageTimer.text = StageTimerValue.ToString();
        }

        // 타이머가 0이 되면 isEndBossTimer를 true로 변경
        isEndBossTimer = true;
    }

    public void Init()
    {
        for (int i = 0; i < StageProgressImgArray.Length; i++) 
        {
            StageProgressImgArray[i].SetActive(false);
        }
    }

    public void UpdateStageProgress(int _count)
    {
        for(int i = 0;i < _count; i++) 
        {
            StageProgressImgArray[i].SetActive(true);
        }
    }
}

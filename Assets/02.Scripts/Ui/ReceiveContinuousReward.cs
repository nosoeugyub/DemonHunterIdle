using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 버튼을 길게 누르면 일정 간격으로 보상을 계속 받는 기능 
/// </summary>

public class ReceiveContinuousReward : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    public Button m_Button = null;

    private float p_ReceiveContinuousRewardStartWaitTime = 0.5f; // 보상을 받기 시작하기 전 대기 시간
    private float p_ReceiveContinuousRewardBetweenWaitTime = 0.1f;   // 보상을 받을 때마다의 대기 시간

    private IEnumerator p_Cor = null;

    private void Awake()
    {
        m_Button = this.gameObject.GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_Button.interactable == false) return;

        SoundManager.Instance.PlayAudio("UIClick");

        if (p_Cor != null)
        {
            StopCoroutine(p_Cor);
        }
        p_Cor = ReceiveReward_Cor();
        StartCoroutine(p_Cor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (p_Cor != null)
        {
            StopCoroutine(p_Cor);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (p_Cor != null)
        {
            StopCoroutine(p_Cor);
        }
    }

    /// <summary>
    /// 보상을 지속적으로 받기 위한 코루틴
    /// </summary>

    IEnumerator ReceiveReward_Cor()
    {
        float _eTime = 0.0f;
        float _delay = p_ReceiveContinuousRewardStartWaitTime;

        // 처음 대기 시간 동안 기다림
        while (_eTime < 1)
        {
            _eTime += Time.deltaTime / _delay;
            yield return new WaitForEndOfFrame();
        }

        // 버튼 클릭 이벤트 호출
        m_Button.onClick.Invoke();

        // 보상을 지속적으로 받기 위한 무한 루프
        while (true)
        {
            _eTime = 0.0f;
            _delay = p_ReceiveContinuousRewardBetweenWaitTime;

            // 대기 시간 동안 기다림
            while (_eTime < 1)
            {
                _eTime += Time.deltaTime / _delay;
                yield return new WaitForEndOfFrame();
            }
            // 버튼 클릭 이벤트 호출
            m_Button.onClick.Invoke();
        }

    }
}

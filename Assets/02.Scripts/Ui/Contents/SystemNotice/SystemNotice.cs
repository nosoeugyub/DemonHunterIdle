using TMPro;
using DG.Tweening;
using UnityEngine;
using static Utill_Enum;
using System.Collections;

/// <summary>
/// 작성일자   : 2024-05-30
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 시스템 알림 객체                                                    
/// </summary>
public class SystemNotice : MonoBehaviour
{
    [SerializeField]
    private TMP_Text noticeText = null;
    [SerializeField]
    private GameObject backgroundObj = null;
    [Space(20)]
    [SerializeField]
    private float duration = 2f;
    [SerializeField]
    private float speed = 150f;
    [SerializeField]
    private bool isDisableBackground = false;
    [Space(20)]
    [SerializeField]
    private Ease moveEaseType;
    [SerializeField]
    private Ease fadeEaseType;
    [SerializeField]
    private SystemNoticeType noticeType;

    private RectTransform rectTransform = null;
    private CanvasGroup canvasGroup = null;
    private Coroutine CheckCoroutine = null; //닷트윈이 비정상적으로 끝날시를 대비하여 n초후에 꺼주는 코루틴

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 켜고 끄는 대신 위치만 조절
    /// </summary>
    public void ActiveObject(bool isactive)
    {
        DoKill();

        if (isactive)
        {
            canvasGroup.alpha = 1; //안 하면 팝업 안 보임
            if (noticeType == SystemNoticeType.NoBackground)
                this.gameObject.transform.localPosition = new Vector3(0, -300, 0);
            else
                this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            SystemNoticeManager.Instance.ReduceNoticeNum();
            this.gameObject.transform.localPosition = new Vector3(9999, 9999, 9999);
        }

    }
    /// <summary>
    /// 닷트윈 초기화
    /// </summary>
    private void DoKill()
    {
        canvasGroup.DOKill();
        rectTransform.DOKill();
    }

    private IEnumerator CheckActive()
    {
        yield return new WaitForSeconds(duration+1);
        if (gameObject.transform.localPosition != new Vector3(9999, 9999, 9999))
            ActiveObject(false);
    }
    /// <summary>
    /// 시스템 메시지 띄움
    /// </summary>
    public void SetSystemNotice(string alertString)
    {
        noticeText.text = alertString;
        //설정값과 배경 유무가 다른지 확인
        if (backgroundObj.activeInHierarchy == isDisableBackground)
            backgroundObj.SetActive(!isDisableBackground);

        if(CheckCoroutine != null)
            StopCoroutine(CheckCoroutine);
        CheckCoroutine = StartCoroutine(CheckActive());

        //시작하기전 닷트윈 초기화
        DoKill();
        SoundManager.Instance.PlayAudio("SystemNotice");
        Vector3 endPosition = rectTransform.localPosition + new Vector3(0, speed * duration, 0);
        rectTransform.DOLocalMove(endPosition, duration).SetEase(moveEaseType).OnComplete(() =>
        {
            if (CheckCoroutine != null)
                StopCoroutine(CheckCoroutine);
            ActiveObject(false); 
        });

        // 팝업창이 서서히 투명해지는 Tween 설정
        //canvasGroup.DOFade(0, duration).SetEase(fadeEaseType).OnComplete(() => ActiveObject(false));
    }
}
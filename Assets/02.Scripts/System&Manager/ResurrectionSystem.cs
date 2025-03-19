using NSY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// </summary>
/// 작성일자   : 2024-09-09
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 부활 시스템 
/// </summary>
public class ResurrectionSystem : MonoSingleton<ResurrectionSystem>
{
    public ResurrectionView resurrectionView;
    public bool IsResurrection;

    private void Awake()
    {
        GameEventSystem.GameSequence_SendGameEventHandler += ResetPopUp;
    }
    private bool ResetPopUp(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.Start:
                // 부활 팝업창 비활성화
                resurrectionView.gameObject.SetActive(false);
                return true;
        }
        return false;
    }
    // 부활 순서를 시작하는 함수
    public void StartResurrectionSequence()
    {
        if (CheatManager.Instance.DisableResurrection == true) //치트
        {
            // 특정 사운드 재생
            SoundManager.Instance.PlayAudio("Revival");
            IsResurrection = true;
            return;
        }

        int timeLeft = GameDataTable.Instance.User.UserResurrectionTime;
        //1.팝업창 띄우고
        resurrectionView.gameObject.SetActive(true);
        resurrectionView.UpdatePopup(timeLeft);
        //2.부활 버튼 비활성화
        resurrectionView.resurrectionButton.gameObject.SetActive(false);
        resurrectionView.ShowTimeObj();//시간 초 텍스트 표시

        StopCoroutine(CountdownToResurrection(timeLeft));
        StartCoroutine(CountdownToResurrection(timeLeft));

    }

    // 카운트다운 코루틴
    private IEnumerator CountdownToResurrection(int timeLeft)
    {
        resurrectionView.resurrectionButton.gameObject.SetActive(true);
        resurrectionView.resurrectionButton.interactable = false;
        //버튼 비활성화  + 클리안됨
        resurrectionView.ResurrenctionBtn.SetTypeButton(Utill_Enum.ButtonType.DeActive);
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeLeft--;
            resurrectionView.UpdatePopup(timeLeft);
            if (timeLeft == 0)
            {
                //2.부활 버튼 활성화
                resurrectionView.HideTimeObj();//시간 초 텍스트 없어짐
                resurrectionView.resurrectionButton.interactable = true;
                //버튼 비활성화  + 클리안됨
                resurrectionView.ResurrenctionBtn.SetTypeButton(Utill_Enum.ButtonType.Active);
                yield break;
            }
        }
    }

    // 부활 버튼을 눌렀을 때 실행되는 함수
    public void ResurrectHunters()
    {
        // 특정 사운드 재생
        SoundManager.Instance.PlayAudio("Revival");
        IsResurrection = true;

        //1.팝업창 지우고
        resurrectionView.gameObject.SetActive(false);

        //부활시간 증가
        User.Set_ResurrectionTime(GameDataTable.Instance.User);

    }


}

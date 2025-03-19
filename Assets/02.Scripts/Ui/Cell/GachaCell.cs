using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-09-11
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 가챠 팝업의 셀
/// </summary>
public class GachaCell : MonoBehaviour
{
    [SerializeField]
    private Image backImg;
    [SerializeField]
    private Image itemImg;
    [SerializeField]
    private TextMeshProUGUI nameTxt;

    // 이펙트 이미지 관련
    [SerializeField]
    private List<Material> arrRarityMat = new();
    [SerializeField]
    private Image effectImg;

    // 카드 뒤집히는 애니메이션 관련
    [SerializeField]
    private GameObject frontObj;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private bool isFront = false;

    public bool isFinalized = false; //생성 완료 (true시 마지막 슬롯가 아니면 다음 슬롯 결과를 보여줌)
    public bool isFlipDone = false; //카드가 완전히 돌아감

    public EquipmentType equipmentType;
    private ItemGachaData data = null;

    public void ResetCell(EquipmentType curPart)
    {
        equipmentType = curPart;
        ReSetRotation();
        isFinalized = false;
        isFlipDone = false;
        isFront = false;
        itemImg.sprite = null;
        effectImg.material = null;
        nameTxt.text = string.Empty;
        effectImg.gameObject.SetActive(false);
        data = null;
        itemImg.gameObject.SetActive(false);
    }

    public void Flip(ItemGachaData data)
    {
        this.data = data;
        StartRotation();
        StartCoroutine(Filp());
    }
    private void SetImage()
    {
        itemImg.gameObject.SetActive(true);

        string spriteName = string.Empty;

        if (data.IsResource)
            spriteName = data.RewardName;
        else
            spriteName = data.ItemRewardFullName;
            
        itemImg.sprite = Utill_Standard.GetItemSprite(spriteName);
    }
    private void SetUI()
    {
        if (data.IsResource)
            nameTxt.text = $"{LocalizationTable.Localization("Common_" + data.RewardName)} \n {data.Count}";
        else
            nameTxt.text = $"{LocalizationTable.Localization("Grade_" + data.RewardName)}\n {data.Count}";
    }
    private IEnumerator Filp()
    {
        float delay = (data.FrameEffect != "") ? GachaManager.Instance.SpecialDelaySec : GachaManager.Instance.DealySec;
        SoundManager.Instance.PlayAudio("CardFlip");
        yield return new WaitForSeconds(delay);
        isFinalized = true;
        if (data.FrameEffect != "")
        {
            effectImg.gameObject.SetActive(true);
            effectImg.material = arrRarityMat.Find(x =>  x.name == data.FrameEffect);
        }
        else
            effectImg.gameObject.SetActive(false);
    }

    #region 카드 뒤집히는 애니메이션
    private void ReSetRotation()
    {
        isFront = false;
        isFlipDone = false;
        frontObj.transform.localRotation = Quaternion.Euler(0, 180, 0);
        backImg.transform.localRotation = Quaternion.identity;
        frontObj.SetActive(false);
        backImg.gameObject.SetActive(true);
    }
    // 회전을 시작하는 함수
    private void StartRotation()
    {
        // 시작 회전 상태 저장
        startRotation = frontObj.gameObject.transform.localRotation;
        endRotation = Quaternion.Euler(frontObj.gameObject.transform.localEulerAngles.x, frontObj.gameObject.transform.localEulerAngles.y + 180, frontObj.gameObject.transform.localEulerAngles.z);

        isFront = !isFront;
        // Coroutine을 시작하여 회전 처리
        StartCoroutine(RotateOverTime(GachaManager.Instance.FlipCardSec, isFront));
    }

    private IEnumerator RotateOverTime(float duration, bool isFront)
    {
        float elapsedTime = 0.0f;
        bool middlePointReached = false;
        isFlipDone = false;

        while (elapsedTime < duration)
        {
            // 경과 시간을 증가시킴
            elapsedTime += Time.deltaTime;
            // 회전 진행 상태 계산
            float t = elapsedTime / duration;
            // 회전 상태 보간 (Lerp)
            frontObj.gameObject.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            backImg.gameObject.transform.localRotation = Quaternion.Lerp(endRotation, startRotation, t);

            //중간지점에서 이미지 바꿔줌
            if (!middlePointReached && elapsedTime >= duration / 2)
            {
                middlePointReached = true;

                if (isFront)
                {
                    backImg.gameObject.SetActive(false);
                    frontObj.gameObject.SetActive(true);
                    SetImage();
                }
                else
                {
                    frontObj.gameObject.SetActive(false);
                    backImg.gameObject.SetActive(true);
                }
            }
            // 다음 프레임까지 대기
            yield return null;
        }
        // 최종 회전 상태를 보장
        frontObj.gameObject.transform.localRotation = endRotation;
        backImg.gameObject.transform.localRotation = startRotation;

        //이름 다 뒤집히고 난 후에 나오도록
        SetUI();
        yield return new WaitForSeconds(0.2f);
        isFlipDone = true;
        yield break;
    }
    #endregion
}

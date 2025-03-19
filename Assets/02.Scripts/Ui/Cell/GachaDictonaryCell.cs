using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-09-12
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 가챠 업적 팝업의 셀
/// </summary>
public class GachaDictonaryCell : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI probText;
    [SerializeField]
    private TextMeshProUGUI itemNameText;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Image resourceIcon;

    private ItemGachaData gachaData;
    public void Setting(ItemGachaData itemGachaData)
    {
        gachaData = itemGachaData;
        object tmpResourceType = Resource_Type.None;
        if (gachaData.IsResource) //재화류라면
        {
            backgroundImage.sprite = Utill_Standard.GetItemSprite("None");
            resourceIcon.sprite = Utill_Standard.GetItemSprite(gachaData.RewardName);
        }
        else
        {
            backgroundImage.sprite = Utill_Standard.GetItemSprite(gachaData.RewardName);
            resourceIcon.sprite = Utill_Standard.GetItemSprite(gachaData.ItemRewardFullName);

        }
        probText.text = $"{gachaData.Prob}%";
        if (gachaData.IsResource) //재화류라면
            itemNameText.text = $"{LocalizationTable.Localization("Common_" + gachaData.RewardName)}\n{gachaData.Count}{LocalizationTable.Localization("Count")}";
        else
            itemNameText.text = $"{LocalizationTable.Localization("Grade_" + gachaData.RewardName)}\n{gachaData.Count}{LocalizationTable.Localization("Count")}";
    }
}

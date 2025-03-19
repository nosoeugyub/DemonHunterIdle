using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 데이터 관련 컨트롤러
/// /// </summary>.
public class ItemDrawerTableData 
{
    public int index;
    public Utill_Enum.DrawerGrade DrawerGrade;
    public int DrawerProb;
    public Utill_Enum.Resource_Type DrawerResourceType;
    public int DrawerResourceCount;
    public int[] DrawerEquipmentLevelRange;
    public int EvolutionProb;
    public Utill_Enum.Resource_Type EvolutionResourceType;
    public int EvolutionResourceCount;
    public int EvolutionReqCurrentEquipmentLvel;
    public int EvolutionReqTotalEquipmentLevl;
    public string EvolutionReqTotalEquipmentGrade;



    //유저가 가진 부위 제외한 장비 진화조건
    public static (bool isCurrentEquipmentlvel , bool isTotalEquipmentLevl , bool isTotalEquipmentGrade) CheckEvolutionDrawer(List<HunteritemData> datalist, HunteritemData data)
    {
        bool _isCurrentEquipmentlvel = false;
        bool _isTotalEquipmentLevl = false;
        bool _isTotalEquipmentGrade = false;
        List<HunteritemData> equipmentlist = new List<HunteritemData>();

        //현재 모루등급데이터가져오기
        ItemDrawerTableData Drawerdata = GameDataTable.Instance.ItemDrawerGradeDic[data.DrawerGrade];

        for (int i = 0; i < datalist.Count; i++) //장비타입 리스트 모으기
        {
            if (datalist[i].ItemContainsType == 0)// 모루 추출만 선택
            {
                equipmentlist.Add(datalist[i]);
            }
        }

        //조건 1 해당 부위 장비의 레벨이 EvolutionReqCurrentEquipmentLevel 변수보다 같거나 크던지
        if (data.TotalLevel >= Drawerdata.EvolutionReqCurrentEquipmentLvel)
        {
            _isCurrentEquipmentlvel = true;
        }


        //조건2 현재 장비를 제외한 나머지 부위의 장비래벨이 EvolutionReqTotalEquipmentLevel 변수를 같거나 넘었는지
        for (int i = 0; i < equipmentlist.Count; i++)
        {
            if (equipmentlist[i].Part != data.Part && equipmentlist[i].TotalLevel >= Drawerdata.EvolutionReqTotalEquipmentLevl)
            {
                _isTotalEquipmentLevl = true;
            }
            else
            {
                _isTotalEquipmentLevl = false;
            }
        }

        //조건3 현재 장비를 제외한 나머지 장비등급이 EvolutionReqTotalEquipmentGrade 같거나 넘었는지
        for (int i = 0; i < equipmentlist.Count; i++)
        {
            int currentgrade = (int)(equipmentlist[i].ItemGrade);
            Utill_Enum.DrawerGrade needgrade =CSVReader.ParseEnum< Utill_Enum.DrawerGrade>(Drawerdata.EvolutionReqTotalEquipmentGrade);
            int intneedgrade  = (int)(needgrade);
            if (equipmentlist[i].Part != data.Part && currentgrade >= intneedgrade)
            {
                _isTotalEquipmentGrade = true;
            }
            else
            {
                _isTotalEquipmentGrade = false;
            }
        }
        return (_isCurrentEquipmentlvel, _isTotalEquipmentLevl, _isTotalEquipmentGrade);
    }
}

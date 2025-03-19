using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 장비/커마 요소의  정보
/// </summary>
[Serializable]
public class CostumeInfo
{
    public string CostumeName; //코스튬을 불러오는데 사용할 이름
    public EquipmentType CostumeType;
    public Grade CostumeSetType;
    public Mesh[] Mesh;
    public Material[] Material;
    public CostumeEffect[] costumeEffect; //착용시 이펙트 (없으면 비워놔도 무방)
    [HideInInspector]
    public int curEquipMaterial = -1; //현재 착용하고 있는 메테리얼 (미착용시 -1)
}

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터의 코스튬들의 정보를 담은 SO
/// </summary>
[CreateAssetMenu(fileName ="Hunter_CostumeSO",menuName = "SO/HunterCostumeSO")]
public class HunterCostumeSO : ScriptableObject
{
    public List<CostumeInfo> WeaponCostumes;
    public List<CostumeInfo> BackCostumes;
    public List<CostumeInfo> CloakCostumes;
    public List<CostumeInfo> ClothCostumes;
    public List<CostumeInfo> GlovesCostumes;
    public List<CostumeInfo> ShoesCostumes;
    public List<CostumeInfo> NecklaceCostumes;
    public List<CostumeInfo> EaringsCostumes;
    public List<CostumeInfo> LeftBraceletCostumes;
    public List<CostumeInfo> RightBraceletCostumes;
    public List<CostumeInfo> LeftRingCostumes;
    public List<CostumeInfo> RightRingCostumes;
    public List<CostumeInfo> PetCostumes;
    public List<CostumeInfo> HatCostumes;
    public List<CostumeInfo> WingCostumes;
    public List<CostumeInfo> MaskCostumes;
    public List<CostumeInfo> HairCostumes;

    private Dictionary<string, CostumeInfo> costumeInfoDic = new Dictionary<string, CostumeInfo>();

    #region 딕셔너리에 costume info 삽입
    public void Init()
    {
        costumeInfoDic.Clear();
        for(int i = 0; i<WeaponCostumes.Count; i++)
        {
            costumeInfoDic.Add(WeaponCostumes[i].CostumeName, WeaponCostumes[i]);
        }
        for(int i = 0; i< BackCostumes.Count; i++)
        {
            costumeInfoDic.Add(BackCostumes[i].CostumeName, BackCostumes[i]);
        }
        for(int i = 0; i< CloakCostumes.Count; i++)
        {
            costumeInfoDic.Add(CloakCostumes[i].CostumeName, CloakCostumes[i]);
        }
        for(int i = 0; i< ClothCostumes.Count; i++)
        {
            costumeInfoDic.Add(ClothCostumes[i].CostumeName, ClothCostumes[i]);
        }
        for(int i = 0; i< GlovesCostumes.Count; i++)
        {
            costumeInfoDic.Add(GlovesCostumes[i].CostumeName, GlovesCostumes[i]);
        }
        for(int i = 0; i< ShoesCostumes.Count; i++)
        {
            costumeInfoDic.Add(ShoesCostumes[i].CostumeName, ShoesCostumes[i]);
        }
        for(int i = 0; i< NecklaceCostumes.Count; i++)
        {
            costumeInfoDic.Add(NecklaceCostumes[i].CostumeName, NecklaceCostumes[i]);
        }
        for(int i = 0; i< EaringsCostumes.Count; i++)
        {
            costumeInfoDic.Add(EaringsCostumes[i].CostumeName, EaringsCostumes[i]);
        }
        for(int i = 0; i< LeftBraceletCostumes.Count; i++)
        {
            costumeInfoDic.Add(LeftBraceletCostumes[i].CostumeName, LeftBraceletCostumes[i]);
        }
        for(int i = 0; i< RightBraceletCostumes.Count; i++)
        {
            costumeInfoDic.Add(RightBraceletCostumes[i].CostumeName, RightBraceletCostumes[i]);
        }
        for(int i = 0; i< LeftRingCostumes.Count; i++)
        {
            costumeInfoDic.Add(LeftRingCostumes[i].CostumeName, LeftRingCostumes[i]);
        }
        for(int i = 0; i< RightRingCostumes.Count; i++)
        {
            costumeInfoDic.Add(RightRingCostumes[i].CostumeName, RightRingCostumes[i]);
        }
        for(int i = 0; i< PetCostumes.Count; i++)
        {
            costumeInfoDic.Add(PetCostumes[i].CostumeName, PetCostumes[i]);
        }
        for(int i = 0; i< HatCostumes.Count; i++)
        {
            costumeInfoDic.Add(HatCostumes[i].CostumeName, HatCostumes[i]);
        }
        for(int i = 0; i< WingCostumes.Count; i++)
        {
            costumeInfoDic.Add(WingCostumes[i].CostumeName, WingCostumes[i]);
        }
        for(int i = 0; i< MaskCostumes.Count; i++)
        {
            costumeInfoDic.Add(MaskCostumes[i].CostumeName, MaskCostumes[i]);
        }
        for(int i = 0; i< HairCostumes.Count; i++)
        {
            costumeInfoDic.Add(HairCostumes[i].CostumeName, HairCostumes[i]);
        }
    }
    #endregion

    public CostumeInfo GetCostumeInfo(string costumeName)
    {
        return costumeInfoDic[costumeName];
    }
}

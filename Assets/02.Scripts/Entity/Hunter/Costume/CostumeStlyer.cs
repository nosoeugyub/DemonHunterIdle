using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-06-11
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 스킨드 메시 렌더러 정보 저장                                          
/// </summary>
public class SkinnedMeshRendererInfo
{
    public EquipmentType Type;
    public Mesh Mesh;
    public Transform[] bones;
    public Transform rootBone;
    public SkinnedMeshRenderer renderer;

    public SkinnedMeshRendererInfo(EquipmentType type,Mesh mesh, Transform[] bones, Transform rootBone, SkinnedMeshRenderer renderer)
    {
        this.Type = type;
        this.Mesh = mesh;
        this.bones = bones;
        this.rootBone = rootBone;
        this.renderer = renderer;
    }
}

/// <summary>
/// 작성일자   : 2024-06-11
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 헌터의 장비/커마에 따른 모델링 관리
/// </summary>
public class CostumeStlyer : MonoBehaviour
{
    //모든 CostumeStlyer 인스턴스의 리스트
    public static List<CostumeStlyer> CostumeStlyerList { get; private set; }
    [SerializeField]
    private Transform rootTransform;
    [SerializeField]
    private HunterCostumeSO costumeSO; //모든 코스튬의 so
    [Header("테스트로 장착할 코스튬의 이름")]
    [SerializeField]
    private string testCostumeName;

    public SubClass subClass;

    private Dictionary<EquipmentType, List<SkinnedMeshRendererInfo>> skinnedMeshRendererInfos = new Dictionary<EquipmentType, List<SkinnedMeshRendererInfo>>();
    private Dictionary<EquipmentType, CostumeInfo> costumeInfos = new Dictionary<EquipmentType, CostumeInfo>();
    //장비마다 가지고있는 이펙트를 저장하는 딕셔너리
    private Dictionary<string, CostumeEffect> costumeEffects = new Dictionary<string, CostumeEffect>();

    private bool isInit = false;
    private bool isWaitForSkinnedMeshSetting = false;
    private void Awake()
    {
        GameEventSystem.GameSequence_SendGameEventHandler += GameSequence;
    }
    
    private bool GameSequence(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch(GameSequence) 
        {
            case Enum_GameSequence.Start:
                // 스킨드 메시 설정을 초기화하고 인벤토리 데이터를 기반으로 코스튬 스타일을 설정
                isInit = false;
                skinnedMeshRendererInfos.Clear();
                costumeInfos.Clear();
                costumeEffects.Clear();

                Init();
                SkinnedmeshSetting();
                switch (subClass)
                {
                    case SubClass.Archer:
                        InitCostumeStyle(subClass,GameDataTable.Instance.HunterItem.Archer);
                        break;
                    case SubClass.Guardian:
                        InitCostumeStyle(subClass, GameDataTable.Instance.HunterItem.Guardian);
                        break;
                    case SubClass.Mage:
                        InitCostumeStyle(subClass, GameDataTable.Instance.HunterItem.Mage);
                        break;
                }
                return true;
        }
        return true;
    }

    public void Init()
    {
        if (CostumeStlyerList == null)
            CostumeStlyerList = new();
        if(!CostumeStlyerList.Contains(this))
            CostumeStlyerList.Add(this);
        costumeSO.Init();
    }
    public void SetTestCostume()
    {
        EquipmentType equipmentType = costumeSO.GetCostumeInfo(testCostumeName).CostumeType;
        RemovePartOfCostume(equipmentType);
        SetPartOfCostume(testCostumeName);
    }

    private void Update()
    {
        if(isWaitForSkinnedMeshSetting)
        {
            if(rootTransform != null)
            {
                SkinnedmeshSetting();
                isWaitForSkinnedMeshSetting = false;
            }
        }
    }

    /// <summary>
    ///  자식 오브젝트를 순회하며 스킨드 메시 설정 초기화
    /// </summary>
    public void SkinnedmeshSetting()
    {
        if(isInit) return;
        if (rootTransform == null)
        {
            isWaitForSkinnedMeshSetting = true;
            return;
        }
        for (int i = 0; i< rootTransform.childCount; i++)
        {
            GameObject child = rootTransform.GetChild(i).gameObject;
            SkinnedMeshRenderer sk = child.GetComponent<SkinnedMeshRenderer>();
            if (sk == null) continue;

            // 자식 오브젝트 이름에서 장비 유형을 추출
            EquipmentType type = Utill_Standard.StringToEnum<EquipmentType>(child.name.Split('_')[1]);

            SkinnedMeshRendererInfo info = new SkinnedMeshRendererInfo
            (
                type,
                sk.sharedMesh,
                (Transform[])sk.bones.Clone(),
                sk.rootBone,
                sk
            );
            if (skinnedMeshRendererInfos.ContainsKey(type))
                skinnedMeshRendererInfos[type].Add(info);
            else
                skinnedMeshRendererInfos.Add(type, new List<SkinnedMeshRendererInfo> { info });
        }
    }
    /// <summary>
    /// CostumeInfo를 사용하여 스킨드 메시 렌더러 설정
    /// </summary>
    private void SetSkinnedMeshWithInfo(CostumeInfo costumeInfo)
    {
        List<SkinnedMeshRendererInfo> skinnedMeshRenderers = skinnedMeshRendererInfos[costumeInfo.CostumeType];
        for(int i = 0;i< skinnedMeshRenderers.Count; i++)
        {
            SkinnedMeshRenderer sk = skinnedMeshRenderers[i].renderer;
            if (costumeInfo.Mesh.Length <= i)
            {
                sk.enabled = true;
                sk.sharedMesh = null;
                sk.sharedMaterials = new Material[0];
                continue;
            }
            
            sk.enabled = true;
            sk.sharedMesh = costumeInfo.Mesh[i];
            //sk.sharedMaterials = costumeInfo.Material;
            sk.sharedMaterials = new Material[1] { costumeInfo.Material[costumeInfo.curEquipMaterial]};
        }
    }

    /// <summary>
    /// 특정 장비 유형과 이름을 사용하여 코스튬의 일부를 설정
    /// </summary>
    /// <param name="costumeName">장비 이름</param>
    /// <param name="materialIndex">장비의 메테리얼 인덱스 (HunterCostumeSO참고)</param>
    public void SetPartOfCostume(string costumeName, int materialIndex = 0)
    {
        if (costumeName.Contains(subClass.ToString()) == false) return; //자신의 아이템이 아니라면 return

        CostumeInfo info = costumeSO.GetCostumeInfo(costumeName);
        if (info.Material.Length <= materialIndex) //요구한 index가 info에 없다면
            Game.Debbug.Debbuger.ErrorDebug("해당 index의 메테리얼을 찾을 수 없습니다");
         
        info.curEquipMaterial = materialIndex;
        SetSkinnedMeshWithInfo(info);

        if(info.costumeEffect != null) //이펙트를 가진 장비라면
        {
            if (costumeEffects.ContainsKey(costumeName))
            {
                for (int i = 0; i < info.costumeEffect.Length; i++)
                {
                    costumeEffects[costumeName].Setting(skinnedMeshRendererInfos[info.CostumeType][0].rootBone);
                    costumeEffects[costumeName].gameObject.SetActive(true);
                }
            }
            else
            {
                for(int i = 0; i < info.costumeEffect.Length; i++)
                {
                    CostumeEffect costumeEffect = Instantiate(info.costumeEffect[i]);
                    costumeEffect.Setting(skinnedMeshRendererInfos[info.CostumeType][0].rootBone);
                    costumeEffects.Add(costumeName, costumeEffect);
                }
            }
        }

        if(costumeInfos.ContainsKey(info.CostumeType)) //costumeInfo 저장
            costumeInfos[info.CostumeType] = info;
        else
            costumeInfos.Add(info.CostumeType, info);
    }
    /// <summary>
    /// 특정 장비 유형의 코스튬을 제거
    /// </summary>
    public void RemovePartOfCostume(EquipmentType costumeType)
    {
        List<SkinnedMeshRendererInfo> skinnedMeshRenderers = new();
        if (!skinnedMeshRendererInfos.ContainsKey(costumeType))
            return;
        
        skinnedMeshRenderers = skinnedMeshRendererInfos[costumeType];
        for (int i = 0; i < skinnedMeshRenderers.Count; i++)
        { 
            SkinnedMeshRenderer sk = skinnedMeshRenderers[i].renderer;
            sk.enabled = false;
        }

        if (costumeInfos.ContainsKey(costumeType) == false) //costumeInfo 세팅 없이 끄는 경우 후처리 불푤요
        {
            return;
        }

        string costumeName = costumeInfos[costumeType].CostumeName;
        //이팩트를 가진 장비이고 이펙트가 생성되어 있다면
        if (costumeInfos[costumeType].costumeEffect != null && costumeEffects.ContainsKey(costumeName))
        {
            //이펙트 비활성화
            costumeEffects[costumeName].gameObject.SetActive(false);
        }

        //장착 정보 삭제
        costumeInfos[costumeType].curEquipMaterial = -1;
        costumeInfos.Remove(costumeType);

    }

    /// <summary>
    /// 특정 코스튬의 메테리얼 변경
    /// </summary>
    /// <param name="costumeType">메테리얼을 변경할 코스튬의 타입</param>
    /// <param name="materialIndex">변경할 메테리얼의 종류 (HunterCostumeSO참고)</param>
    public void SetMaterialOfCostume(EquipmentType costumeType,int materialIndex)
    {
        if (!skinnedMeshRendererInfos.ContainsKey(costumeType))
            return;
        List<SkinnedMeshRendererInfo> skinnedMeshRenderers = new();

        skinnedMeshRenderers = skinnedMeshRendererInfos[costumeType];

        for (int i = 0; i < skinnedMeshRenderers.Count; i++)
        {
            SkinnedMeshRenderer sk = skinnedMeshRenderers[i].renderer;
            sk.sharedMaterials = new Material[1] { costumeInfos[costumeType].Material[materialIndex] };
        }
    }

    /// <summary>
    /// 모든 코스튬을 제거
    /// </summary>
    public void RemoveAllCostume(bool isRemoveBody = true)
    {
        foreach(var renderer in skinnedMeshRendererInfos)
        {
            if (!isRemoveBody && (renderer.Key == EquipmentType.Hair || renderer.Key == EquipmentType.Body))
                continue;
            RemovePartOfCostume(renderer.Key);
        }
    }

    public int GetMaterialIndex(EquipmentType equipmentType)
    {
        return costumeInfos[equipmentType].curEquipMaterial;
    }

    #region 실 장착 코드

    /// <summary>
    /// HunteritemData를 기반으로 해당 서브 클래스의 코스튬 스타일러를 전부 설정함
    /// </summary>
    public static void InitCostumeStyle(SubClass subClass, List<HunteritemData> equippedItem)
    {
        for(int i = 0; i< CostumeStlyerList.Count; i++)
        {
            if(CostumeStlyerList[i] == null) continue;

            if (CostumeStlyerList[i].subClass == subClass)
            {
                if (CostumeStlyerList[i].isInit) return;
                CostumeStlyerList[i].isInit = true;

                for (int j = 1; j <= (int)EquipmentType.Body; j++)
                {
                    if ((EquipmentType)j == EquipmentType.Hair || (EquipmentType)j == EquipmentType.Body)
                        continue;
                    HunteritemData itemData = equippedItem.Find(x => x.Part == (EquipmentType)j);

                    if (itemData != null && itemData.Name != string.Empty && itemData.isEquip) //장착한게 있다면
                    {
                        CostumeStlyerList[i].SetPartOfCostume(itemData.Name);
                    }
                    else
                        CostumeStlyerList[i].RemovePartOfCostume((EquipmentType)j);
                }
            }
        }
    }

    /// <summary>
    /// 해당 서브 클래스의 코스튬을 전부 제거함
    /// </summary>
    public static void RemoveAllCostumeStyle(SubClass subClass)
    {
        for (int i = 0; i < CostumeStlyerList.Count; i++)
        {
            if (CostumeStlyerList[i].subClass == subClass)
            {
                CostumeStlyerList[i].RemoveAllCostume();
            }
        }
    }

    /// <summary>
    /// HunteritemData를 기반으로 해당 서브 클래스의 특정 코스튬을 변경함
    /// HunteritemData의 EquippedItemName이 비어있다면 코스튬을 제거함
    /// </summary>
    public static void ChangePartOfCostumeStyle(SubClass subClass, HunteritemData changedItem)
    {
        for (int i = 0; i < CostumeStlyerList.Count; i++)
        {
            if (CostumeStlyerList[i].subClass == subClass)
            {
                if (changedItem.Name != "")
                    CostumeStlyerList[i].SetPartOfCostume(changedItem.Name);
                else
                    CostumeStlyerList[i].RemovePartOfCostume(changedItem.Part);
            }
        }
    }

    /// <summary>
    /// 해당 서브 클래스의 특정 코스튬 파츠를 삭제함
    /// </summary>
    public static void RemovePartOfCostumeStyle(SubClass subClass, EquipmentType equipmentType)
    {
        for (int i = 0; i < CostumeStlyerList.Count; i++)
        {
            if (CostumeStlyerList[i].subClass == subClass)
            {
                CostumeStlyerList[i].RemovePartOfCostume(equipmentType);
            }
        }
    }


    #region 이전 코드
    ///// <summary>
    ///// 인벤토리 데이터를 기반으로 코스튬 스타일을 설정
    ///// </summary>
    //public void SetCostumeStyle(Dictionary<EquipmentType, Item> equippedItem)
    //{
    //    if (isInit) return;
    //    isInit = true;
    //    for (int i = 1; i < skinnedMeshRendererInfos.Count;i++)
    //    {
    //        if ((EquipmentType)i == EquipmentType.Hair || (EquipmentType)i == EquipmentType.Body)
    //            continue;
    //        if(equippedItem.ContainsKey((EquipmentType)i))
    //        {
    //            SetPartOfCostume(equippedItem[(EquipmentType)i].GetName);
    //        }
    //        else
    //            RemovePartOfCostume((EquipmentType)i);
    //    }
    //}
    ///// <summary>
    ///// 인벤토리 데이터를 기반으로 코스튬 스타일을 설정
    ///// </summary>
    public void SetCostumeStyle(List<Item> equippedItem)
    {
        if (isInit) return;
        isInit = true;

        Dictionary<EquipmentType, Item> equippedItemDic = new Dictionary<EquipmentType, Item>();
        foreach (var item in equippedItem)
        {
            var equipmentItem = item.equipmentItem;
            if (equipmentItem.isEquip == false) continue;

            equippedItemDic[equipmentItem.type] = item;
        }

        for (int i = 1; i <= (int)EquipmentType.Body; i++)
        {
            if ((EquipmentType)i == EquipmentType.Hair || (EquipmentType)i == EquipmentType.Body)
                continue;
            if (equippedItemDic.ContainsKey((EquipmentType)i))
            {
                SetPartOfCostume(equippedItemDic[(EquipmentType)i].GetName);
            }
            else
                RemovePartOfCostume((EquipmentType)i);
        }
    }
    #endregion
    #endregion
}

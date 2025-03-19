using UnityEngine;
using UnityEditor;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : CostumeStlyer의 기능으로 테스트 장착을 구현한 에디터
/// </summary>
[CustomEditor(typeof(CostumeStlyer)),CanEditMultipleObjects]
public class CostumeStlyerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); 
        CostumeStlyer stlyer = (CostumeStlyer)target;
        if (GUILayout.Button("Equip Test Costume")) 
        { 
            stlyer.SetTestCostume();
        }
        if (GUILayout.Button("Remove Test Costume"))
        {
            stlyer.RemoveAllCostume(false);
            //stlyer.SetCostumeStyle(GameDataTable.Instance.InventoryList[(int)stlyer.subClass]);
        }
    }
}

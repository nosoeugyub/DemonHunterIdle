using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-07-25
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 베이스 스크립트의 에디터
/// </summary>
[CustomEditor(typeof(BaseSkill),true)]
public class BaseSkillEditor : Editor
{
    //1
    SerializedProperty skillCastingPositionTypeProp;
    SerializedProperty enemyCheckRadiusProp;
    SerializedProperty posCamXProp;
    SerializedProperty posCamYProp;
    SerializedProperty posCamZProp;
    SerializedProperty AreaXscale;
    SerializedProperty AreaZscale;
    //2
    SerializedProperty SkillRangeProp;
    SerializedProperty IsHunterForwardProp;
    //3
    SerializedProperty SkillRadiusProp;
    //4
    SerializedProperty FindUnitNameProp;

    private void OnEnable()
    {
        // SerializedProperty 객체를 캐싱하여 성능을 향상시킵니다.
        skillCastingPositionTypeProp = serializedObject.FindProperty("_SkillCastingPositionType");
        enemyCheckRadiusProp = serializedObject.FindProperty("EnemyCheckRadius");
        posCamXProp = serializedObject.FindProperty("PosCamX");
        posCamYProp = serializedObject.FindProperty("PosCamY");
        posCamZProp = serializedObject.FindProperty("PosCamZ");

        AreaXscale = serializedObject.FindProperty("AreaXscale");
        AreaZscale = serializedObject.FindProperty("AreaZscale");

        SkillRangeProp = serializedObject.FindProperty("skillDiameter");
        IsHunterForwardProp = serializedObject.FindProperty("TargetDirection");

        SkillRadiusProp = serializedObject.FindProperty("skillRange");

        FindUnitNameProp = serializedObject.FindProperty("FindUnitName");
    }

    public override void OnInspectorGUI()
    {
        // serializedObject 업데이트
        serializedObject.Update();

        // 기본 인스펙터 그리기 (헤더와 기본 필드들)에서 _SkillCastingPositionType 제외
        DrawPropertiesExcluding(serializedObject, new string[] { "_SkillCastingPositionType", "EnemyCheckRadius", "PosCamX", "PosCamY", "PosCamZ", "AreaXscale", "AreaZscale", "skillDistance", "TargetDirection", "skillRange" });

        // 헤더랑 스페이스 추가
        EditorGUILayout.Space(40);
        EditorGUILayout.LabelField("--------------에디터변수--------------", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(enemyCheckRadiusProp);
        // _SkillCastingPositionType 필드 그리기
        EditorGUILayout.PropertyField(skillCastingPositionTypeProp);

        // _SkillCastingPositionType에 따라 조건부 필드 표시
        switch ((Utill_Enum.SkillCastingPositionType)skillCastingPositionTypeProp.enumValueIndex)
        {
            case Utill_Enum.SkillCastingPositionType.RelativeToCamera:
                EditorGUILayout.PropertyField(posCamXProp, new GUIContent("PosCamX"));
                EditorGUILayout.PropertyField(posCamYProp, new GUIContent("PosCamY"));
                EditorGUILayout.PropertyField(posCamZProp, new GUIContent("PosCamZ"));

                EditorGUILayout.PropertyField(AreaXscale, new GUIContent("AreaXscale"));
                EditorGUILayout.PropertyField(AreaZscale, new GUIContent("AreaZscale"));
                break;
            case Utill_Enum.SkillCastingPositionType.SkillDiameterToHunter:
                EditorGUILayout.PropertyField(SkillRangeProp, new GUIContent("SkillDiameter"));
                EditorGUILayout.PropertyField(IsHunterForwardProp, new GUIContent("TargetDirection"));
                break;
            case Utill_Enum.SkillCastingPositionType.SkillRangeToHunter:
                EditorGUILayout.PropertyField(SkillRadiusProp, new GUIContent("SkillRange"));
                break;
            case Utill_Enum.SkillCastingPositionType.None:
                break;
            case Utill_Enum.SkillCastingPositionType.FindSKillUnitList:
                EditorGUILayout.PropertyField(FindUnitNameProp, new GUIContent("FindUnitName"));
                break;
            default:
                break;
        }

        // 변경 사항 저장
        serializedObject.ApplyModifiedProperties();
    }
}

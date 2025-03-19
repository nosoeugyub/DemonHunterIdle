using UnityEngine;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 레이어 구하기 and 레이어 가져오는 용도
/// </summary>
public static class Layer 
{
    public static readonly string Player = "Player";
    public static readonly string Enemy = "Enemy";
    public static readonly string UI = "UI";
    public static readonly string Boss = "Boss";
    public static readonly string SkillObj = "SkillObj";
    public static readonly string Ground = "Ground";


    public static readonly int UIlayer = 5;
    public static readonly int Playerintlayer = 6;
    public static readonly int EnemyLayer = 7;
    public static readonly int Groundlayer = 8;
    // 레이어 마스크
    public static readonly int UIlayerMask = 1 << UIlayer;
    public static readonly int PlayerLayerMask = 1 << Playerintlayer;
    public static readonly int EnemyLayerMask = 1 << EnemyLayer;
    public static readonly int GroundLayerMask = 1 << Groundlayer;

    // 유틸리티 메서드: 레이어 이름으로 레이어 인덱스 가져오기
    public static int GetLayerByName(string layerName)
    {
        return LayerMask.NameToLayer(layerName);
    }

    // 유틸리티 메서드: 레이어 인덱스로 레이어 이름 가져오기
    public static string GetLayerNameByIndex(int layerIndex)
    {
        return LayerMask.LayerToName(layerIndex);
    }

    // 유틸리티 메서드: 특정 레이어가 포함된 레이어 마스크인지 확인하기
    public static bool IsInLayerMask(int layer, int layerMask)
    {
        return (layerMask & (1 << layer)) != 0;
    }

    // 유틸리티 메서드: 특정 게임 오브젝트가 특정 레이어에 있는지 확인하기
    public static bool IsGameObjectInLayerMask(GameObject obj, int layerMask)
    {
        return IsInLayerMask(obj.layer, layerMask);
    }
    // 새로운 메서드 추가
    public static LayerMask GetLayerMask(string layerName)
    {
        return LayerMask.GetMask(layerName);
    }
}

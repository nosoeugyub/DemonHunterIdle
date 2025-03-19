using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// 작성일자   : 2024-08-29
/// 작성자     : 성엽 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 :                                           
/// </summary>
public class Mail
{
    // 중복된 항목을 포함한 리스트
    public List<KeyValuePair<string, int>> Shop2 = new List<KeyValuePair<string, int>>();
    public List<KeyValuePair<string, int>> Shop1 = new List<KeyValuePair<string, int>>();
    public List<KeyValuePair<string, int>> PurchaseAchievement = new List<KeyValuePair<string, int>>();
    public List<KeyValuePair<string, int>> DailyMission = new List<KeyValuePair<string, int>>();
    public List<KeyValuePair<string, int>> GachaAchievement = new List<KeyValuePair<string, int>>();
    public List<KeyValuePair<string, int>> OfflineReward = new List<KeyValuePair<string, int>>();

    #region Shop1
    public void Init_MailData(List<KeyValuePair<string, int>> _itemNumAndToTal)
    {
        Shop1 = _itemNumAndToTal;
    }
    //itemNumAndToTal의 string key값으로 아이템 얻어내는 코드
    public static void Add_Shop1Data(List<KeyValuePair<string, int>> _data, string key, int value)
    {
        // 존재하지 않는 경우, 새로운 `KeyValuePair`를 추가
        _data.Add(new KeyValuePair<string, int>(key, value));
    }

    public static void Remove_Shop1Data(List<KeyValuePair<string, int>> _data, string key)
    {
        // 해당 키를 가진 첫 번째 KeyValuePair를 찾아서 제거
        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].Key == key)
            {
                _data.RemoveAt(i);
                break; // 첫 번째 항목을 제거한 후 루프 종료
            }
        }
    }
    #endregion

    #region PurchaseAchievement
    public void Init_PurchaseAchievement_MailData(List<KeyValuePair<string, int>> _itemNumAndToTal)
    {
        PurchaseAchievement = _itemNumAndToTal;
    }
    //itemNumAndToTal의 string key값으로 아이템 얻어내는 코드
    public static void Add_PurchaseAchievementData(List<KeyValuePair<string, int>> _data, string key, int value)
    {
        // 존재하지 않는 경우, 새로운 `KeyValuePair`를 추가
        _data.Add(new KeyValuePair<string, int>(key, value));
    }

    public static void Remove_PurchaseAchievementData(List<KeyValuePair<string, int>> _data, string key)
    {
        // 해당 키를 가진 첫 번째 KeyValuePair를 찾아서 제거
        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].Key == key)
            {
                _data.RemoveAt(i);
                break; // 첫 번째 항목을 제거한 후 루프 종료
            }
        }
    }
    #endregion

    #region DailyMission
    public void Init_DailyMission_MailData(List<KeyValuePair<string, int>> _itemNumAndToTal)
    {
        DailyMission = _itemNumAndToTal;
    }
    //itemNumAndToTal의 string key값으로 아이템 얻어내는 코드
    public static void Add_DailyMissionData(List<KeyValuePair<string, int>> _data, string key, int value)
    {
        // 존재하지 않는 경우, 새로운 `KeyValuePair`를 추가
        _data.Add(new KeyValuePair<string, int>(key, value));
    }

    public static void Remove_DailyMissionData(List<KeyValuePair<string, int>> _data, string key)
    {
        // 해당 키를 가진 첫 번째 KeyValuePair를 찾아서 제거
        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].Key == key)
            {
                _data.RemoveAt(i);
                break; // 첫 번째 항목을 제거한 후 루프 종료
            }
        }
    }
    #endregion


    #region GachaAchievement
    public void Init_GachaAchievement_MailData(List<KeyValuePair<string, int>> _itemNumAndToTal)
    {
        GachaAchievement = _itemNumAndToTal;
    }
    //itemNumAndToTal의 string key값으로 아이템 얻어내는 코드
    public static void Add_GachaAchievementData(List<KeyValuePair<string, int>> _data, string key, int value)
    {
        // 존재하지 않는 경우, 새로운 `KeyValuePair`를 추가
        _data.Add(new KeyValuePair<string, int>(key, value));
    }

    public static void Remove_GachaAchievementData(List<KeyValuePair<string, int>> _data, string key)
    {
        // 해당 키를 가진 첫 번째 KeyValuePair를 찾아서 제거
        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].Key == key)
            {
                _data.RemoveAt(i);
                break; // 첫 번째 항목을 제거한 후 루프 종료
            }
        }
    }
    #endregion


    #region OfflineReawerd
    public void Init_OfflineReawerd_MailData(List<KeyValuePair<string, int>> _itemNumAndToTal)
    {
        OfflineReward = _itemNumAndToTal;
    }
    //itemNumAndToTal의 string key값으로 아이템 얻어내는 코드
    public static void Add_OfflineReawerdData(List<KeyValuePair<string, int>> _data, string key, int value)
    {
        // 존재하지 않는 경우, 새로운 `KeyValuePair`를 추가
        _data.Add(new KeyValuePair<string, int>(key, value));
    }

    public static void Remove_OfflineReawerdData(List<KeyValuePair<string, int>> _data, string key)
    {
        // 해당 키를 가진 첫 번째 KeyValuePair를 찾아서 제거
        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].Key == key)
            {
                _data.RemoveAt(i);
                break; // 첫 번째 항목을 제거한 후 루프 종료
            }
        }
    }
    #endregion
}





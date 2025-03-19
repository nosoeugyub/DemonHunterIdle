using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 시간 관련 표기 및 사운드 오브젝트 풀링 및 로그인신에서 사용하는 테이블 로드    
/// </summary>
public static class Utility 
{
    /// <summary>
    /// 딕셔너리에서 특정키 키에 해당하는 값들에서 중괄호 제거 및 키가 없을시 로그 출력
    /// </summary>
    public static void RemoveBrackets(IDictionary<string, string> dict, string[] keys)
    {
        foreach (var key in keys)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = dict[key].Replace("{", "").Replace("}", "");
            }
            else
            {
                Debug.LogWarning($"Not Contain Key {key}.");
            }
        }
    }

    public static IEnumerator Disable(float time, Component component)
    {
        yield return new WaitForSeconds(time);
        component.gameObject.SetActive(false);
    }

    /// <summary>
    /// 사운드 매니저에서 사용하는 오브젝트 풀링
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPooling<T>
      where T : Component
    {
        private readonly T _originalPrefabs;
        private readonly Transform _parent;
        private readonly Vector3 _position;
        private readonly bool _isRectTransform;
        private readonly Queue<T> _objects;

        public ObjectPooling(T prefabs, Transform parent = null)
        {
            _originalPrefabs = prefabs;
            _parent = parent;
            _objects = new Queue<T>();
        }

        public ObjectPooling(T prefabs, Transform parent = null, int initializeCount = 0)
        {
            _originalPrefabs = prefabs;
            _parent = parent;
            _objects = new Queue<T>();

            for (var i = 0; i < initializeCount; i++)
            {
                var obj = Object.Instantiate(_originalPrefabs, _parent);
                _objects.Enqueue(obj);
                obj.gameObject.SetActive(false);
            }
        }

        public ObjectPooling(T prefabs, Vector3 initializePosition, Transform parent = null, int initializeCount = 0, bool isRectTransform = false)
        {
            _originalPrefabs = prefabs;
            _parent = parent;
            _position = initializePosition;
            _isRectTransform = isRectTransform;
            _objects = new Queue<T>();

            for (var i = 0; i < initializeCount; i++)
            {
                var obj = Object.Instantiate(_originalPrefabs, _position, _parent.rotation, _parent);
                if (_isRectTransform)
                    ((RectTransform)obj.transform).anchoredPosition = _position;
                else
                    obj.transform.position = _position;
                _objects.Enqueue(obj);
                obj.gameObject.SetActive(false);
            }
        }

        public T PopObject()
        {
            if (_objects.Count == 0 || _objects.Peek().gameObject.activeSelf)
            {
                var returnObject = Object.Instantiate(_originalPrefabs, _position, _parent.rotation, _parent);
                if (_isRectTransform)
                    ((RectTransform)returnObject.transform).anchoredPosition = _position;
                else
                    returnObject.transform.position = _position;
                _objects.Enqueue(returnObject);
                if (_objects.Count != 0) _objects.Enqueue(_objects.Dequeue());

                return returnObject;
            }

            var returnValue = _objects.Peek();
            _objects.Enqueue(_objects.Dequeue());
            returnValue.gameObject.SetActive(true);

            return returnValue;
        }

        public T PopObjectDeActivate()
        {
            if (_objects.Count == 0 || _objects.Peek().gameObject.activeSelf)
            {
                var returnObject = Object.Instantiate(_originalPrefabs, _position, _parent.rotation, _parent);
                if (_isRectTransform)
                    ((RectTransform)returnObject.transform).anchoredPosition = _position;
                else
                    returnObject.transform.position = _position;
                _objects.Enqueue(returnObject);
                if (_objects.Count != 0) _objects.Enqueue(_objects.Dequeue());

                return returnObject;
            }

            var returnValue = _objects.Peek();
            _objects.Enqueue(_objects.Dequeue());

            return returnValue;
        }
    }

    #region Laod Auto Nickname
    private static List<KeyValuePair<string, string>> autoNicknameList = new List<KeyValuePair<string, string>>();
    private static bool isAutoNickname = false;

    private static HashSet<string> adjectiveList = new HashSet<string>();
    private static HashSet<string> nounList = new HashSet<string>();

    public static void LoadAutoNickname()
    {
        if (isAutoNickname)
            return;

        autoNicknameList = CSVReader.AutoNicknameRead("AutoNicknameList");
        
        foreach (var item in autoNicknameList)
        {
            adjectiveList.Add(item.Key);
            nounList.Add(item.Value);
        }

        isAutoNickname = true;
    }

    public static string GenerateRandomUsername()
    {
        // 랜덤으로 형용사와 명사 선택
        string adjective = GetRandomItem(adjectiveList);
        string noun = GetRandomItem(nounList);

        // 조합하여 아이디 생성
        string username = adjective + noun;
        return username;
    }

    /// <summary>
    /// 명사와 형용사 리스트에서 랜덤으로 하나의 단어 선택
    /// </summary>
    private static string GetRandomItem(HashSet<string> itemList)
    {
        List<string> tempList = new List<string>(itemList);
        int randomIndex = Random.Range(0, tempList.Count);
        return tempList[randomIndex];
    }

    #endregion

    #region Load Nickname Filter
    private static HashSet<string> filterList = new HashSet<string>();
    private static bool isLoadedNicknameFilter = false;

    public static void LoadNicknameFilter()
    {
        if (isLoadedNicknameFilter)
            return;

        var dataList = CSVReader.NicknameFilterList("NicknameFilterList");
        
        foreach (var data in dataList)
        {
            string filter = data.ToString();

            if (filter == string.Empty)
                continue;

            filterList.Add(filter);
        }

        isLoadedNicknameFilter = true;
    }

    public static bool NicknameFiltering(string nickname)
    {
        foreach (var nick in filterList)
        {
#if UNITY_EDITOR
            if (nick == "테스터")
                continue;
#endif
            if (nickname.ToLower().Contains(nick.ToLower()))
                return true;
        }

        return false;
    }
    #endregion

    #region 시간 관련 표기 

    public static string TimeFormatDefault(System.TimeSpan time)
    {
        string returnStr = "";
        if (time.TotalDays >= 1)
        {
            returnStr = string.Format(LocalizationTable.Localization("Time_Day_Format"), (int)time.TotalDays, (int)time.Hours, (int)time.Minutes);
            return returnStr;
        }
        else if (time.TotalHours >= 1)
        {
            returnStr = string.Format(LocalizationTable.Localization("Time_Hour_Format"), (int)time.Hours, (int)time.Minutes);
            return returnStr;
        }
        else if (time.TotalMinutes >= 1)
        {
            returnStr = string.Format(LocalizationTable.Localization("Time_Minute_Format"), (int)time.Minutes);
            return returnStr;
        }
        else if (time.TotalSeconds >= 1)
        {
            returnStr = string.Format(LocalizationTable.Localization("Time_Second_Format"), (int)time.Seconds);
            return returnStr;
        }
        else
        {
            returnStr = string.Format(LocalizationTable.Localization("Time_Second_Format"), 0);
            return returnStr;
        }
    }

    public static string TimeFormatRemain(System.TimeSpan time)
    {
        return string.Format(LocalizationTable.Localization("Time_Reset_Format"),TimeFormatDefault(time));
    }

    public static string TimeFormatSkill(float time)
    {
        // 전달된 시간을 반올림 처리
        time = Mathf.Round(time); // 정수로 반올림

        if (time >= 60)
        {
            int minutes = (int)time / 60;
            int seconds = (int)time % 60;

            return $"{minutes}:{seconds:D2}"; // 분:초 형식 (초는 두 자릿수)
        }
        else if (time >= 1)
        {
            return $"{(int)time}"; // 1초 이상 60초 미만은 초만 표시
        }
        else
        {
            // 1초 미만일 때 소수점 첫째 자리까지 표시
            return $"{time:F1}"; // 소수점 첫째 자리까지 표시
        }
    }

    public static string TimeFormatMail(System.TimeSpan time)
    {
        string returnStr = "";
        returnStr = string.Format(LocalizationTable.Localization("Time_MailRemain_Format"), (int)time.TotalDays, (int)time.Hours);
        return returnStr;
    }
    #endregion
}

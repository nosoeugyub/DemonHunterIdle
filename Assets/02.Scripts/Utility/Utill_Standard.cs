using CodeStage.AntiCheat.ObscuredTypes;
using Game.Debbug;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utill_Enum;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 유틸리티 스탠다드 
/// /// </summary>
public class Utill_Standard : MonoBehaviour
{
    static Dictionary<string, Sprite> BGSprites = new Dictionary<string, Sprite>();
    static Dictionary<string, Sprite> itemSprites = new Dictionary<string, Sprite>();
    static Dictionary<string, Sprite> StageSprites = new Dictionary<string, Sprite>();
    static Dictionary<string, Texture2D> StageTexture = new Dictionary<string, Texture2D>();

    static Dictionary<string, Sprite> UiSprites = new Dictionary<string, Sprite>();
    static bool isLoadedSprites = false;

    //변수 관련..
    public static Vector3 Vector3Zero = Vector3.zero;
    public static Vector2 Vector2Zero = Vector2.zero;

    public static Vector3 vector3One = Vector3.one;
    public static Vector2 Vector2One = Vector2.one;

    public static Vector3 vector3Right = Vector3.right;
    public static Vector3 vector3Up = Vector3.up;
    public static Vector3 vector3Down = Vector3.down;

    public static WaitForSeconds WaitTimeOne = new WaitForSeconds(1.0f);
    public static WaitForSeconds WaitTimehalfOne = new WaitForSeconds(0.1f);
    public static WaitForSecondsRealtime WaitTimehalfeight = new WaitForSecondsRealtime(0.99f);
    public static WaitForFixedUpdate WaitTimeFixedUpdate = new WaitForFixedUpdate();

    /// <summary>
    /// 범위 내에서 랜덤한 x, y, z 값을 반환
    /// </summary>
    /// <param name="rangeCollider">범위 측정용 콜라이더</param>
    /// <returns></returns>
    public static Vector3 GetRandomPositionInCollider(BoxCollider rangeCollider)
    {
        if (rangeCollider == null)
        {
            Debug.LogError("Collider is not assigned!");
            return Vector3.zero;
        }

        // 콜라이더의 중심 및 크기 가져오기
        Vector3 center = rangeCollider.center;
        Vector3 size = rangeCollider.size;

        // 콜라이더의 실제 월드 위치 및 크기 계산
        Vector3 worldCenter = rangeCollider.transform.TransformPoint(center);
        Vector3 worldSize = Vector3.Scale(size, rangeCollider.transform.lossyScale);

        // 범위 내에서 랜덤한 x, y, z 값 
        float randomX = UnityEngine.Random.Range(worldCenter.x - worldSize.x / 2, worldCenter.x + worldSize.x / 2);
        float randomY = UnityEngine.Random.Range(worldCenter.y - worldSize.y / 2, worldCenter.y + worldSize.y / 2);
        float randomZ = UnityEngine.Random.Range(worldCenter.z - worldSize.z / 2, worldCenter.z + worldSize.z / 2);

        return new Vector3(randomX, randomY, randomZ);
    }

    #region object find

    /// <summary>
    /// 특정 스크립트를 가진 오브젝트 중 범위 안에 있으면 반환
    /// </summary>
    /// <param name="maxIndex"> 찾을 오브젝트의 최대 갯수 / -1이면 최대 제한 없음</param>
    public static List<T> FindAllObjectsInRadius<T>(Vector3 center, float searchRadius, LayerMask searchLayer, int maxIndex = 200)
    {
        List<T> foundObjects = new List<T>();

        // 주어진 중심점을 기준으로 지정된 반지름 내의 모든 콜라이더를 가져옵니다.
        Collider[] colliders = Physics.OverlapSphere(center, searchRadius, searchLayer);

        foreach (Collider col in colliders)
        {
            // 특정 스크립트가 연결되어 있는지 확인합니다.
            T findObject = col.gameObject.GetComponent<T>();

            if (findObject != null)
            {
                // 스크립트가 있는 경우 해당 게임 오브젝트를 리스트에 추가합니다.
                foundObjects.Add(findObject);
            }
            //제한 없음 상태가 아니고 제한치를 넘길 경우
            if(maxIndex != -1 && foundObjects.Count >= maxIndex)
            {
                break;
            }
        }

        return foundObjects;
    }
    public static T[] FindAllObjectsInRadiusArray<T>(Vector3 center, float searchRadius, LayerMask searchLayer, int maxIndex = 200)
    {
        List<T> foundObjectsList = new List<T>();

        // 주어진 중심점을 기준으로 지정된 반지름 내의 모든 콜라이더를 가져옵니다.
        Collider[] colliders = Physics.OverlapSphere(center, searchRadius, searchLayer);

        foreach (Collider col in colliders)
        {
            // 특정 스크립트가 연결되어 있는지 확인합니다.
            T findObject = col.gameObject.GetComponent<T>();

            if (findObject != null)
            {
                // 스크립트가 있는 경우 해당 게임 오브젝트를 리스트에 추가합니다.
                foundObjectsList.Add(findObject);
            }
            //제한 없음 상태가 아니고 제한치를 넘길 경우
            if (maxIndex != -1 && foundObjectsList.Count >= maxIndex)
            {
                break;
            }
        }

        // List를 배열로 변환하여 반환
        return foundObjectsList.ToArray();
    }
    /// <summary>
    /// 특정 스크립트를 가진 오브젝트 중 가장 가까운 오브젝트 반환
    /// </summary>
    public static T FindNearestObjectInRadius<T>(Vector3 center, float searchRadius, LayerMask searchLayer)
    {
        List<T> foundObjects = new List<T>();
        T nearObject = default(T);

        float closestDistance = float.PositiveInfinity;

        // 주어진 중심점을 기준으로 지정된 반지름 내의 모든 콜라이더를 가져옵니다.
        Collider[] colliders = Physics.OverlapSphere(center, searchRadius, searchLayer);

        foreach (Collider col in colliders)
        {
            // 특정 스크립트가 연결되어 있는지 확인합니다.
            T findObject = col.gameObject.GetComponent<T>();

            if (findObject != null)
            {
                // 스크립트가 있는 경우 해당 게임 오브젝트를 리스트에 추가합니다.
                foundObjects.Add(findObject);

                float distance = (col.transform.position - center).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearObject = findObject;
                }
            }

        }

        return nearObject;
    }

    #endregion

    /// <summary>
    /// 게임 오브젝트의 레이어가 주어진 LayerMask에 포함되어 있는지 확인
    /// </summary>
    public static bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((mask.value & (1 << obj.layer)) != 0);
    }

    // GameObject 가져오기
    public static GameObject GetPrefab(string prefabName)
    {
        // Resources 폴더 내의 Prefabs 폴더 경로
        string path = "Prefabs/" + prefabName; // Prefab 이름은 확장자를 제외한 이름으로 지정

        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError("Failed to load prefab: " + prefabName);
        }

        return prefab;
    }
    public static string ConvertNumberToPattern(int number)
    {
        // 패턴에 사용될 문자열을 초기화합니다.
        string pattern;

        if (number <= 10)
        {
            // 한 자리 숫자의 경우 "1-1" 형태로 변환합니다.
            pattern = "1-" + number;
        }
        else
        {
            // 두 자리 숫자의 경우 첫 자리와 두 번째 자리를 분리합니다.
            //데이터 가공
            int clearstage = number;
            var UserStage = Utill_Math.CalculateStageAndRound(clearstage);
            //현재 유저의 스테이지와 라운드를 계산 
            int userindex = UserStage.stageindex;
            int userRound = UserStage.CurrentRound;
            int totalRound = GameDataTable.Instance.StageTableDic[userindex].ChapterCycle;
            int userStage = GameDataTable.Instance.StageTableDic[userindex].ChapterLevel;

            // 패턴을 생성합니다.
            pattern = userStage + "-" + userRound;
        }

        return pattern;
    }
    public static void DisableObjectsWithTag(string tag)
    {
        // ObjectPooler 클래스에서 해당 태그를 가진 모든 객체를 가져옵니다.
        List<GameObject> objectsWithTag = ObjectPooler.GetAllPools(tag);

        // 가져온 객체들을 모두 비활성화합니다.
        foreach (GameObject obj in objectsWithTag)
        {
            obj.SetActive(false);
        }
    }

    //Image & Sprite
    public static Image GetImageResources(string _name)
    {
        // Resources 폴더에서 _name에 해당하는 Sprite를 로드
        Sprite sprite = Resources.Load<Sprite>(_name);

        // 스프라이트가 존재하지 않으면 null을 반환
        if (sprite == null)
        {
            Debug.LogError($"Sprite with name {_name} not found in Resources folder.");
            return null;
        }

        // 새 GameObject 생성 및 Image 컴포넌트 추가
        GameObject obj = new GameObject(_name);
        Image image = obj.AddComponent<Image>();

        // Image 컴포넌트에 스프라이트 설정
        image.sprite = sprite;

        return image;
    }

    #region stirng 배열에서 랜덤 string 값가져오기
    public static string GetRandomString(string[] array)
    {
        // 배열이 비어 있는지 확인합니다.
        if (array == null || array.Length == 0)
        {
            Debug.LogError("Array is null or empty");
            return null;
        }

        // 배열의 인덱스 범위 내에서 랜덤 인덱스를 생성합니다.
        int randomIndex = UnityEngine.Random.Range(0, array.Length);

        // 랜덤 인덱스의 요소를 반 환합니다.
        return array[randomIndex];
    }
    #endregion

    #region string에서 색 표현식 삭제
    public static string RemoveColorTags(string input)
    {
        // <color=#XXXXXX> 패턴을 찾는 정규식
        string colorTagPattern = @"<color=#([0-9A-Fa-f]{6})>";

        // <color=#XXXXXX> 태그와 </color> 태그를 모두 제거
        string result = Regex.Replace(input, colorTagPattern, string.Empty);
        result = result.Replace("</color>", string.Empty);

        return result;
    }
    #endregion

    #region bool배열에서 true가 몇개인지
    public static int BoolArraytrueCount(bool[] boolarray)
    {
        int count = 0;

        // 배열을 순회하며 true 값을 찾고 카운터를 증가시킴
        for (int i = 0; i < boolarray.Length; i++)
        {
            if (boolarray[i])
            {
                count++;
            }
        }

        return count;
    }
    #endregion

    #region Enum을 int로 바꾸는 함수
    public static int EnumToInt<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        return Convert.ToInt32(enumValue);
    }
    #endregion

    //게임 시작할 때 스프라이트를 받아오는 함수... 스프라이트 이름과 타입의 이름이 똑같아야 버그가 안남 항상 리소스 이름이 잘 되어있는지 확인할것
    #region Sprite 아틀라스 파싱 함수
    public static void OnLoadAllSprite()
    {
        if (isLoadedSprites)
            return;
        OnLoadSpritesByPath("CharacterBG");
        OnLoadSpritesByPath("Item");
        OnLoadSpritesByPath("StageBG");
        OnLoadTexture2DByPath("StageBG");
        OnLoadSpritesByPath("UI");

        isLoadedSprites = true;
    }
    public static Sprite GetItemSprite(string key)
    {
        string trimKey = key.Trim();
        if (itemSprites.ContainsKey(trimKey))
        {
            return itemSprites[trimKey];
        }
        else
        {
            Debug.Log("Null Item Sprite : " + trimKey);
            return null;
        }
    }
    public static Sprite GetBgStageSprite(string key)
    {
        string trimKey = key.Trim();
        if (StageSprites.ContainsKey(trimKey))
        {
            return StageSprites[trimKey];
        }
        else
        {
            Debug.Log("Null Item Sprite : " + trimKey);
            return null;
        }
    }

    public static Texture2D GetBgTextureSprite(string key)
    {
        string trimKey = key.Trim();
        if (StageTexture.ContainsKey(trimKey))
        {
            return StageTexture[trimKey];
        }
        else
        {
            Debug.Log("Null Item Sprite : " + trimKey);
            return null;
        }
    }


    public static Sprite GetUiSprite(string key)
    {
        string trimKey = key.Trim();
        if (UiSprites.ContainsKey(trimKey))
        {
            return UiSprites[trimKey];
        }
        else
        {
            Debug.Log("Null Item Sprite : " + trimKey);
            return null;
        }
    }



    static void OnLoadSpritesByPath(string _path)
    {
        string path = "Images/00.Atlas/" + _path;
        SpriteAtlas atlas = Resources.Load<SpriteAtlas>(path);
        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        for (int i = 0; i < sprites.Length; i++)
        {
            var name = sprites[i].name;

            string str = name.Replace("(Clone)", "");
            switch (_path)
            {
                case "CharacterBG":
                    BGSprites.Add(str, sprites[i]);
                    break;
                case "Item":
                    itemSprites.Add(str, sprites[i]);
                    break;
                case "StageBG":
                    StageSprites.Add(str, sprites[i]);
                    break;
                case "UI":
                    UiSprites.Add(str, sprites[i]);
                    break;
                default:
                    Debug.Log("해당 경로는 존재하지 않습니다.");
                    break;
            }
        }
    }
    static void OnLoadTexture2DByPath(string _path)
    {
        string path = "Images/00.Atlas/" + _path;
        SpriteAtlas atlas = Resources.Load<SpriteAtlas>(path);

        if (atlas == null)
        {
            Debug.LogError("SpriteAtlas not found at path: " + path);
            return;
        }

        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == null)
            {
                Debug.LogError("Sprite is null at index: " + i);
                continue;
            }

            var name = sprites[i].name;
            string str = name.Replace("(Clone)", "");
            Texture2D texture = textureFromSprite(sprites[i]);

            switch (_path)
            {
                case "CharacterBG":
                    break;
                case "Item":
                    break;
                case "StageBG":
                    if (!StageTexture.ContainsKey(str))
                        StageTexture.Add(str, texture);
                    break;
                case "UI":
                    break;
                default:
                    Debug.LogError("Invalid path specified: " + _path);
                    break;
            }
        }
    }


    public static Texture2D textureFromSprite(Sprite sprite)
    {
        // 새 텍스처 생성
        Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

        // Sprite의 텍스처 크기와 rect 크기가 다를 경우 GetPixels 사용
        if (sprite.rect.width != sprite.texture.width || sprite.rect.height != sprite.texture.height)
        {
            // Sprite에서 해당 부분의 픽셀을 가져오기
            Color[] newColors = sprite.texture.GetPixels(
                (int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height
            );

            // newColors 배열의 크기와 newText의 크기 비교
            if (newColors.Length == newText.width * newText.height)
            {
                newText.SetPixels(newColors);
                newText.Apply();
            }
            else
            {
                Debug.LogError("Pixels array size does not match texture size.");
            }
        }
        else
        {
            // 텍스처 크기가 동일하면 원본 텍스처 반환
            return sprite.texture;
        }

        return newText;
    }
    #endregion

    #region 저장을 위한 변환 CurrencyModel의 id와 갯수로 2차원배열을 생성함
    public static ObscuredString[,] ChangeCurrenyData(List<ResourceTableData> list)
    {
        // 결과를 저장할 2차원 배열 선언
        ObscuredString[,] result = new ObscuredString[list.Count, 2];

        // 리스트에서 CurrencyModel 객체들을 순회하며 데이터 추출
        for (int i = 0; i < list.Count; i++)
        {
            result[i, 0] = list[i].ResourceType.ToString();       // id를 배열의 첫 번째 열에 저장
            result[i, 1] = list[i].UserHasValue.ToString();   // 갯수를 배열의 두 번째 열에 저장
        }

        // 생성된 배열 반환
        return result;
    }

    #endregion

   
    //Parse
    public static T StringToEnum<T>(string data) where T : struct
    {
        data = data.Replace("{", "").Replace("}", "");
        if (Enum.TryParse(data, out T result))
        {
            return result;
        }
        return (T)default;
    }
   
    public static List<Dictionary<string, int>> MakeDictionaryListOfStringData(string _data)
    {
        List<Dictionary<string, int>> resultList = new List<Dictionary<string, int>>();

        // 중괄호로 묶인 경우
        if (_data.Contains("{") && _data.Contains("}"))
        {
            _data = _data.Trim('{', '}');
            string[] keyValuePairs = _data.Split(new string[] { "},{" }, System.StringSplitOptions.None);

            foreach (string pair in keyValuePairs)
            {
                // 각 쌍에서 쉼표를 기준으로 분할
                string[] keyValue = pair.Split(',');

                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim('{', '}');
                    int value;
                    if (int.TryParse(keyValue[1].Trim('{', '}'), out value))
                    {
                        Dictionary<string, int> dict = new Dictionary<string, int>
                        {
                            { key, value }
                        };

                        resultList.Add(dict);
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid value for key {key}: {keyValue[1]}. Skipping this entry.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Invalid key-value pair: {pair}. Skipping this entry.");
                }
            }
        }
        else
        {
            // 쉼표로만 구분된 경우와 동일한 처리
            string[] keyValuePairs = _data.Split(',');

            if (keyValuePairs.Length % 2 != 0)
            {
                Debug.LogWarning("Invalid format: Key-value pairs are not in pairs. Skipping data processing.");
                return resultList;
            }

            for (int i = 0; i < keyValuePairs.Length; i += 2)
            {
                string key = keyValuePairs[i].Trim();
                int value;
                if (int.TryParse(keyValuePairs[i + 1].Trim(), out value))
                {
                    Dictionary<string, int> dict = new Dictionary<string, int>
                    {
                        { key, value }
                    };

                    resultList.Add(dict);
                }
                else
                {
                    Debug.LogWarning($"Invalid value for key {key}: {keyValuePairs[i + 1].Trim()}. Skipping this entry.");
                }
            }
        }

        return resultList;
    }
    public static string ConvertHunterUpgradeListToString(List<Dictionary<string, int>> hunterUpgradeList)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // hunterUpgradeList를 순회하며 문자열로 변환
        int listCount = hunterUpgradeList.Count;
        for (int i = 0; i < listCount; i++)
        {
            var upgradeDict = hunterUpgradeList[i];

            int dictCount = upgradeDict.Count;
            int dictIndex = 0;
            foreach (var upgrade in upgradeDict)
            {
                sb.Append($"{{{upgrade.Key},{upgrade.Value}}}");

                // 마지막 업그레이드 데이터가 아니면 쉼표 추가
                if (dictIndex < dictCount - 1)
                {
                    sb.Append(",");
                }

                dictIndex++;
            }

            // 마지막 리스트 데이터가 아니면 전체 문자열에 쉼표 추가
            if (i < listCount - 1)
            {
                sb.Append(",");
            }
        }

        // 최종 문자열 반환
        return sb.ToString();
    }

    #region string을 <string, bool[]> 형태로 바꾸는 함수
    public static Dictionary<string, bool[]> ConvertToDictionary(string input)
    {
        // 결과를 저장할 딕셔너리 초기화
        Dictionary<string, bool[]> skillDictionary = new Dictionary<string, bool[]>();

        // 입력 문자열을 파싱하여 딕셔너리에 추가
        string[] skillEntries = input.Split(new string[] { "},{" }, StringSplitOptions.None);

        foreach (string entry in skillEntries)
        {
            // 각 항목을 파싱
            string cleanedEntry = entry.Replace("{", "").Replace("}", "").Replace("\\", "").Replace("\"", "");
            string[] parts = cleanedEntry.Split(new char[] { ',' }, 2);

            string skillName = parts[0].Trim(new char[] { ' ', '"' });
            string[] boolStrings = parts[1].Split(new char[] { ',' });

            bool[] boolArray = new bool[boolStrings.Length];
            for (int i = 0; i < boolStrings.Length; i++)
            {
                boolArray[i] = bool.Parse(boolStrings[i].Trim());
            }

            // 딕셔너리에 추가
            skillDictionary.Add(skillName, boolArray);
        }

        return skillDictionary;
    }
    #endregion


    /// <summary>
    /// 로드시 스트링 저장값 읽어서 단일 데이터 원하는 자료형으로 반환
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <param name="returnType"> 반환 자료형 타입 </param>
    /// <param name="columnCount"> 이차원 배열일때 열 개수 </param>
    /// <returns></returns>
    public static T LoadStringToGeneric<T>(string input, T defaultValue, int columnCount = 0)
    {
        input = input.Replace("{", "").Replace("}", "");
        int rowCount;
        string[] array = input.Split(',');

        switch (defaultValue)
        {
            #region int, float, double, bool, ObscuredInt ObscuredFloat, ObscuredDouble, ObscuredBool


            case string:
                return (T)(object)input.ToString();
            case int:
                return (T)(object)int.Parse(input);

            case float:
                return (T)(object)float.Parse(input);

            case double:
                return (T)(object)double.Parse(input);

            case bool:
                return (T)(object)bool.Parse(input);

            case ObscuredInt:
                ObscuredInt obscuredInt = int.Parse(input);
                return (T)(object)obscuredInt;

            case ObscuredFloat:
                ObscuredFloat obscuredFloat = float.Parse(input);
                return (T)(object)obscuredFloat;

            case ObscuredDouble:
                ObscuredDouble obscuredDouble = double.Parse(input);
                return (T)(object)obscuredDouble;

            case ObscuredBool:
                ObscuredBool obscuredBool = bool.Parse(input);
                return (T)(object)obscuredBool;

            #endregion

            #region intArray, floatArray, doubleArray, boolArray, stringArray

            case int[]:
                int[] intArray = new int[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    intArray[i] = int.Parse(array[i]);
                }
                return (T)(object)intArray;

            case float[]:
                float[] floatArray = new float[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    floatArray[i] = float.Parse(array[i]);
                }
                return (T)(object)floatArray;

            case double[]:
                double[] doubleArray = new double[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    doubleArray[i] = double.Parse(array[i]);
                }
                return (T)(object)doubleArray;

            case bool[]:
                bool[] boolArray = new bool[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    boolArray[i] = bool.Parse(array[i]);
                }
                return (T)(object)boolArray;

            case string[]:
                return (T)(object)array;

            #endregion

            #region ObscuredIntArray, ObscuredFloatArray, ObscuredDoubleArray, ObscuredBoolArray

            case ObscuredInt[]:
                ObscuredInt[] obscuredIntArray = new ObscuredInt[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    obscuredIntArray[i] = int.Parse(array[i]);
                }
                return (T)(object)obscuredIntArray;

            case ObscuredFloat[]:
                ObscuredFloat[] obscuredFloatArray = new ObscuredFloat[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    obscuredFloatArray[i] = float.Parse(array[i]);
                }
                return (T)(object)obscuredFloatArray;

            case ObscuredDouble[]:
                ObscuredDouble[] obscuredDoubleArray = new ObscuredDouble[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    obscuredDoubleArray[i] = double.Parse(array[i]);
                }
                return (T)(object)obscuredDoubleArray;

            case ObscuredBool[]:
                ObscuredBool[] obscuredBoolArray = new ObscuredBool[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    obscuredBoolArray[i] = bool.Parse(array[i]);
                }
                return (T)(object)obscuredBoolArray;

            #endregion

            #region intTwo-DimensionalArray(인트 2차원 배열), ObscuredIntTwo-DimensionalArray (옵스커드 인트 2차원 배열) stringTwo-DimensionalArray (스트링 2차원 배열)
            case int[,]:
                rowCount = array.Length / columnCount; // 행 개수
                int[,] intDoubleArray = new int[rowCount, columnCount];
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        int index = i * columnCount + j;
                        intDoubleArray[i, j] = int.Parse(array[index]);
                    }
                }
                return (T)(object)intDoubleArray;

            case ObscuredInt[,]:
                rowCount = array.Length / columnCount; // 행 개수
                ObscuredInt[,] obscuredIntDoubleArray = new ObscuredInt[rowCount, columnCount];
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        int index = i * columnCount + j;
                        obscuredIntDoubleArray[i, j] = int.Parse(array[index]);
                    }
                }
                return (T)(object)obscuredIntDoubleArray;

            case string[,]:

                rowCount = array.Length / columnCount; // 행 개수
                string[,] stringDoubleArray = new string[rowCount, columnCount];
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        int index = i * columnCount + j;
                        stringDoubleArray[i, j] = array[index];
                    }
                }
                return (T)(object)stringDoubleArray;

            case ObscuredString[,]:

                rowCount = array.Length / columnCount; // 행 개수
                ObscuredString[,] ObscuredstringDoubleArray = new ObscuredString[rowCount, columnCount];
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        int index = i * columnCount + j;
                        ObscuredstringDoubleArray[i, j] = array[index];
                    }
                }
                return (T)(object)ObscuredstringDoubleArray;

            #endregion

            default:
                throw new ArgumentException("Unsupported type: " + typeof(T));
        }
    }

    #region 딕셔너리를<string , bool[]> 을 string으로..바꾸는함수
    public static string ConvertToString(Dictionary<string, bool[]> skillDictionary)
    {
        // 결과를 저장할 StringBuilder 초기화
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // 딕셔너리의 각 항목을 순회하며 문자열로 변환
        sb.Append("{");
        foreach (var skill in skillDictionary)
        {
            sb.Append("{");
            sb.Append($"\"{skill.Key}\", ");
            sb.Append("{");

            for (int i = 0; i < skill.Value.Length; i++)
            {
                sb.Append(skill.Value[i].ToString().ToLower());
                if (i < skill.Value.Length - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append("}");
            sb.Append("}");
            if (!skill.Equals(skillDictionary.Last()))
            {
                sb.Append(", ");
            }
        }
        sb.Append("}");

        // 최종 문자열 반환
        return sb.ToString();
    }
    #endregion



    /// <summary>
    /// 자료형 값을 문자열로 변환하여 저장
    /// </summary>
    /// <typeparam name="T">저장할 자료형 타입</typeparam>
    /// <param name="value">저장할 값</param>
    /// <param name="columnCount">이차원 배열일 때 열 개수</param>
    /// <returns>저장된 값의 문자열 표현</returns>
    public static string SaveGenericToSring<T>(T value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        switch (value)
        {
            #region int, float, double, bool, ObscuredInt ObscuredFloat, ObscuredDouble, ObscuredBool

            case int intValue:
                return intValue.ToString();

            case float floatValue:
                return floatValue.ToString();

            case double doubleValue:
                return doubleValue.ToString();

            case bool boolValue:
                return boolValue.ToString();

            case ObscuredInt obscuredInt:
                return obscuredInt.GetDecrypted().ToString();

            case ObscuredFloat obscuredFloat:
                return obscuredFloat.GetDecrypted().ToString();

            case ObscuredDouble obscuredDouble:
                return obscuredDouble.GetDecrypted().ToString();

            case ObscuredBool obscuredBool:
                return obscuredBool.GetDecrypted().ToString();

            #endregion

            #region int[], float[], double[], bool[], ObscuredInt[], ObscuredFloat[], ObscuredDouble[], ObscuredBool[] ,string[]

            case int[] intArray:
                return string.Join(",", intArray);

            case float[] floatArray:
                return string.Join(",", floatArray);

            case double[] doubleArray:
                return string.Join(",", doubleArray);

            case bool[] boolArray:
                return string.Join(",", boolArray.Select(b => b.ToString()));

            case ObscuredInt[] obscuredIntArray:
                return string.Join(",", obscuredIntArray.Select(oi => oi.GetDecrypted().ToString()));

            case ObscuredFloat[] obscuredFloatArray:
                return string.Join(",", obscuredFloatArray.Select(of => of.GetDecrypted().ToString()));

            case ObscuredDouble[] obscuredDoubleArray:
                return string.Join(",", obscuredDoubleArray.Select(od => od.GetDecrypted().ToString()));

            case ObscuredBool[] obscuredBoolArray:
                return string.Join(",", obscuredBoolArray.Select(ob => ob.GetDecrypted().ToString()));
            case string[] stringArray:
                // GetDecrypted 메서드가 제대로 작동하는지 확인
                return string.Join(",", stringArray);
            #endregion

            #region intTwo-DimensionalArray(인트 2차원 배열), ObscuredIntTwo-DimensionalArray (옵스커드 인트 2차원 배열) stringTwo-DimensionalArray (스트링 2차원 배열)

            case int[,] intTwoDimArray:
                return ConvertTwoDimArrayToString(intTwoDimArray);

            case ObscuredInt[,] ObscuredIntTwoDimArray:
                return ConvertTwoDimArrayToString(ObscuredIntTwoDimArray);

            case string[,] stringTwoDimArray:
                return ConvertTwoDimArrayToString(stringTwoDimArray);
            case ObscuredString[,] ObscuredStringTwoDimArray:
                return ConvertTwoDimArrayToString(ObscuredStringTwoDimArray);
            #endregion

            default:
                throw new ArgumentException("Unsupported type: " + typeof(T));
        }
    }

    public interface IToStringBool
    {
        string ToString(bool isOn);
        string ToString(bool isOn, string format);
    }

    public static string ListToJson(IList list, bool isOn = false, string outputFormat = "{0}")
    {
        if (list == null || list.Count == 0)
            return "{}";

        var sb = new StringBuilder();
        sb.Append("{");
        foreach (var value in list)
        {
            var str = string.Format(outputFormat,
                (value as IToStringBool)?.ToString(isOn) ?? value);

            sb.Append($"{str},");
        }

        sb.Remove(sb.Length - 1, 1);
        sb.Append("}");

        return sb.ToString();
    }
    public static List<T> JsonToList<T>(string json, Func<string, T> valueConstructor)
    {
        // 처음과 마지막 중괄호를 제거합니다.
        json = json.Remove(0, 1);
        json = json.Remove(json.Length - 1, 1);

        // 줄 바꿈 문자와 큰따옴표를 제거합니다.
        json = json.Replace("\n", "");
        json = json.Replace("\"", "");


        // 쉼표로 문자열을 분리하여 리스트에 저장합니다.
        var lines = json.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        // 각 항목을 valueConstructor 함수로 변환합니다.
        return lines.Select(valueConstructor).ToList();
        {
            //json = json.Remove(0, 1);
            //json = json.Remove(json.Length - 1, 1);
            //json = json.Replace("\n", "");

            //var lines = new List<string>();

            //while (json.Length != 0)
            //{
            //    var index = 0;
            //    var cnt = 0;

            //    while (json.Length != index && (cnt != 0 || json[index] != ','))
            //    {
            //        if (json[index] == '{')
            //            cnt++;
            //        else if (json[index] == '}')
            //            cnt--;
            //        index++;
            //    }
            //    lines.Add(json.Substring(0, index));
            //    json = json.Substring(index, json.Length - index);
            //    if (json.StartsWith(","))
            //        json = json.Remove(0, 1);
            //}

            //return lines.Select(valueConstructor).ToList();
        }
    }
    public static Dictionary<TKey, TValue> JsonToDictionary<TKey, TValue>(string json, Func<string, TKey> keyConstructor, Func<string, TValue> valueConstructor)
    {
        var dict = new Dictionary<TKey, TValue>();

        if (json.Length == 0)
        {
            UnityEngine.Debug.Log("Json is Null");
            return dict;
        }

        json = json.Remove(0, 1);
        json = json.Replace("\n", "");
        json = json.Replace("\r", "");
        json = json.Replace(" ", "");

        var lines = new List<string>();

        while (json.Length != 0)
        {
            var index = json.IndexOf(":", StringComparison.Ordinal) + 2;
            var cnt = 1 + (json.StartsWith("{") ? 1 : 0);

            while (cnt != 0)
            {
                if (json.Length <= index)
                    break;
                if (json[index] == '{')
                    cnt++;
                else if (json[index] == '}')
                    cnt--;
                index++;
            }
            if (index <= json.Length)
            {
                lines.Add(json.Substring(0, index));
                json = json.Substring(index);
                if (json.StartsWith(","))
                    json = json.Remove(0, 1);
            }
        }

        foreach (var line in lines)
        {
            var offset = line.StartsWith("{") ? 1 : 0;

            var index = line.IndexOf(':');
            if (index > offset)
            {
                var key = line.Substring(0 + offset, index - offset);
                var valueStartIndex = index + 1;
                var valueLength = line.Length - valueStartIndex - offset;
                if (valueStartIndex < line.Length && valueLength >= 0)
                {
                    var value = line.Substring(valueStartIndex, valueLength);
                    dict[keyConstructor(key)] = valueConstructor(value);
                }
            }
        }

        return dict;
    }
   

    public static string DictionaryToJson(IDictionary dictionary, bool isOnKey = false, bool isOnValue = false,
    string keyOutputFormat = "{0}", string valueOutputFormat = "{0}")
    {
        if (dictionary == null || dictionary.Count == 0)
            return "{}";

        var sb = new StringBuilder();
        sb.Append("{");
        foreach (var key in dictionary.Keys)
        {
            var keyStr = string.Format(keyOutputFormat,
                (key as IToStringBool)?.ToString(isOnKey) ?? key.ToString());
            var valueStr = string.Format(valueOutputFormat,
                (dictionary[key] as IToStringBool)?.ToString(isOnValue) ?? dictionary[key].ToString());

            sb.AppendFormat("{{{0}:{{{1}}}}},", keyStr, valueStr);
        }

        sb.Remove(sb.Length - 1, 1);
        sb.Append("}");
        return sb.ToString();
    }

    /// <summary>
    /// 이차원 배열을 문자열로 변환합니다.
    /// </summary>
    /// <typeparam name="T">배열 요소의 자료형</typeparam>
    /// <param name="array">변환할 이차원 배열</param>
    /// <param name="columnCount">열 개수</param>
    /// <returns>변환된 문자열</returns>
    private static string ConvertTwoDimArrayToString<T>(T[,] array)
    {
        // 이차원 배열의 행 수와 열 수
        int rowCount = array.GetLength(0);
        int columnCount = array.GetLength(1);

        // 문자열 빌더를 사용하여 최종 문자열을 구성합니다.
        StringBuilder sb = new StringBuilder();

        sb.Append("{");

        for (int i = 0; i < rowCount; i++)
        {
            if (i > 0)
            {
                sb.Append(",");
            }

            sb.Append("{");

            for (int j = 0; j < columnCount; j++)
            {
                if (j > 0)
                {
                    sb.Append(",");
                }

                sb.Append(array[i, j]?.ToString() ?? "Empty");
            }

            sb.Append("}");
        }

        sb.Append("}");

        return sb.ToString();
    }
    //이미지 메테리얼을 바굴띠 흑백만드는데 자주씀
    public static void SetImageMaterial(Image image, string materialName)
    {
        if (image != null)
        {
            Material material = Resources.Load<Material>("Mat/" + materialName);
            if (material != null)
            {
                image.material = material;
            }
            else
            {
                Debug.LogError("Material " + materialName + " not found in Resources/Materials folder.");
            }
        }
    }

    public static List<KeyValuePair<string, int>> JsonToKeyValuePairList(string json)
    {
        var list = new List<KeyValuePair<string, int>>();

        if (string.IsNullOrWhiteSpace(json))
        {
            UnityEngine.Debug.Log("Input string is null or empty");
            return list;
        }

        // Split the input string by commas to get the key-value pairs
        var keyValuePairs = json.Split(',');

        foreach (var kvp in keyValuePairs)
        {
            // Split each key-value pair by the colon character
            var keyValue = kvp.Split(':');
            if (keyValue.Length == 2)
            {
                var key = keyValue[0].Trim(); // Trim any unnecessary whitespace around the key
                var keyvalue = keyValue[1].Replace("}", ""); // Trim any unnecessary whitespace around the key
                if (int.TryParse(keyvalue, out var value))
                {
                    list.Add(new KeyValuePair<string, int>(key, value));
                }
                else
                {
                    UnityEngine.Debug.Log($"Value '{keyValue[1]}' is not a valid integer");
                }
            }
            else
            {
                UnityEngine.Debug.Log("Key-Value pair is not in the expected format");
            }
        }

        return list;
    }

    public static string KeyValuePairListToString(List<KeyValuePair<string, int>> list)
    {
        if (list == null || list.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        foreach (var kvp in list)
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }
            sb.AppendFormat("{0}:{1}", kvp.Key, kvp.Value);
        }

        return sb.ToString();
    }

    public static string SerializeHunteritemDataList(List<HunteritemData> list)
    {
        return string.Join("\n", list.Select(item =>
            $"{{ Class={item.Class}, ItemContainsType={item.ItemContainsType},ItemGrade={item.ItemGrade} , ItemDrawer={item.DrawerGrade}, Part={item.Part},TotalLevel = {item.TotalLevel} ,isEquip ={item.isEquip}  , EquippedItemName=\"{item.Name}\", FixedOptionTypes=[{string.Join(", ", item.FixedOptionTypes.Select(t => $"\"{t}\""))}], FixedOptionValues=[{string.Join(", ", item.FixedOptionValues)}], EquipCountList=[{string.Join(", ", item.EquipCountList)}], HoldOption=[{string.Join(", ", item.HoldOption)}], HoldOptionValue=[{string.Join(", ", item.HoldOptionValue)}] }}"));
    }

    //모루등급 계산하는함수 = 다음등급을 리턴
    public static Utill_Enum.DrawerGrade GetNextDrawerGrade(Utill_Enum.DrawerGrade currentGrade)
    {
        // Enum의 모든 값을 배열로 변환
        Utill_Enum.DrawerGrade[] values = (Utill_Enum.DrawerGrade[])Enum.GetValues(typeof(Utill_Enum.DrawerGrade));
        int currentIndex = Array.IndexOf(values, currentGrade);

        // 다음 인덱스가 배열의 길이를 초과하지 않도록 조건 처리
        int nextIndex = currentIndex + 1;

        // 다음 등급이 배열 범위를 벗어나지 않는 경우에만 반환, 그렇지 않으면 현재 등급을 반환
        if (nextIndex < values.Length)
        {
            return values[nextIndex];
        }
        else
        {
            // 마지막 등급일 경우에도 현재 등급을 반환하거나 필요에 따라 다른 처리를 할 수 있음
            return currentGrade;
        }
    }

    internal static Dictionary<string, int> ReadHunterSkillDic(string jsonString)
    {
        // 중괄호 제거
        jsonString = jsonString.Trim('{', '}');

        // 결과를 저장할 딕셔너리 생성
        Dictionary<string, int> hunterSkillDic = new Dictionary<string, int>();

        // 문자열을 분리하여 각 키-값 쌍을 추출
        string[] keyValuePairs = jsonString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in keyValuePairs)
        {
            // 키와 값을 분리
            string[] keyValue = pair.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (keyValue.Length == 2)
            {
                // 키의 공백 제거 및 문자열을 정수로 변환
                string key = keyValue[0].Trim(' ', '\"');
                if (int.TryParse(keyValue[1].Trim(), out int value))
                {
                    hunterSkillDic[key] = value;
                }
            }
        }

        return hunterSkillDic;
    }


    // Dictionary<string, int>를 JSON 문자열로 변환하는 함수
    public static string SaveHunterSkillDic(Dictionary<string, int> hunterSkillDic)
    {
        // Dictionary<string, int>를 JSON 문자열로 변환
        string jsonString = JsonConvert.SerializeObject(hunterSkillDic, Formatting.Indented);

        return jsonString;
    }

    //스트링으로 옵션, 데미지타입 반환
    public static (Option, AttackDamageType) ParseInput(string input)
    {
        // '_'로 문자열을 분리
        string[] parts = input.Split('_');

        // Option 및 AttackDamageType 결정
        Option option = Option.None; // 기본값
        AttackDamageType damageType = AttackDamageType.Physical; // 기본값

        if (parts.Length == 4)
        {
            // Option 선택
            if (Enum.TryParse(parts[2], out Option parsedOption))
            {
                option = parsedOption;
            }

            // AttackDamageType 선택
            if (Enum.TryParse(parts[3], out AttackDamageType parsedDamageType))
            {
                damageType = parsedDamageType;
            }
        }

        return (option, damageType);
    }

    #region 공격 로직
    /// <summary>
    /// 특정 각도와 반지름 내에 있는 Enemy 객체들을 찾는 공용 함수
    /// </summary>
    /// <param name="center">중심 위치 (Vector3)</param>
    /// <param name="forward">중심의 정면 방향 (Vector3)</param>
    /// <param name="radius">반지름 (float)</param>
    /// <param name="angle">각도 범위 (float)</param>
    /// <returns>지정된 각도와 반지름 내에 있는 Enemy 객체들의 리스트</returns>
    public static List<Enemy> FindEnemiesInCone(Vector3 center, Vector3 forward  , float radius, float angle)
    {
        // 반지름과 각도 내에 있는 적들을 담을 리스트 생성
        List<Enemy> enemiesInRange = new List<Enemy>();

        // 주어진 중심과 반경 내에 있는 모든 콜라이더 검색
        Collider[] collidersInRange = Physics.OverlapSphere(center, radius);

        foreach (Collider collider in collidersInRange)
        {
            // Enemy 컴포넌트가 있는지 확인
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 중심 위치를 기준으로 적의 위치 방향 벡터 계산
                Vector3 directionToEnemy = enemy.transform.position - center;

                // 플레이어 정면을 기준으로 적이 각도 내에 있는지 확인
                float angleToEnemy = Vector3.Angle(forward, directionToEnemy.normalized);

                if (angleToEnemy <= angle / 2)
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }

        return enemiesInRange;
    }

    /// <summary>
    /// 지정된 기준 위치에서 떨어진 거리와 각도, 반지름을 이용해 범위 내의 적을 찾는 함수
    /// </summary>
    /// <param name="center">기준 위치 (Vector3)</param>
    /// <param name="distance">기준 위치에서 떨어진 거리 (float)</param>
    /// <param name="radius">범위의 반지름 (float)</param>
    /// <param name="angle">범위를 지정하는 각도 (float, 도 단위)</param>
    /// <returns>범위 내에 있는 Enemy 객체들의 리스트</returns>
    public static List<Enemy> FindEnemiesInArea(Vector3 center, float distance, float radius, float angle)
    {
        List<Enemy> enemiesInRange = new List<Enemy>();
        Enemy[] allEnemies = GameObject.FindObjectsOfType<Enemy>();

        // 기준 위치에서 지정한 각도와 거리를 이용하여 탐색 중심 위치를 구함
        Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        Vector3 areaCenter = center + direction.normalized * distance;

        foreach (Enemy enemy in allEnemies)
        {
            // 적의 위치와 탐색 중심 간의 거리 계산
            float distanceToEnemy = Vector3.Distance(areaCenter, enemy.transform.position);

            // 적이 반지름 내에 있으면 리스트에 추가
            if (distanceToEnemy <= radius)
            {
                enemiesInRange.Add(enemy);
            }
        }

        return enemiesInRange;
    }
    #endregion
}

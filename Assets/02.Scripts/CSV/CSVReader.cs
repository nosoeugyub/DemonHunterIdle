using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using System.IO;
using BackEnd;
using LitJson;
using static Utill_Enum;
using System.Linq;
using System.Collections;

/// 작성일자   : 2024-05-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : csv 파일읽어오는 코드
/// </summary>
public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
    public static bool MakeToBool(string str)
    {
        bool boolValue = bool.Parse(str);
        return boolValue;
    }

    public static float MakeTofloat(string str)
    {
        if (str == "")
        {
            return 0;
        }
        float floatValue = float.Parse(str);
        return floatValue;
    }

    public static int MakeToInt(string str)
    {
        if (str == "")
        {
            return 0;
        }
        int floatValue = int.Parse(str);
        return floatValue;
    }

    public static float[] MakeTofloatArray(string str)
    {
        // 백슬래시를 제거한 후 쉼표로 분할하여 문자열 배열로 변환합니다.
        string[] values = str.Replace("\"", "").Split(',');

        // 부동 소수점(float) 배열을 만듭니다.
        float[] floatArray = new float[values.Length];

        // 각 문자열 값을 부동 소수점(float)으로 변환하여 배열에 저장합니다.
        for (int i = 0; i < values.Length; i++)
        {
            if (!float.TryParse(values[i], out float floatValue))
            {
                // 변환 실패 시 기본값인 0으로 설정합니다. 다른 처리 방법도 가능합니다.
                floatValue = 0f;
            }
            floatArray[i] = floatValue;
        }

        return floatArray;
    }
    public static string[] MakeToStringArray(string str)
    {
        // 입력 문자열에서 양쪽의 이스케이프 문자를 제거합니다.
        str = str.Trim('\"');

        // 문자열을 쉼표와 공백을 기준으로 분할하여 문자열 배열로 변환합니다.
        string[] stringArray = str.Split(new string[] { ", " }, System.StringSplitOptions.None);

        return stringArray;
    }

    public static int[] MakeToIntArray(string str)
    {
        // 입력 문자열에서 쉼표를 기준으로 분할하여 문자열 배열로 변환합니다.
        string[] values = str.Split(',');

        // 정수형(int) 배열을 만듭니다.
        int[] intArray = new int[values.Length];

        // 각 문자열 값을 정수형(int)으로 변환하여 배열에 저장합니다.
        for (int i = 0; i < values.Length; i++)
        {
            // 따옴표와 역슬래시 제거
            string cleanedValue = values[i].Replace("\\", "").Replace("\"", "").Trim();

            if (!int.TryParse(cleanedValue, out int intValue))
            {
                // 변환 실패 시 기본값인 0으로 설정합니다. 다른 처리 방법도 가능합니다.
                intValue = 0;
            }
            intArray[i] = intValue;
        }

        return intArray;
    }
    public static string[] ConvertStringToStringArray(string input)
    {
        // 주어진 문자열을 공백을 기준으로 분할하여 문자열 배열로 만듭니다.
        string[] stringArray = input.Split(' ');

        return stringArray;
    }


    /// <summary>
    /// --------------DATA READ ---------------------------------
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>

    #region 닉네임 필터
    public static List<string> NicknameFilterList(string file)
    {
        var list = new List<string>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            Dictionary<string, string> valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            list.Add(valueMap["Key"]);
        }

        return list;
    }
    #endregion

    #region 자동 닉네임 
    public static List<KeyValuePair<string, string>> AutoNicknameRead(string file)
    {
        var list = new List<KeyValuePair<string, string>>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            Dictionary<string, string> valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }
            list.Add(new KeyValuePair<string, string>(valueMap["Adjective"], valueMap["Noun"]));
        }

        return list;
    }
    #endregion 

    #region ETC 최대 갯수 구하는 테이블
    public static Dictionary<string, ConstraintsData> ConstraintsRead(string file)
    {
        var dictionary = new Dictionary<string, ConstraintsData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            // 소수점 값 여부를 확인하고 예외 처리 추가
            if (float.TryParse(valueMap["Value"], out float floatValue) && floatValue % 1 != 0)
            {
                var entry = new ConstraintsData
                {
                    Name = valueMap["Name"],
                    _floatValue = MakeTofloat(valueMap["Value"])
                };

                dictionary.Add(valueMap["Name"], entry);
            }
            else
            {
                var entry = new ConstraintsData
                {
                    Name = valueMap["Name"],
                    Value = MakeToInt(valueMap["Value"])
                };

                dictionary.Add(valueMap["Name"], entry);
            }



        }

        return dictionary;
    }
    #endregion

    #region 로컬 경험치 요구량 테이블
    public static Dictionary<string, RequireData> ExpRead(string file)
    {
        var dictionary = new Dictionary<string, RequireData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length < 3 || string.IsNullOrEmpty(values[0]))
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            string[] levelRange = valueMap["LevelRange"].Trim('"').Split(',');
            int startLevel = Convert.ToInt32(levelRange[0]);
            int endLevel = Convert.ToInt32(levelRange[1]);
            int initialValue = Convert.ToInt32(valueMap["InitialValue"]);
            int commonDifference = Convert.ToInt32(valueMap["CommonDifference"]);

            for (int level = startLevel; level <= endLevel; level++)
            {
                int requiredExp = initialValue + (level - startLevel) * commonDifference;
                string key = level.ToString();
                var entry = new RequireData
                {
                    Level = level,
                    Exp = requiredExp
                };
                dictionary.Add(key, entry);
            }
        }

        return dictionary;
    }
    #endregion

    #region 서버 경험치 요구량 테이블
    public static Dictionary<string, RequireData> ServerExpRead()
    {
        string selectedProbabilityFileId = "126826";   // 뒤끝서버 테이블 파일ID 사용 - 경험치 
        var dictionary = new Dictionary<string, RequireData>();

        // 뒤끝 서버로부터 테이블 데이터 가져오기
        var bro = BackendErrorManager.Instance.RetryLogic(() => Backend.Chart.GetChartContents(selectedProbabilityFileId));
        if (bro.IsSuccess())
        {
            // 응답 데이터 확인
            JsonData json = bro.GetReturnValuetoJSON()["rows"];

            // 각 행을 반복하며 데이터를 파싱하여 dictionary에 추가
            for (int i = 0; i < json.Count; i++)
            {
                JsonData row = json[i];
                string[] levelRange = row["LevelRange"]["S"].ToString().Split(',');
                int startLevel = Convert.ToInt32(levelRange[0]);

                int endLevel = Convert.ToInt32(levelRange[1]);
                int initialValue = Convert.ToInt32(row["InitialValue"]["S"].ToString());
                int commonDifference = Convert.ToInt32(row["CommonDifference"]["S"].ToString());

                for (int level = startLevel; level <= endLevel; level++)
                {
                    int requiredExp = initialValue + (level - startLevel) * commonDifference;
                    dictionary[level.ToString()] = new RequireData
                    {
                        Level = level,
                        Exp = requiredExp
                    };
                }
            }
        }
        else
        {
            // 뒤끝 서버에서 데이터를 가져오지 못한 경우 에러 로그 출력
            Game.Debbug.Debbuger.ErrorDebug("뒤끝 서버에서 경험치 데이터를 가져오지 못했습니다: " + bro.GetErrorCode() + " - " + bro.GetMessage());
        }
        return dictionary;
    }
    #endregion

    #region 아이템
    public static Dictionary<SubClass, Dictionary<string, Item>> EquipmentListRead(string file)
    {
        var dictionary = new Dictionary<SubClass, Dictionary<string, Item>>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            Dictionary<string, string> valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap.Add(header[j], values[j]);
            }

            SubClass character = ParseEnum<Utill_Enum.SubClass>(valueMap["SubClass"]);
            string name = valueMap["Name"];

            Grade grade = ParseEnum<Utill_Enum.Grade>(valueMap["Grade"]);
            EquipmentType equipmentType = ParseEnum<Utill_Enum.EquipmentType>(valueMap["Type"]);
            string backgroundName = valueMap["BackgroundName"];

            string str = valueMap["FixedOption"];
            string[] arrStr = str.Split(',');

            Option[] options = new Option[arrStr.Length];
            for (int j = 0; j < options.Length; j++)
            {
                options[j] = ParseEnum<Utill_Enum.Option>(arrStr[j].Trim('"').Trim());
            }
            //FixedOptionMinValue
            int[] FixedOptionMinValue = MakeToIntArray(valueMap["FixedOptionMinValue"]);
            int[] FixedOptionMaxValue = MakeToIntArray(valueMap["FixedOptionMaxValue"]);
            int[] FixedOptionFinalValue = MakeToIntArray(valueMap["FixedOptionFinalValue"]);
            float[] FixedOptionValueUnit = MakeTofloatArray(valueMap["FixedOptionValueUnit"]);
            int[] FixedOptionWeightValue = MakeToIntArray(valueMap["FixedOptionWeightValue"]);

            // HoldOption
            string holdstr = valueMap["HoldOption"];
            string[] HoldOptionarrStr = holdstr.Split(',');
            Option[] HoldOption = new Option[HoldOptionarrStr.Length];

            if (HoldOptionarrStr[0] != string.Empty)
            {
                for (int j = 0; j < HoldOption.Length; j++)
                {
                    HoldOption[j] = ParseEnum<Utill_Enum.Option>(HoldOptionarrStr[j].Trim('"').Trim());
                }
            }

            //HoldValue
            int[] HoldOptionValue = MakeToIntArray(valueMap["HoldOptionValue"]);

            string SaleResourceType1str = valueMap["SaleResourceType1"];
            Utill_Enum.Resource_Type SaleResourceType1 = Resource_Type.None;
            if (SaleResourceType1str != string.Empty)
            {
                SaleResourceType1 = ParseEnum<Utill_Enum.Resource_Type>(SaleResourceType1str);
            }
            int[] SaleResourceCount1 = MakeToIntArray(valueMap["SaleResourceCount1"]);

            Item item = new Item(character, grade, name, backgroundName, options.ToList<Option>(),
                                FixedOptionMinValue, FixedOptionMaxValue, FixedOptionFinalValue, FixedOptionValueUnit, FixedOptionWeightValue, HoldOption, HoldOptionValue,
                                SaleResourceType1, SaleResourceCount1);
            item.SetEquipmentItem(equipmentType, options, false);

            //GameDataTable.Instance.ItemRandomOptionCnt.Add(name, int.Parse(valueMap["RandomOptionCnt"]));

            // 캐릭터를 기준으로 한 dictionary에 아이템 추가
            if (!dictionary.ContainsKey(character))
            {
                dictionary.Add(character, new Dictionary<string, Item>());
            }
            dictionary[character].Add(name, item);
        }

        return dictionary;
    }
    #endregion

    #region 아이템 옵션 수치
    public static SortedTupleBag<Option, ItemValue> ItemValueRead(string file)
    {
        var dictionary = new SortedTupleBag<Option, ItemValue>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            Dictionary<string, string> valueMap = new Dictionary<string, string>();
            valueMap.Clear();
            for (int j = 0; j < header.Length; j++)
            {
                if (!valueMap.ContainsKey(header[j]))
                {
                    valueMap.Add(header[j], values[j]);
                }
            }
            string itemName = valueMap["Name"];
            Grade grade = ParseEnum<Utill_Enum.Grade>(valueMap["Grade"]);
            EquipmentType type = ParseEnum<Utill_Enum.EquipmentType>(valueMap["Type"]);


            Optiontype optionType = ParseEnum<Utill_Enum.Optiontype>(valueMap["OptionType"]);

            Option option = Option.None;

            switch (optionType)
            {
                case Optiontype.Fixed:
                    option = ParseEnum<Utill_Enum.Option>(valueMap["FixedOption"]);
                    break;

                case Optiontype.Random:
                    option = ParseEnum<Utill_Enum.Option>(valueMap["RandomOption"]);
                    break;
            }

            double minValue = float.Parse(valueMap["MinValue"]);
            double maxValue = float.Parse(valueMap["MaxValue"]);
            double valueUnit = float.Parse(valueMap["ValueUnit"]);
            double finalvalue = float.Parse(valueMap["FinalValue"]);
            #region 나중에 옵스커드 변수 들어오면 유틸리티에 셋필드 만들예정
            string weightValueStr = valueMap["WeightValue"];
            string[] weightValueArr = weightValueStr.Split(',');
            for (int j = 0; j < weightValueArr.Length; j++)
            {
                weightValueArr[j] = weightValueArr[j].Trim('"');
            }
            double[] weightValue = new double[weightValueArr.Length];
            for (int k = 0; k < weightValueArr.Length; k++)
            {
                weightValue[k] = double.Parse(weightValueArr[k]);
            }
            #endregion

            ItemValue itemValue = new ItemValue(grade, type, itemName, optionType, option, minValue, maxValue, finalvalue, valueUnit, weightValue);
            dictionary.Add(option, itemValue);
        }
        return dictionary;
    }
    #endregion

    #region MobTable
    public static Dictionary<string, EnemyStat> EnemyStatRead(string file)
    {
        var dictionary = new Dictionary<string, EnemyStat>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new EnemyStat
            {
                Class = ParseEnum<Utill_Enum.EnemyClass>(valueMap["Class"]),
                MobName = valueMap["SubClass"],
                AttackType = ParseEnum<Utill_Enum.Enum_AttackType>(valueMap["AttackType"]),
                AttackDamageType = ParseEnum<Utill_Enum.AttackDamageType>(valueMap["AttackDamageType"]),
                HP = MakeTofloat(valueMap["HP"]),
                PhysicalPower = MakeTofloat(valueMap["PhysicalPower"]),
                AttackSpeed = MakeTofloat(valueMap["AttackSpeed"]),
                AttackRange = MakeTofloat(valueMap["AttackRange"]),
                MoveSpeed = MakeTofloat(valueMap["MoveSpeed"]),
                PhysicalPowerDefense = MakeTofloat(valueMap["PhysicalPowerDefense"]),
                DodgeChance = MakeTofloat(valueMap["DodgeChance"]),
                CriChance = MakeTofloat(valueMap["CriChance"]),
                CriDamage = MakeTofloat(valueMap["CriDamage"]),
                CCResist = MakeTofloat(valueMap["CCResist"]),
                ProjectilePrefab = valueMap["ProjectilePrefab"],
                ProjectileSpeed = MakeTofloat(valueMap["ProjectileSpeed"])
            };
            string key = valueMap["SubClass"];
            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region StageTable
    public static Dictionary<int, StageTableData> StageTableRead(string file)
    {
        var dictionary = new Dictionary<int, StageTableData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new StageTableData
            {
                StageIndex = MakeToInt(valueMap["StageIndex"]),
                ChapterZone = MakeToInt(valueMap["ChapterZone"]),
                ChapterLevel = MakeToInt(valueMap["ChapterLevel"]),
                ChapterCycle = MakeToInt(valueMap["ChapterCycle"]),
                GoalKillCount = MakeToInt(valueMap["GoalKillCount"]),
                HPWeight = MakeTofloatArray(valueMap["HPWeight"].Replace("\\", "").Replace("\"", "")),
                PhysicalPowerWeight = MakeTofloatArray(valueMap["PhysicalPowerWeight"].Replace("\\", "").Replace("\"", "")),
                PhysicalPowerDefenseWeight = MakeTofloatArray(valueMap["PhysicalPowerDefenseWeight"].Replace("\\", "").Replace("\"", "")),
                SpawnArea = MakeTofloatArray(valueMap["SpawnArea"].Replace("\\", "").Replace("\"", "")),
                Count1 = MakeTofloatArray(valueMap["Count1"].Replace("\\", "").Replace("\"", "")),
                MobName1 = ConvertStringToStringArray(valueMap["MobName1"]),
                BossName = valueMap["BossName"],
                BossTimelimited = valueMap["BossTimelimited"],
                GoldDropChance1 = MakeTofloat(valueMap["GoldDropChance1"]),
                Gold1 = MakeToInt(valueMap["Gold1"]),
                Exp1 = MakeToInt(valueMap["Exp1"]),
                StageMap = MakeToStringArray(valueMap["StageMap"]),

                BossHPWeight = MakeTofloat(valueMap["BossHPWeight"]),
                BossPhysicalPowerWeight = MakeTofloat(valueMap["BossPhysicalPowerWeight"]),
                BossPhysicalPowerDefenseWeight = MakeTofloat(valueMap["BossPhysicalPowerDefenseWeight"]),
                BossCount = MakeToInt(valueMap["BossCount"]),
                MobMaterial1 = valueMap["MobMaterial1"],
                BossMaterial = valueMap["BossMaterial"],

                IsSClearStageArray = new bool[MakeToInt(valueMap["ChapterCycle"])],
                IsClearBoss = false
            };

            int key = entry.StageIndex;
            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 헌터 기본 스탯
    public static Dictionary<string, HunterStat> UserStatRead(string file)
    {
        var dictionary = new Dictionary<string, HunterStat>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new HunterStat
            {
                Class = ParseEnum<Utill_Enum.HeroClass>(valueMap["Class"]),
                SubClass = ParseEnum<Utill_Enum.SubClass>(valueMap["SubClass"]),
                AttacckType = ParseEnum<Utill_Enum.Enum_AttackType>(valueMap["AttackType"]),
                AttackDamageType = ParseEnum<Utill_Enum.AttackDamageType>(valueMap["AttackDamageType"]),
                AttackSpeed = MakeTofloat(valueMap["AttackSpeed"]),
                AttackRange = MakeTofloat(valueMap["AttackRange"]),
                MoveSpeed = MakeTofloat(valueMap["MoveSpeed"]),
                GroupMoveSpeed = MakeTofloat(valueMap["GroupMoveSpeed"]),
                PhysicalPower = MakeTofloat(valueMap["PhysicalPower"]),
                MagicPower = MakeTofloat(valueMap["MagicPower"]),
                PhysicalPowerDefense = MakeTofloat(valueMap["PhysicalPowerDefense"]),
                HP = MakeTofloat(valueMap["HP"]),
                MP = MakeTofloat(valueMap["MP"]),
                ArrowCount = MakeToInt(valueMap["ArrowCount"])
            };

            string key = valueMap["SubClass"];
            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 재화
    public static Dictionary<Utill_Enum.Resource_Type, ResourceTableData> LoadResource(string file)
    {
        var dictionary = new Dictionary<Utill_Enum.Resource_Type, ResourceTableData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new ResourceTableData
            {
                ResourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                StartCount = MakeToInt(valueMap["StartCount"]),
                MaxValue = MakeToInt(valueMap["MaxValue"]),
            };

            Utill_Enum.Resource_Type key = entry.ResourceType;
            dictionary.Add(key, entry);
        }

        return dictionary;
    }

    #endregion

    #region Maximum 스텟 데이터
    public static Dictionary<string, StatTableData> LoadStatMaximumLimt(string file)
    {
        var lanuageDict = new Dictionary<string, Dictionary<ELanguage, string>>();
        var dictionary = new Dictionary<string, StatTableData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            #region entity stat
            int tempDisplayDigitCount = 0;
            if (valueMap["DisplayDigitCount"].Split('.').Length > 1)
            {
                tempDisplayDigitCount = valueMap["DisplayDigitCount"].Split('.')[1].ToString().Length;
            }

            var entry = new StatTableData
            {
                Key = valueMap["Key"],
                MaxValue = MakeToInt(valueMap["MaxValue"]),
                DisplayUnit = valueMap["DisplayUnit"],
                DisplayDigitCount = tempDisplayDigitCount,
                DisplayMultiplier = MakeTofloat(valueMap["DisplayMultiplier"])
            };

            string key = entry.Key;
            dictionary.Add(key, entry);
            #endregion

            //스텟은 예외적으로 언어 테이블을 여기에서 작업함
            #region 언어 테이블
            Dictionary<ELanguage, string> languageMap = new();
            languageMap.Add(ELanguage.KR, valueMap["KR"]);
            languageMap.Add(ELanguage.EN, valueMap["EN"]);
            LocalizationTable.localizationDict.Add(valueMap["Key"], languageMap);
            #endregion
        }

        return dictionary;
    }
    #endregion

    #region 헌터업그레이드
    public static Dictionary<int, Hunter_UpgradeData> LoadHunter_Upgrdae(string file)
    {
        var dictionary = new Dictionary<int, Hunter_UpgradeData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new Hunter_UpgradeData
            {
                Level = MakeToInt(valueMap["Level"]),
                LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                ResourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                PhysicalPower = MakeToInt(valueMap["PhysicalPower"]),
                MagicPower = MakeToInt(valueMap["MagicPower"]),
                PhysicalPowerDefense = MakeToInt(valueMap["PhysicalPowerDefense"]),
                HP = MakeToInt(valueMap["HP"]),
                MP = MakeToInt(valueMap["MP"]),
                CriChance = MakeTofloat(valueMap["CriChance"]),
                CriDamage = MakeTofloat(valueMap["CriDamage"]),
                AttackSpeedPercent = MakeTofloat(valueMap["AttackSpeedPercent"]),
                MoveSpeedPercent = MakeTofloat(valueMap["MoveSpeedPercent"]),
                GoldBuff = MakeTofloat(valueMap["GoldBuff"]),
                ExpBuff = MakeTofloat(valueMap["ExpBuff"]),
                ItemBuff = MakeTofloat(valueMap["ItemBuff"])
            };

            int key = entry.Level;
            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 속성구성 리스트
    public static Dictionary<Utill_Enum.Hunter_Attribute, HunterAttributeData> LoadAttributeCompositione(string file)
    {
        var dictionary = new Dictionary<Utill_Enum.Hunter_Attribute, HunterAttributeData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new HunterAttributeData
            {
                Type = ParseEnum<Utill_Enum.Hunter_Attribute>(valueMap["Type"]),
                AttackSpeed = MakeTofloat(valueMap["AttackSpeed"]),
                AttackRange = MakeTofloat(valueMap["AttackRange"]),
                MoveSpeed = MakeTofloat(valueMap["MoveSpeed"]),
                PhysicalPower = MakeTofloat(valueMap["PhysicalPower"]),
                MagicPower = MakeTofloat(valueMap["MagicPower"]),
                PhysicalPowerDefense = MakeTofloat(valueMap["PhysicalPowerDefense"]),
                Hp = MakeTofloat(valueMap["HP"]),
                Mp = MakeTofloat(valueMap["MP"])
            };

            Utill_Enum.Hunter_Attribute key = entry.Type;
            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 속성지급 리스트
    public static Dictionary<int, AttributeAllocationData> LoadAttributeAlloc(string file)
    {
        var dictionary = new Dictionary<int, AttributeAllocationData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);
        Dictionary<string, int> headerMap = new Dictionary<string, int>();

        // 헤더 맵 생성
        for (int h = 0; h < header.Length; h++)
        {
            headerMap.Add(header[h], h);
        }

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            // 따옴표 제거
            for (int j = 0; j < values.Length; j++)
            {
                values[j] = values[j].Replace("\"", "");
            }

            // valueMap 사용
            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new AttributeAllocationData();
            int key = i;

            entry.LevelRange = MakeToIntArray(valueMap["LevelRange"]);
            entry.AttributeCount = MakeToIntArray(valueMap["AttributeCount"]);

            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 승급시 필요한 조건
    public static Dictionary<int, PromotionAllocationData> LoadPromotionRequirement(string file)
    {
        var dictionary = new Dictionary<int, PromotionAllocationData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new PromotionAllocationData();

            // 이름을 기준으로 데이터 채우기
            string numberString = Regex.Replace(valueMap["Type"], @"\D", "");
            int key = int.Parse(numberString);


            entry.ResourceType = valueMap["ResourceType"];
            entry.ResourceCount = int.Parse(valueMap["ResourceCount"]);
            entry.ReqHunterLevel = int.Parse(valueMap["ReqHunterLevel"]);
            entry.ReqClearChapterZone = int.Parse(valueMap["ReqClearChapterZone"]);

            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 승급시 오르는 능력치

    public static Dictionary<int, PromotionAbilityData> LoadPromotionReward(string file)
    {
        var dictionary = new Dictionary<int, PromotionAbilityData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);
        Dictionary<string, int> headerMap = new Dictionary<string, int>();

        // 헤더 맵 생성
        for (int h = 0; h < header.Length; h++)
        {
            headerMap.Add(header[h], h);
        }

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new PromotionAbilityData();

            string numberString = Regex.Replace(valueMap["Type"], @"\D", "");
            int key = int.Parse(numberString);

            //사라진 키값들 주석처리
            //entry.AttackSpeed = float.Parse(valueMap["AttackSpeed"]);
            //entry.AttackRange = float.Parse(valueMap["AttackRange"]);
            //entry.MoveSpeed = float.Parse(valueMap["MoveSpeed"]);
            entry.PhysicalPower = float.Parse(valueMap["PhysicalPower"]);
            entry.MagicPower = float.Parse(valueMap["MagicPower"]);
            //entry.PhysicalPowerDefense = float.Parse(valueMap["PhysicalPowerDefense"]);
            //entry.Hp = float.Parse(valueMap["HP"]);
            //entry.Mp = float.Parse(valueMap["MP"]);

            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 랭귀지 테이블
    public static Dictionary<string, Dictionary<ELanguage, string>> LoadLanguageSheet(string file)
    {
        var dict = new Dictionary<string, Dictionary<ELanguage, string>>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dict;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1)
        {
            Debug.LogError("No data found in file.");
            return dict;
        }

        // 헤더 읽기
        var header = Regex.Split(lines[0], SPLIT_RE);

        // 언어 맵 생성
        Dictionary<string, ELanguage> languageMap = new Dictionary<string, ELanguage>();
        for (var j = 1; j < header.Length; j++)
        {
            var langString = header[j].Trim('\"'); // 언어 헤더의 앞뒤 따옴표 제거
            if (Enum.TryParse(langString, out ELanguage lang))
            {
                languageMap.Add(langString, lang);
            }
        }

        // 각 라인 파싱
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            // 각 행을 사전으로 변환
            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                var columnName = header[j].Trim('\"'); // 헤더의 앞뒤 따옴표 제거
                if (j < values.Length)
                {
                    valueMap[columnName] = values[j].Trim('\"'); // 각 값의 앞뒤 따옴표 제거
                }
            }

            // 키 값 가져오기
            if (!valueMap.TryGetValue("Key", out var key))
            {
                Debug.LogError("Key column not found.");
                continue;
            }

            dict.Add(key, new Dictionary<ELanguage, string>());

            // 언어별로 데이터 추가
            foreach (var langEntry in languageMap)
            {
                if (valueMap.TryGetValue(langEntry.Key, out var text))
                {
                    var str = text.Replace("\\n", Environment.NewLine).Trim('\"'); // \n을 줄바꿈으로 변환하고 각 값의 앞뒤 따옴표 제거
                    dict[key].Add(langEntry.Value, str);
                }
            }
        }

        // Assuming LocalizationTable is a class with a static field 'localizationDict' and an 'isInit' flag
        var type = typeof(LocalizationTable);

        var field = type.GetField("localizationDict", BindingFlags.Static | BindingFlags.NonPublic);

        if (field == null)
            return dict;

        field.SetValue(null, dict);
        LocalizationTable.isInit = true;
        LocalizationTable.languageSettings?.Invoke();

        return dict;
    }
    #endregion

    #region 요일스킬 등급
    public static Dictionary<string, DailySkillData> LoadDailySkillData(string file)
    {
        var dictionary = new Dictionary<string, DailySkillData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new DailySkillData
            {
                Key = (valueMap["Stat"]),
                NoramlValue = MakeTofloat(valueMap["Normal"]),
                Superior = MakeTofloat(valueMap["Superior"]),
                Rare = MakeTofloat(valueMap["Rare"]),
                Unique = MakeTofloat(valueMap["Unique"]),
                Epic = MakeTofloat(valueMap["Epic"])
            };

            string key = entry.Key;
            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 과금 업적 보상
    public static Dictionary<int, PurchaseAchievementData> LoadPurchaseAchievementData(string file)
    {
        var dictionary = new Dictionary<int, PurchaseAchievementData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new PurchaseAchievementData
            {
                index = MakeToInt(valueMap["Index"]),
                totalPurchase = MakeToInt(valueMap["TotalPurchase"]),
                resourceType = valueMap["ResourceType"],
                resourceCount = MakeToInt(valueMap["ResourceCount"]),
            };

            int key = entry.index;
            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 상점 골드 2
    public static Dictionary<int, ShopGoldCellData> LoadGoldCell_Two_Data(string file)
    {
        var dictionary = new Dictionary<int, ShopGoldCellData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }
            var tmpPriceStr = valueMap["Price"].Split('_');

            var entry = new ShopGoldCellData
            {
                index = MakeToInt(valueMap["Index"]),
                IOS = (valueMap["Ios"]),
                Aos = (valueMap["Aos"]),
                SpriteName = (valueMap["SpriteName"]),
                Type = ParseEnum<Utill_Enum.ShopType>(valueMap["Type"]),
                Goods = valueMap["Goods"],
                Count = MakeToInt(valueMap["Count"]),
                AddPercent = MakeToInt(valueMap["AddPercent"]),
                OriginalPrice = MakeToInt(tmpPriceStr[0]),
                Discount = MakeToInt(tmpPriceStr[1]),
                DiscountedPrice = MakeToInt(tmpPriceStr[2]),
                TBC = MakeToInt(valueMap["TBC"]),
                ShopDesc = (valueMap["상품 설명"]),
            };

            int key = entry.index;
            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 상점 골드 1
    public static Dictionary<int, ShopGoldCellData> LoadGoldCell_One_Data(string file)
    {
        var dictionary = new Dictionary<int, ShopGoldCellData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }
            var tmpPriceStr = valueMap["Price"].Split('_');

            var entry = new ShopGoldCellData
            {
                index = MakeToInt(valueMap["Index"]),
                IOS = (valueMap["Ios"]),
                Aos = (valueMap["Aos"]),
                SpriteName = (valueMap["SpriteName"]),
                Type = ParseEnum<Utill_Enum.ShopType>(valueMap["Type"]),
                Goods = valueMap["Goods"],
                Count = MakeToInt(valueMap["Count"]),
                AddPercent = MakeToInt(valueMap["AddPercent"]),
                OriginalPrice = MakeToInt(tmpPriceStr[0]),
                Discount = MakeToInt(tmpPriceStr[1]),
                DiscountedPrice = MakeToInt(tmpPriceStr[2]),
                TBC = MakeToInt(valueMap["TBC"]),
                ShopDesc = (valueMap["상품 설명"]),
            };

            int key = entry.index;
            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion

    #region 모루 등급 테이블
    public static Dictionary<Utill_Enum.DrawerGrade, ItemDrawerTableData> LoaditemDarwerGradeData(string file)
    {
        var dictionary = new Dictionary<Utill_Enum.DrawerGrade, ItemDrawerTableData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[4], SPLIT_RE);

        for (var i = 5; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new ItemDrawerTableData
            {
                index = MakeToInt(valueMap["Index"]),
                DrawerGrade = ParseEnum<Utill_Enum.DrawerGrade>(valueMap["DrawerGrade"]),
                DrawerProb = MakeToInt(valueMap["DrawerProb"]),
                DrawerResourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["DrawerResourceType"]),
                DrawerResourceCount = MakeToInt(valueMap["DrawerResourceCount"]),
                DrawerEquipmentLevelRange = MakeToIntArray(valueMap["DrawerEquipmentLevelRange"]),
                EvolutionProb = MakeToInt(valueMap["EvolutionProb"]),
                EvolutionResourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["EvolutionResourceType"]),
                EvolutionResourceCount = MakeToInt(valueMap["EvolutionResourceCount"]),
                EvolutionReqCurrentEquipmentLvel = MakeToInt(valueMap["EvolutionReqCurrentEquipmentLevel"]),
                EvolutionReqTotalEquipmentLevl = MakeToInt(valueMap["EvolutionReqTotalEquipmentLevel"]),
                EvolutionReqTotalEquipmentGrade = (valueMap["EvolutionReqTotalEquipmentGrade"])
            };

            Utill_Enum.DrawerGrade index = entry.DrawerGrade;
            dictionary.Add(index, entry);
        }

        return dictionary;
    }
    #endregion

    #region 일일 보상
    public static List<DailyMissionData> LoadDailyMissionData(string file)
    {
        var list = new List<DailyMissionData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return list;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            Dictionary<DailyMissionType, DailyMission> tmpMissionData = new();
            //일일 임무 딕셔너리 초기화
            for (int j = 0; j < 3; j++) //enum길이만큼 순회하며 딕셔너리 채움
            {
                int needVal = MakeToInt(valueMap[((DailyMissionType)j).ToString()]);
                string resourceType = valueMap[$"ResourceType{j + 1}"];
                int ResourceCount = MakeToInt(valueMap[$"ResourceCount{j + 1}"]);
                tmpMissionData.Add((DailyMissionType)j, new DailyMission
                {
                    NeedValue = needVal,
                    ResourceCount = ResourceCount,
                    ResourceType = resourceType
                });
            }
            var tmpIndex = valueMap["StageLevel"].Replace(" ", "").Trim('\"').Split(',');

            var entry = new DailyMissionData
            {
                StageLevelMin = MakeToInt(tmpIndex[0]),
                StageLevelMax = MakeToInt(tmpIndex[1]),
                Missions = tmpMissionData
            };

            list.Add(entry);
        }

        return list;
    }
    #endregion

    #region 아이템 가챠 테이블
    public static Dictionary<int, ItemGachaData> LoadItemGachaDataTable(string file)
    {
        var dic = new Dictionary<int, ItemGachaData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dic;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dic;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            if (valueMap.ContainsKey("Index") == false)
                continue;
            string FrameEffect = (valueMap["FrameEffect"] == "FALSE") ? "" : valueMap["FrameEffect"];
            bool isResource = GachaManager.Instance.IsResource(valueMap["RewardName"]);
            var entry = new ItemGachaData
            {
                Index = MakeToInt(valueMap["Index"]),
                RewardName = valueMap["RewardName"],
                Prob = MakeTofloat(valueMap["Prob"]),
                Count = MakeToInt(valueMap["Count"]),
                FrameEffect = FrameEffect,
                ChatBroadcast = MakeToBool(valueMap["ChatBroadcast"]),
                IsResource = isResource
            };

            dic.Add(MakeToInt(valueMap["Index"]), entry);
        }

        return dic;
    }
    #endregion

    #region 아이템 가챠 합성
    public static Dictionary<int, ItemGachaMerge> LoadItemGachaMerge(string file)
    {
        var dic = new Dictionary<int, ItemGachaMerge>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dic;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dic;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            if (valueMap.ContainsKey("Index") == false)
                continue;
            var entry = new ItemGachaMerge
            {
                Index = MakeToInt(valueMap["Index"]),
                Grade = ParseEnum<Utill_Enum.Grade>(valueMap["Name"]),
                Count = MakeToInt(valueMap["Count"]),
            };

            dic.Add(MakeToInt(valueMap["Index"]), entry);
        }

        return dic;
    }
    #endregion


    #region 과금 업적 보상
    public static Dictionary<int, GachaAchievementData> LoadGachaAchievementData(string file)
    {
        var dictionary = new Dictionary<int, GachaAchievementData>();
        string path = "CSV/" + file;
        TextAsset data = Resources.Load(path) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"Failed to load file at path: {path}");
            return dictionary;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return dictionary;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var valueMap = new Dictionary<string, string>();
            for (int j = 0; j < header.Length; j++)
            {
                valueMap[header[j]] = values[j];
            }

            var entry = new GachaAchievementData
            {
                index = MakeToInt(valueMap["Index"]),
                totalPurchase = MakeToInt(valueMap["TotalGacha"]),
                resourceType = valueMap["ResourceType"],
                resourceCount = MakeToInt(valueMap["ResourceCount"]),
            };

            int key = entry.index;
            dictionary.Add(key, entry);
        }

        return dictionary;
    }
    #endregion


    #region 헌터 스킬 데이터
    public static Dictionary<string, List<BaseSkillData>> ReadHunterSkill()
    {
        // 각 스킬 데이터를 스킬 이름을 키로, 데이터를 리스트로 관리
        Dictionary<string, List<BaseSkillData>> skillDictionary = new Dictionary<string, List<BaseSkillData>>();
        // Resources/CSV/HunterSkill 폴더의 모든 파일 읽기 경로 수동으로 해줘야함
        string folderPath = "CSV/Skill/Archer";
        TextAsset[] csvFiles = Resources.LoadAll<TextAsset>(folderPath);

        if (csvFiles == null || csvFiles.Length == 0)
        {
            Debug.LogError($"헌터 스킬 폴더에 파일이 존재하지 않음: {folderPath}");
            return skillDictionary;
        }

        // 각 CSV 파일을 처리
        foreach (var file in csvFiles)
        {
            var lines = Regex.Split(file.text, LINE_SPLIT_RE);
            if (lines.Length <= 1) continue;

            var header = Regex.Split(lines[0], SPLIT_RE);

            List<BaseSkillData> skillDataList = new List<BaseSkillData>(); // 스킬 데이터를 저장할 리스트

            // 각 라인을 파싱하여 데이터를 리스트에 추가
            for (var i = 1; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || string.IsNullOrEmpty(values[0]))
                    continue;

                var valueMap = new Dictionary<string, string>();
                for (int j = 0; j < header.Length; j++)
                {
                    valueMap[header[j]] = values[j];
                }


                switch (file.name) // CSV 파일의 이름을 스킬 이름으로 사용
                {
                    case "SplitPiercing":
                        SplitPiercingData splitPiercingData = new SplitPiercingData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),
                            CHGValue_SplitDamageRate = MakeToInt(valueMap["CHGValue_SplitDamageRate"]),
                            CHGValue_SplitPiercingChance = MakeToInt(valueMap["CHGValue_SplitPiercingChance"]),
                            CHGValue_SplitChance = MakeToInt(valueMap["CHGValue_SplitChance"]),
                        };
                        splitPiercingData.CHGValue.Add(splitPiercingData.CHGValue_UseMP.ToString());
                        splitPiercingData.CHGValue.Add(splitPiercingData.CHGValue_SkillDuration.ToString());
                        splitPiercingData.CHGValue.Add(splitPiercingData.CHGValue_SplitDamageRate.ToString());
                        splitPiercingData.CHGValue.Add(splitPiercingData.CHGValue_SplitPiercingChance.ToString());
                        splitPiercingData.CHGValue.Add(splitPiercingData.CHGValue_SplitChance.ToString());
                        splitPiercingData.CHGName.AddRange(GetCHGHeader(header));

                        skillDataList.Add(splitPiercingData); // 리스트에 추가

                        

                        break;

                    case "RageShot":
                        RageShotData rageShotData = new RageShotData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),
                            CHGValue_AttackSpeedPercent = MakeToInt(valueMap["CHGValue_AttackSpeedPercent"])
                        };
                        rageShotData.CHGValue.Add(rageShotData.CHGValue_UseMP.ToString());
                        rageShotData.CHGValue.Add(rageShotData.CHGValue_SkillDuration.ToString());
                        rageShotData.CHGValue.Add(rageShotData.CHGValue_AttackSpeedPercent.ToString()); // 공격 속도 퍼센트 추가
                        rageShotData.CHGName.AddRange(GetCHGHeader(header));

                   


                        skillDataList.Add(rageShotData); // 리스트에 추가
                        break;

                    case "ArrowRain":
                        ArrowRainData arrowRainData = new ArrowRainData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_DMG_MagicPower_Magic = MakeTofloat(valueMap["CHGValue_DMG_MagicPower_Magic"])
                        };
                        arrowRainData.CHGValue.Add(arrowRainData.CHGValue_UseMP.ToString());
                        arrowRainData.CHGValue.Add(arrowRainData.CHGValue_DMG_MagicPower_Magic.ToString());

                        arrowRainData.DMGValue.Add(arrowRainData.CHGValue_DMG_MagicPower_Magic);

                        arrowRainData.CHGName.AddRange(GetCHGHeader(header));
                        arrowRainData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(arrowRainData); // 리스트에 추가

                        var option_1 =  Utill_Standard.ParseInput("CHGValue_DMG_MagicPower_Magic");
                        arrowRainData.optionliast.Add(option_1.Item1);
                        arrowRainData.attackdamagetype.Add(option_1.Item2);

                        break;

                    case "Electric":
                        ElectricData electricData = new ElectricData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),
                            CHGValue_DMG_MagicPower_Magic = MakeTofloat(valueMap["CHGValue_DMG_MagicPower_Magic"]),
                            CHGValue_CastingChance = MakeToInt(valueMap["CHGValue_CastingChance"])
                        };

                        electricData.CHGValue.Add(electricData.CHGValue_UseMP.ToString());
                        electricData.CHGValue.Add(electricData.CHGValue_SkillDuration.ToString());
                        electricData.CHGValue.Add(electricData.CHGValue_DMG_MagicPower_Magic.ToString());
                        electricData.CHGValue.Add(electricData.CHGValue_CastingChance.ToString());

                        electricData.DMGValue.Add(electricData.CHGValue_DMG_MagicPower_Magic);
                        
                        electricData.CHGName.AddRange(GetCHGHeader(header));
                        electricData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(electricData); // 리스트에 추가

                        var electricData_option = Utill_Standard.ParseInput("CHGValue_DMG_MagicPower_Magic"); //이름에서 대미지 타입 ,옵션 뽑아내서 저장
                        electricData.optionliast.Add(electricData_option.Item1);
                        electricData.attackdamagetype.Add(electricData_option.Item2);

                        break;

                    case "StrongShot":
                        StrongShotData strongShotData = new StrongShotData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_DMG_MagicPower_Magic = MakeTofloat(valueMap["CHGValue_DMG_MagicPower_Magic"])
                        };
                        strongShotData.CHGValue.Add(strongShotData.CHGValue_UseMP.ToString());
                        strongShotData.CHGValue.Add(strongShotData.CHGValue_DMG_MagicPower_Magic.ToString());

                        strongShotData.DMGValue.Add(strongShotData.CHGValue_DMG_MagicPower_Magic);

                        strongShotData.CHGName.AddRange(GetCHGHeader(header));
                        strongShotData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(strongShotData); // 리스트에 추가

                        var strongShotData_option = Utill_Standard.ParseInput("CHGValue_DMG_MagicPower_Magic"); //이름에서 대미지 타입 ,옵션 뽑아내서 저장
                        strongShotData.optionliast.Add(strongShotData_option.Item1);
                        strongShotData.attackdamagetype.Add(strongShotData_option.Item2);
                        break;

                    case "Focus":
                        FocusData focusData = new FocusData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                        };

                        focusData.CHGValue.Add(focusData.CHGValue_UseMP.ToString());

                        focusData.CHGName.AddRange(GetCHGHeader(header));
                        focusData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(focusData); // 리스트에 추가
                        break;
                    case "CoinFilp":
                        CoinFilpData luckyDiceData = new CoinFilpData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),

                            CHGValue_GoldEarned = MakeTofloat(valueMap["CHGValue_GoldEarned"]),
                            CHGValue_ExpEarned = MakeTofloat(valueMap["CHGValue_ExpEarned"]),
                           
                        };

                        luckyDiceData.CHGValue.Add(luckyDiceData.CHGValue_UseMP.ToString());
                        luckyDiceData.CHGValue.Add(luckyDiceData.CHGValue_SkillDuration.ToString());
                        luckyDiceData.CHGValue.Add(luckyDiceData.CHGValue_GoldEarned.ToString());
                        luckyDiceData.CHGValue.Add(luckyDiceData.CHGValue_ExpEarned.ToString());


                        luckyDiceData.CHGName.AddRange(GetCHGHeader(header));

                        skillDataList.Add(luckyDiceData); // 리스트에 추가
                        break;
                    case "CriticalShot":
                        CriticalShotData criticalShotData = new CriticalShotData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),

                            CHGValue_CriChance = MakeTofloat(valueMap["CHGValue_CriChance"]),
                            CHGValue_CriDamage = MakeTofloat(valueMap["CHGValue_CriDamage"])
                        };

                        criticalShotData.CHGValue.Add(criticalShotData.CHGValue_UseMP.ToString());
                        criticalShotData.CHGValue.Add(criticalShotData.CHGValue_SkillDuration.ToString());

                        criticalShotData.CHGValue.Add(criticalShotData.CHGValue_CriChance.ToString());
                        criticalShotData.CHGValue.Add(criticalShotData.CHGValue_CriDamage.ToString());

                        criticalShotData.CHGName.AddRange(GetCHGHeader(header));

                        skillDataList.Add(criticalShotData); // 리스트에 추가
                        break;
                    case "RapidShot":
                        RapidShotData rapidShotData = new RapidShotData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),
                            CHGValue_DMG_PhysicalPower_Physical = MakeTofloat(valueMap["CHGValue_DMG_PhysicalPower_Physical"])
                        };

                        rapidShotData.CHGValue.Add(rapidShotData.CHGValue_UseMP.ToString());
                        rapidShotData.CHGValue.Add(rapidShotData.CHGValue_SkillDuration.ToString());
                        rapidShotData.CHGValue.Add(rapidShotData.CHGValue_DMG_PhysicalPower_Physical.ToString());

                        rapidShotData.DMGValue.Add(rapidShotData.CHGValue_DMG_PhysicalPower_Physical);

                        rapidShotData.CHGName.AddRange(GetCHGHeader(header));
                        rapidShotData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(rapidShotData); // 리스트에 추가

                        var rapidShotData_option = Utill_Standard.ParseInput("CHGValue_DMG_PhysicalPower_Physical"); //이름에서 대미지 타입 ,옵션 뽑아내서 저장
                        rapidShotData.optionliast.Add(rapidShotData_option.Item1);
                        rapidShotData.attackdamagetype.Add(rapidShotData_option.Item2);
                        break;
                    case "KillDash":
                        KillDashData killDashData = new KillDashData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),
                            CHGValue_CriChance = MakeTofloat(valueMap["CHGValue_CriChance"]),
                            CHGValue_CoupChance = MakeTofloat(valueMap["CHGValue_CoupChance"]),
                        };

                        killDashData.CHGValue.Add(killDashData.CHGValue_UseMP.ToString());
                        killDashData.CHGValue.Add(killDashData.CHGValue_SkillDuration.ToString());

                        killDashData.CHGValue.Add(killDashData.CHGValue_CriChance.ToString());
                        killDashData.CHGValue.Add(killDashData.CHGValue_CoupChance.ToString());

                        killDashData.CHGName.AddRange(GetCHGHeader(header));
                        killDashData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(killDashData); // 리스트에 추가

                        break;
                    case "BloodSurge":
                        BloodSurgeData bloodSurgeData = new BloodSurgeData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),
                            CHGValue_CastingChance = MakeToInt(valueMap["CHGValue_CastingChance"]),
                            CHGValue_DMG_PhysicalPower_Physical = MakeTofloat(valueMap["CHGValue_DMG_PhysicalPower_Physical"]),
                        };

                        bloodSurgeData.CHGValue.Add(bloodSurgeData.CHGValue_UseMP.ToString());
                        bloodSurgeData.CHGValue.Add(bloodSurgeData.CHGValue_SkillDuration.ToString());
                        bloodSurgeData.CHGValue.Add(bloodSurgeData.CHGValue_CastingChance.ToString());
                        bloodSurgeData.CHGValue.Add(bloodSurgeData.CHGValue_DMG_PhysicalPower_Physical.ToString());

                        bloodSurgeData.DMGValue.Add(bloodSurgeData.CHGValue_DMG_PhysicalPower_Physical);

                        bloodSurgeData.CHGName.AddRange(GetCHGHeader(header));
                        bloodSurgeData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(bloodSurgeData); // 리스트에 추가

                        var bloodSurgeData_option = Utill_Standard.ParseInput("CHGValue_DMG_PhysicalPower_Physical"); //이름에서 대미지 타입 ,옵션 뽑아내서 저장
                        bloodSurgeData.optionliast.Add(bloodSurgeData_option.Item1);
                        bloodSurgeData.attackdamagetype.Add(bloodSurgeData_option.Item2);

                        break;
                    case "HunterStance":
                        HunterStacneData HunterStacneData = new HunterStacneData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),

                            CHGValue_DodgeChance = MakeTofloat(valueMap["CHGValue_DodgeChance"]),
                            CHGValue_CriChance = MakeTofloat(valueMap["CHGValue_CriChance"])
                        };

                        HunterStacneData.CHGValue.Add(HunterStacneData.CHGValue_SkillDuration.ToString());
                        HunterStacneData.DMGValue.Add(HunterStacneData.CHGValue_DodgeChance);
                        HunterStacneData.DMGValue.Add(HunterStacneData.CHGValue_CriChance);

                        HunterStacneData.CHGName.AddRange(GetCHGHeader(header));
                        HunterStacneData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(HunterStacneData); // 리스트에 추가

                        break;
                }
            }

            // 스킬 이름을 키로 하고, 스킬 데이터를 리스트로 저장
            if (!skillDictionary.ContainsKey(file.name))
            {
                skillDictionary[file.name] = new List<BaseSkillData>();
            }
            skillDictionary[file.name].AddRange(skillDataList); // 리스트에 스킬 데이터 추가
        }

        return skillDictionary;
    }



    #region 수호자 스킬 데이터
    public static Dictionary<string, List<BaseSkillData>> ReadGurdainSkill()
    {
        // 각 스킬 데이터를 스킬 이름을 키로, 데이터를 리스트로 관리
        Dictionary<string, List<BaseSkillData>> skillDictionary = new Dictionary<string, List<BaseSkillData>>();
        // Resources/CSV/HunterSkill 폴더의 모든 파일 읽기 경로 수동으로 해줘야함
        string folderPath = "CSV/Skill/Guardian";
        TextAsset[] csvFiles = Resources.LoadAll<TextAsset>(folderPath);

        if (csvFiles == null || csvFiles.Length == 0)
        {
            Debug.LogError($"헌터 스킬 폴더에 파일이 존재하지 않음: {folderPath}");
            return skillDictionary;
        }

        // 각 CSV 파일을 처리
        foreach (var file in csvFiles)
        {
            var lines = Regex.Split(file.text, LINE_SPLIT_RE);
            if (lines.Length <= 1) continue;

            var header = Regex.Split(lines[0], SPLIT_RE);

            List<BaseSkillData> skillDataList = new List<BaseSkillData>(); // 스킬 데이터를 저장할 리스트

            // 각 라인을 파싱하여 데이터를 리스트에 추가
            for (var i = 1; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || string.IsNullOrEmpty(values[0]))
                    continue;

                var valueMap = new Dictionary<string, string>();
                for (int j = 0; j < header.Length; j++)
                {
                    valueMap[header[j]] = values[j];
                }


                switch (file.name) // CSV 파일의 이름을 스킬 이름으로 사용
                {
                    case "ForceField":
                        ForceFieldData forceFieldData = new ForceFieldData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),

                            CHGValue_DMG_PhysicalPower_Physical = MakeTofloat(valueMap["CHGValue_DMG_PhysicalPower_Physical"]),
                            CHGValue_TargetNumber = MakeTofloat(valueMap["CHGValue_TargetNumber"]),
                        };
                        forceFieldData.CHGValue.Add(forceFieldData.CHGValue_UseMP.ToString());
                        forceFieldData.CHGValue.Add(forceFieldData.CHGValue_DMG_PhysicalPower_Physical.ToString());
                        forceFieldData.CHGValue.Add(forceFieldData.CHGValue_TargetNumber.ToString());
                        forceFieldData.DMGValue.Add(forceFieldData.CHGValue_DMG_PhysicalPower_Physical);

                        forceFieldData.CHGName.AddRange(GetCHGHeader(header));
                        forceFieldData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(forceFieldData); // 리스트에 추가

                        var forceFieldData_option = Utill_Standard.ParseInput("CHGValue_DMG_PhysicalPower_Physical"); //이름에서 대미지 타입 ,옵션 뽑아내서 저장
                        forceFieldData.optionliast.Add(forceFieldData_option.Item1);
                        forceFieldData.attackdamagetype.Add(forceFieldData_option.Item2);
                        break;

                    case "RageAttack":
                        RangeAttackData rageShotData = new RangeAttackData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),

                            CHGValue_AttackSpeedPercent = MakeTofloat(valueMap["CHGValue_AttackSpeedPercent"]),
                            CHGValue_MoveSpeedPercent = MakeTofloat(valueMap["CHGValue_MoveSpeedPercent"])
                        };
                        rageShotData.CHGValue.Add(rageShotData.CHGValue_UseMP.ToString());
                        rageShotData.CHGValue.Add(rageShotData.CHGValue_SkillDuration.ToString());
                        rageShotData.CHGValue.Add(rageShotData.CHGValue_AttackSpeedPercent.ToString()); // 공격 속도 퍼센트 추가
                        rageShotData.CHGValue.Add(rageShotData.CHGValue_MoveSpeedPercent.ToString()); // 이동속도  퍼센트 추가

                        rageShotData.CHGName.AddRange(GetCHGHeader(header));



                        skillDataList.Add(rageShotData); // 리스트에 추가
                        break;

                    case "LastLeaf":
                        LastLeafData lastLeafdata = new LastLeafData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),

                            CHGValue_CriChance = MakeTofloat(valueMap["CriChance1"]),
                            CHGValue_CriDamage = MakeTofloat(valueMap["CriDamage1"]),
                            CHGValue_AttackSpeedPercent1 = MakeTofloat(valueMap["AttackSpeedPercent1"]),

                            CHGValue_CriChance2 = MakeTofloat(valueMap["CriChance2"]),
                            CHGValue_CriDamage2 = MakeTofloat(valueMap["CriDamage2"]),
                            CHGValue_AttackSpeedPercent2 = MakeTofloat(valueMap["AttackSpeedPercent2"]),

                            CHGValue_CriChance3 = MakeTofloat(valueMap["CriChance3"]),
                            CHGValue_CriDamage3 = MakeTofloat(valueMap["CriDamage3"]),
                            CHGValue_AttackSpeedPercent3 = MakeTofloat(valueMap["AttackSpeedPercent3"]),

                            CHGValue_CriChanceSet = valueMap["CHGValue_CriChanceSet"],
                            CHGValue_CriDamageSet = valueMap["CHGValue_CriDamageSet"],
                            CHGValue_AttackSpeedPercentSet = valueMap["CHGValue_AttackSpeedPercentSet"]
                        };

                        //lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_CriChance);
                        //lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_CriDamage);
                        //lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_AttackSpeedPercent1);

                        //lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_CriChance2);
                        //lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_CriDamage2);
                        //lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_AttackSpeedPercent2);

                        //lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_CriChance3);
                        //lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_CriDamage3);
                        //lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_AttackSpeedPercent3);

                        lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_CriChanceSet);
                        lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_CriDamageSet);
                        lastLeafdata.CHGValue.Add(lastLeafdata.CHGValue_AttackSpeedPercentSet);


                        lastLeafdata.CHGName.AddRange(GetCHGHeader(header));
                        

                        skillDataList.Add(lastLeafdata); // 리스트에 추가
                        break;

                    case "HammerSummon":
                        HammerSummonData HammerSummonData = new HammerSummonData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),

                            CHGValue_DMG_PhysicalPower_Physical = MakeTofloat(valueMap["CHGValue_DMG_PhysicalPower_Physical"]),
                            CHGValue_StunDuration = MakeTofloat(valueMap["CHGValue_StunDuration"])
                        };

                        HammerSummonData.CHGValue.Add(HammerSummonData.CHGValue_UseMP.ToString());
                        HammerSummonData.CHGValue.Add(HammerSummonData.CHGValue_DMG_PhysicalPower_Physical.ToString());
                        HammerSummonData.CHGValue.Add(HammerSummonData.CHGValue_StunDuration.ToString());
                        HammerSummonData.DMGValue.Add(HammerSummonData.CHGValue_DMG_PhysicalPower_Physical);


                        HammerSummonData.CHGName.AddRange(GetCHGHeader(header));
                        HammerSummonData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(HammerSummonData); // 리스트에 추가

                        var HammerSummonData_option = Utill_Standard.ParseInput("CHGValue_DMG_PhysicalPower_Physical"); //이름에서 대미지 타입 ,옵션 뽑아내서 저장
                        HammerSummonData.optionliast.Add(HammerSummonData_option.Item1);
                        HammerSummonData.attackdamagetype.Add(HammerSummonData_option.Item2);

                        break;

                    case "ChainLightning":
                        ChainLighingData chainlighing = new ChainLighingData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),

                            CHGValue_DMG_MagicPower_Magic = MakeTofloat(valueMap["CHGValue_DMG_MagicPower_Magic"]),
                            CHGValue_ChainLightningNumber = MakeTofloat(valueMap["CHGValue_ChainLightningNumber"]),
                            CHGValue_CastingChance = MakeTofloat(valueMap["CHGValue_CastingChance"])
                        };

                        chainlighing.CHGValue.Add(chainlighing.CHGValue_UseMP.ToString());
                        chainlighing.CHGValue.Add(chainlighing.CHGValue_SkillDuration.ToString());
                        chainlighing.CHGValue.Add(chainlighing.CHGValue_DMG_MagicPower_Magic.ToString());
                        chainlighing.CHGValue.Add(chainlighing.CHGValue_ChainLightningNumber.ToString());
                        chainlighing.CHGValue.Add(chainlighing.CHGValue_CastingChance.ToString());

                        chainlighing.DMGValue.Add(chainlighing.CHGValue_DMG_MagicPower_Magic);

                        chainlighing.CHGName.AddRange(GetCHGHeader(header));
                        chainlighing.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(chainlighing); // 리스트에 추가

                        var strongShotData_option = Utill_Standard.ParseInput("CHGValue_DMG_MagicPower_Magic"); //이름에서 대미지 타입 ,옵션 뽑아내서 저장
                        chainlighing.optionliast.Add(strongShotData_option.Item1);
                        chainlighing.attackdamagetype.Add(strongShotData_option.Item2);
                        break;
                    case "WhirlwindRush":
                        WhirlwindRushData whirlwindRushdata = new WhirlwindRushData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),

                            CHGValue_DMG_PhysicalPower_Physical = MakeTofloat(valueMap["CHGValue_DMG_PhysicalPower_Physical"])
                        };

                        whirlwindRushdata.CHGValue.Add(whirlwindRushdata.CHGValue_UseMP.ToString());
                        whirlwindRushdata.CHGValue.Add(whirlwindRushdata.CHGValue_SkillDuration.ToString());
                        whirlwindRushdata.CHGValue.Add(whirlwindRushdata.CHGValue_DMG_PhysicalPower_Physical.ToString());
                        whirlwindRushdata.DMGValue.Add(whirlwindRushdata.CHGValue_DMG_PhysicalPower_Physical);

                        whirlwindRushdata.CHGName.AddRange(GetCHGHeader(header));
                        whirlwindRushdata.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(whirlwindRushdata); // 리스트에 추가

                        var whirlwindRushdata_option = Utill_Standard.ParseInput("CHGValue_DMG_PhysicalPower_Physical"); //이름에서 대미지 타입 ,옵션 뽑아내서 저장
                        whirlwindRushdata.optionliast.Add(whirlwindRushdata_option.Item1);
                        whirlwindRushdata.attackdamagetype.Add(whirlwindRushdata_option.Item2);
                        break;
                    case "GuardianShield":
                        GuardianShieldData guardianshielddata = new GuardianShieldData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),
                            CHGValue_DamageShieldRate = MakeToInt(valueMap["CHGValue_DamageShieldRate"]),
                        };
                        guardianshielddata.CHGValue.Add(guardianshielddata.CHGValue_UseMP.ToString());
                        guardianshielddata.CHGValue.Add(guardianshielddata.CHGValue_SkillDuration.ToString());
                        guardianshielddata.CHGValue.Add(guardianshielddata.CHGValue_DamageShieldRate.ToString());

                        guardianshielddata.CHGName.AddRange(GetCHGHeader(header));

                        skillDataList.Add(guardianshielddata); // 리스트에 추가
                        break;
                    case "Electricshock":
                        ElectricshockData electricshockdata = new ElectricshockData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),

                            CHGValue_DMG_MagicPower_Magic = MakeTofloat(valueMap["CHGValue_DMG_MagicPower_Magic"]),
                        };

                        electricshockdata.CHGValue.Add(electricshockdata.CHGValue_UseMP.ToString());
                        electricshockdata.CHGValue.Add(electricshockdata.CHGValue_SkillDuration.ToString());
                        electricshockdata.CHGValue.Add(electricshockdata.CHGValue_DMG_MagicPower_Magic.ToString());
                        electricshockdata.DMGValue.Add(electricshockdata.CHGValue_DMG_MagicPower_Magic);

                        var electricshockdata_option = Utill_Standard.ParseInput("CHGValue_DMG_MagicPower_Magic"); //이름에서 대미지 타입 ,옵션 뽑아내서 저장
                        electricshockdata.optionliast.Add(electricshockdata_option.Item1);
                        electricshockdata.attackdamagetype.Add(electricshockdata_option.Item2);
                        electricshockdata.CHGName.AddRange(GetCHGHeader(header));
                        electricshockdata.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(electricshockdata); // 리스트에 추가
                        break;
                    case "DokkaebiFire":
                        DokkaebiFireData dokkaebiFireData = new DokkaebiFireData
                        {
                            SkillName = file.name,
                            Level = MakeToInt(valueMap["Level"]),
                            REsourceType = ParseEnum<Utill_Enum.Resource_Type>(valueMap["ResourceType"]),
                            ResourceCount = MakeToInt(valueMap["ResourceCount"]),
                            LevelLimit = MakeToInt(valueMap["LevelLimit"]),
                            CHGValue_UseMP = MakeToInt(valueMap["CHGValue_UseMP"]),
                            CHGValue_SkillDuration = MakeTofloat(valueMap["CHGValue_SkillDuration"]),

                            CHGValue_DMG_HP_Magic = MakeTofloat(valueMap["CHGValue_DMG_HP_Magic"])
                        };

                        dokkaebiFireData.CHGValue.Add(dokkaebiFireData.CHGValue_UseMP.ToString());
                        dokkaebiFireData.CHGValue.Add(dokkaebiFireData.CHGValue_SkillDuration.ToString());
                        dokkaebiFireData.CHGValue.Add(dokkaebiFireData.CHGValue_DMG_HP_Magic.ToString());
                        dokkaebiFireData.DMGValue.Add(dokkaebiFireData.CHGValue_DMG_HP_Magic);


                        dokkaebiFireData.CHGName.AddRange(GetCHGHeader(header));
                        dokkaebiFireData.DMGName.AddRange(GetDMGHeader(header));

                        skillDataList.Add(dokkaebiFireData); // 리스트에 추가

                        var dokkaebiFireData_option = Utill_Standard.ParseInput("CHGValue_DMG_HP_Magic"); //이름에서 대미지 타입 ,옵션 뽑아내서 저장
                        dokkaebiFireData.optionliast.Add(dokkaebiFireData_option.Item1);
                        dokkaebiFireData.attackdamagetype.Add(dokkaebiFireData_option.Item2);
                        break;
                }
            }

            // 스킬 이름을 키로 하고, 스킬 데이터를 리스트로 저장
            if (!skillDictionary.ContainsKey(file.name))
            {
                skillDictionary[file.name] = new List<BaseSkillData>();
            }
            skillDictionary[file.name].AddRange(skillDataList); // 리스트에 스킬 데이터 추가
        }

        return skillDictionary;
    }
    #endregion


    /// <summary>
    /// csv의 헤더중 chg 값인 요소들의 이름 리스트 반환
    /// </summary>
    private static List<string> GetCHGHeader(string[] headers)
    {
        List<string> chgName = new List<string>();
        for (int j = 0; j < headers.Length; j++)
        {
            if (headers[j].Contains("CHGValue_"))
            {
                chgName.Add(headers[j]);
            }
        }
        return chgName;
    }
    /// <summary>
    /// csv의 헤더중 dmg 값인 요소들의 이름 리스트 반환
    /// </summary>
    private static List<string> GetDMGHeader(string[] headers)
    {
        List<string> dmgName = new List<string>();
        for (int j = 0; j < headers.Length; j++)
        {
            if (headers[j].Contains("DMG_"))
            {
                dmgName.Add(headers[j]);
            }
        }
        return dmgName;
    }
    #endregion

}

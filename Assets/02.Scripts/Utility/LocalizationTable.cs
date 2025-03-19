using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-30
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 로컬라이제이션 기능
/// </summary>
public enum ELanguage
{
    KR, EN
}
public static class LocalizationTable
{
    public static Dictionary<string, Dictionary<ELanguage, string>> localizationDict =
    new Dictionary<string, Dictionary<ELanguage, string>>();
    public static ELanguage currentLanguage { get; set; }
    public static Action languageSettings;

    public static bool isInit = false;

    /// <summary>
    /// 주어진 키에 대해 현재 설정된 언어에 해당하는 번역된 문자열을 반환합니다.
    /// </summary>
    public static string Localization(string key)
    {
        if (!localizationDict.ContainsKey(key))
            return key;

        if (!localizationDict[key].ContainsKey(currentLanguage))
            return currentLanguage.ToString();

        return localizationDict[key][currentLanguage];
    }
    
    public static void SetLanguage(ELanguage lang)
    {
        currentLanguage = lang;
        languageSettings?.Invoke();
        Debug.Log("언어 변경: " + lang.ToString()) ;
        PlayerPrefs.SetString("Language", lang.ToString());
    }
}

using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-15
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 암호화 되있는 변수들 RandomizeCryptoKey 실행
/// </summary>
public class GameDataManager : MonoBehaviour
{
    void Start()
    {
        // 최초 한번 작동
        InvokeRepeating("RandomizeKey_UserData", 0.20f, Random.Range(1.0f, 1.0f));      //(유저데이터)  n~n초 간격으로 암호화 변경
        InvokeRepeating("RandomizeKey_Inventory", 0.25f, Random.Range(1.0f, 1.0f));     //(인벤)        n~n초 간격으로 암호화 변경
        InvokeRepeating("RandomizeKey_Option", 0.25f, Random.Range(1.0f, 1.0f));     //(옵션)        n~n초 간격으로 암호화 변경
    }

    public void RandomizeKey_Inventory()
    {
        InventoryManager.Instance.RandomizeKey_Inventory();
    }
    public void RandomizeKey_UserData() { GameDataTable.Instance.User.RandomizeKey_UserData(); }

    public void RandomizeKey_Option()
    {
        foreach (Option option in System.Enum.GetValues(typeof(Option)))
        {
            int key = ObscuredInt.GenerateKey(); // 각 옵션에 대한 새로운 키 생성

            ObscuredInt encryptedOption = ObscuredInt.Encrypt((int)option, key); // 옵션을 암호화
            //int decryptedValue = ObscuredInt.Decrypt(encryptedOption, key); // 암호화된 값을 복호화하여 확인

            char[] charKey = ObscuredString.GenerateKey(); // 새로운 키 생성
            string charKeyString = new string(charKey);
            char[] encryptedOptionStringChars = ObscuredString.Encrypt(option.ToString(), charKeyString); // 옵션을 암호화
            //string a = new string(encryptedOptionStringChars);
            //string encryptedOptionString = ObscuredString.Decrypt(encryptedOptionStringChars, charKeyString); // 암호화된 문자열 복호화

            // 출력
            //Debug.Log($"{option}: {a} (Key: {key})   ssss    {encryptedOptionString}");
          //  Debug.Log($"{option}: {encryptedOption} (Key: {key})   ssss    {encryptedOptionString}");
        }
    }

    /// <summary>
    /// 암호화 되있는 변수 치트 프로그램으로 수정시 불리는 함수
    /// </summary>
    public void CachedMemory()
    {
        Debug.Log("CachedMemory");
        BackendManager.Instance.SendLogToServer("MemoryChange", "메모리 변조 의심", "MemoryChange");
    }
}

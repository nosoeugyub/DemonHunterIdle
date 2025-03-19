using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 작성일자   : 2024-07-31
/// 작성자     : 민영 (gksalsdud1234@gmail.com)                                            
/// 클래스용도 : 닉네임 UI 관리 팝업 
/// </summary>
public class NicknamePopUp : MonoBehaviour, IPopUp
{
    [SerializeField] private TMP_InputField nicknameInputField = null; // 닉네임 입력 필드
    
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button reNicknameBtn;
    private bool isInit = false;

    private void Awake()
    {
        if (!isInit)
        {
            Initilize();
        }
    }

    private void Initilize()
    {
        Time.timeScale = 1;

        LanguageSetting();
        OnClickEventSetting();

        isInit = true;
    }
    private void OnClickEventSetting()
    {
        confirmBtn.onClick.AddListener(SetNickName);
        reNicknameBtn.onClick.AddListener(SetAutoNickname);
    }

    private void SetAutoNickname()
    {
        string randomAutoNickname = Utility.GenerateRandomUsername();
#if UNITY_EDITOR
        randomAutoNickname = "테스터" + Random.Range(10000, 99999);
#endif
        nicknameInputField.text = randomAutoNickname;
        Debug.Log(randomAutoNickname);
    }

    void LanguageSetting()
    {
        (nicknameInputField.placeholder as TextMeshProUGUI).text = LocalizationTable.Localization("EnterNickname");

        if (confirmBtn != null)
        {
            confirmBtn.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationTable.Localization("Button_Check");
        }

        LocalizationTable.languageSettings += LanguageSetting;
    }
    public void SetNickName()
    {
        string nickname = nicknameInputField.text;

        Utility.LoadNicknameFilter();

        if (NickNameCheck(nickname) == false) //닉네임 제한 뒤끝에서 제공해주는 기능 + 알파 
        {
            return;
        }

        LoginManager.Instance.UpdateNickname(nickname);
    }

    private bool NickNameCheck(string nickname)
    {
        if (nickname.Length > 8 || nickname.Contains(" ")) // + 알파 자릿수 변경
        {
            LoginManager.Instance.OnNotice("400");
            return false;
        }
        if (Utility.NicknameFiltering(nickname)) //욕
        {
            LoginManager.Instance.OnNotice("Filter");
            return false;
        }

        return true;
    }
    public void Show()
    {
        Utility.LoadAutoNickname();
        string randomAutoNickname = Utility.GenerateRandomUsername();
#if UNITY_EDITOR
        randomAutoNickname = "테스터" + Random.Range(10000, 99999);
#endif
        nicknameInputField.text = randomAutoNickname;
        gameObject.SetActive(true);
    }


    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

  

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPopUp : MonoBehaviour,IPopUp
{
    [SerializeField]
    private ChatScrollController chatScrollController = null;
    [SerializeField]
    private TMP_InputField debugInputField = null; //개발자용 인풋 필드
    [SerializeField]
    private List<Button> mobileKeyboardBtnList;

    public ChatScrollController ChatScrollController { get { return chatScrollController; } }

    private TouchScreenKeyboard keyboard; //가상키보드
    private string inputText; //가상키보드로 작성한 내용

    private int ClickCount = 0;

    private void Start()
    {
        for(int i = 0; i < mobileKeyboardBtnList.Count; i++)
        {
            mobileKeyboardBtnList[i].onClick.RemoveAllListeners();
            mobileKeyboardBtnList[i].onClick.AddListener(()=>ShowMobileKeyboard());
        }
#if UNITY_EDITOR
        debugInputField.gameObject.SetActive(true);
        debugInputField.GetComponent<CanvasGroup>().alpha = 1;
        debugInputField.GetComponent<CanvasGroup>().blocksRaycasts = true;
        debugInputField.GetComponent<CanvasGroup>().interactable = true;
        debugInputField.onSubmit.AddListener((string str)=> {
            ChatManager.Instance.ChatMsgWithoutChannelInfo(str);
            debugInputField.text = string.Empty;
            debugInputField.ActivateInputField();
        });
#else
        debugInputField.gameObject.SetActive(true);
        debugInputField.GetComponent<CanvasGroup>().alpha = 0;
        debugInputField.GetComponent<CanvasGroup>().blocksRaycasts = false;
        debugInputField.GetComponent<CanvasGroup>().interactable = false;
        debugInputField.onSubmit.AddListener((string str)=> {
            ChatManager.Instance.ChatMsgWithoutChannelInfo(str);
            debugInputField.text = string.Empty;
        });
#endif
    }

    private void Update()
    {
        //// 가상 키보드가 열려 있고 유저가 입력을 완료했다면
        //if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done && keyboard.active != false)
        //{
        //    inputText = keyboard.text;
        //    ChatManager.Instance.ChatMsgWithoutChannelInfo(inputText);
        //    keyboard.text = string.Empty;
        //    // 키보드 닫기
        //    Debug.Log("done");
        //    keyboard.active = false;
        //}
        //if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.LostFocus && keyboard.active != false)
        //{
        //    Debug.Log("lost focus");
        //}
    }

    public void Show()
    {
        ClickCount++;
        if (ClickCount == 1)
        {
            PopUpSystem.Instance.EscPopupListAdd(this);
            gameObject.transform.localPosition = new Vector3(0, 0, 0);
            chatScrollController.ResizeScroller();
            ChatManager.Instance.RecentChat.gameObject.SetActive(false);
        }
        else
        {
            Hide();
        }
    }

    public void Hide()
    {
        ClickCount = 0;
        PopUpSystem.Instance.EscPopupListRemove();
        ChatManager.Instance.RecentChat.gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector3(9999, 9999, 9999);
    }

    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void ShowMobileKeyboard()
    {
#if UNITY_EDITOR
        if (debugInputField.text == string.Empty) return;
        ChatManager.Instance.ChatMsgWithoutChannelInfo(debugInputField.text);
        debugInputField.text = string.Empty;
        debugInputField.ActivateInputField();
#else
        debugInputField.ActivateInputField();
#endif
    }
}

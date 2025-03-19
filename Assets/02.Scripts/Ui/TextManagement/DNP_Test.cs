using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : DNP_MainScene씬에서 사용하는 텍스트 에셋 테스트 스크립트
/// </summary>
[RequireComponent(typeof(NoticeManager))]
public class DNP_Test : MonoBehaviour
{
    [SerializeField]
    private NoticeType type;

    public bool ShowAddPercent = false;

    private void Start()
    {
        NoticeManager.Instance.GetGameSequence(Enum_GameSequence.CreateAndInit);
    }
    void Update()
    {
        //On leftclick.
        if (Input.GetMouseButtonDown(0))
        {
            if(!ShowAddPercent)
            {
                NoticeManager.Instance.SpawnNoticeText(type,transform.position, Random.Range(1, 500));
            }
            else
            NoticeManager.Instance.SpawnNoticeText(type, transform.position, Random.Range(1, 500),Random.Range(10,100));
        }
        //재화 테스트
        if(Input.GetKeyDown(KeyCode.G)) 
        {
            NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.Exp, transform.position, Random.Range(1, 500));
            NoticeManager.Instance.SpawnNoticeText(Utill_Enum.NoticeType.Gold, transform.position, ResourceManager.Instance.GetGold());

        }
    }
}

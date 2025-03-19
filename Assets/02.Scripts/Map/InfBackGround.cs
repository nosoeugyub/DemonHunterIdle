using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// 작성일자   : 2024-05-22
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 지형 스크립트 몬스터 스폰관련
/// </summary>
public class InfBackGround : MonoBehaviour
{
    public static InfBackGround inst = null;  //속도 맞추기 위해 임시 싱글턴 사용

    [SerializeField] private bool _isbattle ;

    public bool IsBattle
    {
        get { return _isbattle; }
        set { _isbattle = value; }
    }

    public GameObject BackgroundPrefab;
    public float BackgroundHight;
    [SerializeField]
    private float ScrollSpeed = 4;

    public float GetScrollSpeed => ScrollSpeed;

    private GameObject[] backgrounds;
    private Transform playerTransform;
    [SerializeField] private Transform ParentObject;

    private void Awake()
    {
        inst = this;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        InitializeBackgrounds();

        GameEventSystem.GameSequence_SendGameEventHandler += StartScroll;
        GameEventSystem.GameBattleSequence_SendEventHandler += ((bool isBattle)=> IsBattle = isBattle);
    }

    private void Update()
    {
        ScrollBackgrounds(IsBattle);
        CheckAndMoveBackground();
    }

    private bool StartScroll(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            //case Utill_Enum.Enum_GameSequence.CreateAndInit:  //나중에 이속 확정되면 static으로 init 해주기 위한 용도
            //    Speed = ScrollSpeed;
            //    return true;
            case Utill_Enum.Enum_GameSequence.InGame:
                IsBattle = true;
                return true;
        }
        return false;
    }

    private void InitializeBackgrounds()
    {
        // 화면 너비에 따라 배경의 개수 결정
        int numberOfBackgrounds = Mathf.CeilToInt(Camera.main.aspect) + 2;
        // 배경 배열 초기화
        backgrounds = new GameObject[numberOfBackgrounds];

        // 배경 오브젝트 생성
        for (int i = 0; i < numberOfBackgrounds; i++)
        {
            Vector3 spawnPosition = new Vector3(0, 0, i * BackgroundHight);
            backgrounds[i] = Instantiate(BackgroundPrefab, spawnPosition, Quaternion.identity);
            backgrounds[i].transform.SetParent(ParentObject, false);
            backgrounds[i].transform.eulerAngles = new Vector3 (90, 0, 0);
        }
    }

    private void ScrollBackgrounds(bool isBattle)
    {
        if (!isBattle)
        {
            foreach (var background in backgrounds)
            {
                background.transform.Translate(Vector3.down * ScrollSpeed * Time.deltaTime);
            }
        }
        
    }

    //backgrounds 배열들의 오브젝트중 가장 첫번재 오브젝트가 화면을 넘어가면 해당 오브젝트위치를 제일상단으로 움직여주는로직
    private void CheckAndMoveBackground()
    {
        float bottomYPosition = backgrounds[0].transform.position.z;
        float topYPosition = backgrounds[backgrounds.Length - 1].transform.position.z;

        // 첫 번째 배경이 화면 밖으로 벗어나면
        if (bottomYPosition + BackgroundHight < playerTransform.position.z - Camera.main.orthographicSize)
        {
            // 첫 번째 배경을 맨 위로 이동
            MoveBackground(backgrounds[0], topYPosition + BackgroundHight);

            // 맨 마지막 배경을 첫 번째로 옮겨줌
            GameObject temp = backgrounds[0];
            for (int i = 0; i < backgrounds.Length - 1; i++)
            {
                backgrounds[i] = backgrounds[i + 1];
            }
            backgrounds[backgrounds.Length - 1] = temp;
        }
    }

    private void MoveBackground(GameObject background, float newYPosition)
    {
        background.transform.position = new Vector3(0, 0, newYPosition);
    }
}

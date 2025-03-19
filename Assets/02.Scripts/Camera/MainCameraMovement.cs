using NSY;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 전투를 보여주는 카메라의 움직임을 제어
/// </summary>
public class MainCameraMovement : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [Range(0f, 10f)]
    [SerializeField]
    private float clampX = 5f;

    [Header("평상시 헌터-카메라간의 기준 조절")]
    [SerializeField]
    private Vector3 firstVector = Vector3.zero;
    [Header("휴식시 헌터-카메라간의 기준 조절")]
    [SerializeField]
    private Vector3 restModeVector = Vector3.zero;
    [Header("카메라의 이동 속도")]
    [SerializeField]
    private float lerpSpeed = 5f;
    [Header("캐릭터 재스폰시 갱신 속도")]
    [SerializeField]
    private float hunterRenewSpeed = 3f;
    [Header("헌터 스폰 범위")]
    [SerializeField]
    private BoxCollider spawnRangeCollider = null;

    private Vector3 moveZVector = Vector3.zero;
    private Vector3 moveXVector = Vector3.zero;

    private bool isLimitMove = false;
    private Vector2 limitGroundZ = Vector2.zero;

    private List<Hunter> calculateHunter = new();

    public bool CanMove = true; //false면 움직이는 로직 정지


    private void Awake()
    {
        moveZVector = Vector3.forward;
        moveXVector = Vector3.right;
        GameEventSystem.GameSequence_SendGameEventHandler += DataLoad;
        //firstVector = cam.transform.position;
    }

    private void LateUpdate()
    {
        if (!CanMove) return;
        // Lerp를 사용하여 부드럽게 카메라를 이동
        cam.transform.position = Vector3.Lerp(cam.transform.position, CalculatePos(), lerpSpeed * Time.deltaTime);
    }


    private bool DataLoad(Utill_Enum.Enum_GameSequence GameSequence)
    {
        switch (GameSequence)
        {
            case Utill_Enum.Enum_GameSequence.DataLoad:
                isLimitMove = false;
                CanMove = true;
                limitGroundZ = Utill_Standard.Vector2Zero;
                return true;
        }
        return true;
    }

    /// <summary>
    /// 카메라 이동 위치를 그라운드 기준으로 변경
    /// </summary>
    public void LimitCameraPos(Ground ground)
    {
        isLimitMove = true;
        //limitGround=ground;
        Bounds bounds = ground.Plane.bounds;
        //카메라는 상하 추가 제한
        limitGroundZ = new Vector2(bounds.min.z - 10f, bounds.max.z - 10f);
    }
    public void UnLimitCameraPos()
    {
        isLimitMove = false;
    }
    /// <summary>
    /// 카메라 위치 재설정
    /// </summary>
    public void CalculateCamPos()
    {
        cam.transform.position = CalculatePos();
    }
    /// <summary>
    /// 카메라 위치 계산
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculatePos()
    {
        Vector3 targetPosition = Utill_Standard.Vector3Zero;

        for (int i = 0; i < calculateHunter.Count; i++)
        {
            if (calculateHunter[i].gameObject.activeInHierarchy == false)
                RemoveCalculateHunter(calculateHunter[i]);
        }
        Vector3 camCorrectionVal = Utill_Standard.Vector3Zero;

        if (IdleModeRestCycleSystem.Instance.IsSitCamp()) //현재 캠프에 앉았다면
        {
            camCorrectionVal = restModeVector;
        }
        else
        {
            camCorrectionVal = firstVector;
        }

        if (isLimitMove)
        {
            float clampedZ = Mathf.Clamp(GetHuntersMidPosition().z, limitGroundZ.x, limitGroundZ.y);
            targetPosition = camCorrectionVal + moveZVector * clampedZ + moveXVector * 0;
        }
        else
        {
            float clampedX = Mathf.Clamp(GetHuntersMidPosition().x, -1f * clampX, clampX);
            //float clampedZ = Mathf.Clamp( GetHuntersMidPosition().z, -1f * clampZ,clampZ);
            targetPosition = camCorrectionVal + moveZVector * GetHuntersMidPosition().z + moveXVector * clampedX;
        }
        return targetPosition;
    }
    public void ClearCalculateHutner()
    {
        calculateHunter.Clear();
    }
    /// <summary>
    /// 딜레이가 필요하지 않을 경우에 사용
    /// </summary>
    public void InitCalculateHunter(Hunter hunter)
    {
        calculateHunter.Add(hunter);
    }
    public void RemoveCalculateHunter(Hunter hunter)
    {
        if (!calculateHunter.Contains(hunter)) return;
        calculateHunter.Remove(hunter);
        if(calculateHunter.Count <= 0 && waitHunterList.Count>0) //기존 헌터가 다 사망하였다면
        {
            StopCoroutine(WaitforInit(waitHunterList[0]));
            calculateHunter.Add(waitHunterList[0]); //지연시간 없이 바로 넣어줌
        }

    }
    List<Hunter> waitHunterList = new();
    public void AddCalculateHunter(Hunter hunter)
    {
        if (calculateHunter.Contains(hunter)) return;
        if(waitHunterList.Count > 0) //누군가 미리 기다리고 있다면
        {
            calculateHunter.AddRange(waitHunterList); //그냥 싹 넣어줌
            waitHunterList.Clear();
        }
        StopCoroutine(WaitforInit(hunter));
        StartCoroutine(WaitforInit(hunter));
        waitHunterList.Add(hunter);
    }
    private IEnumerator WaitforInit(Hunter hunter)
    {
        yield return new WaitForSeconds(hunterRenewSpeed);
        calculateHunter.Add(hunter);
        waitHunterList.Remove(hunter);
    }

    public Vector3 GetCurrentCamPosition()
    {
        if(GameDataTable.Instance.User.CurrentEquipHunter.Count == 0)
            return Vector3.zero;
        return DataManager.Instance.Hunters[(int)GameDataTable.Instance.User.CurrentEquipHunter[0]].transform.position + (Vector3.forward * 1.2f);
    }

    public Vector3 GetSpawnPosition()
    {
        Vector3 tmpSpawnPosition = Utill_Standard.GetRandomPositionInCollider(spawnRangeCollider);
        tmpSpawnPosition.y = 0;
        return tmpSpawnPosition;
    }

    public Vector3 GetHuntersMidPosition()
    {
        List<Vector3> hunterVec = new();
        for(int i = 0; i < DataManager.Instance.Hunters.Count; i++)
        {
            if (calculateHunter.Contains(DataManager.Instance.Hunters[i]))
            {
                hunterVec.Add(DataManager.Instance.Hunters[i].transform.position);
            }
            else
            {
                hunterVec.Add(Vector3.zero);
            }
        }
        return Utill_Math.CalculateMidpoint(hunterVec[0], hunterVec[1], hunterVec[2]);
    }
}

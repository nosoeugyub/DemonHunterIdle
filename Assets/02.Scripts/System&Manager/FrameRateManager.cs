using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 작성일자   : 2024-09-24
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 렌더링 프레임을 제어
/// </summary>
public class FrameRateManager : MonoSingleton<FrameRateManager>
{
    private int lastRequestedFrame = 0;
    private int fullFrameCount = 30;

    public bool UseLimitedFrame = false; //프레임 조절 여부

    protected override void Awake()
    {
        base.Awake();
        //초당 렌더링 횟수
        Application.targetFrameRate = fullFrameCount;
        //렌더링을 몇 프레임마다 시킬것인지
        //1이면 1프레임마다 렌더링을 함
        OnDemandRendering.renderFrameInterval = 1;
    }

    private void Update()
    {
        if (UseLimitedFrame)
        {
            if (Time.frameCount - lastRequestedFrame < 3)
                OnDemandRendering.renderFrameInterval = 1;  //fullFrameCount fps
            else
                OnDemandRendering.renderFrameInterval = fullFrameCount; //1 fps
        }
    }
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        OnDemandRendering.renderFrameInterval = 1; //종료시에는 기존 프레임으로 변경해 추후 영향을 미치지 않도록
    }

    /// <summary>
    /// 프레임이 줄어든 상태에서 일시적으로 풀 프레임을 사용해야하는 경우
    /// (ex 유저입력) 호출하는 함수
    /// </summary>
    public void RequestFullFrameRate()
    {
        lastRequestedFrame = Time.frameCount;
    }

    /// <summary>
    /// 프레임 조절 시작
    /// </summary>
    public void StartLimitFrame()
    {
        UseLimitedFrame = true;
        OnDemandRendering.renderFrameInterval = fullFrameCount;
    }

    /// <summary>
    /// 프레임 조절 종료
    /// </summary>
    public void EndLimitFrame()
    {
        UseLimitedFrame = false;
        OnDemandRendering.renderFrameInterval = 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 집중 스킬 사용시 나오는 이펙트 객체
/// </summary>
public class FocusParticle : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem mainParticle;
    [SerializeField]
    private ParticleSystem subParticle;

    public void SetEffectAlpa(float alpaAmount)
    {
        alpaAmount = Mathf.Clamp01(alpaAmount);

        Color prevColor = subParticle.main.startColor.color;
        ParticleSystem.MainModule tmpParticle = subParticle.main;
        tmpParticle.startColor = new Color(prevColor.r, prevColor.g, prevColor.b, alpaAmount);

        Color prevColorMain = mainParticle.main.startColor.color;
        ParticleSystem.MainModule tmpMainParticle = mainParticle.main;
        tmpMainParticle.startColor = new Color(prevColor.r, prevColor.g, prevColor.b, alpaAmount);
    }
    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만
        CancelInvoke();     // Monobehaviour에 Invoke가 있다면
    }
}

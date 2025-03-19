using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utill_Enum;

/// <summary>
/// 작성일자   : 2024-07-19
/// 작성자     : 예은 (OhSolDev@gmail.com)                                                      
/// 클래스용도 : 가시덫의 장판
/// </summary>
public class SpikeTrapAOE : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particle;

    private List<Enemy> debuffEnemyList = new();

    private void OnEnable()
    {
        Invoke(nameof(DeactiveDelay), SkillManager.Instance.spikeTrap.AoeDuration);
    }
    void DeactiveDelay() => gameObject.SetActive(false);
    private void OnDisable()
    {
        for(int i = 0; i < debuffEnemyList.Count; i++)
        {
            RemoveDebuff(debuffEnemyList[i]);
            debuffEnemyList.Remove(debuffEnemyList[i]);
        }
        debuffEnemyList.Clear();
        ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만
        CancelInvoke();    // Monobehaviour에 Invoke가 있다면 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag.Enemy))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (debuffEnemyList.Contains(enemy))
                return;
            // 범위 안에 들어온 적에게 디버프 적용
            ApplyDebuff(enemy);
            debuffEnemyList.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tag.Enemy))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (!debuffEnemyList.Contains(enemy))
                return;
            // 범위 밖으로 나간 적에게서 디버프 제거
            RemoveDebuff(enemy);
            debuffEnemyList.Remove(enemy);
        }
    }

    private void ApplyDebuff(Enemy enemy)
    {
        EnemyStat.UseSkillMinusEnemyStat(enemy._EnemyStat, SkillManager.Instance.spikeTrap.SpeedDebuffAmount, Upgrade_Type.PhysicalPowerDefense);
        EnemyStat.UseSkillMinusEnemyStat(enemy._EnemyStat, SkillManager.Instance.spikeTrap.DefenseDebuffAmount, Upgrade_Type.MoveSpeedPercent);
    }
    private void RemoveDebuff(Enemy enemy)
    {
        EnemyStat.UseSkillAddEnemyStat(enemy._EnemyStat, SkillManager.Instance.spikeTrap.SpeedDebuffAmount, Upgrade_Type.PhysicalPowerDefense);
        EnemyStat.UseSkillAddEnemyStat(enemy._EnemyStat, SkillManager.Instance.spikeTrap.DefenseDebuffAmount, Upgrade_Type.MoveSpeedPercent);
    }
}

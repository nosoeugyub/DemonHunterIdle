using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// 작성일자   : 2024-05-22
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 지형 스크립트 몬스터 스폰관련
/// </summary>
public class Cground : Ground
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Hunter_"))
        {
            CheckPlayer();
        }
    }


    public void CheckPlayer()
    {
        if (IsArriveW)
        {
            return;
        }
        else
        {
            IsArriveW = true;
        }
    }


    void OnDisable()
    {
        //    ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만 
        //  CancelInvoke();    // Monobehaviour에 Invoke가 있다면 
    }
}

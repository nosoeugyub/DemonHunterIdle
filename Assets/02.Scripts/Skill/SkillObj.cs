using Lightning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-05-31
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스킬오브젝트 풀링 용
/// </summary>
public class SkillObj : MonoBehaviour
{



    #region 감전용
    [SerializeField] private LightningObject lightningObject;

    public void SetLinePosition(Vector3 originPos, Vector3 endPos)
    {

        originPos.y = 1f;
        endPos.y = 1f;
        transform.position = originPos;
        lightningObject.Show(endPos);
    }


    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만 
        CancelInvoke();    // Monobehaviour에 Invoke가 있다면 
    }
    #endregion
}

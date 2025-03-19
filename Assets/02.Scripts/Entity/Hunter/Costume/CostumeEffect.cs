using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성일자   : 2024-07-05
/// 작성자     : 예은 (OhSolDev@gmail.com)
/// 클래스용도 : 헌터의 장비/커마에 부착된 이펙트 객체
/// </summary>
public class CostumeEffect : MonoBehaviour
{
    [SerializeField]
    private Vector3 effectPos = Vector3.zero;

    public void Setting(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = effectPos;

        //layer setting
        Transform[] children = transform.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = parent.gameObject.layer;
        }
        gameObject.layer = parent.gameObject.layer;
    }
}

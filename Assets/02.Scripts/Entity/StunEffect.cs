using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// 작성일자   : 2024-07-18
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스턴이펙트 진짜 이펙트만 돌아가는로직
/// </summary>
public class StunEffect : MonoBehaviour
{
    private static Camera mainCamera;
    [SerializeField]
    protected Image stunImage = null;
    [SerializeField]
    protected RectTransform rectTransform;

    protected Transform targetTransform;
    protected Vector3 offset;
    protected bool isfly;

    [Header("스턴 무늬 돌아가는 속도, -가 시계방향")]
    [SerializeField]
    private float rotVelocity = -20;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(Spin());
        StartCoroutine(Move());
    }

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만 
        CancelInvoke();    // Monobehaviour에 Invoke가 있다면 
        targetTransform = null;
    }
    public void Initialize(Transform _targetTransform, Vector3 _offset, bool _isfly = false)
    {
        gameObject.SetActive(true);
        targetTransform = _targetTransform;
        offset = _offset;
        isfly = _isfly;
        // rectTransform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
    }

    IEnumerator Spin()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            Vector3 rot = new Vector3(0, 0, rotVelocity);
            rectTransform.Rotate(rot, Space.Self);
        }
    }
    IEnumerator Move()
    {
        var delay = new WaitForEndOfFrame();
        // Debug.Log(offset.ToString());
        while (true)
        {
            if (targetTransform)
            {
                //오브젝트에 가리지 않도록 y 방향으로 올려줌.
                rectTransform.position = targetTransform.position + Vector3.up + offset;
            }

            if (isfly)
                rectTransform.position += new Vector3(0, 0.5f, 0);
            yield return delay;
        }
    }


}

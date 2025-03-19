using UnityEngine;
using System.Collections;

public class csDestroyEffect : MonoBehaviour 
{
    public bool autoDeactive = false;
    public float deactiveTime = 1f;

    private void OnEnable()
    {
        if(autoDeactive)
            Invoke(nameof(DeactiveDelay), deactiveTime);
    }

    void DeactiveDelay() => gameObject.SetActive(false);
    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만
        CancelInvoke();     // Monobehaviour에 Invoke가 있다면
    }
}

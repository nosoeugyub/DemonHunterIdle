using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUIInBuild : MonoBehaviour
{
    public GameObject timeSpeedObj;

    private void Start()
    {
#if !UNITY_EDITOR 
        timeSpeedObj.gameObject.SetActive(false);
#endif
    }
}

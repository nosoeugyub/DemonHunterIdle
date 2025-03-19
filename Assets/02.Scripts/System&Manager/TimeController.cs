using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TimeController : MonoBehaviour
{
    private int minTimeScale = 0;
    private int maxTimeScale = 20;

    private int currentTimeSpeed = 1;
    [SerializeField] private TextMeshProUGUI currentTimeText = null;

    private void Start()
    {
        currentTimeSpeed = 1;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(InputManager.Instance.speedUpKey))
        {
            currentTimeSpeed += 1;
            currentTimeSpeed = Mathf.Clamp(currentTimeSpeed, minTimeScale, maxTimeScale); 
         
            Time.timeScale = currentTimeSpeed;

            SetTimeText();
        }

        if (Input.GetKeyDown(InputManager.Instance.speedDownKey))
        {
            currentTimeSpeed -= 1;
            currentTimeSpeed = Mathf.Clamp(currentTimeSpeed, minTimeScale, maxTimeScale);
           
            Time.timeScale = currentTimeSpeed;
          
            SetTimeText();
        }
#endif 
    }

    private void SetTimeText()
    {
        currentTimeText.SetText($"x{currentTimeSpeed}");
    }
}

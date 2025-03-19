using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Lightning;

public class ExampleTimeScaler : BaseLightningTimer
{
    public Text timeScaleText;

    /// <summary>
    ///     You can implement anything you wan, even such useless code
    /// </summary>
    /// <returns></returns>
    public override float GetDeltaTime()
    {
        return Random.Range(0.001f, 0.1f);
    }

    public void SetTimeScale(Slider slider)
    {
        Time.timeScale = slider.value;
        this.OnEnable();
    }

    private void OnEnable()
    {
        timeScaleText.text = string.Format("Current time scale:{0:0.00}", Time.timeScale);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}

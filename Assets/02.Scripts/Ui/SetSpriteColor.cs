using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetSpriteColor : MonoBehaviour
{
    public Color NormalColor;
    public Color SuperiorColor;
    public Color RareColor;
    public Color UniqueColor;
    public Color EpicColor;


    [SerializeField] Color _color;
    public Color Color
    {
        get { return _color; }
        set { _color = value; }
    }


    [SerializeField] Image _image;
    public Image _Image
    {
        get { return _image; }
        set { _image = value; }
    }

    public void SetColorOptionGradeSprite(Utill_Enum.Grade grade)
    {
        _Image.gameObject.SetActive(true);

        switch (grade)
        {
            case Utill_Enum.Grade.Normal:
                _Image.color = NormalColor;
                break;
            case Utill_Enum.Grade.Superior:
                _Image.color = SuperiorColor;
                break;
            case Utill_Enum.Grade.Rare:
                _Image.color = RareColor;
                break;
            case Utill_Enum.Grade.Unique:
                    _Image.color = UniqueColor;
                break;
            case Utill_Enum.Grade.Epic:
                    _Image.color = EpicColor;
                break;
        }
    }
}

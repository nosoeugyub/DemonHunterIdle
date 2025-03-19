using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
/// 작성일자   : 2024-06-4
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : 스프라이트르 받아와서 처리 하는 부분  
/// /// </summary>
public static class SpriteManager 
{
    static Dictionary<string, Sprite> uiSprites = new Dictionary<string, Sprite>();
    static Dictionary<string, Sprite> itemSprites = new Dictionary<string, Sprite>();

    static bool isLoadedSprites = false;

    public static Sprite GetUISprite(string key)
    {
        if (uiSprites.ContainsKey(key))
        {
            return uiSprites[key];
        }
        else
        {
            Debug.LogWarning("Null UI Sprite: " + key);
            return null;
        }
    }


    public static Sprite GetItemSprite(string key)
    {
        string trimKey = key.Trim();
        if (itemSprites.ContainsKey(trimKey))
        {
            return itemSprites[trimKey];
        }
        else
        {
            Debug.Log("Null Item Sprite : " + trimKey);
            return null;
        }
    }
    public static void OnLoadAllSprite()
    {
        return;

        //쓰이지 않음
        if (isLoadedSprites)
            return;

        OnLoadSpritesByPath("UI");
        OnLoadSpritesByPath("Items");


        isLoadedSprites = true;
    }

    static void OnLoadSpritesByPath(string _path)
    {
        string path = "Images/00.Atlas/" + _path;
        SpriteAtlas atlas = Resources.Load<SpriteAtlas>(path);
        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        for (int i = 0; i < sprites.Length; i++)
        {
            var name = sprites[i].name;

            string str = name.Replace("(Clone)", "");
            switch (_path)
            {
                case "UI":
                    uiSprites.Add(str, sprites[i]);
                    break;
                case "Items":
                    itemSprites.Add(str, sprites[i]);
                    break;
                default:
                    Debug.Log("해당 경로는 존재하지 않습니다.");
                    break;
            }
        }
    }

}

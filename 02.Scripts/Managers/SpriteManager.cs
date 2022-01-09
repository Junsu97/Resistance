using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
public class SpriteManager : Singleton<SpriteManager>
{
    public SpriteAtlas itemAtlas;
    public Sprite LoadItemImage(int itemNo)
    {
        return itemAtlas.GetSprite("img_" + itemNo);
    }
}

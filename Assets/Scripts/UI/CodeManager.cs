using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeManager : MonoBehaviour
{
    public List<Image> codeImages;

    public void CodeImageSetting(List<GameObject> players)
    {
        for(int i = 0; i < players.Count; i++)
        {
            Texture2D texture = players[i].GetComponent<PlayerStat>().currentCodeImage;
            if (texture != null)
            {
                Sprite sp = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                codeImages[i].sprite = sp;
            }
            else
            {
                Debug.LogWarning($"Player {i} does not have a currentCodeImage assigned.");
            }
        }
    }
}

using UnityEngine;

[System.Serializable]
public class CardData
{
    public Sprite image;
    public string effectText;

    public CardData(Sprite img, string text)
    {
        image = img;
        effectText = text;
    }
}

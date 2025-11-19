using UnityEngine;

[System.Serializable]
public class ListItemData
{
    public string id;
    public string title;
    public string description;
    public Sprite icon;
    public Color gradientColor1 = Color.blue;
    public Color gradientColor2 = Color.cyan;


    public ListItemData(string title, string description, Sprite icon)
    {
        this.id = System.Guid.NewGuid().ToString();
        this.title = title;
        this.description = description;
        this.icon = icon;
    }
}

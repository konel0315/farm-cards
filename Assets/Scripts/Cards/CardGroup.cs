using System.Collections.Generic;

[System.Serializable]
public class CardGroup
{
    public string Title;
    public string Name;
    public string TitleImage;
    public string Description;
    public int Priority;
    public int Weight;
    public bool OneTime;
    public int SeasonType;
    public List<CardOption> Options;
}
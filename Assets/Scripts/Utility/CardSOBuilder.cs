using System.Collections.Generic;
using System.IO;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEditor;

public static class CardSOBuilder
{
    [MenuItem("Tools/Build CardGroupsSO")]
    public static void BuildCardGroupsSO()
    {
        string csvPath = "Data/CardData.csv";
        List<CardData> allCards = ExcelLoader.LoadCardDataFromCSV(csvPath);
        SerializedDictionary<int, CardGroup> groups = new SerializedDictionary<int, CardGroup>();

        foreach (var card in allCards)
        {
            // GroupID가 null이면 스킵
            if (!card.GroupID.HasValue)
                continue;

            int groupId = card.GroupID.Value;

            if (!groups.TryGetValue(groupId, out var group))
            {
                group = new CardGroup
                {
                    Name = card.Name,
                    TitleImage = card.TitleImage,
                    Title = card.Title,
                    Description = card.Description,
                    Priority = card.Priority ?? 0,
                    Weight = card.Weight ?? 0,
                    OneTime = card.OneTime ?? false,
                    Options = new List<CardOption>()
                };
                groups.Add(groupId, group);
            }

            if (!string.IsNullOrEmpty(card.OptionText))
            {
                var option = group.Options.Find(o => o.OptionText == card.OptionText);
                if (option == null)
                {
                    option = new CardOption
                    {
                        OptionText = card.OptionText,
                        OptionImage = card.OptionImage,
                        Variants = new List<CardVariant>()
                    };
                    group.Options.Add(option);
                }

                option.Variants.Add(new CardVariant
                {
                    Probability = card.Probability ?? 1f,
                    Command = card.Command,
                    //NextCardID = card.CardID ?? -1
                });
            }
        }

        string soFolder = "Assets/Data/SO";
        if (!Directory.Exists(soFolder))
            Directory.CreateDirectory(soFolder);

        string soPath = Path.Combine(soFolder, "CardGroupsSO.asset");
        var soInstance = ScriptableObject.CreateInstance<CardGroupsSO>();
        soInstance.CardGroupsByID = groups;

        AssetDatabase.CreateAsset(soInstance, soPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("CardGroupsSO 생성 완료!");
    }
}

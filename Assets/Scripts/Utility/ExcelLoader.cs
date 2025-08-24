using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ExcelLoader
{
    public static List<CardData> LoadCardDataFromCSV(string path)
    {
        var result = new List<CardData>();
        string fullPath = Path.Combine(Application.dataPath, path);

        using (var reader = new StreamReader(fullPath))
        {
            bool isFirstLine = true;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (isFirstLine)
                {
                    isFirstLine = false; 
                    continue;
                }
                var split = line.Split(',');
                
                int? SafeInt(string s) => string.IsNullOrWhiteSpace(s) ? (int?)null : int.Parse(s);
                float? SafeFloat(string s) => string.IsNullOrWhiteSpace(s) ? (float?)null : float.Parse(s);
                bool? SafeBool(string s) => string.IsNullOrWhiteSpace(s) ? (bool?)null : bool.Parse(s);
                
                var card = new CardData
                {
                    GroupID    = SafeInt(split[0]),
                    Title      = split[1],
                    Name       = split[2],
                    TitleImage = split[3],
                    Description= split[4],
                    OptionText = split[5],
                    OptionImage= split[6],
                    Probability= SafeFloat(split[7]),
                    Command    = split[8],
                    Priority   = SafeInt(split[9]),
                    Weight     = SafeInt(split[10]),
                    OneTime    = SafeBool(split[11])
                };
                result.Add(card);
            }
        }
    return result;
    }
}
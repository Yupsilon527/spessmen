using UnityEngine;
using System.Collections;
using System.IO;

public static class GameDirectory
{

    public static string SaveDataFolder = "/Custom/";
    
    public static string[] ImageFileExtension = new string[] { ".tga", ".png" };
    public static string DataFileExtension = ".xml";
    public static string MapFolder = "Maps/";

    public static string DataFolder = "Characters/";

    public static string AssetFolder = "";

    public static string Separators = "-|";

    public static string CleanseString(string name)
    {
        return System.Text.RegularExpressions.Regex.Replace(name, @"[^a-zA-Z0-9 -_.']", "");

    }

    public static void CreateRequiredFiles()
    {
        if (!Directory.Exists(Application.dataPath + GameDirectory.SaveDataFolder))
        {
            Directory.CreateDirectory(Application.dataPath + GameDirectory.SaveDataFolder);
        }
        /*if (File.Exists (Application.dataPath+GameDirectory.SaveDataFolder+GameDirectory.DataList) == false) {
			SaveList(new List<List<string>>());
		}*/
        if (!Directory.Exists(Application.dataPath + GameDirectory.SaveDataFolder + GameDirectory.MapFolder))
        {
            Directory.CreateDirectory(Application.dataPath + GameDirectory.SaveDataFolder + GameDirectory.MapFolder);
        }
    }
}
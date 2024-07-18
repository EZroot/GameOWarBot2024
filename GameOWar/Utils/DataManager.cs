using GameOWar.Entities;
using GameOWar.Utils.JsonConverter;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using GameOWar.World;

public static class DataManager
{
    //public static void SaveBase(Base baseObject, string filePath)
    //{
    //    var jsonString = JsonConvert.SerializeObject(baseObject, Formatting.Indented, new JsonSerializerSettings
    //    {
    //        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
    //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    //        Converters = new List<JsonConverter> { new BuildingConverter() }
    //    });
    //    File.WriteAllText(filePath, jsonString);
    //}

    //public static Base? LoadBase(string filePath)
    //{
    //    if (!File.Exists(filePath))
    //        throw new FileNotFoundException($"The file at {filePath} was not found.");

    //    var jsonString = File.ReadAllText(filePath);
    //    return JsonConvert.DeserializeObject<Base>(jsonString, new JsonSerializerSettings
    //    {
    //        //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
    //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    //        Converters = new List<JsonConverter> { new BuildingConverter() }
    //    });
    //}
    public static void SaveWorldTiles(WorldTile[,] tiles, string filePath)
    {
        var jsonString = JsonConvert.SerializeObject(tiles, Formatting.None, new JsonSerializerSettings
        {
            //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //Converters = new List<JsonConverter> { new BuildingConverter() }

        });
        Console.WriteLine("Saved Map " + filePath);
        File.WriteAllText(filePath, jsonString);
    }

    public static WorldTile[,]? LoadWorldTiles(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"The file at {filePath} was not found.");
            return null;
        }
        Console.WriteLine("Loading Map " + filePath);

        var jsonString = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<WorldTile[,]>(jsonString, new JsonSerializerSettings
        {
            //Converters = new List<JsonConverter> { new BuildingConverter() }

            //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }

    public static void SavePlayer(Player playerObject, string filePath)
    {
        var jsonString = JsonConvert.SerializeObject(playerObject, Formatting.Indented, new JsonSerializerSettings
        {
            //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //Converters = new List<JsonConverter> { new BuildingConverter() }

        });
        Console.WriteLine("Saved player " + filePath);
        File.WriteAllText(filePath, jsonString);
    }

    public static Player? LoadPlayer(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file at {filePath} was not found.");
        Console.WriteLine("Loading player " + filePath);

        var jsonString = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Player>(jsonString, new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new BuildingConverter() }

            //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }
}

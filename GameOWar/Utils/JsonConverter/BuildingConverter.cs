

namespace GameOWar.Utils.JsonConverter
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    public class BuildingConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Building).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var buildingType = jsonObject["Name"]?.ToString(); // Change "Name" to "Type" if you add a "Type" property in your JSON

            if (buildingType == null)
                throw new JsonSerializationException("Building type not specified");

            Building building;
            switch (buildingType)
            {
                case "House":
                    building = new House();
                    break;
                case "Barracks":
                    building = new Barracks();
                    break;
                case "Farm":
                    building = new Farm();
                    break;
                case "Mine":
                    building = new Mine();
                    break;
                case "Logging":
                    building = new Logging();
                    break;
                case "MarketPlace":
                    building = new MarketPlace();
                    break;
                // Add more cases here for other Building types
                default:
                    throw new InvalidOperationException($"Unknown building type: {buildingType}");
            }

            serializer.Populate(jsonObject.CreateReader(), building);
            return building;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jsonObject = JObject.FromObject(value, serializer);
            jsonObject.AddFirst(new JProperty("Name", value.GetType().Name)); // Change "Name" to "Type" if you add a "Type" property in your JSON
            jsonObject.WriteTo(writer);
        }
    }
}

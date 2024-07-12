﻿using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alphatag_Game
{
public class BsonValueConverter : Newtonsoft.Json.JsonConverter<BsonValue>
{
    private readonly HashSet<object> _visitedObjects = new HashSet<object>();

    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, BsonValue value, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (value.IsNull)
        {
            writer.WriteNull();
        }
        else if (value.IsBoolean)
        {
            writer.WriteValue(value.AsBoolean);
        }
        else if (value.IsDateTime)
        {
            writer.WriteValue(value.AsDateTime);
        }
        else if (value.IsDouble)
        {
            writer.WriteValue(value.AsDouble);
        }
        else if (value.IsInt32)
        {
            writer.WriteValue(value.AsInt32);
        }
        else if (value.IsInt64)
        {
            writer.WriteValue(value.AsInt64);
        }
        else if (value.IsDecimal)
        {
            writer.WriteValue(value.AsDecimal);
        }
        else if (value.IsString)
        {
            writer.WriteValue(value.AsString);
        }
        else if (value.IsGuid)
        {
            writer.WriteValue(value.AsGuid);
        }
        else if (value.Type == BsonType.Binary)
        {
            writer.WriteValue(Convert.ToBase64String(value.AsBinary));
        }
        else if (value.IsArray)
        {
            if (_visitedObjects.Add(value.AsArray))
            {
                writer.WriteStartArray();
                foreach (var item in value.AsArray)
                {
                    serializer.Serialize(writer, item);
                }
                writer.WriteEndArray();
                _visitedObjects.Remove(value.AsArray);
            }
            else
            {
                writer.WriteNull();
            }
        }
        else if (value.IsDocument)
        {
            if (_visitedObjects.Add(value.AsDocument))
            {
                writer.WriteStartObject();
                foreach (var field in value.AsDocument)
                {
                    writer.WritePropertyName(field.Key);
                    serializer.Serialize(writer, field.Value);
                }
                writer.WriteEndObject();
                _visitedObjects.Remove(value.AsDocument);
            }
            else
            {
                writer.WriteNull();
            }
        }
        else
        {
            throw new JsonSerializationException($"Unsupported BsonValue type: {value.RawValue.GetType()}");
        }
    }

    public override BsonValue ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, BsonValue existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanRead => false;

    public override bool CanWrite => true;
}

    // Main program class
    public class Program
    {
        static void Main()
        {
            string dbFilePath = @"C:\Users\DineshReddy\Desktop\liteDB_to_json\database_simple\alphatag1.db";

            using (var db = new LiteDatabase(dbFilePath))
            {
                var jsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new BsonValueConverter());

                var collectionNames = db.GetCollectionNames();

                foreach (var collectionName in collectionNames)
                {
                    var collection = db.GetCollection(collectionName);
                    var items = collection.FindAll();

                    if (items.Any())
                    {
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented, jsonSerializerSettings);
                        File.WriteAllText($"{collectionName}.json", json);

                        Console.WriteLine($"Data from collection '{collectionName}' serialized to {collectionName}.json successfully.");
                    }
                }
            }

            Console.WriteLine("Data serialization complete.");
        }
    }
}
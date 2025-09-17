﻿using System;
using System.Collections.Generic;
using System.Globalization;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace BitDesk.Models;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(List<JsonAsset>))]
[JsonSerializable(typeof(JsonAssetResult))]
[JsonSerializable(typeof(JsonAssetData))]
[JsonSerializable(typeof(JsonAsset))]
internal partial class AssetJsonSerializerContext : JsonSerializerContext
{
}

public partial class JsonAssetResult
{
    [JsonPropertyName("success")]
    public long Success { get; set; }

    [JsonPropertyName("data")]
    public JsonAssetData? Data { get; set; }
}

public partial class JsonAssetData
{
    [JsonPropertyName("assets")]
    public List<JsonAsset>? Assets { get; set; }
}

public partial class JsonAsset
{
    [JsonPropertyName("asset")]
    public string? AssetAsset { get; set; }

    [JsonPropertyName("amount_precision")]
    public long AmountPrecision { get; set; }

    [JsonPropertyName("onhand_amount")]
    public string? OnhandAmount { get; set; }

    [JsonPropertyName("locked_amount")]
    public string? LockedAmount { get; set; }

    [JsonPropertyName("free_amount")]
    public string? FreeAmount { get; set; }

    [JsonPropertyName("stop_deposit")]
    public bool StopDeposit { get; set; }

    [JsonPropertyName("stop_withdrawal")]
    public bool StopWithdrawal { get; set; }
    
    /*
    [JsonPropertyName("withdrawal_fee")]
    public WithdrawalFeeUnion WithdrawalFee { get; set; }
    */
}

/*
public partial class WithdrawalFeeClass
{
    [JsonPropertyName("threshold")]
    public string? Threshold { get; set; }

    [JsonPropertyName("under")]
    public string? Under { get; set; }

    [JsonPropertyName("over")]
    public string? Over { get; set; }
}

public partial struct WithdrawalFeeUnion
{
    public string String;
    public WithdrawalFeeClass? WithdrawalFeeClass;

    public readonly bool IsNull => WithdrawalFeeClass == null && String == null;
}

public partial class JsonAssetResult
{
    public static JsonAssetResult? FromJson(string json) => JsonConvert.DeserializeObject<JsonAssetResult>(json, Converter.Settings);
}

public static class Serialize
{
    public static string ToJson(this JsonAssetResult self) => JsonConvert.SerializeObject(self, Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new()
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters = {
            WithdrawalFeeUnionConverter.Singleton,
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
    };
}

internal class WithdrawalFeeUnionConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(WithdrawalFeeUnion) || t == typeof(WithdrawalFeeUnion?);

    public override object ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.String:
            case JsonToken.Date:
                var stringValue = serializer.Deserialize<string>(reader);
                return new WithdrawalFeeUnion { String = stringValue ?? "" };
            case JsonToken.StartObject:
                var objectValue = serializer.Deserialize<WithdrawalFeeClass>(reader);
                return new WithdrawalFeeUnion { WithdrawalFeeClass = objectValue };
        }
        throw new Exception("Cannot unmarshal type WithdrawalFeeUnion");
    }

    public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
    {
        if (untypedValue != null)
        {
            var value = (WithdrawalFeeUnion)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.WithdrawalFeeClass != null)
            {
                serializer.Serialize(writer, value.WithdrawalFeeClass);
                return;
            }
        }

        throw new Exception("Cannot marshal type WithdrawalFeeUnion");
    }

    public static readonly WithdrawalFeeUnionConverter Singleton = new();
}

*/


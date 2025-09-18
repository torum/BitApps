﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitApps.Core.Models;

[JsonSerializable(typeof(JsonData))]
[JsonSerializable(typeof(JsonErrorObject))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class ErrorJsonSerializerContext : JsonSerializerContext
{
}

public class JsonErrorData
{
    public int Code { get; set; } = 0;
    public string Ja { get; set; } = string.Empty;
}

public class JsonErrorObject
{
    public int Success { get; set; } = 0;
    public JsonErrorData? Data { get; set; }
}

// Undocumented error number
/*
GetAsset: {
"success": 0,
"data": {
"code": 10007,
"ja": "ただいまメンテナンスのため一時サービスを停止しております。 今しばらくお待ちください。"
}
}
 */

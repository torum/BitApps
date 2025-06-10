using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDesk.Models;

public class JsonErrorData
{
    public int Code { get; set; } = 0;
    //public string ja { get; set; } = string.Empty;
}

public class JsonErrorObject
{
    //public int success { get; set; } = 0;
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

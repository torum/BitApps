﻿
namespace BitApps.Core.Models.APIClients;

// TODO: 
public class ResponseBodyWrapper
{
    public bool IsSuccess
    {
        get; set;
    }
    public string BodyText = string.Empty;
    public ClientError HTTPError = new();
}

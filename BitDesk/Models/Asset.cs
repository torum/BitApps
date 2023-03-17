namespace BitDesk.Models;

public class Asset
{
    public string? Name
    {
        get; set;
    }

    public string? NameText
    {
        get; set;
    }

    public decimal Amount
    {
        get; set;
    }

    public decimal FreeAmount
    {
        get; set;
    }

    public decimal EstimateYenAmount
    {
        get; set;
    }

    public Asset()
    {

    }
}

public class Assets
{
    public List<Asset> AssetList
    {
        get; set;
    }

    public Assets()
    {
        AssetList = new List<Asset>();
    }
}


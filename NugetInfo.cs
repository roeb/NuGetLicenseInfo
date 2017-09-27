using System.Collections.Generic;
using Newtonsoft.Json;

public class NugetInfo
{
    [JsonProperty(PropertyName = "@context")]
    public Context Context { get; set; }

    [JsonProperty(PropertyName = "totalHits")]
    public int TotalHits { get; set; }

    [JsonProperty(PropertyName = "data")]
    public IEnumerable<Data> Data { get; set; }
}

public class Context
{
    [JsonProperty(PropertyName = "@vocab")]
    public string Vocab { get; set; }

    [JsonProperty(PropertyName = "@base")]
    public string Base { get; set; }
}

public class Data
{
    [JsonProperty(PropertyName = "licenseUrl")]
    public string LicenseUrl { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "authors")]
    public List<string> Authors { get; set; }
}

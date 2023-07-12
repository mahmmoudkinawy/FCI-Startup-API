namespace API.Helpers;
public class Concept
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class Data
{
    [JsonPropertyName("image")]
    public Image Image { get; set; }

    [JsonPropertyName("concepts")]
    public List<Concept> Concepts { get; set; }
}

public class Image
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public class Input
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("data")]
    public Data Data { get; set; }
}

public class Metadata
{
}

public class Model
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("modified_at")]
    public DateTime? ModifiedAt { get; set; }

    [JsonPropertyName("app_id")]
    public string AppId { get; set; }

    [JsonPropertyName("model_version")]
    public ModelVersion ModelVersion { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("model_type_id")]
    public string ModelTypeId { get; set; }

    [JsonPropertyName("visibility")]
    public Visibility Visibility { get; set; }

    [JsonPropertyName("toolkits")]
    public List<object> Toolkits { get; set; }

    [JsonPropertyName("use_cases")]
    public List<object> UseCases { get; set; }

    [JsonPropertyName("languages")]
    public List<object> Languages { get; set; }

    [JsonPropertyName("languages_full")]
    public List<object> LanguagesFull { get; set; }

    [JsonPropertyName("check_consents")]
    public List<object> CheckConsents { get; set; }

    [JsonPropertyName("workflow_recommended")]
    public bool? WorkflowRecommended { get; set; }
}

public class ModelVersion
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("status")]
    public Status Status { get; set; }

    [JsonPropertyName("visibility")]
    public Visibility Visibility { get; set; }

    [JsonPropertyName("app_id")]
    public string AppId { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; }
}

public class Output
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("status")]
    public Status Status { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("model")]
    public Model Model { get; set; }

    [JsonPropertyName("input")]
    public Input Input { get; set; }

    [JsonPropertyName("data")]
    public Data Data { get; set; }
}

public class Root
{
    [JsonPropertyName("status")]
    public Status Status { get; set; }

    [JsonPropertyName("outputs")]
    public List<Output> Outputs { get; set; }
}

public class Status
{
    [JsonPropertyName("code")]
    public int? Code { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("req_id")]
    public string ReqId { get; set; }
}

public class Visibility
{
    [JsonPropertyName("gettable")]
    public int? Gettable { get; set; }
}


namespace API.Services;
public sealed class ClarityImage : IClarityImage
{
    public async Task<string> GetResultsAsync(string imageUrl)
    {
        var modelId = "aaa03c23b3724a16a56b629203edc62c";
        var url = $"https://api.clarifai.com/v2/models/{modelId}/outputs";

        string body = @"{
                    ""inputs"": [
                        {
                            ""data"": {
                                ""image"": {
                                    ""url"": """ + imageUrl + @"""
                                }
                            }
                        }
                    ]
                }";

        var content = new StringContent(body, Encoding.UTF8, "application/json");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Key 43e00bbf56c14f13a2f97fc1cf3b820f");

        var response = await client.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        var data = JsonConvert.DeserializeObject<Root>(responseContent);

        if (data.Status.Description == "Failure")
        {
            return string.Empty;
        }

        var concepts = data.Outputs[0].Data.Concepts;

        if (!concepts.Any())
        {
            return string.Empty;
        }

        var conceptsAsArray = concepts.Select(c => c.Name).ToArray();

        return $"Image contains: {string.Join(", ", conceptsAsArray).TrimEnd()}.";
    }
}
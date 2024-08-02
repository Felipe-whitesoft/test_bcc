using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace mysb_forms.core;

public class Item {
    [JsonProperty(PropertyName = "id")]
    public string? Id { get; set; }

    [JsonProperty(PropertyName = "category")]
    public string? Category { get; set; }

    [JsonProperty(PropertyName = "formName")]
    public string? FormName { get; set; }

    [JsonProperty(PropertyName = "formId")]
    public string? FormId { get; set; }

    [JsonProperty(PropertyName = "enabled")]
    public bool? Enabled { get; set; }

}

public interface IDatabaseService {
    Task<IEnumerable<Item>> GetItemsAsync(string queryString);
    Task<Item> GetItemByFormIdAsync(string formId);
    Task AddItemAsync(Item item);
    Task UpdateItemAsync(string id, Item updatedItem);
    Task DeleteItemAsync(string id);
}

public class CosmosDbService : IDatabaseService {
    private readonly Container _container;

    public CosmosDbService(IOptionsSnapshot<CosmosDBConfiguration> configurationOptions, CosmosClient cosmosClient) {
        var configuration = configurationOptions.Value;
        _container = cosmosClient.GetContainer(
            configuration.DatabaseName,
            configuration.ContainerName);
    }

    public async Task<IEnumerable<Item>> GetItemsAsync(string queryString) {
        var query = _container.GetItemQueryIterator<Item>(new QueryDefinition(queryString));
        var results = new List<Item>();
        while (query.HasMoreResults) {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task AddItemAsync(Item item) {
        await _container.CreateItemAsync(item, new PartitionKey(item.Id));
    }

    public async Task UpdateItemAsync(string id, Item updatedItem) {
        await _container.UpsertItemAsync(updatedItem, new PartitionKey(id));
    }

    public async Task DeleteItemAsync(string id) {
        await _container.DeleteItemAsync<Item>(id, new PartitionKey(id));
    }

    public async Task<Item> GetItemByFormIdAsync(string formId) {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.formId = @formId")
             .WithParameter("@formId", formId);
        var iterator = _container.GetItemQueryIterator<Item>(query);
        var result = new Item() { Enabled = false, FormId = formId };
        while (iterator.HasMoreResults) {
            var response = await iterator.ReadNextAsync();
            result = response.FirstOrDefault() ?? result;
        }
        return result;
    }
}

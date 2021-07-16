using Microsoft.Azure.Cosmos.Table;

public class TodoEntity : TableEntity
{
    public TodoEntity()
    {
    }
    public TodoEntity(string id, string content)
    {
        PartitionKey = id;
        RowKey = id;
    }

    public string Uuid { get; set; }

    public string Content { get; set; }
}

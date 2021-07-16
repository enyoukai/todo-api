using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

public class Table {
    public static async Task InsertOrMergeEntityAsync(CloudTable table, TodoEntity entity)
    {
    if (entity == null)
    {
        throw new ArgumentNullException("entity");
    }

    try
    {

        // Create the InsertOrReplace table operation
        TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

        // Execute the operation.
        await table.ExecuteAsync(insertOrMergeOperation);
    }
    catch (StorageException e)
    {
        Console.WriteLine(e.Message);
        Console.ReadLine();
        throw;
    }
    }
    public static async Task DeleteEntityAsync(CloudTable table, TodoEntity deleteEntity)
{
    try
    {
        if (deleteEntity == null)
        {
            throw new ArgumentNullException("deleteEntity");
        }

        TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
        TableResult result = await table.ExecuteAsync(deleteOperation);

        if (result.RequestCharge.HasValue)
        {
            Console.WriteLine("Request Charge of Delete Operation: " + result.RequestCharge);
        }

    }
    catch (StorageException e)
    {
        Console.WriteLine(e.Message);
        Console.ReadLine();
        throw;
    }
}
    public static async Task<TodoEntity> RetrieveEntityUsingPointQueryAsync(CloudTable table, string partitionKey, string rowKey)
    {
        try
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<TodoEntity>(partitionKey, rowKey);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            return result.Result as TodoEntity;
        }
        catch (StorageException e)
        {
            Console.WriteLine(e.Message);
            Console.ReadLine();
            throw;
        }
    }
}
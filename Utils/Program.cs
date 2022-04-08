using static Utils.PartsManager;

Console.WriteLine("Hello, World!");

await WritePartsFromDatabaseIntoFile("test.json");
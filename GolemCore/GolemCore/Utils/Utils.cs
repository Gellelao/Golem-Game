namespace GolemCore.Utils;

public class Utils
{
    public static void PrintArray<T>(T[][] array)
    {
        Console.WriteLine("============");
        for (int y = 0; y < array.Length; y++)
        {
            for (int x = 0; x < array[y].Length; x++)
            {
                Console.Write($"[{(array[x][y] == null ? " " : array[x][y])}]");
            }
            Console.WriteLine();
        }
    }
}
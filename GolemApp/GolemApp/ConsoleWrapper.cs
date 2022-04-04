namespace GolemApp
{
    public class ConsoleWrapper : IConsoleWrapper
    {
        public ConsoleWrapper()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public ConsoleKey ReadKey()
        {
            return Console.ReadKey(true).Key;
        }
        
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}
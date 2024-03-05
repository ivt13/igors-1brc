namespace OneBillionRowChallenge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filePath = args[0];

            var result = new Dictionary<string, Stat>(StringComparer.Ordinal);

            using (var sr = new StreamReader(new FileStream(filePath,FileMode.Open)))
            {
                string? line = null;
                while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                {
                    var indexOfDelimiter = line.IndexOf(';');
                    var name = line.Substring(0,indexOfDelimiter);
                    var tempStr = line.Substring(indexOfDelimiter + 1);

                    var temp = double.Parse(tempStr);

                    if (!result.TryGetValue(name, out var stat))
                    {
                        stat = new Stat();
                        result[name] = stat;
                    }

                    stat.Add(temp);

                }
            }

            var i = 0;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Write("{");
            foreach (var kvp in result.OrderBy(x => x.Key))
            {
                Console.Write($"{kvp.Key}={kvp.Value.Min}/{kvp.Value.Average()}/{kvp.Value.Max}");
                if (i < result.Count - 1)
                {
                    Console.Write(",");
                }
                ++i;
            }
            Console.Write("}");
        }
    }
}

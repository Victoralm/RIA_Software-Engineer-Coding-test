namespace DenominationRoutine_Challenge;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var cartridges = new[] { 100, 50, 10 };
        var payouts = new[] { 30, 50, 60, 80, 140, 230, 370, 610, 980 };

        var engine = new DenominationEngine();

        foreach (var amount in payouts)
        {
            Console.WriteLine($"\n=== {amount} EUR ===");
            foreach (var combo in engine.Enumerate(amount))
                Console.WriteLine($"  {combo}");
        }
    }
}

public readonly record struct NotePack(int N100, int N50, int N10)
{
    public override string ToString()
    {
        var parts = new List<string>(3);
        if (N100 > 0) parts.Add($"{N100} × 100 €");
        if (N50 > 0) parts.Add($"{N50} × 50 €");
        if (N10 > 0) parts.Add($"{N10} × 10 €");
        return string.Join(" + ", parts);
    }
}

public class DenominationEngine()
{
    public IEnumerable<NotePack> Enumerate(int amount)
    {
        for (int n100 = 0; n100 <= amount / 100; n100++)
            for (int n50 = 0; n50 <= (amount - n100 * 100) / 50; n50++)
            {
                int rest = amount - (n100 * 100 + n50 * 50);
                if (rest % 10 == 0)
                {
                    int n10 = rest / 10;
                    yield return new NotePack(n100, n50, n10);
                }
            }
    }
}
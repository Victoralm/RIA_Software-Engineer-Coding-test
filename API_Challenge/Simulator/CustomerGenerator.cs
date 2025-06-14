public class CustomerGenerator
{
    private static readonly List<string> FirstNames = new()
    {
        "Leia", "Sadie", "Jose", "Sara", "Frank", "Dewey", "Tomas", "Joel", "Lukas", "Carlos"
    };

    private static readonly List<string> LastNames = new()
    {
        "Liberty", "Ray", "Harrison", "Ronan", "Drew", "Powell", "Larsen", "Chan", "Anderson", "Lane"
    };

    private readonly Random _random = new();

    public List<CustomerDto> GenerateCustomers(int count, ref int nextId)
    {
        var customers = new List<CustomerDto>();
        var usedCombinations = new HashSet<string>();

        while (customers.Count < count)
        {
            var firstName = FirstNames[_random.Next(FirstNames.Count)];
            var lastName = LastNames[_random.Next(LastNames.Count)];
            var key = $"{firstName}-{lastName}";
            if (usedCombinations.Contains(key)) continue;
            usedCombinations.Add(key);

            int age = _random.Next(10, 91);
            customers.Add(new CustomerDto
            {
                Id = nextId++,
                FirstName = firstName,
                LastName = lastName,
                Age = age
            });
        }
        return customers;
    }
}

public class CustomerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int Age { get; set; }
}
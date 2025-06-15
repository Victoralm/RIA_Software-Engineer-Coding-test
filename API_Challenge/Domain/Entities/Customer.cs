namespace Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }

    public static List<Customer> OrderByName(IEnumerable<Customer> customers)
    {
        var list = customers.ToList();
        MergeSort(list, 0, list.Count - 1);
        return list;
    }

    private static void MergeSort(List<Customer> list, int left, int right)
    {
        if (left >= right) return;

        int mid = (left + right) / 2;
        MergeSort(list, left, mid);
        MergeSort(list, mid + 1, right);
        Merge(list, left, mid, right);
    }

    private static void Merge(List<Customer> list, int left, int mid, int right)
    {
        var temp = new List<Customer>(right - left + 1);
        int i = left, j = mid + 1;

        while (i <= mid && j <= right)
        {
            if (Compare(list[i], list[j]) <= 0)
                temp.Add(list[i++]);
            else
                temp.Add(list[j++]);
        }
        while (i <= mid) temp.Add(list[i++]);
        while (j <= right) temp.Add(list[j++]);

        for (int k = 0; k < temp.Count; k++)
            list[left + k] = temp[k];
    }

    private static int Compare(Customer a, Customer b)
    {
        int cmp = string.Compare(a.LastName, b.LastName, StringComparison.Ordinal);
        if (cmp != 0) return cmp;
        return string.Compare(a.FirstName, b.FirstName, StringComparison.Ordinal);
    }
}

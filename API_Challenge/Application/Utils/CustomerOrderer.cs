using Domain.Entities;

namespace Application.Utils;

public static class CustomerOrderer
{
    public static List<Customer> OrderByName(IEnumerable<Customer> customers)
    {
        #region Ordering the Customers with Binary Search
        List<Customer> ordered = new();

        foreach (var customer in customers)
        {
            int left = 0;
            int right = ordered.Count;

            // Binary search to find correct insertion index
            while (left < right)
            {
                int mid = (left + right) / 2;
                int cmp = string.Compare(
                    ordered[mid].LastName,
                    customer.LastName,
                    StringComparison.Ordinal
                );

                if (cmp < 0)
                {
                    left = mid + 1;
                }
                else if (cmp > 0)
                {
                    right = mid;
                }
                else
                {
                    // LastName equal, compare FirstName
                    cmp = string.Compare(
                        ordered[mid].FirstName,
                        customer.FirstName,
                        StringComparison.Ordinal
                    );
                    if (cmp < 0)
                        left = mid + 1;
                    else
                        right = mid;
                }
            }

            ordered.Insert(left, customer);
        }
        #endregion

        return ordered;
    }
}

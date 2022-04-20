using System.Collections.Generic;
using System.Linq;

public class Register<K, V>
{
    Dictionary<K, List<V>> dictionary = new Dictionary<K, List<V>>();

    public bool HasKey(K key) => dictionary.Keys.Any(current => current.Equals(key));

    public bool HasValue(V value) => dictionary.Values.Any(current => current.Contains(value));

    public void Add(K key, V value)
    {
        if (dictionary.TryGetValue(key, out var values) == false)
        {
            values = new List<V>();
            dictionary.Add(key, values);
        }

        values.Add(value);
    }

    public bool Remove(K key, V value)
    {
        if (dictionary.TryGetValue(key, out var values))
        {
            return values.Remove(value);
        }

        return false;
    }

    public IEnumerable<(K key, List<V> values)> Entries()
    {
        foreach (var entry in dictionary) 
            yield return (entry.Key, entry.Value);
    }
}

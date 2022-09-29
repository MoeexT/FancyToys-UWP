using System.Collections.Generic;


namespace FancyLibrary.Utils.Collections; 

public class DoubledMap<K, V> {

    private readonly Dictionary<K, V> keyedMap;
    private readonly Dictionary<V, K> valuedMap;

    public DoubledMap() {
        keyedMap = new Dictionary<K, V>();
        valuedMap = new Dictionary<V, K>();
    }


    public V this[K k] {
        get => keyedMap[k];
        set => keyedMap[k] = value;
    }

    public K this[V v] {
        get => valuedMap[v];
        set => valuedMap[v] = value;
    }
    
    public void Set(K key, V value) {
        keyedMap[key] = value;
        valuedMap[value] = key;
    }

    public V Get(K key) => keyedMap[key];

    public K Get(V value) => valuedMap[value];

    public bool TryGetValue(K key, out V value) => keyedMap.TryGetValue(key, out value);

    public bool TryGetKey(V value, out K key) => valuedMap.TryGetValue(value, out key);

}

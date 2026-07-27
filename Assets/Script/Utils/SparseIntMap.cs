// Simple wrapper
using System;

public struct SparseIntMap
{
    public (int key, int value)[] _entries;
    private int _count;

    public SparseIntMap(int capacity = 16)
    {
        _entries = new (int, int)[capacity];
        _count = 0;
    }

    public void Set(int key, int value)
    {
        int i = Find(key);
        if (i >= 0) { _entries[i].value = value; return; }

        // insert in sorted position
        int pos = ~i;
        if (_count == _entries.Length)
            Array.Resize(ref _entries, _entries.Length * 2);
        Array.Copy(_entries, pos, _entries, pos + 1, _count - pos);
        _entries[pos] = (key, value);
        _count++;
    }

    public bool TryGet(int key, out int value)
    {
        int i = Find(key);
        if (i >= 0) { value = _entries[i].value; return true; }
        value = 0; return false;
    }

    public bool Contains(int key) => Find(key) >= 0;

    private int Find(int key)
    {
        int lo = 0, hi = _count - 1;
        while (lo <= hi)
        {
            int mid = (lo + hi) >> 1;
            int k = _entries[mid].key;
            if (k == key) return mid;
            if (k < key) lo = mid + 1; else hi = mid - 1;
        }
        return ~lo; // negative = not found, ~result = insert position
    }
}
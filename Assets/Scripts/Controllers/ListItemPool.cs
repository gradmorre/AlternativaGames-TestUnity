using UnityEngine;
using System.Collections.Generic;

public class ListItemPool
{
    private readonly Queue<ListItemView> _pool = new Queue<ListItemView>();
    private readonly GameObject _prefab;
    private readonly Transform _parent;

    public ListItemPool(GameObject prefab, Transform parent, int initialSize = 10)
    {
        _prefab = prefab;
        _parent = parent;
        
        for (int i = 0; i < initialSize; i++)
        {
            var item = CreateNewItem();
            item.gameObject.SetActive(false);
            _pool.Enqueue(item);
        }
    }

    public ListItemView GetItem()
    {
        ListItemView item;
        if (_pool.Count > 0)
        {
            item = _pool.Dequeue();
        }
        else
        {
            item = CreateNewItem();
        }
        
        item.gameObject.SetActive(true);
        return item;
    }

    public void ReturnItem(ListItemView item)
    {
        item.gameObject.SetActive(false);
        _pool.Enqueue(item);
    }

    private ListItemView CreateNewItem()
    {
        var go = Object.Instantiate(_prefab, _parent);
        return go.GetComponent<ListItemView>();
    }
}
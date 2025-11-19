using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListViewController : MonoBehaviour
{
    private readonly Vector2 VIEW_OFFSET = new Vector2(0f, 60f);

    [Header("UI References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject listItemPrefab;

    [Header("Settings")]
    [SerializeField] private int itemsCount = 50;
    [SerializeField] private float scrollDuration = 0.5f;
    [SerializeField] private AnimationCurve scrollCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private DataService _dataService;
    private Dictionary<string, ListItemView> _itemViews = new Dictionary<string, ListItemView>();
	private List<ListItemData> _itemsData;
	private ListItemPool _itemPool;
	
    private ListItemView _selectedItem;
    private Coroutine _scrollCoroutine;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        HandleKeyboardInput();
    }

    private void Initialize()
    {
		_itemPool = new ListItemPool(listItemPrefab, contentParent);
        _dataService = GetComponent<DataService>() ?? gameObject.AddComponent<DataService>();
        
		_itemsData = _dataService.GenerateTestData(itemsCount);
        CreateListItems();

        if (_itemsData.Count > 0)
        {
            SelectItem(_itemViews[_itemsData[0].id]);
        }
    }

    private void CreateListItems()
    {
        foreach (var data in _itemsData)
        {
            var itemView = _itemPool.GetItem();

            itemView.Initialize(data);
            itemView.OnClicked += OnItemClicked;

            _itemViews[data.id] = itemView;
        }
    }

    private void OnItemClicked(ListItemView itemView)
    {
        if (_selectedItem == itemView)
        {
            itemView.ToggleExpanded();
        }
        else
        {
            SelectItem(itemView);
            itemView.SetExpanded(true);
        }
    }

    private void SelectItem(ListItemView itemView)
    {
        if (_selectedItem != null)
        {
            _selectedItem.SetSelected(false);
            _selectedItem.SetExpanded(false);
        }

        _selectedItem = itemView;
        _selectedItem.SetSelected(true);

        ScrollToItem(itemView);
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Navigate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Navigate(1);
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (_selectedItem != null)
            {
                _selectedItem.ToggleExpanded();
            }
        }
    }

    private void Navigate(int direction)
    {
        if (_itemsData.Count == 0) return;

        var currentIndex = _itemsData.FindIndex(data => data.id == _selectedItem?.ItemId);
        var newIndex = Mathf.Clamp(currentIndex + direction, 0, _itemsData.Count - 1);

        if (newIndex != currentIndex)
        {
            var newItem = _itemViews[_itemsData[newIndex].id];
            SelectItem(newItem);
        }
    }

    private void ScrollToItem(ListItemView itemView)
    {
        if (_scrollCoroutine != null)
            StopCoroutine(_scrollCoroutine);

        _scrollCoroutine = StartCoroutine(AnimateScrollToItem(itemView.transform as RectTransform));
    }


    private IEnumerator AnimateScrollToItem(RectTransform target)
    {
        var contentRT = scrollRect.content;
        var startPosition = contentRT.anchoredPosition;

        var targetPosition =
            (Vector2)scrollRect.transform.InverseTransformPoint(contentRT.position) 
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position) 
            - VIEW_OFFSET;
        targetPosition.x = 0f;

        var elapsed = 0f;

        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            var progress = elapsed / scrollDuration;
            var curvedProgress = scrollCurve.Evaluate(progress);
            contentRT.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, curvedProgress);

            yield return null;
        }
    }
}

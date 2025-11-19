using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ListItemView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	private const float DEFAULT_HEIGHT = 60.0f;
    private readonly Color lightGray = new Color(0.9f, 0.9f, 0.9f, 1f);
	
    [Header("References")]
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image selectionBorder;
    private LayoutElement layoutElement;
    private CanvasGroup descriptionGroup;
	
	
    
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // События для коммуникации с контроллером
    public System.Action<ListItemView> OnClicked;
    public System.Action<ListItemView> OnSelected;
    
    private ListItemData _data;
    private bool _isExpanded = false;
    private bool _isSelected = false;
    private Coroutine _animationCoroutine;
	private float expandedHeight;
    
    public string ItemId => _data?.id;
    public bool IsExpanded => _isExpanded;
    public bool IsSelected => _isSelected;
    
    public void Initialize(ListItemData data)
    {
        _data = data;
        if (background.material != null)
        {
            background.material = new Material(background.material);
        }
		layoutElement = GetComponent<LayoutElement>();
        descriptionGroup = descriptionText.transform.GetComponent<CanvasGroup>();
        UpdateView();
    }
    
    private void UpdateView()
    {
        if (_data == null) return;
        
        titleText.text = _data.title;
        descriptionText.text = _data.description;
        icon.sprite = _data.icon;
		
		expandedHeight = 110f;
        
        if (background.material != null)
        {
            //background.material.SetColor("_Color", _data.gradientColor1);
            //background.material.SetColor("_Color2", _data.gradientColor2);
            
        }
        background.color = lightGray;
    }
    
    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        selectionBorder.enabled = selected;
        
        // Дополнительные визуальные эффекты при выделении
        if (selected)
        {
            transform.localScale = Vector3.one * 1.02f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
    
    public void ToggleExpanded()
    {
        SetExpanded(!_isExpanded);
    }
    
    public void SetExpanded(bool expanded)
    {
        if (_isExpanded == expanded) return;
        _isExpanded = expanded;
        
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);
            
        _animationCoroutine = StartCoroutine(AnimateExpansion());
    }

    private System.Collections.IEnumerator AnimateExpansion()
    {
		var rt = transform as RectTransform;
        var startHeight =rt.rect.height;
        var targetHeight = _isExpanded ? expandedHeight : DEFAULT_HEIGHT;

        var startAlpha = descriptionGroup.alpha;
        var targetAlpha = _isExpanded ? 1f : 0f;

        var elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            var progress = elapsed / animationDuration;
            var curvedProgress = animationCurve.Evaluate(progress);
            layoutElement.preferredHeight = Mathf.Lerp(startHeight, targetHeight, curvedProgress);
            descriptionGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curvedProgress);

            yield return null;
        }

        layoutElement.preferredHeight = targetHeight;
        descriptionGroup.alpha = targetAlpha;
    }
	

    // Input handlers
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke(this);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.01f;
        background.color = Color.white;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isSelected)
            transform.localScale = Vector3.one;
        background.color = lightGray;
    }
}
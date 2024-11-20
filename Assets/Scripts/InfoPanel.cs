using System.Collections.Generic;
using System.Linq;
using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject outputLinePrefab;
    private List<GameObject> _infoLines = new List<GameObject>();
    
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject infoPanelContent;
    [SerializeField] private LayerMask layerMask;
    
    private RectTransform _infoPanelRectTransform;
    private Camera _mainCamera;
    
    private List<string> _outputLines = new List<string>();
    private Collider2D _lastCollider2D;
    private bool _isNewInfo = true;
    
    private void Start()
    {
        _infoPanelRectTransform = infoPanelContent.GetComponent<RectTransform>();
        _mainCamera = Camera.main;
    }

    private void OnGUI()
    {
        var newInfo = UpdateObjectInfo();
        
        if (!_isNewInfo)
            return;
        
        ClearInfoPanel();
        var lineCount = AddInfoLines(newInfo);
        ScrollToBottom(lineCount);
    }

    private void ClearInfoPanel()
    {
        foreach (var infoLine in _infoLines)
            Destroy(infoLine);
        _infoLines.Clear();
        _infoPanelRectTransform.sizeDelta = Vector2.zero;
    }

    private int AddInfoLines(List<string> outputLines)
    {
        foreach (var line in outputLines)
        {
            var responseLine = Instantiate(outputLinePrefab, infoPanelContent.transform);
            _infoLines.Add(responseLine);
            responseLine.transform.SetAsLastSibling();
            var infoContentSize = _infoPanelRectTransform.sizeDelta;
            _infoPanelRectTransform.sizeDelta = new Vector2(infoContentSize.x, infoContentSize.y + 35f);
            responseLine.GetComponentInChildren<TextMeshProUGUI>().text = line;
        }

        return outputLines.Count;
    }

    private List<string> UpdateObjectInfo()
    {
        _outputLines.Clear();
        
        Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);
        
        _isNewInfo = hit.collider != _lastCollider2D;
        _lastCollider2D = hit.collider;
        
        if (hit.collider == null)
            return _outputLines;
        
        var objectInfo = hit.collider.GetComponent<IObjectInfo>();
        
        if (objectInfo != null)
        {
            _outputLines = objectInfo.GetInfo().Split('\n').ToList();
        }
        
        return _outputLines;
    }
    
    private void ScrollToBottom(int lineCount)
    {
        if (lineCount > 4)
        {
            scrollRect.velocity = new Vector2(0, 450f);
        }
        else
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}

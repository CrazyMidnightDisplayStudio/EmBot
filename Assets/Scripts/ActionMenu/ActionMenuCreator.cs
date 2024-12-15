using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ActionMenu
{
    public class ActionMenuCreator: MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField]public Button buttonPrefab;
        [SerializeField]public GameObject menuPrefab;
        
        [Header("UI")]
        private GameObject _menu = null;
        private VerticalLayoutGroup _verticalLayoutGroup;
        private List<Button> _buttons = new List<Button>();
        
        [Header("Objects")]
        [SerializeField] private WaypointCreator waypointCreator;
        [SerializeField] private GameObject activeObject;
        private IInteractAction _interactAction;

        private void Awake()
        {
            if (!activeObject)
                throw new NullReferenceException("No active object");
            
            if (!activeObject.TryGetComponent(out _interactAction))
                throw new NullReferenceException("The active object is not IInteractAction");
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                   Vector3 mouseScreenPosition = Input.mousePosition;
                   mouseScreenPosition.z = Camera.main.nearClipPlane + 2;
                   Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
                   
                   InitMenu();
                   ClearButtons();
                   
                   var listActionObject = GetGameObjectsPoint(mouseWorldPosition);
                   
                   if (listActionObject.Count == 0) // Default Actions of activeObject
                   {
                        var pointTarget = waypointCreator.CreatePoint(mouseWorldPosition);
                        listActionObject = BaseCommandList(pointTarget);
                   }
                   
                   if (listActionObject.Count > 0) // Unique actions
                   {
                       CreateButtons(listActionObject);
                       BindMenu(mouseWorldPosition);
                   }
                }
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                ClearButtons();
                if(_menu)
                    _menu.gameObject.SetActive(false);
            }
        }
        
        private void InitMenu()
        {
            if (_menu) 
                return;
            
            _menu = Instantiate(menuPrefab);
            _verticalLayoutGroup = _menu.GetComponentInChildren<VerticalLayoutGroup>();
        }
       
        private void BindMenu(Vector3 position)
        {
            _menu.transform.position = position;
            foreach (Button button in _buttons)
            {
                button.transform.SetParent(_verticalLayoutGroup.transform, false);
            }
            _menu.gameObject.SetActive(true);
        }

        private List<IInteractAction> GetGameObjectsPoint(Vector3 point)
        {
            List<IInteractAction> gameObjects = new List<IInteractAction>();
        
            // Получаем все объекты, пересекаемые лучом
            RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero);
        
            foreach (RaycastHit2D hit in hits)
            {
                // Получаем объект, по которому был произведен клик
                GameObject clickedObject = hit.collider.gameObject;
                var interact = clickedObject.GetComponent<IInteractAction>();
                if (interact != null) gameObjects.Add(interact);
            }
        
            return gameObjects;
        }
        
        private void CreateButtons(List<IInteractAction> interactActions)
        {
            foreach (var interact in interactActions)
            {
                var dictionaryActions = interact.GetActions();
                foreach (var action in dictionaryActions)
                {
                    var button = SubscribeButton(CreateButton(action.Key), action.Value);
                    _buttons.Add(button);
                }
            }
        }
        
        //Создаем подписку кнопки на переданный action, после нажатия на кнопку меню исчезает
        private Button SubscribeButton(Button button, Action action)
        {
            if (button && action != null)
            {
                button.onClick.AddListener(() => action());
            }
            
            return button;
        }
        
        
        private Button CreateButton(string text)
        {
            var btn = Instantiate(buttonPrefab);
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = text;
            return btn;
        }
        
        private void ClearButtons()
        {
            foreach (var item in _buttons)
            {
                Destroy(item.transform.gameObject);
            }
            _buttons.Clear();
        }
        
        private List<IInteractAction> BaseCommandList(Transform point)
        {
            _interactAction.SetWaypoint(point);
            
            return new List<IInteractAction>() { _interactAction };
        }
    }
}

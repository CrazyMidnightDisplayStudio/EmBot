using UnityEngine;

namespace ActionMenu
{
    public class WaypointCreator : MonoBehaviour
    {
        [SerializeField] private GameObject pointPrefab;
        
        private GameObject _lastClick;
        
        public Transform CreatePoint(Vector3 mousePosition)
        {
            mousePosition.z = 0;

            var point = Instantiate(pointPrefab, mousePosition, Quaternion.identity);
            DeletePrevPoint(point);
            return point.transform;
        }
        
        private void DeletePrevPoint(GameObject newPoint)
        {
            if(_lastClick) 
            { 
                Destroy(_lastClick); _lastClick = null; 
            } 
            _lastClick = newPoint;
        }
    }
}
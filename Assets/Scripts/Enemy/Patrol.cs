using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class Patrol : MonoBehaviour
    {
        private const string DoorTag = "Door";

        [SerializeField] private List<Transform> patrolPoints;
        [SerializeField] private string highPriorityTag = "Human";

        [SerializeField] private float nextPatrolPointDistance = 0.05f;
        [SerializeField] private float secToLostHuman = 5f;
    
        private AIDestinationSetter _destinationSetter;
        private EnemyVision _enemyVision;
        private bool _isPatrol = true;
        
        [Header("For Debug")]
        [SerializeField] private int currentPointIndex = 0;
        [SerializeField] private Transform currentTarget;

        private void Start()
        {
            _destinationSetter = GetComponent<AIDestinationSetter>();
            _enemyVision = GetComponent<EnemyVision>();
            _destinationSetter.target = null;
            if (patrolPoints.Count > 0)
            {
                _destinationSetter.target = patrolPoints[currentPointIndex];
            }
        }

        private void Update()
        {
            currentTarget = _destinationSetter.target; // for Debug
            
            if (_enemyVision && _enemyVision.TargetDetected() && _enemyVision.GetDetectedTarget().gameObject.CompareTag(highPriorityTag)) // Seek Human
            {
                if (_isPatrol)
                    StartCoroutine(ForgetAboutHuman());

                _isPatrol = false;
                _destinationSetter.target = _enemyVision.GetDetectedTarget();
            }
            else if (_destinationSetter.target) // Patrol
            {
                _isPatrol = true;
                float distanceToTarget = Vector3.Distance(transform.position, _destinationSetter.target.position);
                
                if (distanceToTarget < nextPatrolPointDistance) // if near patrolPoint
                {
                    
                    if (patrolPoints[currentPointIndex].CompareTag(DoorTag)) // Check if the point is a Door
                    {
                        var doorPoint = patrolPoints[currentPointIndex];
                        var door = doorPoint.GetComponent<Door>();

                        if (!door.IsOpen)
                        {
                            RearrangeList(currentPointIndex);
                            currentPointIndex = 0;
                            _destinationSetter.target = patrolPoints[currentPointIndex]; // From start
                            return;
                        }
                    }
                    
                    currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count; // next point
                    _destinationSetter.target = patrolPoints[currentPointIndex];
                }
            }
        }

        public void AddPatrolPoint(Transform newPp)
        {
            patrolPoints.Add(newPp);
        }

        public void RemovePatrolPoint(Transform newPp)
        {
            if (patrolPoints.Contains(newPp))
                patrolPoints.Remove(newPp);
        }
        
        private void RearrangeList(int index)
        {
            if (index < 0 || index >= patrolPoints.Count)
                throw new System.ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

            var rearrangedList = new List<Transform>();

            // Добавляем элементы до индекса в обратном порядке
            for (int i = index - 1; i >= 0; i--)
            {
                rearrangedList.Add(patrolPoints[i]);
            }

            // Добавляем элементы после индекса в обратном порядке
            for (int i = patrolPoints.Count - 1; i > index; i--)
            {
                rearrangedList.Add(patrolPoints[i]);
            }

            // Добавляем элемент по указанному индексу
            rearrangedList.Add(patrolPoints[index]);
            
            patrolPoints.Clear();
            patrolPoints = rearrangedList;
        }

        private IEnumerator ForgetAboutHuman()
        {
            yield return new WaitForSeconds(secToLostHuman);
            _destinationSetter.target = patrolPoints[currentPointIndex];
            _enemyVision.LostHuman();
            _isPatrol = true;
        }
    }
}
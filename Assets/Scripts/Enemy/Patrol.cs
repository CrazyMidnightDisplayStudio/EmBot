using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace Enemy
{
    public class Patrol : MonoBehaviour
    {
    
        [SerializeField] private List<Transform> patrolPoints;
        [SerializeField] private string highPriorityTag = "Human";

        [SerializeField] private float nextPatrolPointDistance = 0.05f;
        [SerializeField] private float secToLostHuman = 5f;
    
        private AIDestinationSetter _destinationSetter;
        private EnemyVision _enemyVision;
        private int _currentPointIndex = 0;
        private bool _isPatrol = true;

        private void Start()
        {
            _destinationSetter = GetComponent<AIDestinationSetter>();
            _enemyVision = GetComponent<EnemyVision>();
            _destinationSetter.target = null;
            if (patrolPoints.Count > 0)
            {
                _destinationSetter.target = patrolPoints[_currentPointIndex];
            }
        }

        void Update()
        {
            if (_enemyVision && _enemyVision.TargetDetected() && _enemyVision.GetDetectedTarget().gameObject.CompareTag(highPriorityTag))
            {
                if (_isPatrol)
                    StartCoroutine(ForgetAboutHuman());

                _isPatrol = false;
                _destinationSetter.target = _enemyVision.GetDetectedTarget();
            }
            else if (_destinationSetter.target)
            {
                _isPatrol = true;
                float distanceToTarget = Vector3.Distance(transform.position, _destinationSetter.target.position);
                if (distanceToTarget < nextPatrolPointDistance)
                {
                    _currentPointIndex = (_currentPointIndex + 1) % patrolPoints.Count;
                    _destinationSetter.target = patrolPoints[_currentPointIndex];
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

        private IEnumerator ForgetAboutHuman()
        {
            yield return new WaitForSeconds(secToLostHuman);
            _destinationSetter.target = patrolPoints[_currentPointIndex];
            _enemyVision.LostHuman();
            _isPatrol = true;
        }
    }
}
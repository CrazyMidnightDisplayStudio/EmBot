using UnityEngine;

namespace Enemy
{
    public class EnemyVision : MonoBehaviour
    {
        [SerializeField] private float viewDistance = 4f;
        [SerializeField] private float viewAngle = 45f;
        [SerializeField] private string[] targetTags;

        [SerializeField] private LayerMask rayCastMask;

        private Collider2D _detectedTarget;
        private Vector3 _lastDirection;

        private void Update()
        {
            DetectTargets();
        }

        private void DetectTargets()
        {
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewDistance);

            foreach (Collider2D target in targetsInViewRadius)
            {
                if (IsTargetInSight(target))
                    CheckTarget(target);
            }
        }

        private void CheckTarget(Collider2D target)
        {
            foreach (var targetTag in targetTags)
            {
                if (!target.CompareTag(targetTag)) 
                    continue;
                
                _detectedTarget = target;
                break;
            }
        }

        private bool IsTargetInSight(Collider2D target)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            _lastDirection = directionToTarget;

            if (Vector3.Angle(transform.up, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, rayCastMask);

                if (hit.collider && hit.collider.CompareTag(target.tag))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, viewDistance);

            Vector3 leftBoundary = Quaternion.Euler(0, 0, viewAngle / 2) * transform.up * viewDistance;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -viewAngle / 2) * transform.up * viewDistance;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + _lastDirection * viewDistance);
        }

        public bool TargetDetected()
        {
            return _detectedTarget;
        }

        public void LostHuman()
        {
            _detectedTarget = null;
        }

        public Transform GetDetectedTarget()
        {
            return _detectedTarget.transform;
        }
    }
}

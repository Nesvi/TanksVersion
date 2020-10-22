using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Complete
{
    [DefaultExecutionOrder(-100)]
    public class TankAI : MonoBehaviour
    {
        NavMeshPath path;

        public float m_MovementInputValue;
        public float m_TurnInputValue;
        public float deltaAngleout;

        private Vector3 currentPosition;

        private void Start()
        {
            path = new NavMeshPath();
        }

        private void Update()
        {
            currentPosition = transform.position;

            FindPathToClosestTank();

            Steering();

            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }

        private void FindPathToClosestTank()
        {
            Vector3 closestTankPosition = GetClosestTankPosition();

            NavMesh.CalculatePath(transform.position, closestTankPosition, NavMesh.AllAreas, path);
        }

        private void Steering()
        {
            if (path.corners.Length >= 2) {
                Vector3 nextPosition = path.corners[1];

                Vector3 directionToNextPosition = nextPosition - currentPosition;
                Vector3 currentDirection = transform.forward;

                float targetAngle = Mathf.Atan2( directionToNextPosition.x, directionToNextPosition.z) *Mathf.Rad2Deg;
                float currentAngle = Mathf.Atan2( currentDirection.x, currentDirection.z) * Mathf.Rad2Deg;

                float deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
                
                m_TurnInputValue = Mathf.Clamp(deltaAngle * 10.0f, -1.0f, 1.0f);

                deltaAngleout = deltaAngle;
                if (Mathf.Abs(deltaAngle) < 5.0f)
                {
                    m_MovementInputValue = 1.0f;
                }
                else
                {
                    m_MovementInputValue = 0.0f;
                }
            }
        }

        private Vector3 GetClosestTankPosition()
        {
            float bestDistance = 9999.0f;
            Vector3 bestPosition = Vector3.zero;

            for(int i = 0; i < GameManager.instance.m_Tanks.Length; i++)
            {
                if (GameManager.instance.m_Tanks[i].m_Instance.gameObject != gameObject)
                {
                    Vector3 otherTankPosition = GameManager.instance.m_Tanks[i].m_Instance.transform.position;
                    float currentDistance = Vector3.Distance(otherTankPosition, currentPosition);
                    
                    if(currentDistance < bestDistance)
                    {
                        bestDistance = currentDistance;
                        bestPosition = otherTankPosition;
                    }
                }
            }

            return bestPosition;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

namespace Complete
{
    [DefaultExecutionOrder(-100)]
    public class TankAI : MonoBehaviour
    {
        NavMeshPath path;

        public float turnSpeed = 0.1f;

        public float m_MovementInputValue;
        public float m_TurnInputValue;

        public bool wantToShoot = false;
        public float targetShootPower;

        public bool trainShootingAI = false;

        private Vector3 currentPosition;

        protected struct ShootResult
        {
            public float power;
            public float distance;
        }

        protected static List<ShootResult> results;

        public static TankAI instance;

        public Timer shootTrainTimer = new Timer(4.0f);


        public float thresholdDistance = 14.0f;
        private Timer thresholdDistanceChange = new Timer(3.0f);

        private void Start()
        {
            path = new NavMeshPath();
            instance = this;
            results = new List<ShootResult>();

            if (trainShootingAI)
            {
                targetShootPower = 30.0f;
                shootTrainTimer.Reset();
            }
            else
            {
                LoadResults();
            }

            thresholdDistanceChange.Reset();
        }

        private void Update()
        {
            currentPosition = transform.position;

            if (trainShootingAI)
                TrainAI();
            else
                AI();

#if UNITY_EDITOR
            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
#endif
        }

        private void AI()
        {

            float distance = 0.0f;
            Vector3 closestTankPosition = GetClosestTankPosition(out distance);

            if (distance < thresholdDistance && IsTargetVisible(closestTankPosition))
            {
                Shoot(true, distance);
                StopMovement();
                Steer(closestTankPosition);
            }
            else
            {
                FindPathToPosition(closestTankPosition);
                SteeringAndMovement();
                Shoot(false, distance);
            }

            if (thresholdDistanceChange.CheckOneTimeEvent())
            {
                thresholdDistanceChange.Reset();
                NewRandomThresholdDistance();
            }
        }

        private void TrainAI()
        {
            wantToShoot = false;

            if (targetShootPower  < 15.0f)
            {
                SaveResults();
                Debug.Break();
            }
            else if (shootTrainTimer.CheckOneTimeEvent())
            {
                GetComponent<TankShooting>().TrainingFire(targetShootPower);
                targetShootPower -= 2.5f;
                shootTrainTimer.Reset();
            }
        }


        private void Shoot(bool tryToShoot, float distance)
        {
            wantToShoot = false;
            return;
            if (tryToShoot)
            {
                wantToShoot = true;
                targetShootPower = 30.0f;
                SetShootParameters(distance);
            }
            else
            {
                wantToShoot = false;
                SetMinimumShootParameters();
            }
        }

        private void FindPathToPosition(Vector3 position)
        {
            NavMesh.CalculatePath(transform.position, position, NavMesh.AllAreas, path);
        }

        private void SteeringAndMovement()
        {
            if (path.corners.Length >= 2)
            {
                Vector3 nextPosition = path.corners[1];

                Vector3 directionToNextPosition = nextPosition - currentPosition;
                Vector3 currentDirection = transform.forward;

                float targetAngle = Mathf.Atan2(directionToNextPosition.x, directionToNextPosition.z) * Mathf.Rad2Deg;
                float currentAngle = Mathf.Atan2(currentDirection.x, currentDirection.z) * Mathf.Rad2Deg;

                float deltaAngle = Steer(nextPosition);

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

        private float Steer(Vector3 position)
        {
            Vector3 directionToNextPosition = position - currentPosition;
            Vector3 currentDirection = transform.forward;

            float targetAngle = Mathf.Atan2(directionToNextPosition.x, directionToNextPosition.z) * Mathf.Rad2Deg;
            float currentAngle = Mathf.Atan2(currentDirection.x, currentDirection.z) * Mathf.Rad2Deg;

            float deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);

            m_TurnInputValue = Mathf.Clamp(deltaAngle * turnSpeed, -1.0f, 1.0f);

            return deltaAngle;
        }

        private void StopMovement()
        {
            m_MovementInputValue = 0.0f;
        }

        private void NewRandomThresholdDistance()
        {
            thresholdDistance = Random.Range(14.0f, 50.0f);
        }

        private bool IsTargetVisible(Vector3 position)
        {
            RaycastHit hit;

            Vector3 direction = (position - transform.position).normalized;

            if (Physics.Raycast(transform.position + Vector3.up + direction * 2.0f, direction, out hit, 100.0f))
            {
                if (hit.collider.gameObject.name.Contains("CompleteTank"))
                    return true;
            }

            return false;
        }

        private Vector3 GetClosestTankPosition(out float distance)
        {
            float bestDistance = 9999.0f;
            Vector3 bestPosition = Vector3.zero;

            for (int i = 0; i < GameManager.instance.m_Tanks.Length; i++)
            {
                GameObject otherTank = GameManager.instance.m_Tanks[i].m_Instance.gameObject;

                if (otherTank != gameObject &&
                    !otherTank.GetComponent<TankHealth>().m_Dead)
                {
                    Vector3 otherTankPosition = GameManager.instance.m_Tanks[i].m_Instance.transform.position;
                    float currentDistance = Vector3.Distance(otherTankPosition, currentPosition);

                    if (currentDistance < bestDistance)
                    {
                        bestDistance = currentDistance;
                        bestPosition = otherTankPosition;
                    }
                }
            }
            distance = bestDistance;

            return bestPosition;
        }

        public void SetShootParameters(float targetDistance)
        {

            int nearestLowerIndex;
            int nearestHigherIndex;

            SearchNearestLowerHigherPosition(targetDistance, out nearestLowerIndex, out nearestHigherIndex);

            if (nearestLowerIndex == -1 )
            {
                SetMinimumShootParameters();
            }
            else if (nearestHigherIndex == -1)
            {
                wantToShoot = false;
            }
            else
            {
                InterpolateResults(nearestLowerIndex, nearestHigherIndex, targetDistance);
            }
        }

        private void SearchNearestLowerHigherPosition(float targetDistance, out int nearestLowerIndex, out int nearestHigherIndex)
        {
            float bestLower = 0.0f;
            nearestLowerIndex = -1;

            float bestHigher = 9999999.0f;
            nearestHigherIndex = -1;

            float resultDistance = 0.0f;
            for (int i = 0; i < results.Count; i++)
            {
                resultDistance = results[i].distance;
                if (resultDistance < targetDistance && resultDistance > bestLower)
                {
                    nearestLowerIndex = i;
                    bestLower = resultDistance;
                }

                if (resultDistance > targetDistance && resultDistance < bestHigher)
                {
                    nearestHigherIndex = i;
                    bestHigher = resultDistance;
                }
            }
        }

        private void SetMinimumShootParameters()
        {
            targetShootPower = 16.0f; // Random.Range(15.0f, 30.0f);
        }

        private void InterpolateResults(int lowerIndex, int higherIndex, float targetDistance)
        {
            float lerp = 0.5f;

            const bool percentageApproximation = true;
            if (percentageApproximation)
            {
                lerp = targetDistance - results[lowerIndex].distance;
                lerp /= (results[higherIndex].distance - results[lowerIndex].distance);
                lerp = Mathf.Clamp01(lerp);
            }

            targetShootPower = Mathf.Lerp(results[lowerIndex].power, results[higherIndex].power, lerp);
        }

        public void StoreResults(Vector3 collisionPosition, float power)
        {

            float newDistance = Vector3.Distance(transform.position, collisionPosition);
            results.Add(new ShootResult { power = power, distance = newDistance });

            Debug.LogWarning("Results count: " + results.Count);
        }

        public void SaveResults()
        {
            string path = Application.dataPath + "/results.data";

            if (!File.Exists(path))
            {
                using (BinaryWriter sw = new BinaryWriter(File.Open(path, FileMode.Create)))
                {
                    sw.Write(results.Count);
                    for (int i = 0; i < results.Count; i++)
                    {
                        sw.Write(results[i].distance);
                        sw.Write(results[i].power);
                    }
                    sw.Close();
                }
            }
        }

        public void LoadResults()
        {
            string path = Application.dataPath + "/results.data";
            Debug.Log(path);

            if (File.Exists(path))
            {
                using (BinaryReader sw = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    int count = sw.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        results.Add(new ShootResult { distance = sw.ReadSingle(), power = sw.ReadSingle() });
                    }
                    sw.Close();
                }
            }
        }
    }
}
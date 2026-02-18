using System.Collections.Generic;
using UnityEngine;

public class PatrolPointManager : MonoBehaviour
{
    public BasicEnemyAI[] enemies;
    public Transform[] patrolPoints;

    public int minAssignedPatrolPoint = 2;

    void Awake()
    {
        // Use this code if you want to add patrol points automatically
        // Otherwise you need to add patrol points from inspector
        // Adding patrol points from inspector making debugging easier
        /*
        Transform[] all = GetComponentsInChildren<Transform>();
        List<Transform> pts = new List<Transform>();

        foreach (var t in all)
        {
            if (t != transform) // For excluding PatrolPointManager game object
                pts.Add(t);
        }
        patrolPoints = pts.ToArray();
        */

        AssignPatrolPoints();
    }

    void AssignPatrolPoints()
    {
        if (enemies.Length == 0 || patrolPoints.Length == 0) return;

        int patrolPointPerEnemy = Mathf.Max(1, patrolPoints.Length / enemies.Length);

        List<Transform> availablePatrolPoints = new List<Transform>(patrolPoints);

        foreach (var enemy in enemies)
        {
            int maxAssignable = Mathf.Min(patrolPointPerEnemy, availablePatrolPoints.Count);
            int minAssignable = Mathf.Min(minAssignedPatrolPoint, maxAssignable);

            int assignedPatrolPoints = Random.Range(minAssignable, maxAssignable + 1);

            List<Transform> enemyPatrolPoints = new List<Transform>();

            for (int i = 0; i < assignedPatrolPoints && availablePatrolPoints.Count > 0; i++)
            {
                int index = Random.Range(0, availablePatrolPoints.Count);
                enemyPatrolPoints.Add(availablePatrolPoints[index]);

                availablePatrolPoints.RemoveAt(index);
            }

            enemy.SetPatrolPoints(enemyPatrolPoints.ToArray());
        }

        while (availablePatrolPoints.Count > 0)
        {
            int enemyIndex = Random.Range(0, enemies.Length);
            BasicEnemyAI enemyAI = enemies[enemyIndex];
            if (enemyAI == null) continue;

            List<Transform> currentPoints = new List<Transform>(enemyAI.patrolPoints);
            int index = Random.Range(0, availablePatrolPoints.Count);

            currentPoints.Add(availablePatrolPoints[index]);
            availablePatrolPoints.RemoveAt(index);

            enemyAI.SetPatrolPoints(currentPoints.ToArray());
        }
    }

    void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        foreach (var point in patrolPoints)
        {
            if (point == null)
            {
                continue;
            }  

            Gizmos.color = new Color(1, 0, 0, 1f);
            Gizmos.DrawSphere(point.position, 0.3f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (enemies == null) return;

        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.patrolPoints == null) continue;

            Gizmos.color = Color.yellow;

            foreach (var p in enemy.patrolPoints)
            {
                if (p == null) continue;
                Gizmos.DrawLine(enemy.transform.position, p.position);
            }
        }
    }
}

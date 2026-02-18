using System.Collections.Generic;
using UnityEngine;

public class AlarmEnemy : MonoBehaviour
{
    public static AlarmEnemy Instance;

    List<BasicEnemyAI> enemies = new List<BasicEnemyAI>();
    public bool globalAlarm;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    public void RegisterEnemy(BasicEnemyAI enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void TriggerGlobalAlarm(Vector3 playerPos)
    {
        globalAlarm = true;

        foreach (var enemy in enemies)
        {
            enemy.OnGlobalAlarm(playerPos);
        }
    }

    public void ResetAlarm()
    {
        globalAlarm = false;
    }
}

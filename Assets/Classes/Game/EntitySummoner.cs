using System.Collections.Generic;
using UnityEngine;

public class EntitySummoner : MonoBehaviour
{
    public static List<Enemy> EnemiesInGame;
    public static List<Transform> EnemiesInGameTransform;

    public static Dictionary<Transform, Enemy> EnemyTransformPairs;
    public static Dictionary<int, GameObject> EnemyPrefabs;
    public static Dictionary<int, Queue<Enemy>> EnemyObjectPools;

    public static bool IsInitialized;

    private static float HealthIncrement = 0f; // Tracks how much health to add per spawn

    public static void Init()
    {
        if (!IsInitialized)
        {
            EnemyTransformPairs = new Dictionary<Transform, Enemy>();
            EnemyPrefabs = new Dictionary<int, GameObject>();
            EnemyObjectPools = new Dictionary<int, Queue<Enemy>>();
            EnemiesInGameTransform = new List<Transform>();
            EnemiesInGame = new List<Enemy>();

            EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");

            foreach (EnemySummonData Enemy in Enemies)
            {
                EnemyPrefabs.Add(Enemy.EnemyID, Enemy.EnemyPrefab);
                EnemyObjectPools.Add(Enemy.EnemyID, new Queue<Enemy>());
            }

            IsInitialized = true;
        }
        else
        {
            Debug.LogWarning("EntitySummoner is already initialized");
        }
    }

    public static Enemy SummonEnemy(int EnemyID)
    {
        Enemy SummonedEnemy = null;

        if (EnemyPrefabs.ContainsKey(EnemyID))
        {
            Queue<Enemy> ReferencedQueue = EnemyObjectPools[EnemyID];

            if (ReferencedQueue.Count > 0)
            {
                // Dequeue enemy and initialize
                SummonedEnemy = ReferencedQueue.Dequeue();
                SummonedEnemy.Init();
                SummonedEnemy.gameObject.SetActive(true);
            }
            else
            {
                // Instantiate new instance of enemy and initialize
                Vector3 spawnPosition = GameLoopManager.NodePositions[0];
                spawnPosition.y = 0; // Adjust y-coordinate as needed
                GameObject NewEnemy = Instantiate(EnemyPrefabs[EnemyID], spawnPosition, Quaternion.identity);
                SummonedEnemy = NewEnemy.GetComponent<Enemy>();
                SummonedEnemy.Init();
            }

            // Add the health increment to the enemy's max health
            SummonedEnemy.MaxHealth += HealthIncrement;
            SummonedEnemy.Health = SummonedEnemy.MaxHealth;

            // Increase the health increment for the next enemy
            HealthIncrement += 0.1f; // Add 0.1 health per spawn
        }
        else
        {
            Debug.Log("EntitySummoner: EnemyID " + EnemyID + " not found");
            return null;
        }

        if (!EnemiesInGame.Contains(SummonedEnemy)) EnemiesInGame.Add(SummonedEnemy);
        if (!EnemiesInGameTransform.Contains(SummonedEnemy.transform)) EnemiesInGameTransform.Add(SummonedEnemy.transform);
        if (!EnemyTransformPairs.ContainsKey(SummonedEnemy.transform)) EnemyTransformPairs.Add(SummonedEnemy.transform, SummonedEnemy);

        SummonedEnemy.ID = EnemyID;
        return SummonedEnemy;
    }

    public static void RemoveEnemy(Enemy EnemyToRemove)
    {
        EnemyTransformPairs.Remove(EnemyToRemove.transform);
        EnemyObjectPools[EnemyToRemove.ID].Enqueue(EnemyToRemove);
        EnemyToRemove.gameObject.SetActive(false);
        EnemiesInGameTransform.Remove(EnemyToRemove.transform);
        EnemiesInGame.Remove(EnemyToRemove);
    }

    public static void ClearEnemies() // New method to clear enemy references
    {
        EnemiesInGame.Clear();
        EnemiesInGameTransform.Clear();
        EnemyTransformPairs.Clear();
        foreach (var pool in EnemyObjectPools)
        {
            pool.Value.Clear();
        }
    }
}

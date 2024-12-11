using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class GameLoopManager : MonoBehaviour
{
    private static Queue<EnemyDamageData> DamageData;
    public static List<TowerBehaviour> TowersInGame;
    public static Vector3[] NodePositions;
    public static float[] NodeDistances;
    private static Queue<Enemy> EnemiesToRemove;
    private static Queue<int> EnemyIDsToSummon;
    public bool LoopShouldEnd;

    public Transform NodeParent;
    // Start is called before the first frame update
    void Start()
    {
        DamageData = new Queue<EnemyDamageData>();
        TowersInGame = new List<TowerBehaviour>();
        EnemyIDsToSummon = new Queue<int>();
        EnemiesToRemove = new Queue<Enemy>();
        EntitySummoner.Init();
        
        NodePositions = new Vector3[NodeParent.childCount];
        for (int i = 0; i < NodePositions.Length; i++)
        {
            NodePositions[i] = NodeParent.GetChild(i).position;
        }
        
        NodeDistances = new float[NodePositions.Length - 1];
        for (int i = 0; i < NodeDistances.Length; i++)
        {
            NodeDistances[i] = Vector3.Distance(NodePositions[i], NodePositions[i + 1]);
        }

        StartCoroutine(GameLoop());
        InvokeRepeating("SummonTest",0f,1f);
        
    }

    void SummonTest()
    {
        EnqueueEnemyIDToSummon(1);
    }

    // Update is called once per frame
    IEnumerator GameLoop()
    {
        while (LoopShouldEnd==false)
        {
            //spawn enemies

            if (EnemyIDsToSummon.Count > 0)
            {
                for(int i = 0; i < EnemyIDsToSummon.Count; i++)
                {
                    EntitySummoner.SummonEnemy(EnemyIDsToSummon.Dequeue());
                }
            }
            
            //spawn towers
            //move enemies
            
            NativeArray<int> NodeIndices = new NativeArray<int>(EntitySummoner.EnemiesInGame.Count(),Allocator.TempJob);
            NativeArray<float> EnemySpeeds = new NativeArray<float>(EntitySummoner.EnemiesInGame.Count(),Allocator.TempJob);
            NativeArray<Vector3> NodestoUse = new NativeArray<Vector3>(NodePositions,Allocator.TempJob);
            TransformAccessArray EnemyAccess= new TransformAccessArray(EntitySummoner.EnemiesInGameTransform.ToArray(), 2);
            
            for (int i = 0; i < EntitySummoner.EnemiesInGame.Count; i++)
            {
                NodeIndices[i] = EntitySummoner.EnemiesInGame[i].NodeIndex;
                EnemySpeeds[i] = EntitySummoner.EnemiesInGame[i].Speed;
            }
            MoveEnemiesJob MoveJob= new MoveEnemiesJob
            {
                NodeIndices = NodeIndices,
                EnemySpeeds = EnemySpeeds,
                NodePositions = NodestoUse,
                deltaTime = Time.deltaTime
              
            }; 
            
            JobHandle MoveJobHandle= MoveJob.Schedule(EnemyAccess);
            MoveJobHandle.Complete();
            
            for(int i=0 ; i<EntitySummoner.EnemiesInGame.Count; i++)
            {
                EntitySummoner.EnemiesInGame[i].NodeIndex = NodeIndices[i];
                
                if(EntitySummoner.EnemiesInGame[i].NodeIndex>=NodePositions.Length)
                {
                    EnqueueEnemyToRemove(EntitySummoner.EnemiesInGame[i]);
                }
            }

            NodestoUse.Dispose();
            EnemySpeeds.Dispose();
            NodeIndices.Dispose();
            EnemyAccess.Dispose();
            //tick towers
            foreach (TowerBehaviour tower in TowersInGame)
            {
                tower.Target = TowerTargeting.GetTarget(tower, TowerTargeting.TargetType.First);
                tower.Tick();
            }
            //apply effects
            //damage enemies
            if(DamageData.Count>0) 
            {
                for(int i = 0; i < DamageData.Count; i++)
                {
                   EnemyDamageData CurrentDamageData = DamageData.Dequeue();
                     CurrentDamageData.TargetedEnemy.Health -= CurrentDamageData.TotalDamage / CurrentDamageData.Resistance;
                     
                     if(CurrentDamageData.TargetedEnemy.Health<=0)
                     {
                         EnqueueEnemyToRemove(CurrentDamageData.TargetedEnemy);
                     }
                }
            }
            //remove enemies
            if(EnemiesToRemove.Count>0) 
            {
                for(int i = 0; i < EnemiesToRemove.Count; i++)
                {
                    EntitySummoner.RemoveEnemy(EnemiesToRemove.Dequeue());
                }
            }
            //remove towers

            yield return null;
        }
    }
    
    public static void EnqueueDamageData(EnemyDamageData damageData)
    {
        DamageData.Enqueue(damageData);
    }
    
    public static void EnqueueEnemyIDToSummon(int EnemyID)
    {
        EnemyIDsToSummon.Enqueue(EnemyID);
    }
    
    public static void EnqueueEnemyToRemove(Enemy EnemyToRemove)
    {
        EnemiesToRemove.Enqueue(EnemyToRemove);
    }
}

public struct EnemyDamageData
{
    public EnemyDamageData(Enemy target, float damage, float resistance)
    {
        TargetedEnemy = target;
        TotalDamage = damage;
        Resistance = resistance;
    }
    
    public Enemy TargetedEnemy;
    public float TotalDamage;
    public float Resistance;
}

public struct MoveEnemiesJob : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<int> NodeIndices;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> EnemySpeeds;
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> NodePositions;
    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        if (NodeIndices[index] < NodePositions.Length)
        {


            Vector3 PositiontoMoveTo = NodePositions[NodeIndices[index]];
            transform.position =
                Vector3.MoveTowards(transform.position, PositiontoMoveTo, EnemySpeeds[index] * deltaTime);

            if (transform.position == PositiontoMoveTo)
            {
                NodeIndices[index]++;
            }
        }
    }
}
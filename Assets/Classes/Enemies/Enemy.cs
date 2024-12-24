using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public List<Effect> ActiveEffects;
    public Transform RootPart;
    public float DamageResistance = 1f;
    public int NodeIndex;
    public float Health;
    public float MaxHealth;
    public float Speed;
    public int ID;
    public void Init()
    {
        ActiveEffects = new List<Effect>();
        transform.position=GameLoopManager.NodePositions[0];
        Health = MaxHealth;
        NodeIndex=0;

    }

    public void IncreaseHealthOverTime(float increment)
    {
        Health += increment;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    public void Tick()
    {
        for(int i=0;i<ActiveEffects.Count;i++)
        {
            if (ActiveEffects[i].ExpireTime > 0f)
            {
                if (ActiveEffects[i].DamageDelay > 0f)
                {
                    ActiveEffects[i].DamageDelay -= Time.deltaTime;
                }
                else
                {
                    GameLoopManager.EnqueueDamageData(new EnemyDamageData(this, ActiveEffects[i].Damage, 1f));
                    ActiveEffects[i].DamageDelay = 1f/ActiveEffects[i].DamageRate;
                }
                ActiveEffects[i].ExpireTime -= Time.deltaTime;
            }
        }
        ActiveEffects.RemoveAll(x => x.ExpireTime <= 0f);
        IncreaseHealthOverTime(0.1f);
    }
}
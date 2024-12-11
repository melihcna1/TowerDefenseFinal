using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public interface IDamageMethod
{
    public void DamageTick(Enemy Target);
    public void Init(float Damage, float Firerate);
}

public class StandartDamage : MonoBehaviour, IDamageMethod
{
    private float Damage;
    private float FireRate;
    private float Delay;

    public void Init(float Damage, float Firerate)
    {
        this.Damage = Damage;
        this.FireRate = Firerate;
        Delay = 1 / Firerate;
    }

    public void DamageTick(Enemy Target)
    { 
        if (Target){
            if (Delay > 0f)
            {
                Delay -= Time.deltaTime;
                return;
            }

        GameLoopManager.EnqueueDamageData(new EnemyDamageData(Target, Damage, Target.DamageResistance));
        Delay = 1f / FireRate;
    }
}

}

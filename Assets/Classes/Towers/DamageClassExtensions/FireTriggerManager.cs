using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTriggerManager : MonoBehaviour
{

    [SerializeField] private FlameThrowerDamage BaseClass;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Effect FlameEffect = new Effect("Fire", BaseClass.Damage, 5f, BaseClass.FireRate);
            ApplyEffectData EffectData = new ApplyEffectData(EntitySummoner.EnemyTransformPairs[other.transform.parent], FlameEffect);
            GameLoopManager.EnqueueEffectToApply(EffectData);
        }
    }
}

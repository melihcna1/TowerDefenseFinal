using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDamage : MonoBehaviour, IDamageMethod
{
    public LayerMask EnemiesLayer;
    [SerializeField] private ParticleSystem MissileSystem;
    [SerializeField] private Transform TowerHead;

    private ParticleSystem.MainModule MissileSystemMain;
    public float Damage;
    private float FireRate;
    private float Delay;

    public void Init(float Damage, float Firerate)
    {
        MissileSystemMain = MissileSystem.main;
        
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

            MissileSystemMain.startRotationX = TowerHead.forward.x;
            MissileSystemMain.startRotationY = TowerHead.forward.y;
            MissileSystemMain.startRotationZ = TowerHead.forward.z;

            MissileSystem.Play();
            Delay = 1f / FireRate;
        }
    }

}

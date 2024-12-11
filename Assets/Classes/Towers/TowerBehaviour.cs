using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public Enemy Target;
    public Transform TowerPivot;
    
    public LayerMask EnemiesLayer;
    public float Damage;
    public float Firerate;
    public float Range;
    public float Delay;

    private IDamageMethod CurrentDamageMethodClass;
    void Start()
    {
        CurrentDamageMethodClass = GetComponent<IDamageMethod>();

        if (CurrentDamageMethodClass == null)
        {
            Debug.LogError("No IDamageMethod component found on tower");
        }
        else
        {
            CurrentDamageMethodClass.Init(Damage, Firerate);
        }

        Delay = 1 / Firerate;
    }

    public void Tick()
    {
        CurrentDamageMethodClass.DamageTick(Target);
        if (Target != null)
        {
            TowerPivot.transform.rotation=Quaternion.LookRotation(Target.transform.position-transform.position);
        }
    }
}

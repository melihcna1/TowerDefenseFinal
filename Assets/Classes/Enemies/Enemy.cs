using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   public Transform RootPart;
   public float DamageResistance = 1f;
   public int NodeIndex;
   public float Health;
   public float MaxHealth;
   public float Speed;
   public int ID;
   public void Init()
   {
      transform.position=GameLoopManager.NodePositions[0];
      Health = MaxHealth;
      NodeIndex=0;

   }
}

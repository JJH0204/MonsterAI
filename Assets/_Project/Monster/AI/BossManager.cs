using System.Collections;
using System.Collections.Generic;
using Monster.AI;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private int currentPhase = 0;
    [SerializeField] private List<AIController> monsters = new();
    void OnMonsterDefeated()
    {}
}

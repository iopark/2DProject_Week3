using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private int health;

    [Header("Combat Events")]
    public UnityAction<int> HealthChange; 

    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            HealthChange?.Invoke(value);
            health = value;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Already exists");
            Destroy(this); 
        }
        instance = this;
        DontDestroyOnLoad(this); 
    }
}

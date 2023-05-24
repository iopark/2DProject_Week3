using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private static DataManager dataManager;
    public static GameManager Instance
    {
        get { return instance; }
    }

    public static DataManager Data
    {
        get { return dataManager; }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Already exists");
            Destroy(this); 
        }
        instance = this;
        SetManagers(); 
        DontDestroyOnLoad(this); 
    }

    private void SetManagers()
    {
        GameObject data = new GameObject("Data");
        data.transform.SetParent(transform);
        dataManager = data.GetComponent<DataManager>();
    }
}

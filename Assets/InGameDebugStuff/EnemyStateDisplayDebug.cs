using System.Collections.Generic;
using UnityEngine;

public class EnemyStateDisplayDebug : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject textDisplayTemplate;
    GameObject[] allEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject go in allEnemy)
        {
            GameObject spawnText = Instantiate(textDisplayTemplate, canvas.transform);
            spawnText.GetComponent<StateCheck>().enemy = go.GetComponent<EnemyClass>();
            spawnText.GetComponent<UIFollowObj>().obj = go.transform;
        }
    }
}

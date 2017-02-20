using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject enemyPref;

	// Use this for initialization
	void Start () {
        //Transform [] children = gameObject.GetComponentsInChildren<Transform>();


        
        for (int i =0; i < 50; i++)
        {
            float spawnPointIndex = Random.Range(0.0f, (float)transform.childCount);

            GameObject.Instantiate(enemyPref, transform.GetChild((int)spawnPointIndex).position, Quaternion.identity);
            Debug.Log((int)spawnPointIndex);
        }

        

        
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}

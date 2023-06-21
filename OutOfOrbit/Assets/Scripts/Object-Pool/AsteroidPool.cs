using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Asteroid
{
    public GameObject asteroidPrefab;
    public int amount;
    public bool expandable;
}

public sealed class Pool : MonoBehaviour
{
    public static Pool singleton { set; get; }

    public List<Asteroid> items;
    public List<GameObject> pooledItems;


    public void Awake()
    {
        if(singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        pooledItems = new List<GameObject>();
        
        foreach (Asteroid item in items)
        {
            for (int i=0; i<item.amount; i++)
            {
                //instantiate and deactivate
                GameObject obj = Instantiate(item.asteroidPrefab);
                obj.SetActive(false);
                pooledItems.Add(obj);
            }
        }
    }

    public GameObject Get(string tag)
    {
        for (int i=0; i<pooledItems.Count; i++)
        {
            if(!pooledItems[i].activeInHierarchy && pooledItems[i].tag == tag)
            {
                return pooledItems[i];
            }
        }

        foreach(Asteroid item in items)
        {
            if (item.asteroidPrefab.tag == tag && item.expandable)
            {
                GameObject go = Instantiate(item.asteroidPrefab);
                go.SetActive(false);
                pooledItems.Add(go);
                return go;
            }
        }

        return null;
    }
}

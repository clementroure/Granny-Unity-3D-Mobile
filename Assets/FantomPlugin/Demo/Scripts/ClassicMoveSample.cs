using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//従来の方法でのサンプル
public class ClassicMoveSample : MonoBehaviour {

    [SerializeField] private int spawnNum = 10000;
    public GameObject spawnArea;

    public GameObject prefab;
    public Text countDisplay;

    //Local Values
    List<GameObject> spawnObjects;


    // Use this for initialization
    private void Start () {
        spawnObjects = new List<GameObject>(spawnNum);
    }

    private void OnDisable()
    {
        Clear();
    }

    private void OnDestroy()
    {
        Clear();
    }

    // Update is called once per frame
    //private void Update () {

    //}


    private void Spawn()
    {
        if (prefab == null || spawnArea == null || spawnObjects.Count > 0)
            return;

        Vector3 area = spawnArea.transform.position;
        Vector3 scale = spawnArea.transform.localScale;
        float radius = Mathf.Max(scale.x, scale.y, scale.z);

        spawnObjects.Clear();
        for (int i = 0; i < spawnNum; i++)
        {
            Vector3 pos = Random.insideUnitSphere * radius + area;
            GameObject go = Instantiate(prefab, pos, prefab.transform.rotation);
            spawnObjects.Add(go);
        }

        DisplayCount();
    }

    private void Clear()
    {
        if (spawnObjects == null || spawnObjects.Count == 0)
            return;

        Debug.Log("ClassicMoveSample.Clear");
        for (int i = spawnObjects.Count - 1; i >= 0; i--)
        {
            if (spawnObjects[i] != null)
                Destroy(spawnObjects[i]);
        }
        spawnObjects.Clear();

        DisplayCount();
    }

    private void DisplayCount()
    {
        if (countDisplay != null)
            countDisplay.text = spawnObjects.Count.ToString();
    }

    //Callback handlers for UI

    public void OnSpawnClick()
    {
        if (gameObject.activeInHierarchy)
            Spawn();
    }

    public void OnClearClick()
    {
        if (gameObject.activeInHierarchy)
            Clear();
    }
}


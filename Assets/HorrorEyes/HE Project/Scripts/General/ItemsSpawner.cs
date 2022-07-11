using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsSpawner : MonoBehaviour
{
  
    public int m_maxPillsCount;
    public List<GameObject> m_picturesPrefabs = new List<GameObject>();
    public GameObject m_eyePillsPrefab;
    [HideInInspector]
    public List<ItemSpawnPlace> m_picturesSpawnPoints = new List<ItemSpawnPlace>();
    [HideInInspector]
    public List<ItemSpawnPlace> m_eyePillsSpawnPoints = new List<ItemSpawnPlace>();
    int r_point;
    int r_pictPref;

    private void Awake()
    {
        ItemSpawnPlace[] spawn = FindObjectsOfType<ItemSpawnPlace>();
        for (int i = 0; i < spawn.Length; i++)
        {
            if (spawn[i].m_paperSpawnPoint)
            {
                m_picturesSpawnPoints.Add(spawn[i]);
            }
            else
            {
                if (spawn[i].m_eyePillsSpawnPoint)
                {
                    m_eyePillsSpawnPoints.Add(spawn[i]);
                }
            }
        }

    }


    public void SpawnPills()
    {
        for (int i = 0; i < m_maxPillsCount; i++)
        {
            if (m_eyePillsSpawnPoints.Count > 0)
            {
                r_point = Random.Range(0, m_eyePillsSpawnPoints.Count);
                GameObject pills = Instantiate(m_eyePillsPrefab);
                pills.transform.position = m_eyePillsSpawnPoints[r_point].transform.position;
                pills.transform.rotation = m_eyePillsSpawnPoints[r_point].transform.rotation;
                pills.transform.parent = m_eyePillsSpawnPoints[r_point].transform;
                m_eyePillsSpawnPoints.RemoveAt(r_point);
            }

        }
    }

    public void SpawnPictures(int maxPictCount)
    {
        for (int i = 0; i < maxPictCount; i++)
        {
            r_point = Random.Range(0, m_picturesSpawnPoints.Count);
            r_pictPref = Random.Range(0, m_picturesPrefabs.Count);

            GameObject pict = Instantiate(m_picturesPrefabs[r_pictPref]);
            pict.transform.position = m_picturesSpawnPoints[r_point].transform.position;
            pict.transform.rotation = m_picturesSpawnPoints[r_point].transform.rotation;
            pict.transform.parent = m_picturesSpawnPoints[r_point].transform;

            m_picturesPrefabs.RemoveAt(r_pictPref);
            m_picturesSpawnPoints.RemoveAt(r_point);

        }
    }

 
}

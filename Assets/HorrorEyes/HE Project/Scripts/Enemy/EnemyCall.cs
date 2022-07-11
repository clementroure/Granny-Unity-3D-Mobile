using UnityEngine;

public class EnemyCall : MonoBehaviour {

    private GameObject[] enemy;

    private void Awake()
    {
        enemy = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public void CallEnemy()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].GetComponent<Enemy>().CallEnemy(transform.position);
        }
     
    }
}

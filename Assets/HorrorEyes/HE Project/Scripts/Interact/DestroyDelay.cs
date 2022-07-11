using System.Collections;
using UnityEngine;

public class DestroyDelay : MonoBehaviour {

    [Tooltip("Time after which the object will be destroyed")]
    public float destroyDelay;

    private void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);

    }

}

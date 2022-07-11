using UnityEngine;

public class CollideEvent : MonoBehaviour {

    [Tooltip("The sound that will be played when an object falls")]
    public AudioClip collideSound;

    void OnCollisionEnter(Collision collision)
    {     
        if (collision.relativeVelocity.magnitude > 2)
        {
            AudioSource.PlayClipAtPoint(collideSound, transform.position);
            GetComponent<EnemyCall>().CallEnemy();
        }
         
    }

}

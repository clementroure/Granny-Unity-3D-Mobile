using UnityEngine;
using Random = UnityEngine.Random;

//従来の方法でのサンプル
public class ClassicMove : MonoBehaviour {

    public Vector3 moveDirection = Vector3.forward;
    public float minMoveSpeed = 5f;
    public float maxMoveSpeed = 15f;

    public bool useRotate = false;
    public Vector3 rotateAngle = Vector3.forward;
    public float minRotateSpeed = 120f;
    public float maxRotateSpeed = 360f;


    // Use this for initialization
    private void Start () {
        moveDirection *= Random.Range(minMoveSpeed, maxMoveSpeed);
        rotateAngle *= Random.Range(minRotateSpeed, maxRotateSpeed);
    }
        
    // Update is called once per frame
    private void Update () {
        transform.Translate(moveDirection * Time.deltaTime, Space.Self);
        if (useRotate)
            transform.Rotate(rotateAngle * Time.deltaTime, Space.Self);
    }
}

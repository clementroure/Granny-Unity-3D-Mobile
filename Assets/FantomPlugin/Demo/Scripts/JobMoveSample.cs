using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

#if UNITY_2018_1_OR_NEWER
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
#endif

//Job System でのサンプル
public class JobMoveSample : MonoBehaviour {

#pragma warning disable 0414    //'spawnNum' is assigned but its value is never used. (However, it uses on the 'UNITY_2018_1_OR_NEWER'.)

    [SerializeField] private int spawnNum = 10000;
    public GameObject spawnArea;

    public GameObject prefab;
    public Text countDisplay;

    public Vector3 moveDirection = Vector3.forward;
    public float minMoveSpeed = 5f;
    public float maxMoveSpeed = 15f;

    public bool useRotate = false;
    public Vector3 rotateAngle = Vector3.forward;
    public float minRotateSpeed = 120f;
    public float maxRotateSpeed = 360f;

#if UNITY_2018_1_OR_NEWER

    //Local Values
    List<GameObject> spawnObjects;

    TransformAccessArray transformAccessArray;
    NativeArray<Vector3> moveDirections;
    NativeArray<Vector3> rotateAngles;

    struct ParallelUpdateJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Vector3> moveDirection;
        [ReadOnly] public NativeArray<Vector3> rotateAngle;
        [ReadOnly] public bool useRotate;
        [ReadOnly] public float deltaTime;

        public void Execute(int i, TransformAccess transform)
        {
            transform.position += transform.rotation * moveDirection[i] * deltaTime;
            if (useRotate)
                transform.rotation *= Quaternion.Euler(rotateAngle[i] * deltaTime);
        }
    }


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

    JobHandle jobHandle;

    // Update is called once per frame
    private void Update()
    {
        if (spawnObjects.Count == 0)
            return;

        var job = new ParallelUpdateJob()
        {
            moveDirection = moveDirections,
            rotateAngle = rotateAngles,
            useRotate = useRotate,
            deltaTime = Time.deltaTime
        };
        jobHandle = job.Schedule(transformAccessArray);
        JobHandle.ScheduleBatchedJobs();
    }

    private void LateUpdate()
    {
        jobHandle.Complete();
    }


    private void Spawn()
    {
        if (prefab == null || spawnArea == null || spawnObjects.Count > 0)
            return;

        Vector3 area = spawnArea.transform.position;
        Vector3 scale = spawnArea.transform.localScale;
        float radius = Mathf.Max(scale.x, scale.y, scale.z);

        transformAccessArray = new TransformAccessArray(spawnNum);
        moveDirections = new NativeArray<Vector3>(spawnNum, Allocator.Persistent);
        rotateAngles = new NativeArray<Vector3>(spawnNum, Allocator.Persistent);

        spawnObjects.Clear();
        for (int i = 0; i < spawnNum; i++)
        {
            Vector3 pos = Random.insideUnitSphere * radius + area;
            GameObject go = Instantiate(prefab, pos, prefab.transform.rotation);
            spawnObjects.Add(go);

            transformAccessArray.Add(go.transform);
            moveDirections[i] = moveDirection * Random.Range(minMoveSpeed, maxMoveSpeed);
            rotateAngles[i] = rotateAngle * Random.Range(minRotateSpeed, maxRotateSpeed);
        }

        DisplayCount();
    }

    private void Clear()
    {
        if (spawnObjects == null || spawnObjects.Count == 0)
            return;

        Debug.Log("JobMoveSample.Clear");
        if (transformAccessArray.isCreated)
            transformAccessArray.Dispose();
        if (moveDirections.IsCreated)
            moveDirections.Dispose();
        if (rotateAngles.IsCreated)
            rotateAngles.Dispose();

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
#endif

    //Callback handlers for UI

    //Register 'Button.OnClick'
    public void OnSpawnClick()
    {
#if UNITY_2018_1_OR_NEWER
        if (gameObject.activeInHierarchy)
            Spawn();
#endif
    }
        
    //Register 'Button.OnClick'
    public void OnClearClick()
    {
#if UNITY_2018_1_OR_NEWER
        if (gameObject.activeInHierarchy)
            Clear();
#endif
    }
}


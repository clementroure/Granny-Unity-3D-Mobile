using UnityEditor;
using UnityEngine;

public class HorrorEyesManager : Editor
{
    [SerializeField]
    private static string path;

    [MenuItem("Horror Template/Clear Game Progress")]
    static void ClearProgress()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("<color=green>Game Progress Deleted!: </color>A");
    }


    [MenuItem("Horror Template/Create Enemy From Selected Gameobject")]
    static void CreateEnemy()
    {
        GameObject enm = Selection.activeGameObject;
        if (enm != null)
        {
            if (enm.GetComponent<Animator>())
            {
                if (!enm.GetComponent<Enemy>())
                {
                    enm.tag = "Enemy";
                    enm.layer = 11;
                    enm.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Enemy/Monster_Controller") as RuntimeAnimatorController;
                    enm.AddComponent<CapsuleCollider>();
                    enm.GetComponent<CapsuleCollider>().center = Vector3.zero;
                    enm.GetComponent<CapsuleCollider>().height = 3f;
                    enm.GetComponent<CapsuleCollider>().radius = 0.5f;
                    enm.transform.rotation = Quaternion.identity;
                    enm.GetComponent<Animator>().applyRootMotion = false;
                    ///Add Rigidbody
                    enm.AddComponent<Rigidbody>();
                    enm.GetComponent<Rigidbody>().isKinematic = true;
                    ///Add Navmesh Agent
                    enm.AddComponent<UnityEngine.AI.NavMeshAgent>();
                    enm.GetComponent<UnityEngine.AI.NavMeshAgent>().radius = 0.5f;
                    enm.GetComponent<UnityEngine.AI.NavMeshAgent>().height = 2.7f;
                    ///Add Audiosource
                    enm.AddComponent<AudioSource>();
                    enm.GetComponent<AudioSource>().spatialBlend = 1f;
                    enm.GetComponent<AudioSource>().minDistance = 0.5f;
                    enm.GetComponent<AudioSource>().maxDistance = 10f;
                    ////Setup Enmy Script
                    enm.AddComponent<Enemy>();

                    GameObject eyesCamera = Instantiate(Resources.Load("Enemy/EnemyEyesCamera") as GameObject);
                    GameObject handsItems = Instantiate(Resources.Load("Enemy/EnemyHandsItems") as GameObject);
                    GameObject eyes = new GameObject("Eyes");

                    eyes.transform.parent = enm.transform;
                    handsItems.transform.parent = enm.transform;
                    eyesCamera.transform.parent = enm.transform;

                    eyes.transform.localPosition = Vector3.zero;
                    handsItems.transform.localPosition = Vector3.zero;
                    eyesCamera.transform.localPosition = Vector3.zero;

                    enm.GetComponent<Enemy>().head = eyes.transform;
                    enm.GetComponent<Enemy>().m_enemyHeadCamera = eyesCamera;

                    enm.GetComponent<Enemy>().m_enemyHandsObjects = new GameObject[1];
                    enm.GetComponent<Enemy>().m_enemyHandsObjects[0] = handsItems.transform.GetChild(0).gameObject;

                    if (enm.GetComponent<Animator>().avatar.isHuman)
                    {
                        Transform head = enm.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
                        Transform rHand = enm.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
                        eyes.transform.parent = head;
                        eyes.transform.localPosition = Vector3.zero;
                        eyes.transform.rotation = Quaternion.identity;
                        handsItems.transform.parent = rHand;
                        handsItems.transform.localPosition = Vector3.zero;
                        handsItems.transform.localRotation = Quaternion.identity;                   
                        eyesCamera.transform.parent = head;
                        eyesCamera.transform.localPosition = Vector3.zero;
                        eyesCamera.transform.rotation = Quaternion.identity;
                        eyesCamera.gameObject.SetActive(false);

                    } else
                    {
                        foreach (Transform child in enm.GetComponentsInChildren<Transform>())
                        {
                            string nm = child.gameObject.name;

                            if (nm.Contains("Head") || nm.Contains("head"))
                            {
                                if (!child.gameObject.GetComponent<Renderer>())
                                {
                                    string nm2 = child.transform.parent.gameObject.name;

                                    if (!nm2.Contains("Head") && !nm2.Contains("head"))
                                    {
                                        Transform head = child.transform;
                                        eyes.transform.parent = head;
                                        eyes.transform.localPosition = Vector3.zero;
                                        eyes.transform.rotation = Quaternion.identity;
                                        eyesCamera.transform.parent = head;
                                        eyesCamera.transform.localPosition = Vector3.zero;
                                        eyesCamera.transform.rotation = Quaternion.identity;
                                        eyesCamera.gameObject.SetActive(false);
                                    }
                                }
                            }
                        }
                    }
                    int[] cnt = new int[3] { 0, 8, 9 };
                    enm.GetComponent<Enemy>().searchLayers = (1 << cnt[0]) | (1 << cnt[1]) | (1 << cnt[2]);
                    enm.GetComponent<Enemy>().seeDistance = 10f;
                    enm.GetComponent<Enemy>().lostTime = 3f;
                    enm.GetComponent<Enemy>().patrolTime = 5f;
                    enm.GetComponent<Enemy>().wayPointWaitTime = 3f;
                    enm.GetComponent<Enemy>().enemyFOV = new Vector2(-70,70);
                    enm.GetComponent<Enemy>().closeDistance = 5f;
                    enm.GetComponent<Enemy>().walkSpeed = 1.5f;
                    enm.GetComponent<Enemy>().killDistance = 2f;
                    enm.GetComponent<Enemy>().playerLookSpeed = 100f;
                    enm.GetComponent<Enemy>().m_radiusToFindInteractObjects = 15f;
                    GameObject pkp = new GameObject("PlayerKillPosition");
                    pkp.transform.parent = enm.transform;
                    pkp.transform.localPosition = new Vector3(0,1.36f,0.7f);
                    pkp.transform.localEulerAngles = new Vector3(0, 180, 0);
                    enm.GetComponent<Enemy>().playerKillPos = pkp.transform;
                    Transform wp = FindObjectOfType<WayPoints>().transform;
                    Transform[] children = new Transform[wp.childCount];
                    for (int i = 0; i < wp.childCount; i++)
                    {
                        children[i] = wp.GetChild(i);
                    }

                    enm.GetComponent<Enemy>().wayPoints = children;

                    string name = enm.name;
                    Debug.Log("Gameobject <color=green>" + name + "</color> ready as enemy gameobject! Don't forget to change " +
                        "capsule colider size, set up foot steps and other sounds!");

                }
                else
                {
                    string name = enm.name;
                    Debug.Log("<color=red>Gameobject " + name + " already have 'Enemy.cs' script! Pleace, select clean gameobject without other components, only with Animator!!: </color>");
                }
            } else
            {
                string name = enm.name;
                Debug.Log("<color=red>Gameobject " + name + " must have a Animator component!: </color>");
            }
        }else
        {
            Debug.Log("<color=red>You need select enemy model gameobject!: </color>");
        }
    }
}
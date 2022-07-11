using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    [Header("General Settings")]
    private PlayerController player;
    [Tooltip("Transform of enemy head")]
    public Transform head;
    [Tooltip("Layers by which the enemy will search for the player (Must include player layer, as well as obstacle layers (Default,Interact еtc))")]
    public LayerMask searchLayers;
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Animator animator;

    [Header("AI Settings")]
    [Tooltip("Distance through which the enemy sees the player")]
    public float seeDistance;
    [Tooltip("Enemy Field of View (x - minimal, y - maximal)")]
    public Vector2 enemyFOV;
    [Tooltip("Radius at which the enemy will notice the player if the player is too close (if player crouch this value = / 2) ")]
    public float closeDistance;
    [Tooltip("The time after which the enemy loses the player(after the time has passed, the player’s coordinates are transferred to the enemy and he goes to these coordinates)")]
    public float lostTime;
    float m_currentSearchTime;
    float m_currentFollowTime;
    [Tooltip("The time that the enemy spends at the point where he last saw or heard the player")]
    public float patrolTime;
    [Tooltip("Controll enemy walk speed")]
    public float walkSpeed;
    [HideInInspector]
    public bool seePlayer;
    [HideInInspector]
    public bool searchPlayer;
    [HideInInspector]
    public bool alwaysSeePlayer;


    [Header("Kill Settings")]
    [Tooltip("Distance to kill player")]
    public float killDistance;
    public Transform playerKillPos;
    public float playerLookSpeed;
    int killState = 0;

    [Header("Eyes")]
    public GameObject m_enemyHeadCamera;

    [Header("Sound Settings")]
    [Tooltip("Sound of enemy footsteps")]
    public AudioClip[] footSteps;
    [Tooltip("The sound that the enemy makes when he catch a player")]
    public AudioClip catchSound;
    [Tooltip("Player Killing Sound")]
    public AudioClip killSound;
    private AudioSource AS;
    

    [Header("Patrol Settings")]
    [Tooltip("Transforms of way points for enemy patrolling")]
    public Transform[] wayPoints;
    [Tooltip("The time that the enemy waits on points")]
    public float wayPointWaitTime;
    private int wpID;
    private Vector3 lastSawPoint;

    [Header("Enemy Interact Point Settings")]

    public bool m_useEnemyInteractPoints;
    [HideInInspector]
    public EnemyInteractPoint m_interactPoint;
    [HideInInspector]
    public bool m_enemyUsingPoint;
    public GameObject[] m_enemyHandsObjects;
    public float m_radiusToFindInteractObjects;
    private float m_findRadius;
    int radDegres = 0;

    private void Awake()
    {
        Transform wp = FindObjectOfType<WayPoints>().transform;
        Transform[] children = new Transform[wp.childCount];
        for (int i = 0; i < wp.childCount; i++)
        {
            children[i] = wp.GetChild(i);
        }

        wayPoints = children;
        player = FindObjectOfType<PlayerController>();
        AS = GetComponent<AudioSource>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = walkSpeed;
        animator = GetComponent<Animator>();
        lastSawPoint = Vector3.zero;
        wpID = -1;
    }

    private void Update()
    {
     

        if (killState == 0)
        {
            UpdateMovement();

            if (!seePlayer && !searchPlayer)
            {
           

                if (m_interactPoint == null)
                {
                    PatrolWayPoints();
                }
            }

            if(!seePlayer)
            {
                if (m_useEnemyInteractPoints)
                {
                    if(radDegres == 0)
                    {
                        m_findRadius += 20f * Time.deltaTime;
                        if(m_findRadius >= m_radiusToFindInteractObjects)
                        {
                            radDegres = 1;
                        }
                    }else
                    {
                        m_findRadius -= 20f * Time.deltaTime;
                        if (m_findRadius <= 0)
                        {
                            radDegres = 0;
                        }
                    }

                    SearchInteractPoints();
                }
            }

            if (alwaysSeePlayer)
            {
                searchPlayer = true;
                lastSawPoint = player.transform.position;
            }

            SearchPlayer();

           
        }

        if(seePlayer && !m_enemyUsingPoint)
        {
            if (!player.lockedByDying)
            {
                KillPlayer();
            }else
            {
                agent.enabled = false;
                KillPlayer();
            }
        }

    }

    private void UpdateMovement()
    {
        float speed = agent.velocity.magnitude;
        Mathf.Clamp(speed,0,1);
        animator.SetFloat("MoveSpeed",speed,0.2f, Time.deltaTime);
    }

    private void SearchInteractPoints()
    {
        if(m_interactPoint == null)
        {
        
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, m_findRadius, transform.up, out hit, m_radiusToFindInteractObjects))
            {
                if (hit.transform.gameObject.tag == "Interact")
                {
                  
                    if (hit.transform.GetComponent<EnemyInteractPoint>())
                    {
                        if(!hit.transform.GetComponent<EnemyInteractPoint>().m_used && !hit.transform.GetComponent<EnemyInteractPoint>().m_alreadyUsed)
                        {
                            if (searchPlayer && hit.transform.GetComponent<EnemyInteractPoint>().m_useIfSearch)
                            {
                                float dist = Vector3.Distance(lastSawPoint, hit.transform.GetComponent<EnemyInteractPoint>().m_enemyStartActionPoint.position);
                                if (dist <= 15f)
                                {
                                    ResetPatrol();
                                    hit.transform.GetComponent<EnemyInteractPoint>().m_alreadyUsed = true;
                                    m_interactPoint = hit.transform.GetComponent<EnemyInteractPoint>();
                                }
                                
                            }else
                            {
                                if (!searchPlayer && !hit.transform.GetComponent<EnemyInteractPoint>().m_useIfSearch)
                                {
                                    hit.transform.GetComponent<EnemyInteractPoint>().m_alreadyUsed = true;
                                    m_interactPoint = hit.transform.GetComponent<EnemyInteractPoint>();
                                }
                            }
                        }
                    }
                }
            }
        }else
        {
            if (!m_enemyUsingPoint)
            {
                EnemySetDestination(m_interactPoint.m_enemyStartActionPoint.position);

                float dist = Vector3.Distance(transform.position, m_interactPoint.m_enemyStartActionPoint.position);
                if (dist <= agent.stoppingDistance)
                {
                    transform.position = Vector3.Lerp(transform.position, m_interactPoint.m_enemyStartActionPoint.position, 1.5f * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, m_interactPoint.m_enemyStartActionPoint.rotation, 1.5f * Time.deltaTime);


                    float ang = Quaternion.Angle(transform.rotation, m_interactPoint.m_enemyStartActionPoint.rotation);

                    if (ang < 5)
                    {
                        m_interactPoint.m_enemyAI = this;
                        m_enemyUsingPoint = true;
                        m_interactPoint.EnemyStartAction();

                    }
            }   }
        }
    }

    private void KillPlayer()
    {
        if (killState == 0 && !player.lockedByDying)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist <= killDistance)
            {
                agent.enabled = false;
                player.locked = true;
                player.CatchPlayer(1);
                killState = 1;
                AS.PlayOneShot(catchSound);
            }
        }

        if(killState == 1)
        {
            Vector3 lTargetDir = head.position - player.cameraTransform.position;
            player.cameraTransform.rotation = Quaternion.RotateTowards(player.cameraTransform.rotation, Quaternion.LookRotation(lTargetDir), playerLookSpeed * Time.deltaTime);
            player.transform.position = Vector3.Slerp(player.transform.position,playerKillPos.position, 10f * Time.deltaTime);
            Vector3 lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            Quaternion rotation2 = Quaternion.LookRotation(-lookPos);


            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, playerLookSpeed * Time.deltaTime);
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, rotation2, playerLookSpeed * Time.deltaTime);


            float ang = Quaternion.Angle(player.cameraTransform.rotation, Quaternion.LookRotation(lTargetDir));

            if (ang <= 4.0)
            {
                AS.PlayOneShot(killSound);
                transform.rotation = rotation;
                player.transform.position = playerKillPos.position;
                player.transform.rotation = rotation2;
                lTargetDir = head.position - player.cameraTransform.position;
                player.cameraTransform.rotation = Quaternion.LookRotation(lTargetDir);
                animator.SetBool("KillPlayer",true);
                killState = 2;
                player.CatchPlayer(2);
            }

        }

        if(killState == 2)
        {
            Vector3 lTargetDir = head.position - player.cameraTransform.position;
            player.cameraTransform.rotation = Quaternion.RotateTowards(player.cameraTransform.rotation, Quaternion.LookRotation(lTargetDir), playerLookSpeed * Time.deltaTime);
            player.transform.position = Vector3.Slerp(player.transform.position, playerKillPos.position, 10f * Time.deltaTime);
            Vector3 lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            Quaternion rotation2 = Quaternion.LookRotation(-lookPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, playerLookSpeed * Time.deltaTime);
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, rotation2, playerLookSpeed * Time.deltaTime);
        }


    }

    private void SearchPlayer()
    {
        if(PlayerFOV() && PlayerRaycast())
        {
            if(m_interactPoint != null)
            {
                if (m_enemyUsingPoint)
                {
                    m_interactPoint.BreakAction();
                }
                m_interactPoint = null;
            }
            ResetPatrol();
            seePlayer = true;
            searchPlayer = true;
            lastSawPoint = player.transform.position;
            m_currentSearchTime = patrolTime;
            m_currentFollowTime = lostTime;

        }else
        {
            if(!alwaysSeePlayer)
            {
                m_currentFollowTime -= 2f * Time.deltaTime;
                if (m_currentFollowTime <= 0f)
                {
                    seePlayer = false;
                }
            }
        }


        if(seePlayer)
        {
            EnemySetDestination(player.transform.position);
        }

        if(!seePlayer && searchPlayer)
        {
            if (lastSawPoint != Vector3.zero)
            {
                EnemySetDestination(lastSawPoint);
                float dista = Vector3.Distance(transform.position, lastSawPoint);

                if (dista <= 2f)
                {
                    m_currentSearchTime -= 2f * Time.deltaTime;
                    if (m_currentSearchTime <= 0f)
                    {
                        ResetPatrol();
                    }
                  

                }
            }else
            {
                ResetPatrol();
     
            }
        }

       

    }

    private void PatrolWayPoints()
    {
        if (!seePlayer && !searchPlayer && wpID == -1)
        {
            StopAllCoroutines();
            int r = Random.Range(1, wayPoints.Length);
            EnemySetDestination(wayPoints[r].position);
            wpID = r;
            wayPoints[r].SetSiblingIndex(0);


        }

        if (wpID != -1)
        {
            float dist = Vector3.Distance(transform.position, wayPoints[wpID].position);
            if (dist <= agent.stoppingDistance)
            {
                EnemySetDestination(transform.position);
                StartCoroutine(WaitPatrolTime());
            }
        }
    }

    public void CallEnemy(Vector3 pos)
    {
        if(!seePlayer)
        {
            if (m_interactPoint != null)
            {
                if (m_enemyUsingPoint)
                {
                    m_interactPoint.BreakAction();
                }
                m_interactPoint = null;
            }
            searchPlayer = true;
            lastSawPoint = pos;                                   
        }
    }

    private bool PlayerRaycast()
    {
        RaycastHit hit;
      
        if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, searchLayers))
        {
            if (hit.transform.gameObject == player.gameObject)
            {
                float dist = Vector3.Distance(transform.position,player.transform.position);
                if(dist <= seeDistance)
                {
                    return true;
                }
               
            }
        }

        return false;
    }

    private bool PlayerFOV()
    {
        Vector3 targetDir = player.transform.position - head.transform.position;
        float angleToPlayer = (Vector3.Angle(targetDir, head.forward));

        if (angleToPlayer >= enemyFOV.x && angleToPlayer <= enemyFOV.y) // 180° FOV

        {
            return true;
        }

        return false;
    }

    public void ResetPatrol()
    {
        m_currentSearchTime = 0f;
        searchPlayer = false;
        lastSawPoint = Vector3.zero;
        wpID = -1;    
    }

    private void EnemySetDestination(Vector3 pos)
    {
        if(agent.enabled)
        {
            agent.SetDestination(pos);
        }
    }

    IEnumerator WaitLostTime()
    {
        yield return new WaitForSeconds(lostTime);
        lastSawPoint = player.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(lastSawPoint, out hit, 1f, NavMesh.AllAreas))
        {
            lastSawPoint = hit.position;
        }

    
    }

    IEnumerator WaitPatrolTime()
    {
        yield return new WaitForSeconds(patrolTime);
        ResetPatrol();
    }

}

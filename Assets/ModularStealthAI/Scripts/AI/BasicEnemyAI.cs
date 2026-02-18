using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyAI : MonoBehaviour
{
    [Header("Referances")]
    [HideInInspector] public Transform[] patrolPoints;
    [SerializeField] Transform player;

    Light spotLight;

    [Header("AI Vision Settigns")]
    public float detectionRange = 16f;
    public float visionRange = 20f;

    [Header("AI Movement Settigns")]
    public float walking = 5f;
    public float running = 9f;
    public float attackRange = 2f;

    [Header("Patrol Point Settings")]
    public float minWaitSec = 0.5f;
    public float maxWaitSec = 1.5f;

    [Header("Detection Settings")]
    [Tooltip("For adjusting 'Suspicion Time' decrement speed")]
    public float checkCooldown = 0.1f;
    public float suspicionTime = 0.2f;
    [Tooltip("Suspicion Time * Alarm Multiplier = Detection Time")]
    public float alarmMultiplier = 2f;
    public LayerMask visionMask = ~0;

    [Header("Search Settings")]
    public float searchDuration = 4f;

    [Header("AI View Settings")]
    [Range(0, 360)]
    public float viewAngle = 120f;
    [Range(0, 360)]
    public float chaseViewAngle = 180f;
    public float adjustViewAngle = 0.8f;

    float targetSpotAngle;

    public float suspicionTimer;
    float searchTimer = 0f;

    Vector3 lastSeenPlayerPos;
    bool hasLastSeenPos = false;
    float detectDistance;

    float waitSec;
    float waitAtPatrolTimer = 0f;

    int currentPatrolIndex = 0;
    int direction = 1;

    enum State { Patrol, Chase, Attack, Search, Check }

    enum PatrolState { Loop, PingPong, Random }

    NavMeshAgent agent;

    State currentState;
    PatrolState currentPatrolState;
    Color currentColor;

    const float minRemainingDist = 2f;
    const float distance = 1.5f;
    const float rotateAtSearch = 120f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            Debug.LogError(name + " has no NavMeshAgent!");
            enabled = false;
            return;
        }

        currentPatrolState = (PatrolState)Random.Range(0, 3);
        Debug.Log("Patrol State: " + currentPatrolState);

        spotLight = GetComponentInChildren<Light>();

        if (spotLight != null)
        {
            spotLight.spotAngle = viewAngle;
            targetSpotAngle = viewAngle;
            currentColor = Color.green;
            currentColor = spotLight.color;
        }

        if(AlarmEnemy.Instance != null)
        {
            AlarmEnemy.Instance.RegisterEnemy(this);
        }

        SwitchState(State.Patrol);

        waitSec = Random.Range(minWaitSec, maxWaitSec);

        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }

    void Update()
    {
        AIState();
        SmoothLightAngle();
    }

    void AIState()
    {
        if (player == null)
        {
            AdjustPatrolState();
            return;
        }

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (currentState)
        {
            case State.Patrol:
                AdjustPatrolState();
                break;
            
            case State.Chase:
                ChaseState(distanceToPlayer);
                break;
            
            case State.Attack:
                AttackState(distanceToPlayer);
                break;

            case State.Search:
                SearchState();
                break;

            case State.Check:
                CheckState();
                break;
            
            default:
                SwitchState(State.Patrol);
                break;
        }
    }

    void AdjustPatrolState()
    {
        bool canSee = CanSeePlayer();
        Color suspicionColor = Color.orangeRed;

        if (suspicionTimer < 0f)
        {
            suspicionTimer = 0f;
        }

        if (!agent.pathPending && agent.remainingDistance < minRemainingDist)
        {
            StopAtPatrolPoint();
            if (!agent.isStopped)
            {
                GoToNextPatrolPoint();
            }
        }

        if (canSee)
        {
            UpdateLight(currentColor, suspicionColor, suspicionTime, suspicionTimer);
            suspicionTimer += Time.deltaTime * detectDistance;

            if (suspicionTimer >= suspicionTime)
            {
                SwitchState(State.Check);
            }
        }
        else if (!canSee)
        {
            UpdateLight(currentColor, suspicionColor, suspicionTime, suspicionTimer);
            if (suspicionTimer > 0)
            {
                suspicionTimer -= Time.deltaTime;
            }
        }

        suspicionTimer = Mathf.Clamp(suspicionTimer, 0f, suspicionTime);
    }

    void CheckState()
    {
        if (player == null)
        {
            return;
        }

        bool canSee = CanSeePlayer();
        float detectTime = suspicionTime * alarmMultiplier;
        Color alertColor = Color.violetRed;

        // Intentionally continues tracking player during suspicion phase for more aggressive gameplay
        if(Vector3.Distance(agent.destination, player.position) > distance)
        {
            agent.SetDestination(player.position);
        }

        if (canSee)
        {
            UpdateLight(currentColor, alertColor, detectTime, suspicionTimer);
            suspicionTimer += Time.deltaTime * detectDistance;
        }
        else if (!canSee)
        {
            UpdateLight(currentColor, alertColor, detectTime, suspicionTimer);
            if (suspicionTimer > 0)
            {
                suspicionTimer -= Time.deltaTime * checkCooldown;
            }
        }
        
        if (suspicionTimer <= 0)
        {
            agent.ResetPath();
            StopAtPatrolPoint();
            if (!agent.isStopped)
            {
                SwitchState(State.Patrol);
            }
        }
        else if (suspicionTimer >= detectTime)
        {
            SwitchState(State.Chase);
        }

        suspicionTimer = Mathf.Clamp(suspicionTimer, 0f, detectTime);
    }

    void ChaseState(float distanceToPlayer)
    {
        if (player == null)
        {
            return;
        }

        lastSeenPlayerPos = player.position;
        hasLastSeenPos = true;

        if (Vector3.Distance(agent.destination, player.position) > distance)
        {
            agent.SetDestination(player.position);
        }

        if (distanceToPlayer < attackRange)
        {
            SwitchState(State.Attack);
        }
        else if (distanceToPlayer > visionRange)
        {
            SwitchState(State.Search);
        }
    }

    void AttackState(float distanceToPlayer)
    {
        if (player == null)
        {
            return;
        }

        transform.LookAt(player);
        bool canSee = CanSeePlayer();

        if (distanceToPlayer > attackRange && canSee)
        {
            SwitchState(State.Chase);
        }

        if (!canSee)
        {
            float searchDistanceOffset = 1f;
            if (distanceToPlayer <= attackRange + searchDistanceOffset)
            {
                return; // Do not search yet if it is behind the wall but still too close
            }

            SwitchState(State.Search);
            return;
        }
    }

    void SearchState()
    {
        if (!hasLastSeenPos)
        {
            SwitchState(State.Patrol);
            return;
        }

        if(Vector3.Distance(agent.destination, lastSeenPlayerPos) > distance)
        {
            agent.SetDestination(lastSeenPlayerPos);
        }

        if (!agent.pathPending && agent.remainingDistance < minRemainingDist)
        {
            agent.isStopped = true;
            searchTimer += Time.deltaTime;

            transform.Rotate(0f, rotateAtSearch * Time.deltaTime, 0f);

            if (searchTimer >= searchDuration)
            {
                hasLastSeenPos = false;
                SwitchState(State.Patrol);
                searchTimer = 0f;
            }
        }

        if (CanSeePlayer())
        {
            agent.isStopped = false;
            SwitchState(State.Chase);
        }

        //Debug.Log($"[{name}] state:{currentState} hasPath:{agent.hasPath} remaining:{agent.remainingDistance} stopped:{agent.isStopped}");
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }

        switch (currentPatrolState)
        {
            case PatrolState.Loop:
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                break;
            
            case PatrolState.PingPong:
                if (patrolPoints.Length <= 1)
                {
                    currentPatrolState = PatrolState.Loop;
                    return;
                }
                
                currentPatrolIndex += direction;
                if (currentPatrolIndex == patrolPoints.Length)
                {
                    currentPatrolIndex = patrolPoints.Length - 2;
                    direction = -1;
                }
                else if (currentPatrolIndex == -1)
                {
                    currentPatrolIndex = 1;
                    direction = 1;
                }
                break;

            case PatrolState.Random:
                int newIndex;
                if (patrolPoints.Length > 1)
                {
                    do
                    {
                        newIndex = Random.Range(0, patrolPoints.Length);
                    } while (newIndex == currentPatrolIndex);
                    currentPatrolIndex = newIndex;
                }
                break;

            default:
                currentPatrolState = PatrolState.Loop;
                break;
        }

        agent.destination = patrolPoints[currentPatrolIndex].position;
    }

    void StopAtPatrolPoint()
    {
        agent.isStopped = true;

        waitAtPatrolTimer += Time.deltaTime;
        if (waitAtPatrolTimer >= waitSec)
        {
            waitAtPatrolTimer = 0f;
            waitSec = Random.Range(minWaitSec, maxWaitSec);
            agent.isStopped = false;  
        }
    }

    bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }

        Vector3 origin = transform.position;
        Vector3 target = player.position;
        Vector3 toPlayer = target - origin;

        float dist = toPlayer.magnitude;


        if (dist > detectionRange)
        {
            return false;
        } 

        if (dist < 0.001f)
        {
            return true;
        }

        // If player far away from enemy, detect player slower
        detectDistance = 1f - Mathf.InverseLerp(0f, detectionRange, dist);
        detectDistance *= detectDistance;

        Vector3 dir = toPlayer / dist;

        float angleToPlayer = Vector3.Angle(transform.forward, dir);
        if (angleToPlayer > viewAngle * adjustViewAngle)
        {
            return false;
        }

        Debug.DrawRay(origin, dir * dist, Color.red);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, visionMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.GetComponent<Controller>() || hit.transform.root == player.root)
            {
                return true;
            }

            return false;
        }

        return false;
    }

    void TriggerAlarm()
    {
        if (AlarmEnemy.Instance != null && !AlarmEnemy.Instance.globalAlarm && player != null)
        {
            AlarmEnemy.Instance.TriggerGlobalAlarm(player.position);
        }
    }

    public void OnGlobalAlarm(Vector3 playerPos)
    {
        lastSeenPlayerPos = playerPos;
        hasLastSeenPos = true;

        suspicionTimer = suspicionTime * alarmMultiplier;

        SwitchState(State.Search);
    }

    void SwitchState(State newState)
    {
        OnExitState(currentState);
        agent.ResetPath();

        agent.isStopped = false;
        waitAtPatrolTimer = 0f;

        currentState = newState;

        OnEnterState(newState);
    }

    void OnEnterState(State state)
    {
        switch(state)
        {
            case State.Chase:
                if (AlarmEnemy.Instance != null)
                {
                    TriggerAlarm();
                }
                
                if (spotLight != null)
                {
                    spotLight.color = Color.red;
                    targetSpotAngle = chaseViewAngle;
                }
                agent.speed = running;
                break;

            case State.Patrol:
                if (AlarmEnemy.Instance != null)
                {
                    AlarmEnemy.Instance.ResetAlarm();   
                }

                if (spotLight != null)
                {
                    currentColor = Color.green;
                    targetSpotAngle = viewAngle;
                    agent.speed = walking;
                    suspicionTimer = 0;
                }
                break;

            case State.Attack:
                break;
            
            case State.Search:
                if (spotLight != null)
                {
                    spotLight.color = Color.yellow;
                }
                agent.speed = walking;
                searchTimer = 0f;
                break;

            case State.Check:
                if (spotLight != null)
                {
                    currentColor = Color.orangeRed;
                }
                break;

            default:
                currentState = State.Patrol;
                break;
        }
    }

    void OnExitState(State state)
    {
        // for animations etc.
    }

    public void SetPatrolPoints(Transform[] points)
    {
        patrolPoints = points;
        currentPatrolIndex = 0;

        string pointNames = "";
        foreach (var p in patrolPoints)
            pointNames += p.name + ", ";
        //Debug.Log(name + " patrol points set: " + pointNames);

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
            agent.destination = patrolPoints[0].position;
    }

    void UpdateLight(Color startColor, Color targetColor, float time, float timer)
    {
        if (spotLight != null)
        {
            float t = timer / time;
            t = Mathf.Clamp01(timer / time); // t is used for normalizaton of the value.
            spotLight.color = Color.Lerp(startColor, targetColor, t);
        }
        
    }

    void SmoothLightAngle()
    {
        if (spotLight != null)
        {
            spotLight.spotAngle = Mathf.Lerp(spotLight.spotAngle, targetSpotAngle, Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(lastSeenPlayerPos, 0.2f);
    }
}
using UnityEngine;
using System.Collections;
using Scripts.Weapon;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAI : Enemy
{
    public GameObject BulletImpactPrefab;
    public ImpactAudioData ImpactAudioData;
    public AudioClip ShootingAudioClip;

    private int OriginHp;
    private bool isGetDamage;

    // 巡逻数据
    private List<Transform> Map;
    private int index;               //计数
    public Transform PathTransform;//所有路径点的父物体
    private Transform[] childrens;  //所有路径点的集合

    private GlobleVar.EnemyState CurrentState;
    private float Timer;
    private float ChaseTime;

    private float AttackDistance;
    private FindPath AFindPath;

    private bool isShoot;

    // Use this for initialization
    void Start()
    {
        isDied = false;
        playerTransform = GameObject.FindWithTag("Player").transform;
        //agent = GetComponent<NavMeshAgent>();
        shootAudioSource = GetComponent<AudioSource>();
        EnemyAnimator = GetComponent<Animator>();
        hpManager = gameObject.transform.GetChild(0).GetComponent<HPManager>();
        LookAt_fix = new Vector3(0, 1.6f, 0);
        isDamage = true;
        SightRange = 15.0f;
        SightAngle = 90.0f;
        SoundRange = 15.0f;
        OriginHp = hpManager.HP;
        isGetDamage = false;
        CurrentState = GlobleVar.EnemyState.Idle;
        ChaseTime = 8.0f;
        AttackDistance = 10.0f;
        AFindPath = GetComponent<FindPath>();
        isShoot = false;

        // 巡逻数据初始化
        Map = new List<Transform>();
        index = 0;
        //PathTransform = GameObject.Find("PatrolPath").transform;
        childrens = PathTransform.GetComponentsInChildren<Transform>();
        foreach (var children in childrens)// 遍历
        {
            if (children != PathTransform)// 不要把父物体算进路径点中
            {
                Map.Add(children);// 添加到list中
            }
        }
        AFindPath.destPos = Map[index];
    }

    // Update is called once per frame
    void Update()
    {
        // 暂停
        if (GlobleVar.isPause || isDied)
        {
            return;
        }
        Distance = Vector3.Distance(transform.position, playerTransform.position);
        FSMController();
        //EnemyAnimator.Play("Shoot");

        //******Debug*****//
        /*if(Input.GetKeyDown(KeyCode.P))
            Debug.Log(CurrentState);
        if (Input.GetKeyDown(KeyCode.O))
            Debug.Log(isGetDamage);*/
        //****************//
    }
    // 敌人状态机
    protected override void FSMController()
    {
        if (AmmoInMag <= 0 || isShoot)
        {
            return;
        }
        if (hpManager.HP <= 0)
        {
            CurrentState = GlobleVar.EnemyState.Died;
        }
        switch (CurrentState)
        {
            case GlobleVar.EnemyState.Idle:
                Timer = ChaseTime;
                CurrentState = GlobleVar.EnemyState.Patrol;
                break;

            case GlobleVar.EnemyState.Patrol:
                Patrol();
                Timer = ChaseTime;
                if (isPlayerbeSeen() || isEnemybeHit())
                {
                    if (Distance > AttackDistance)
                    {
                        CurrentState = GlobleVar.EnemyState.Chase;
                    }
                    else
                    {
                        CurrentState = GlobleVar.EnemyState.Attack;
                    }
                }
                else if (isPlayerbeHeard())
                {
                    CurrentState = GlobleVar.EnemyState.Chase;
                }
                break;

            case GlobleVar.EnemyState.Chase:
                Chase();
                if (Distance <= AttackDistance && isPlayerbeSeen())
                {
                    CurrentState = GlobleVar.EnemyState.Attack;
                    break;
                }
                if (!isPlayerbeSeen() && !isPlayerbeHeard())
                {
                    if (Timer <= 0)
                    {
                        Timer = ChaseTime;
                        CurrentState = GlobleVar.EnemyState.Patrol;
                        isGetDamage = false;
                    }
                    else
                    {
                        Timer -= Time.deltaTime;
                    }
                    
                }
                break;

            case GlobleVar.EnemyState.Attack:
                Attack();
                Timer = ChaseTime;
                if (Distance > AttackDistance)
                {
                    CurrentState = GlobleVar.EnemyState.Chase;
                }
                break;

            case GlobleVar.EnemyState.Died:
                Timer = ChaseTime;
                //agent.speed = 0;
                Died();
                return;

            default:
                break;
        }
    }

    //*******************  敌人状态 *****************************//
    protected override void Died()
    {
        isDied = true;
        //agent.isStopped = true;
        EnemyAnimator.Play("FallingBack");
        gameLevelController.EnemyNum--;
        Destroy(gameObject, 3);
    }
    protected override void Attack()
    {
        
        // 转身
        transform.LookAt(playerTransform.position - LookAt_fix);

        //agent.speed = 0;
        if (!isAllowShooting() || AmmoInMag <= 0)
        {
            return;
        }

        // 动画
        EnemyAnimator.Play("Shoot");
        isShoot = true;

        // 子弹
        CreateBullet();
        lastFireTime = Time.time;
        
        StartCoroutine(CheckShootAmmoAnimationEnd());
        //MuzzleParticle.Play();

        // 声音
        shootAudioSource.clip = ShootingAudioClip;
        shootAudioSource.Play();

        // 子弹数量
        AmmoInMag--;
        if (AmmoInMag <= 0)
        {
            Invoke("Reload", 0.3f);
            //Reload();
        }
    }
    protected override void Chase()
    {
        AFindPath.destPos = playerTransform;
        EnemyAnimator.Play("sprint");
        /*agent.speed = RunSpeed;
        agent.SetDestination(playerTransform.position);*/
        if (AFindPath.Path.Count > 0)
        {
            SetDestinaion(AFindPath.Path[0].pos, RunSpeed);
        }
    }
    protected override void Patrol()
    {
        AFindPath.destPos = Map[index];
        EnemyAnimator.Play("walk");
        //transform.LookAt(Map[index]);//看向下个路径点
        //transform.Translate(Time.deltaTime * WalkSpeed * Vector3.forward);//移动
        if (AFindPath.Path.Count > 0)
        {
            SetDestinaion(AFindPath.Path[0].pos, WalkSpeed);
        }
        //agent.SetDestination(Map[index].position);
        if (Vector3.Distance(transform.position, Map[index].position) < 1.0f)//判断是否到达
        {
            index++;
            index %= Map.Count;//取余
        }
    }
    // **********************************************************//


    //*************************** 敌人感知 **********************//
    // 敌人视觉
    protected override bool isPlayerbeSeen()
    {

        // TODO: 玩家是否在视野范围内
        // 玩家在可视距离
        if (Distance <= SightRange)
        {
            // 判断是否在可视角度
            Vector3 direction = playerTransform.position - EnemyHead.transform.position;
            // 计算角度
            float degree = Vector3.Angle(direction, transform.forward);

            /*Debug.DrawLine(EnemyHead.transform.position, playerTransform.position, Color.yellow);
            Debug.DrawRay(EnemyHead.transform.position, transform.forward, Color.red);
            Debug.Log(degree);*/

            // 不再视野范围
            if (!(degree <= SightAngle / 2))
            {
                return false;
            }

            // 判断是否有遮挡物
            Ray ray = new Ray
            {
                origin = EnemyHead.transform.position,
                direction = direction
            };
            if (Physics.Raycast(ray, out RaycastHit hitInfo, SightRange))
            {
                if (hitInfo.transform == playerTransform)
                {
                    //Debug.Log("Player was Seen");
                    return true;
                }
            }
        }

        return false;
    }

    // 敌人听觉
    protected override bool isPlayerbeHeard()
    {
        // 在听觉范围内
        if (Distance <= SoundRange)
        {
            // 玩家发出脚步声
            if (Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.D))
            {
                //Debug.Log("Player was heard");
                return true;
            }

            // 玩家枪声
            if (Input.GetMouseButton(0))
            {
                //Debug.Log("Player was heard");
                return true;
            }
        }
        return false;
    }

    // 受伤感知
    protected override bool isEnemybeHit()
    {
        if (OriginHp > hpManager.HP)
        {
            isGetDamage = true;
            OriginHp = hpManager.HP;
            return true;
        }
        return false;
    }
    //***********************************************************//

    // 生成子弹
    protected override GameObject CreateBullet()
    {
        //GameObject tmp_Bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);
        GameObject tmp_Bullet = ObjectPool.GetInstance().GetObj(BulletPrefab.name);
        tmp_Bullet.transform.position = MuzzlePoint.position;
        tmp_Bullet.transform.rotation = MuzzlePoint.rotation;

        tmp_Bullet.transform.eulerAngles += CalculateSpreadOffset();

        var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
        tmp_BulletScript.BulletImpactPrefab = BulletImpactPrefab;
        tmp_BulletScript.BulletSpeed = BulletSpeed;
        tmp_BulletScript.BulletPrefab = tmp_Bullet;
        tmp_BulletScript.ImpactAudioData = ImpactAudioData;

        return tmp_Bullet;
    }

    protected override void Reload()
    {
        isDamage = false;
        EnemyAnimator.Play("Reload");
        //agent.isStopped = true;
        StartCoroutine(CheckReloadAmmoAnimationEnd());
    }
    // 判断换弹动画结束
    private IEnumerator CheckReloadAmmoAnimationEnd()
    {
        while (true)
        {
            yield return null;
            var StateInfo = EnemyAnimator.GetCurrentAnimatorStateInfo(0);

            // 获取换弹动画状态
            if (StateInfo.IsTag("Reload"))
            {
                if (StateInfo.normalizedTime >= 0.9f)
                {
                    AmmoInMag = 8;
                    //agent.isStopped = false;
                    isDamage = true;
                    yield break;
                }
            }
        }
    }

    // 判断射击动画结束
    private IEnumerator CheckShootAmmoAnimationEnd()
    {
        while (true)
        {
            yield return null;
            var StateInfo = EnemyAnimator.GetCurrentAnimatorStateInfo(0);
            // 获取换弹动画状态
            if (StateInfo.IsName("Shoot"))
            {
                if (StateInfo.normalizedTime >= 0.9f)
                {
                    isShoot = false;
                    yield break;
                }
            }
            else
            {
                isShoot = false;
                yield break;
            }
        }
    }

    public override void Damage()
    {
        if (!isDamage)
        {
            return;
        }
        EnemyAnimator.Play("Damage");
    }

    public bool isAllowShooting()
    {
        // 限制射击频率
        if ((Time.time - lastFireTime) > (1 / FireRate))
        {
            return true;
        }
        return false;
    }

    // 子弹散射
    protected override Vector3 CalculateSpreadOffset()
    {
        float tmp_SpreadPercent = SpreadAngle / EyeCamera.fieldOfView;

        return tmp_SpreadPercent * Random.insideUnitCircle;
    }

    // 设置敌人移动
    private void SetDestinaion(Vector3 destPos, float speed)
    {
        transform.LookAt(destPos - new Vector3(0, 0.1f, 0));//看向下个路径点
        transform.Translate(Time.deltaTime * speed * Vector3.forward);//移动
    }

}

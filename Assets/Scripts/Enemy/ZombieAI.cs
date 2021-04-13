using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 状态机枚举类型
public enum ZombieState
{ 
    idle,
    run,
    walk,
    attack,
    fallingBack
}

public class ZombieAI : MonoBehaviour
{
    public ZombieState CurrentState = ZombieState.idle;

    private Animator ZombieAnimator;
    private Transform playerTransform;
    private NavMeshAgent agent;
    private HPManager hpManager;

    // Start is called before the first frame update
    void Start()
    {
        ZombieAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        hpManager = GetComponentInChildren<HPManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // 暂停
        if (GlobleVar.isPause)
        {
            return;
        }

        if (hpManager.HP <= 0)
        {
            CurrentState = ZombieState.fallingBack;
        }
        // 状态转移
        switch (CurrentState)
        {
            case ZombieState.idle:
                idleHandler();
                break;

            case ZombieState.walk:
                //walkHandler();
                break;

            case ZombieState.run:
                runHandler();
                break;

            case ZombieState.attack:
                //attackHandler();
                break;

            case ZombieState.fallingBack:
                fallingBackHandler();
                break;

            default:
                break;
        }
    }

    private void fallingBackHandler()
    {
        ZombieAnimator.speed = 0;
        ZombieAnimator.SetInteger("State", 2);
        agent.isStopped = true;
        ZombieAnimator.speed = 1;
        Destroy(this.gameObject, 5);
    }

    private void runHandler()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (distance >= 3)
        {
            CurrentState = ZombieState.idle;
        }
        ZombieAnimator.SetInteger("State", 1);
        agent.isStopped = false;
        agent.SetDestination(playerTransform.position);
    }

    private void idleHandler()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if(distance > 1 && distance <= 3)
        {
            CurrentState = ZombieState.run;
        }
        ZombieAnimator.SetInteger("State", 0);
        agent.isStopped = true;
    }

}

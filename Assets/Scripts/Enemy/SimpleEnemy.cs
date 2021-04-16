using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : Enemy
{
    
    private void Start()
    {
        isDied = false;
        EnemyAnimator = GetComponent<Animator>();
        hpManager = GetComponentInChildren<HPManager>();
    }

    private void Update()
    {
        if (GlobleVar.isPause || isDied)
        {
            return;
        }

        if (hpManager.HP <= 0)
        {
            Died();
        }
    }

    public override void Damage()
    {
        if (!isDied)
        {
            EnemyAnimator.Play("Damage");
        }
    }
    protected override void Died()
    {
        isDied = true;
        EnemyAnimator.SetBool("isDied", true);
        gameLevelController.EnemyNum--;
        Destroy(gameObject, 3);
    }

    protected override void FSMController()
    {
        throw new System.NotImplementedException();
    }

    /*protected override void Reload()
    {
        throw new System.NotImplementedException();
    }*/

    protected override void Attack()
    {
        throw new System.NotImplementedException();
    }

    protected override void Chase()
    {
        throw new System.NotImplementedException();
    }

    protected override void Patrol()
    {
        throw new System.NotImplementedException();
    }

    protected override bool isPlayerbeSeen()
    {
        throw new System.NotImplementedException();
    }

    protected override bool isPlayerbeHeard()
    {
        throw new System.NotImplementedException();
    }

    protected override bool isEnemybeHit()
    {
        throw new System.NotImplementedException();
    }

    protected override GameObject CreateBullet()
    {
        throw new System.NotImplementedException();
    }

    protected override Vector3 CalculateSpreadOffset()
    {
        throw new System.NotImplementedException();
    }
}

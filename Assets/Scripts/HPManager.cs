using UnityEngine;
using System.Collections;

public class HPManager : MonoBehaviour
{
    public int HP;

    public void HPDown(int damage)
    {
        HP -= damage;
    }

    private void Update()
    {
        /*if (HP <= 0)
        {
            Destroy(gameObject);
        }*/
    }
}

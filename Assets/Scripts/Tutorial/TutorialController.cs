using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public Transform MoveTransform;
    public Transform RunTransform;
    public Transform ShootTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRunTutorial()
    {
        Debug.Log("Run");
        MoveTransform.gameObject.SetActive(false);
        RunTransform.gameObject.SetActive(true);
    }

    public void StartShootTutorial()
    {
        Debug.Log("Shoot");
        RunTransform.gameObject.SetActive(false);
        ShootTransform.gameObject.SetActive(true);
    }
}

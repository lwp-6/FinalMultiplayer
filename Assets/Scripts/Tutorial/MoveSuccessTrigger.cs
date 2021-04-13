using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSuccessTrigger : MonoBehaviour
{
    public TutorialController tutorialController;
    private bool isCollider;
    // Start is called before the first frame update
    void Start()
    {
        isCollider = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isCollider == false)
        {
            tutorialController.StartRunTutorial();
            isCollider = true;
        }

    }
}

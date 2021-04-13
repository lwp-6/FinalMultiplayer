using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public TextMesh textMesh;
    public bool isClosed = true;
    public void OpenDoor()
    {
        transform.Rotate(Vector3.up, -90);
        isClosed = false;
    }

    public void CloseDoor()
    {
        transform.Rotate(Vector3.up, 90);
        isClosed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Next");
            OpenDoor();
            textMesh.gameObject.SetActive(false);
        }
       
    }
}
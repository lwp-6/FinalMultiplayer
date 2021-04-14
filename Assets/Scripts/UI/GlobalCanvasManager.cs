using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalCanvasManager : MonoBehaviour
{
    public Transform LifeNum;
    public Text CountDownText;

    private Image[] LifeTransforms;
    private BattleRoomManager battleRoomManager;

    private void Awake()
    {
        LifeTransforms = LifeNum.GetComponentsInChildren<Image>();
        battleRoomManager = FindObjectOfType<BattleRoomManager>();
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        CountDownText.text = "3";
        if (battleRoomManager.life >= 1)
        {
            LifeTransforms[battleRoomManager.life - 1].color = new Color(255, 255, 255, 0);
        }
        Debug.Log(battleRoomManager.life);
    }
}

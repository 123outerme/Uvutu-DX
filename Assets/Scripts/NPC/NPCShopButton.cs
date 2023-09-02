using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCShopButton : MonoBehaviour
{
    public GameObject player;

    public NPCDialogue dialogue = null;
    public NPCShop shop = null;
    
    private PlayerController pController;

    // Start is called before the first frame update
    void Start()
    {
        pController = player.GetComponent<PlayerController>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenShopFromButton()
    {
        shop.ShowShop(dialogue);
        pController.SetMovementLock(true);
    }
}

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
        UpdateScripts();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateScripts()
    {
        if (pController == null)
            pController = player.GetComponent<PlayerController>();  
    }

    public void OpenShopFromButton()
    {
        UpdateScripts();
        dialogue.showingShop = true;
        shop.ShowShop(dialogue);
        pController.SetMovementLock(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCState
{
    public Vector3 position;
    public bool enableMovement;
    public bool startMovement;
    public int step;
    public int frame;
    public string name;
    public bool prevEnableMove;
    public List<string> curDialogueList;
    public int dialogueItem;
    public bool showingTurnInButton;
    public bool showingTurnIn;
    public bool showingShopButton;
    public bool showingShop;
    public InventorySlot[] shop;

    public NPCState(Vector3 pos,
        bool enableMove,
        bool startMove,
        int curStep,
        int curFrame,
        string npcName,
        bool prevEnableMoveSetting,
        List<string> curDialogue,
        int dialogueIndex,
        bool showTurnInBtn,
        bool showTurnIn,
        bool showShopBtn,
        bool showShop,
        InventorySlot[] stock
    )
    {
        position = pos;
        enableMovement = enableMove;
        startMovement = startMove;
        step = curStep;
        frame = curFrame;
        name = npcName;
        prevEnableMove = prevEnableMoveSetting;
        curDialogueList = curDialogue;
        dialogueItem = dialogueIndex;
        showingTurnInButton = showTurnInBtn;
        showingTurnIn = showTurnIn;
        showingShopButton = showShopBtn;
        showingShop = showShop;
        shop = stock;
    }
}

public class NPCStats : MonoBehaviour
{
    public bool saveStatus = true;
    public NPCState state = new NPCState(new Vector3(0,0,0), false, false, 0, 0, "", false, new List<string>(), 0, false, false, false, false, new InventorySlot[] {});

    private NPCMovement movement = null;
    private NPCDialogue dialogue = null;
    private NPCShop shop = null;

    private SaveHandler saver;

    public void Start()
    {
        GetNPCScripts();   
        saver = GameObject.Find("SaveHandler").GetComponent<SaveHandler>();
        //SaveNPCState();
    }

    private void GetNPCScripts()
    {
        if (movement == null)
            movement = gameObject.GetComponent<NPCMovement>();

        if (dialogue == null)
            dialogue = gameObject.GetComponent<NPCDialogue>();

        if (shop == null)
            shop = gameObject.GetComponent<NPCShop>();
    }

    public void SaveNPCState()
    {
        GetNPCScripts();

        List<string> dialogueList = new List<string>();
        if (dialogue.inDialogue)
            dialogueList = dialogue.GetCurDialogue();

        state = new NPCState(
            transform.position,
            movement.enableMovement,
            movement.startMovement,
            movement.step,
            movement.frame,
            gameObject.name,
            dialogue.GetPrevEnableMove(),
            dialogueList,
            dialogue.dialogueItem,
            dialogue.showingTurnInButton,
            dialogue.showingTurnIn,
            dialogue.showingShopButton,
            dialogue.showingShop,
            shop.GetItems()
        );
    }

    public void LoadNPCState(NPCState s)
    {
        if (s != null)
        {
            GetNPCScripts();
            
            state = s;
            transform.position = state.position;
            movement.enableMovement = state.enableMovement;
            movement.step = state.step;
            movement.frame = state.frame;

            movement.lastMinusNext = movement.posSteps[movement.step] - state.position;
            movement.startMovement = state.startMovement;

            dialogue.GetQuestsToTurnInHere();
            dialogue.SetCurDialogue(state.curDialogueList);
            dialogue.SetPrevEnableMove(state.prevEnableMove);
            dialogue.inDialogue = (state.curDialogueList.Count > 0);
            if (dialogue.inDialogue)
                dialogue.SetPlayerLock(true);
            
            dialogue.dialogueItem = state.dialogueItem;
            dialogue.showingTurnInButton = state.showingTurnInButton;
            dialogue.showingTurnIn = state.showingTurnIn;
            dialogue.showingShopButton = state.showingShopButton;
            dialogue.showingShop = state.showingShop;
            dialogue.UpdateDialogueText();

            if (dialogue.showingShopButton)
                dialogue.ShowShopButton();

            if (dialogue.showingShop)
            {
                dialogue.hasShop = true;
                NPCShopButton shopButton = GameObject.Find("WorldCanvas/ButtonPanel").transform.Find("ShopButton").GetComponent<NPCShopButton>();
                shopButton.dialogue = dialogue;
                shopButton.shop = shop;
                shopButton.OpenShopFromButton();
            }

            if (dialogue.showingTurnInButton)
                dialogue.ShowTurnInButton();

            if (dialogue.showingTurnIn)
            {
                TurnInButton turnInButton = GameObject.Find("WorldCanvas/ButtonPanel").transform.Find("TurnInButton").GetComponent<TurnInButton>();
                turnInButton.dialogue = dialogue;
                turnInButton.turnInName = dialogue.gameObject.name;
                turnInButton.OpenTurnInQuests();
            }

            shop.items = new List<InventorySlot>(state.shop);
            shop.LoadAllInventorySlots();
            shop.loaded = true;
            //Debug.Log("given state is not null.");
        }
        else
        {
            Debug.Log("given state is null!!");
        }
    }
}

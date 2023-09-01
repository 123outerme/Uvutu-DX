using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestStepPanel : MonoBehaviour
{
    public QuestDetailsPanel parentPanel = null;
    public QuestStep step = null;
    public string shortStepDetail = null;

    private TMP_Text stepNameText = null;
    private TMP_Text stepProgressText = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetComponentReferences()
    {   
        if (stepNameText == null)
            stepNameText = transform.Find("StepNameText").GetComponent<TMP_Text>();
        
        if (stepProgressText == null)
            stepProgressText = transform.Find("StepProgressText").GetComponent<TMP_Text>();
    }

    public void UpdateFromQuestStep()
    {
        GetComponentReferences();

        if (step == null)
            return;
        
        stepNameText.text = step.name;
        stepProgressText.text = shortStepDetail;
    }

    public void ViewStepDetails()
    {
        parentPanel.ViewQuestStep(step);
    }
}

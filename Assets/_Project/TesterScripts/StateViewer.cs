using System.Collections;
using System.Collections.Generic;
using Monster.AI.Blackboard;
using UnityEngine;

public class StateViewer : MonoBehaviour
{
    [SerializeField] private Blackboard blackboard;
    [SerializeField] private TMPro.TMP_Text text;
    
    private void Update()
    {
        if (text is null || blackboard is null) return;

        text.text = blackboard.State.ToString();
    }
}

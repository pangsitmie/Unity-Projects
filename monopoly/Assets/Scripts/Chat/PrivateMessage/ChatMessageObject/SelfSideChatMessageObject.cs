using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelfSideChatMessageObject : TextTypeChatMessage
{
    public override void UpdateDisplay()
    {
        Debug.Log("SelfSideChat: " + chatData.message);
        base.UpdateDisplay();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OtherSideChatMessageObject : TextTypeChatMessage
{
    public override void UpdateDisplay()
    {
        Debug.Log("OtherSideChat: " + chatData.message);
        base.UpdateDisplay();
    }
}

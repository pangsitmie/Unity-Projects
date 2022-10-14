using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextTypeChatMessage : BaseChatMessage
{
    public Text message_text;
    public override void UpdateDisplay()
    {
        message_text.text = chatData.message;
    }
}

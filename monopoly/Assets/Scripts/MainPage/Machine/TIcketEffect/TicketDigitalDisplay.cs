using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TicketDigitalDisplay : MonoBehaviour
{
    public Text showTicketAmount_text;
    public void ChangeTicketAmount(int _ticketAmount_int)
    {
        showTicketAmount_text.text = _ticketAmount_int.ToString();
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

    }
}

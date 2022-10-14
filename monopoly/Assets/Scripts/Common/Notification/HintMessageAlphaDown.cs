using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintMessageAlphaDown : MonoBehaviour
{
    private float time_f = 0;
    [SerializeField] MoveOut this_moveOut;
    [SerializeField] CanvasGroup this_canvasGroup;

    //====================================================================
    public void Init()
    {
        time_f = 0;
        this_moveOut.Move(true);
        this_moveOut.Move(Vector2.zero);
        this_canvasGroup.alpha = 1;
    }

    //====================================================================
    private void Update()
    {
        if (time_f >= 1)
        {
            if (this_canvasGroup.alpha > 0)
            {
                this_canvasGroup.alpha -= Time.deltaTime;
            }
            else
            {
                this.enabled = false;
            }
        }
        else
        {
            time_f += Time.deltaTime;
        }
    }

    //====================================================================
    private void OnDisable()
    {
        this_moveOut.Move(false);
    }

    //====================================================================
}

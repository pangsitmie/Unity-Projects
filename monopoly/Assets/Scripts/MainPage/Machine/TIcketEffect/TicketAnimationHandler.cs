using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class TicketChangeEvent : UnityEvent<int> { }
public class TicketAnimationHandler : MonoBehaviour
{

    [SerializeField] Transform ticketContent_transform;
    [SerializeField] TicketPool ticketPool;
    [SerializeField] float ticketBaseSpeed_f = 2.0f;
    [SerializeField] float ticketSpeedLimit_f = 10f;
    [SerializeField] float delayTime_f = 0.1f;
    [SerializeField] int unpushTicketAmount_int = 0;
    Queue<GameObject> availableTicket_queue = new Queue<GameObject>();
    RectTransform ticketContent_RectTransform;
    [SerializeField] Vector3 ticketStart_position, ticketTarget_position;
    [SerializeField] float currentTicketSpeed_f = 0;
    [SerializeField] float boost_f = 0;
    [SerializeField] int currentTicketCount_int = 0;
    float ticketProgress_f = 0;
    float currentDelayTime_f = 0;
    [SerializeField] float beginBoost_f = 0;
    [SerializeField] float closeDelay_f = 1f;
    float currentCloseDelayTime_f = 0;
    [SerializeField] float downRate_f = 0;
    GameObject pushingTicket_gObj = null, lastTicket_gObj = null;
    public TicketChangeEvent OnTicketChangeEvent;
    bool startPush_bl = false;
    // Start is called before the first frame update
    void Start()
    {
        ticketContent_RectTransform = ticketContent_transform.GetComponent<RectTransform>();
        availableTicket_queue.Clear();
        float _ticketPrefabHeight_f = ticketPool.m_prefab.GetComponent<RectTransform>().rect.height;
        //ticketStart_position = new Vector3(ticketContent_transform.position.x, (ticketContent_transform.position.y + ticketContent_RectTransform.rect.height * 0.5f + _ticketPrefabHeight_f * 0.5f), 0);
        //ticketTarget_position = new Vector3(ticketContent_transform.position.x, (ticketContent_transform.position.y + ticketContent_RectTransform.rect.height * 0.5f - _ticketPrefabHeight_f * 0.5f), 0);
        ticketStart_position = new Vector3(ticketContent_transform.position.x, (ticketContent_transform.position.y + _ticketPrefabHeight_f * 0.5f), 0);
        ticketTarget_position = new Vector3(ticketContent_transform.position.x, (ticketContent_transform.position.y - _ticketPrefabHeight_f * 0.5f), 0);
        UnityEvent _returnEvent = new UnityEvent();
        _returnEvent.AddListener(Close);
        ReturnManager.Instance.PushReturnEvent(_returnEvent);
    }
    public void Set(int _ticketAmount_int)
    {
        QualitySettings.vSyncCount = 1;   // 把垂直同步打開
        Application.targetFrameRate = 60;
        Debug.Log(" _ticketAmount_int: " + _ticketAmount_int);
        unpushTicketAmount_int = _ticketAmount_int;
        startPush_bl = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (startPush_bl)
        {
            if (unpushTicketAmount_int > 0)
            {
                if (pushingTicket_gObj == null)
                {
                    if (lastTicket_gObj == null)
                    {
                        pushingTicket_gObj = ticketPool.GetPooledInstance(ticketStart_position, ticketContent_transform.rotation, ticketContent_transform);
                        var _pushingTicket_rectTransform = pushingTicket_gObj.GetComponent<RectTransform>();
                        availableTicket_queue.Enqueue(pushingTicket_gObj);
                        boost_f = (ticketSpeedLimit_f - ticketBaseSpeed_f) * (1.0f - 1.0f / unpushTicketAmount_int) * (1.0f - 1.0f / unpushTicketAmount_int);
                        Debug.Log(boost_f);
                        beginBoost_f = boost_f;
                        downRate_f = beginBoost_f / (unpushTicketAmount_int * unpushTicketAmount_int - 2 * unpushTicketAmount_int + 1);
                        //downRate_f = boost_f / unpushTicketAmount_int;
                        OnTicketChangeEvent.Invoke(currentTicketCount_int);
                    }
                    else
                    {
                        // System.Text.StringBuilder _string_strBd = new System.Text.StringBuilder();
                        var _childTicket_gObj = lastTicket_gObj;
                        var _childticket_position = _childTicket_gObj.transform.position;
                        pushingTicket_gObj = ticketPool.GetPooledInstance(ticketStart_position, ticketContent_transform.rotation, ticketContent_transform);
                        //_string_strBd.Clear().Append("childticket_position y: ").Append(_childTicket_gObj.transform.position.y).Append("\n");
                        _childTicket_gObj.transform.SetParent(pushingTicket_gObj.transform, true);
                        //_string_strBd.Append("childticket_position y: ").Append(_childTicket_gObj.transform.position.y).Append("\n");
                        var _pushingTicket_rectTransform = pushingTicket_gObj.GetComponent<RectTransform>();

                        _childTicket_gObj.transform.position = new Vector3(_pushingTicket_rectTransform.transform.position.x, _pushingTicket_rectTransform.transform.position.y - _pushingTicket_rectTransform.rect.height - 3, 0);
                        //_string_strBd.Append("childticket_position y: ").Append(_childTicket_gObj.transform.position.y).Append("\n");
                        //Debug.Log(_string_strBd.ToString());

                        availableTicket_queue.Enqueue(pushingTicket_gObj);
                    }

                }
                else
                {
                    if (ticketProgress_f < 1.0f)
                    {

                        ticketProgress_f += (ticketBaseSpeed_f + boost_f) * Time.deltaTime;
                        pushingTicket_gObj.transform.position = Vector3.Lerp(ticketStart_position, ticketTarget_position, ticketProgress_f);
                        if (currentTicketCount_int != 0)
                        {
                            boost_f = downRate_f * (-currentTicketCount_int * currentTicketCount_int + 2 * currentTicketCount_int - 1) + beginBoost_f;
                            //boost_f = -downRate_f * currentTicketCount_int + beginBoost_f;
                            currentTicketSpeed_f = ticketBaseSpeed_f + boost_f;
                        }
                    }
                    else
                    {
                        pushingTicket_gObj.transform.position = Vector3.Lerp(ticketStart_position, ticketTarget_position, 1.0f);
                        if (currentDelayTime_f == 0)
                        {
                            unpushTicketAmount_int--;
                            currentTicketCount_int++;
                            OnTicketChangeEvent.Invoke(currentTicketCount_int);
                        }
                        if (currentDelayTime_f < delayTime_f)
                        {
                            currentDelayTime_f += 1.0f * Time.deltaTime;
                        }
                        else
                        {
                            currentDelayTime_f = 0;
                            ticketProgress_f = 0;


                            lastTicket_gObj = pushingTicket_gObj;

                            pushingTicket_gObj = null;
                        }

                    }
                }

                if (availableTicket_queue.Count > 4)
                {
                    var _toBeRecycledTicket = availableTicket_queue.Dequeue();
                    ticketPool.Recycle(_toBeRecycledTicket);
                }


            }
            else
            {
                if (currentCloseDelayTime_f < closeDelay_f)
                {
                    currentCloseDelayTime_f += 1.0f * Time.deltaTime;
                }
                else
                {
                    currentCloseDelayTime_f = 0;
                    Close();
                }
            }
        }
    }
    public void Close()
    {
        QualitySettings.vSyncCount = 0;   // 把垂直同步關掉
        Application.targetFrameRate = 30;
        Destroy(this.gameObject);
        ReturnManager.Instance.PopReturnEvent();
    }
}

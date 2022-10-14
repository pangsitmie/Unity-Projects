using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketPool : MonoBehaviour
{
    public Transform pool_transform;
    public GameObject m_prefab;
    [SerializeField]
    private int m_initailSize = 5;

    private Queue<GameObject> m_availableObjects = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < m_initailSize; i++)
        {
            GameObject go = Instantiate<GameObject>(m_prefab, this.transform);
            Recycle(go);
        }
    }

    public GameObject GetPooledInstance(Transform parent)
    {
        lock (m_availableObjects)
        {
            if (m_availableObjects.Count > 0)
            {
                GameObject go = m_availableObjects.Dequeue();
                if (go.transform.parent != parent)
                {
                    go.transform.SetParent(parent);
                    go.transform.position = parent.position;
                    go.transform.rotation = parent.rotation;
                }
                return go;
            }
            else
            {
                GameObject go = Instantiate<GameObject>(m_prefab, parent);
                return go;
            }
        }
    }
    public GameObject GetPooledInstance(Vector3 position, Quaternion rotation, Transform parent)
    {
        lock (m_availableObjects)
        {
            if (m_availableObjects.Count > 0)
            {
                GameObject go = m_availableObjects.Dequeue();
                if (go.transform.parent != parent)
                {
                    go.transform.SetParent(parent);
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                }
                return go;
            }
            else
            {
                GameObject go = Instantiate<GameObject>(m_prefab, position, rotation, parent);
                return go;
            }
        }
    }

    public void Recycle(GameObject _recycledObject)
    {
        lock (m_availableObjects)
        {
            m_availableObjects.Enqueue(_recycledObject);
            _recycledObject.transform.SetParent(pool_transform);
            _recycledObject.transform.position = new Vector3(-10000, 10000, 0);
        }
    }
}

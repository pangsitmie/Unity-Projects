using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityObjectPool<TUnityObject> where TUnityObject : MonoBehaviour
{
    //=================================================================
    // 暫存未使用物件的Position
    Vector3 poolPosition = new Vector3(-10000, 10000, 0);

    //=================================================================
    [SerializeField] Transform poolContent_transform;
    [SerializeField] private int initailAmount_int = 5;

    //=================================================================
    Func<Transform, TUnityObject> OnNeedInstantiateObject;

    //=================================================================
    private Queue<TUnityObject> availableObjects_queue = new Queue<TUnityObject>();

    //=================================================================
    /// <summary>
    /// Unity物件池的建構式
    /// </summary>
    /// <param name="_poolContent_transform">暫存未使用物件的父物件</param>
    /// <param name="_OnNeedInstantiateObject">物件的生成Function</param>
    /// <param name="_initailAmount_int">物件的初始生成數量</param>
    public UnityObjectPool(Transform _poolContent_transform, Func<Transform, TUnityObject> _OnNeedInstantiateObject, int _initailAmount_int = 5)
    {
        //-------------------------------------------------------------
        //初始化參數
        this.poolContent_transform = _poolContent_transform;
        initailAmount_int = _initailAmount_int;
        OnNeedInstantiateObject = _OnNeedInstantiateObject;

        //-------------------------------------------------------------
        // 初始化生成預設數量的物件
        lock (availableObjects_queue)
        {
            for (int i = 0; i < initailAmount_int; i++)
            {
                var _target = OnNeedInstantiateObject(_poolContent_transform);
                _target.transform.localPosition = poolPosition;
                availableObjects_queue.Enqueue(_target);
            }
        }
    }

    //=================================================================
    /// <summary>
    /// 取得物件的Function
    /// </summary>
    /// <param name="_parent_transform">設定欲取得物件的父物件</param>
    /// <returns>回傳取得的該物件</returns>
    public TUnityObject GetPooledInstance(Transform _parent_transform)
    {
        lock (availableObjects_queue)
        {
            TUnityObject _target;
            if (availableObjects_queue.TryDequeue(out _target))
            {
                if (_target.transform.parent != _parent_transform)
                {
                    _target.transform.SetParent(_parent_transform);
                }
                return _target;
            }
            else
            {
                _target = OnNeedInstantiateObject(_parent_transform);
                return _target;
            }
        }
    }

    //=================================================================
    /// <summary>
    /// 回收物件進物件池
    /// </summary>
    /// <param name="_target">欲回收之物件</param>
    public void BackToPool(TUnityObject _target)
    {
        lock (availableObjects_queue)
        {
            availableObjects_queue.Enqueue(_target);
            _target.transform.localPosition = poolPosition;
            _target.transform.SetParent(poolContent_transform);
        }
    }

    //=================================================================
}
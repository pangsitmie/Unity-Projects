using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 顯示載入中的畫面動畫
/// <para>操作方式: 請配合LoadingObjectManager使用</para>
/// </summary>
public class LoadingObject : MonoBehaviour
{
    [SerializeField] Image background_Image; //變黑的背景畫面
    [SerializeField] Image loading_Image; //存放載入動畫的Image
    [SerializeField] Sprite[] loading_Sprite; //載入動畫的圖片
    public Color backgroundColorBeforeDelay = Color.clear; //背景畫面延遲前的色調
    public Color backgroundColorAfterDelay = new Color(0.12f, 0.13f, 0.17f, 0.85f); //背景畫面延遲後的色調
    public float delaySeconds = 0; //背景變色的延遲時間
    public float loadingCycleTime = 0.1f; //載入動畫切換圖片的間隔時間
    private Coroutine loadCoroutine; //載入動畫的協程

    //====================================================================
    /// <summary>
    /// 開始顯示載入動畫
    /// <para>編輯者: zxft</para>
    /// </summary>
    public void StartLoading()
    {
        if (loadCoroutine == null)
        {
            Debug.Log("StartLoading");
            background_Image.color = backgroundColorBeforeDelay;
            loadCoroutine = StartCoroutine(Loading_IEnumerator());
        }
    }

    //====================================================================
    /// <summary>
    /// 關閉載入動畫
    /// <para>編輯者: zxft</para>
    /// </summary>
    public void StopLoading()
    {
        if (loadCoroutine != null)
        {
            Debug.Log("StopLoading");
            StopCoroutine(loadCoroutine);
            loadCoroutine = null;
        }
    }

    //====================================================================
    /// <summary>
    /// 載入動畫處理
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 在開始撥放動畫前如果有設定延遲時間，雖然仍會將畫面的操作都擋下來，但先不會顯示載入動畫</para>
    /// </summary>
    IEnumerator Loading_IEnumerator()
    {
        if (delaySeconds > 0)
            yield return new WaitForSeconds(delaySeconds);

        background_Image.color = backgroundColorAfterDelay;

        int _spriteCount = 0;
        while (true)
        {
            loading_Image.sprite = loading_Sprite[_spriteCount];
            if (_spriteCount >= loading_Sprite.Length - 1)
            {
                _spriteCount = 0;
            }
            else
            {
                _spriteCount++;
            }

            yield return new WaitForSeconds(loadingCycleTime);
        }
    }

    //====================================================================
}

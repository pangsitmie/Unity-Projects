using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.Events;
[System.Serializable]
public class UnityStringEvent : UnityEvent<string> { }
public class ImageObject : MonoBehaviour
{
    //-------------------------------
    //控制是否能互動
    public bool interactable = true;
    //-------------------------------
    //顯示圖片用
    public RawImage this_rawImg;
    public GameObject nullBackground_gObj, whiteBackground_gObj;
    //-------------------------------
    //移除圖片的按鈕 當有圖片時才會顯示
    [SerializeField] Button setImage_btn, removeImage_btn;
    //-------------------------------
    //圖片上的提示字
    public Text imageHint_tx;
    //-------------------------------
    //圖片資料(用來確認寬高)
    public Texture2D mealImage_texture2D;
    //-------------------------------
    //圖片資料
    private byte[] currentImageByte;
    private byte[] newImageByte;
    public byte[] NewImageByte { get => newImageByte; }
    //-------------------------------
    //是否有新資料 || 是否遭移除圖片
    public bool isNew_bl, isRemove_bl;
    //-------------------------------
    private Coroutine setImageCoroutine;
    protected Vector2 m_rectSize = Vector2.one;

    //========================================================================
    public UnityStringEvent OnLoadImageWithPath;
    public UnityEvent OnRemoveImage;

    //========================================================================
    private void Awake()
    {
        GetRectSize();
        setImage_btn.enabled = interactable;
        mealImage_texture2D = new Texture2D((int)this_rawImg.rectTransform.rect.width, (int)this_rawImg.rectTransform.rect.height);
        ShowBackground(false);
    }

    //====================================================================
    /// <summary>
    /// 開啟相簿
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SelectImage()
    {
        if (interactable)
        {
            TakePhoto.PickImage(this);
        }
    }

    //====================================================================
    /// <summary>
    /// 移除圖片
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void RemoveImage()
    {
        if (interactable)
        {
            isNew_bl = false;
            if (currentImageByte != null)
                isRemove_bl = true;
            newImageByte = null;
            this_rawImg.texture = null;
            ShowBackground(false);
            removeImage_btn.gameObject.SetActive(false);
            imageHint_tx.text = "";
            OnRemoveImage?.Invoke();
        }
    }

    //====================================================================
    /// <summary>
    /// 依路徑開啟圖片
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void LoadImageWithPath(string _path)
    {
        if (!string.IsNullOrEmpty(_path))
        {
            OnLoadImageWithPath?.Invoke(_path);
            Debug.Log("ImageObject.SetImagePath(). Path = " + _path);

            if (setImageCoroutine != null)
                StopCoroutine(setImageCoroutine);

            FileStream fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);
            var _imageByte = new byte[fileStream.Length];
            fileStream.Read(_imageByte, 0, _imageByte.Length);
            fileStream.Close();
            SetImageWithByte(_imageByte);
        }
        else
        {
            isNew_bl = false;
            if (currentImageByte != null)
                isRemove_bl = true;
            newImageByte = null;
            this_rawImg.texture = null;
            ShowBackground(false);
            removeImage_btn.gameObject.SetActive(false);
            imageHint_tx.text = "";
            OnRemoveImage?.Invoke();
        }

    }

    //========================================================================
    public void SetImageWithByte(byte[] _imageByte)
    {
        Debug.Log("ImageObject.SetImageWithByte(). Byte length: " + _imageByte.Length);

        mealImage_texture2D.LoadImage(_imageByte);
        Debug.Log("ImageObject.SetImageWithByte(). Image width: " + mealImage_texture2D.width);
        Debug.Log("ImageObject.SetImageWithByte(). Image height: " + mealImage_texture2D.height);

        ShowBackground(true);
        float _scale_f = mealImage_texture2D.height / this_rawImg.rectTransform.rect.height;
        this_rawImg.texture = mealImage_texture2D;
        newImageByte = _imageByte;
        imageHint_tx.text = "";
        removeImage_btn.gameObject.SetActive(true);
        isNew_bl = true;
        isRemove_bl = false;
        DoAspectFit();
    }

    //====================================================================
    /// <summary>
    /// 以URL設定圖片
    /// <para>編輯者: Dimos</para>
    /// </summary>
    public void SetImageWithURL(string _mealImage_url)
    {
        Debug.Log("ImageObject.SetImageWithURL(). Url: " + _mealImage_url);

        if (setImageCoroutine != null)
            StopCoroutine(setImageCoroutine);
        setImageCoroutine = null;

        isNew_bl = false;
        isRemove_bl = false;
        this_rawImg.texture = null;
        ShowBackground(false);
        imageHint_tx.text = "圖片讀取中...請稍後";

        if (!string.IsNullOrEmpty(_mealImage_url))
        {
            if (interactable)
                removeImage_btn.gameObject.SetActive(true);

            setImageCoroutine = StartCoroutine(ImageSet(_mealImage_url));
        }
        else
        {
            removeImage_btn.gameObject.SetActive(false);
            imageHint_tx.text = "";
            currentImageByte = null;
        }
    }

    //====================================================================
    /// <summary>
    /// 讀取URL的圖片
    /// <para>編輯者: Dimos</para>
    /// </summary>
    private IEnumerator ImageSet(string _mealImage_url)
    {
        Debug.Log("ImageObject.ImageSet(). Url: " + _mealImage_url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(_mealImage_url + "?width=1000&height=730");//載入網路圖片
        yield return www.SendWebRequest();
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            //獲取Texture
            mealImage_texture2D = ((DownloadHandlerTexture)www.downloadHandler).texture;
            //因為我們定義的是Image，所以這裡需要把Texture2D轉化為Sprite
            imageHint_tx.text = "";
            currentImageByte = mealImage_texture2D.EncodeToPNG();//將texture轉換為byte[]
            //image.sprite = Sprite.Create(mealImage_txtue2D, new Rect(0, 0, mealImage_txtue2D.width, mealImage_txtue2D.height), new Vector2(0.5f, 0.5f));
            ShowBackground(true);
            this_rawImg.texture = mealImage_texture2D;
            DoAspectFit();
        }
    }

    //========================================================================
    protected void GetRectSize()
    {
        RectTransform tf = transform as RectTransform;

        m_rectSize = tf.sizeDelta;
        if (m_rectSize.x == 0 || m_rectSize.y == 0)
        {
            m_rectSize = (transform.parent as RectTransform).sizeDelta;
        }

        if (m_rectSize.x <= 0 || m_rectSize.y <= 0)
        {    // Fail back logic 
            m_rectSize = Vector2.one;
        }
    }

    //========================================================================
    protected virtual void DoAspectFit()
    {
        if (this_rawImg == null)
        {
            Debug.Log("AspectFitRawImage: image is null");
            return;
        }

        Texture texture = this_rawImg.texture;
        if (texture == null)
        {
            Debug.Log("AspectFitRawImage: texture is null");
            return;
        }

        float w = texture.width;
        float h = texture.height;
        if (w == h)
        {
            this_rawImg.uvRect = new Rect(0, 0, 1, 1);
            return;     // nothing to do
        }

        float textureRatio = w / h;
        float viewRatio = m_rectSize.x / m_rectSize.y;

        // ken: some debug value
        // Debug.Log("rectSize=" + m_rectSize);
        // Debug.Log("w=" + w + " h=" + h);
        // Debug.Log("textureRatio=" + textureRatio + " viewRatio=" + viewRatio);

        if (viewRatio == textureRatio)
        {
            if (this_rawImg != null)
            {
                this_rawImg.uvRect = new Rect(0, 0, 1, 1);
            }
        }

        if (viewRatio > textureRatio)
        {   // middle potion of the image 
            float potionRatio = textureRatio / viewRatio;   // (diff - 1) * textureRatio;
            float offset = (1 - potionRatio) * 0.5f;
            this_rawImg.uvRect = new Rect(0, offset, 1, potionRatio);
        }
        else
        {
            float potionRatio = viewRatio / textureRatio;   // (diff - 1) * textureRatio;
            float offset = (1 - potionRatio) * 0.5f;
            this_rawImg.uvRect = new Rect(offset, 0, potionRatio, 1);
        }
    }

    //========================================================================
    void ShowBackground(bool _status_bl)
    {
        if (_status_bl)
        {
            if (nullBackground_gObj != null)
                nullBackground_gObj.SetActive(false);
            if (whiteBackground_gObj != null)
                whiteBackground_gObj.SetActive(true);
        }
        else
        {
            if (nullBackground_gObj != null)
                nullBackground_gObj.SetActive(true);
            if (whiteBackground_gObj != null)
                whiteBackground_gObj.SetActive(false);
        }
    }


    //========================================================================

}

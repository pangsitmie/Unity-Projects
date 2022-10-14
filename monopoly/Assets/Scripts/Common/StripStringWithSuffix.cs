using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 省略字數
/// <para>詳細內容: 當文字過長時，將後段部份省略掉，並添加上設定的"省略符號"</para>
/// </summary>
public class StripStringWithSuffix : MonoBehaviour
{
    [SerializeField] Text targetText;
    private Font targetFont;

    /// <summary>
    /// 詳細內容: 設定"省略符號"的字串
    /// </summary>
    [SerializeField] string suffix = "...";
    private int suffixWidth_int;

    /// <summary>
    /// 詳細內容: 右方的預留空間，至少要大於30
    /// </summary>
    [SerializeField] int right = 30;
    private int maxWidth_int;
    [HideInInspector] public int thisObjectWidth_int;

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 設定目標text，並計算寬度</para>
    /// </summary>
    void Awake()
    {
        thisObjectWidth_int = Mathf.RoundToInt(this.GetComponent<RectTransform>().rect.width);
        maxWidth_int = thisObjectWidth_int - right;
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 計算"省略符號"的長度</para>
    /// </summary>
    void Start()
    {
        suffixWidth_int = CalculateLengthOfText(suffix);
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 計算輸入的字串長度(依照Font的字體)</para>
    /// </summary>
    public int CalculateLengthOfText(string _input_str)
    {
        int _totalLength_int = 0;
        targetFont = targetText.font;
        targetFont.RequestCharactersInTexture(_input_str, targetText.fontSize, targetText.fontStyle);
        CharacterInfo _characterInfo = new CharacterInfo();
        char[] arr = _input_str.ToCharArray();
        foreach (char c in arr)
        {
            targetFont.GetCharacterInfo(c, out _characterInfo, targetText.fontSize);

            _totalLength_int += _characterInfo.advance;
        }
        return _totalLength_int;
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 依照是否需要縮短字串，設定該text</para>
    /// </summary>
    public void StripString(string _input_str)
    {
        targetText.text = StripLengthWithSuffix(_input_str, maxWidth_int);
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 判斷是否需要縮短字串，並回傳正確的輸出</para>
    /// </summary>
    public string StripLengthWithSuffix(string _input_str, int _maxWidth_int)
    {
        int _length_int = CalculateLengthOfText(_input_str);

        if (_length_int > _maxWidth_int) //需縮減
        {
            return StripLength(_input_str, _maxWidth_int - suffixWidth_int) + suffix;
        }
        else
        {
            return _input_str;
        }
    }

    //====================================================================
    /// <summary>
    /// <para>編輯者: Dimos</para>
    /// <para>詳細內容: 依照最大寬度刪減字串，在最後加上設定的"省略符號"，並回傳修改完的字串</para>
    /// </summary>
    public string StripLength(string _input_str, int _maxWidth_int)
    {
        int _totalLength_int = 0;
        targetFont = targetText.font;
        targetFont.RequestCharactersInTexture(_input_str, targetText.fontSize, targetText.fontStyle);

        CharacterInfo _characterInfo = new CharacterInfo();

        char[] arr = _input_str.ToCharArray();

        int _charCount_int = 0;
        foreach (char c in arr)
        {
            targetFont.GetCharacterInfo(c, out _characterInfo, targetText.fontSize);

            /*2021_06_22改寫為下方6行程式碼(不知原本為何要做如此複雜的檢查，猜測類似四捨五入)
            int _newLength_int = _totalLength_int + _characterInfo.advance;
            if (_newLength_int > _maxWidth_int)
            {
                //如果(新的長度 減掉 限制長度)比(限制長度 減掉 舊的長度)大
                if (Mathf.Abs(_newLength_int - _maxWidth_int) > Mathf.Abs(_maxWidth_int - _totalLength_int))
                {
                    break;
                }
                else
                {
                    _totalLength_int = _newLength_int;
                    charCount_int++;
                    break;
                }
            }
            _totalLength_int += _characterInfo.advance;
            _charCount_int++;
            */
            
            _totalLength_int += _characterInfo.advance;
            if (_totalLength_int > _maxWidth_int)
            {
                break;
            }
            _charCount_int++;

        }
        return _input_str.Substring(0, _charCount_int);
    }

    //====================================================================
}

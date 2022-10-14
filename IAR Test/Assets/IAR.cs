using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IAR : MonoBehaviour
{
    // Start is called before the first frame update
    // Create instance of ReviewManager
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    int launchCount;
    [SerializeField] Text textView;

    void Start()
    {
        launchCount = PlayerPrefs.GetInt("TimesLaunch", 0);
        launchCount = launchCount + 1;
        PlayerPrefs.SetInt("TimesLaunch", launchCount);
        Debug.Log("launch count" + launchCount);

        Debug.Log("Start");
        if (launchCount % 5 == 0)
        {
            StartCoroutine(androidReview());
            Debug.Log("IAR start coroutine called");
            textView.text = "androidReview called";
        }

    }
    //Update is called once per frame
    IEnumerator androidReview()
    {
        _reviewManager = new ReviewManager();

        //Request a reveive info object
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            textView.text = requestFlowOperation.Error.ToString();
            Debug.Log(requestFlowOperation.Error.ToString());
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();

        //=====================Launch the in-app review flow====================
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            textView.text = launchFlowOperation.Error.ToString();
            Debug.Log(launchFlowOperation.Error.ToString());
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }

    public void changeScene(int idx)
    {
        Debug.Log("sceneBuildIndex to load: " + idx);
        SceneManager.LoadScene(idx);
    }
}

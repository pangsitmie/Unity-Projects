using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class loginVIew : MonoBehaviour
{
    private ReviewManager _reviewManager;
    PlayReviewInfo _playReviewInfo;

    void Start()
    {
        _reviewManager = new ReviewManager();
        StartCoroutine(androidReview());
        Debug.Log("Login view start finish");
    }

    IEnumerator androidReview()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
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
            Debug.Log(launchFlowOperation.Error.ToString());
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
}

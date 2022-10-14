using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePhoto : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public static void PickImage(ImageObject _imageObject)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
       {
           Debug.Log("Image path: " + path);
           if (path != null)
           {
               _imageObject.LoadImageWithPath(path);
           }
       });

        Debug.Log("Permission result: " + permission);
    }
}

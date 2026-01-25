using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class QuestionUnit : MonoBehaviour
{

    // Fetches image for a question

    RawImage image;

    private void Awake()
    {
        image = GetComponent<RawImage>();
    }

    public void SetImage(Question question)
    {
        if (question.imageLink != "")
            StartCoroutine(downloadImage(question.imageLink));
        /*
        image.sprite = question.image;
        image.preserveAspect = true;
        */
    }

    IEnumerator downloadImage(string imagePath)
    {
        Debug.Log("" + imagePath);
        yield return null;
        //UnityWebRequest texture = UnityWebRequestTexture.GetTexture ("https://tornquisterik.github.io/Webpage/images/" + imagePath);
        //yield return texture.SendWebRequest ();
        //Texture2D question = DownloadHandlerTexture.GetContent (texture);
        //image.texture = question;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class QuestionUnit : MonoBehaviour
{
    RawImage image;

    private void Awake()
    {
        image = GetComponent<RawImage>();
    }

    public void SetImage(Question question)
    {
        if (question.image != "")
            StartCoroutine(downloadImage(question.image));
        /*
        image.sprite = question.image;
        image.preserveAspect = true;
        */
    }

    IEnumerator downloadImage(string imagePath)
    {
        Debug.Log("" + imagePath);
        UnityWebRequest texture = UnityWebRequestTexture.GetTexture ("https://tornquisterik.github.io/Webpage/images/" + imagePath);
        yield return texture.SendWebRequest ();
        Texture2D question = DownloadHandlerTexture.GetContent (texture);
        image.texture = question;
    }
}

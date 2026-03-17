using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class QuestionUnit : MonoBehaviour
{
    // Handles the question image and doctor sprite during battle
    // When a question has an image, it shows in place of the doctor,
    // and the doctor shifts to peek out from the left side
    // When there is no question image, the doctor stands in his normal position

    RawImage image;

    // Serialized for Unity inspector
    [SerializeField] private RawImage doctorSprite;
    [SerializeField] private GameObject questionImageBackground;

    // Doctor's normal standing position when no question image is present
    private readonly Vector2 DOCTOR_NORMAL_POS = new Vector2(200f, -19f);

    // Doctor's peeking position when a question image is present
    // Shifted left so he peeks out from the left side of the image
    private readonly Vector2 DOCTOR_PEEK_POS = new Vector2(40f, -19f);

    private void Awake()
    {
        image = GetComponent<RawImage>();
    }

    public void SetImage(Question question)
    {
        if (!string.IsNullOrEmpty(question.imageLink))
        {
            // Show image and background, shift doctor to peek position
            gameObject.SetActive(true);
            questionImageBackground.SetActive(true);
            doctorSprite.rectTransform.anchoredPosition = DOCTOR_PEEK_POS;
            StartCoroutine(DownloadImage(question.imageLink));
        }
        else
        {
            // No question image hides image and background
            // restore doctor to his normal standing position
            gameObject.SetActive(false);
            questionImageBackground.SetActive(false);
            doctorSprite.rectTransform.anchoredPosition = DOCTOR_NORMAL_POS;
        }
    }

    IEnumerator DownloadImage(string url)
    {
        // Use UnityWebRequestTexture to download raw image bytes directly
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            image.texture = texture;

            // Scale the image to fit within the max size while preserving aspect ratio.
            float maxWidth = 300f;
            float maxHeight = 150f;
            float widthRatio = texture.width / maxWidth;
            float heightRatio = texture.height / maxHeight;
            float scale = Mathf.Max(widthRatio, heightRatio);

            Vector2 size = new Vector2(texture.width, texture.height) / scale;
            image.rectTransform.sizeDelta = size;
        }
        else
        {
            Debug.LogError("Question image load error: " + request.error);
            Debug.LogError("Failed URL: " + url);
            // If download failed, hide image and background, restore doctor.
            gameObject.SetActive(false);
            questionImageBackground.SetActive(false);
            doctorSprite.rectTransform.anchoredPosition = DOCTOR_NORMAL_POS;
        }
    }
}
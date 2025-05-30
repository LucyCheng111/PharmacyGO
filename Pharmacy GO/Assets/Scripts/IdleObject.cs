using UnityEngine;

public class IdleObject : MonoBehaviour
{

    // Provides movement for idling objects

    [Header ("Rotation")]
    public bool rotate;
    public float rotateAmplitude = 1f;
    public float rotateWavelength = 1f;

    [Header ("Heartbeat Scaling")]
    public bool heartbeatScale;
    public float scaleAmplitude = 1f;
    public float scaleWavelength = 1f;
    public float heartbeatDowntime = 1f;

    private void Update ()
    {
        TryGetComponent<RectTransform> (out RectTransform rectTransform);
        if (rotate)
        {
            Vector3 rotateAmount = new Vector3 (0, 0, SampleRotation (Time.time));
            rectTransform.eulerAngles = rotateAmount;
        }

        if (heartbeatScale)
        {
            Vector3 scaleAmount = Vector3.one * SampleScale (Time.time);
            rectTransform.localScale = Vector3.one + scaleAmount;
        }
    }

    private float SampleRotation (float time)
    {
        float speed = Mathf.PI / rotateWavelength;
        float sample = Mathf.Sin (speed * time) * rotateAmplitude;
        return sample;
    }
    private float SampleScale (float time)
    {
        time = time % (2 * scaleWavelength + heartbeatDowntime);
        float speed = Mathf.PI / scaleWavelength;
        float sample = Mathf.Pow (Mathf.Sin (speed * time), 2) * scaleAmplitude;
        if (time < (2 * scaleWavelength))
        {
            return sample;
        }
        return 0f;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class SprintButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerControl.Instance.sprintPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerControl.Instance.sprintPressed = false;
    }
}
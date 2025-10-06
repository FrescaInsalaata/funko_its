using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIFirstSelected : MonoBehaviour
{
    public GameObject firstSelected;

    void OnEnable()
    {
        StartCoroutine(SetSelected());
    }

    IEnumerator SetSelected()
    {
        yield return null; // aspetta un frame
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }
}

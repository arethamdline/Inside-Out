using System.Collections;
using UnityEngine;
using TMPro;

public class ResponseText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float textSpeed = 0.02f;

    private Coroutine typingCoroutine;

    void Start()
    {
        textComponent.text = "";
    }

    public void DisplayResponse(string newText)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(newText));
    }

    IEnumerator TypeLine(string line)
    {
        textComponent.text = "";

        foreach (char c in line)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
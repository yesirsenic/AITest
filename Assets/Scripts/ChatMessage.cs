using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatMessage : MonoBehaviour
{
    public Text messageText;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetMessage(string msg)
    {
        messageText.text = msg;
        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(3f); // 3초 유지

        float duration = 2f; // 사라지는 데 걸리는 시간
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}

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
        yield return new WaitForSeconds(3f); // 3�� ����

        float duration = 2f; // ������� �� �ɸ��� �ð�
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

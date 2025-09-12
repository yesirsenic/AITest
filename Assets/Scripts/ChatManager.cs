using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public GameObject chatMessagePrefab; // ChatMessage 프리팹
    public Transform chatContent;        // 메시지를 담을 부모(ScrollView Content 등)

    void AppendMessage(string message)
    {
        GameObject newMsg = Instantiate(chatMessagePrefab, chatContent);
        ChatMessage chatMsg = newMsg.GetComponent<ChatMessage>();
        chatMsg.SetMessage(message);  
    }
}

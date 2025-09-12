using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public GameObject chatMessagePrefab; // ChatMessage ������
    public Transform chatContent;        // �޽����� ���� �θ�(ScrollView Content ��)

    void AppendMessage(string message)
    {
        GameObject newMsg = Instantiate(chatMessagePrefab, chatContent);
        ChatMessage chatMsg = newMsg.GetComponent<ChatMessage>();
        chatMsg.SetMessage(message);  
    }
}

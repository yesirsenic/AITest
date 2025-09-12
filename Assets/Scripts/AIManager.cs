using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using UnityEngine.UI;

//Ollama 서버(로컬 http://localhost:11434/api/generate)에 보낼 요청(request) 데이터 구조.
[System.Serializable]
public class OllamaRequest
{
    public string model;
    public string prompt;
    public bool stream;
}

//Ollama 서버가 보내주는 응답(response) 데이터 구조입니다.
[System.Serializable]
public class OllamaResponse
{
    public string model;
    public string created_at;
    public string response;
    public bool done;
}

public class AIManager : MonoBehaviour
{

    [Header("UI Components")]
    public InputField inputField;
    public GameObject chatMessagePrefab;
    public Transform chatContent;

    public PromptLoader promptLoader; 
    private string currentSystemPrompt;
    private string currentNpcId;

    string apiUrl = "http://localhost:11434/api/generate";

    public void OnSendButton()
    {
        string userInput = inputField.text.Trim();
        if (string.IsNullOrEmpty(userInput)) return;

        CreateMessage("나: " + userInput);

        StartCoroutine(SendMessageToOllama(userInput));

        inputField.text = "";
        inputField.ActivateInputField();
    }

    //채팅 로그에 줄 추가하는 기능.
    void CreateMessage(string text)
    {
        GameObject newMsg = Instantiate(chatMessagePrefab, chatContent);
        newMsg.GetComponent<ChatMessage>().SetMessage(text);
    }

    //대화 시작 전 캐릭터 성격/설정 불러오기.
    public void StartConversation()
    {
        currentSystemPrompt = promptLoader.GetPrompt(currentNpcId);
        Debug.Log("대화 시작! NPC 프롬프트: " + currentSystemPrompt);
    }

    //외부에서 NPC를 지정해주는 메서드.
    public void SetNPC(string npcId)
    {
        currentNpcId = npcId;
    }

    //Ollama 서버에 메시지를 보내고 응답을 받는 실제 통신 부분.
    IEnumerator SendMessageToOllama(string userInput)
    {
        string userPrompt = "사용자: " + userInput + "\n주민:";

        OllamaRequest request = new OllamaRequest()
        {
            model = "llama3",
            prompt = currentSystemPrompt + userPrompt,
            stream = false
        };

        string json = JsonUtility.ToJson(request);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw); //서버를 보낼 데이터를 지정함.
            www.downloadHandler = new DownloadHandlerBuffer(); //서버 응답을 어디에 저장할지 지정함.
            www.SetRequestHeader("Content-Type", "application/json"); //서버에게 "내가 보내는 데이터는 JSON 형식이야" 라고 알려주는 것.

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseJson = www.downloadHandler.text;
                OllamaResponse parsed = JsonUtility.FromJson<OllamaResponse>(responseJson);
                CreateMessage("AI: " + parsed.response);
            }
            else
            {
                CreateMessage("⚠ 오류: " + www.error + "\n내용: " + www.downloadHandler.text);
            }
        }
    }
    

    
}


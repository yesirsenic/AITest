using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

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
    public ScrollRect scrollRect;

    public PromptLoader promptLoader;
    public MemoryManager memoryManager;
    private string currentSystemPrompt;
    private string currentNpcId;
    private string lastAIResponse = "";

    string apiUrl = "http://localhost:11434/api/generate";

    void Start()
    {
        StartAIDialogue();
    }

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

        StartCoroutine(ScrollToBottomNextFrame());
    }

    //메시지를 추가한 직후 화면을 자동으로 맨 아래로 당기는 기능
    IEnumerator ScrollToBottomNextFrame()
    {
        Canvas.ForceUpdateCanvases();
        
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)chatContent);
        scrollRect.verticalNormalizedPosition = 0f; 
    }

    // 대화 시작 전 캐릭터 성격/설정 불러오기.
    public void StartConversation()
    {
        
        string basePrompt = promptLoader.GetPrompt(currentNpcId);

        
        List<MemoryItem> memories = memoryManager.GetMemories(currentNpcId);

        string memoryText = "";
        if (memories.Count > 0)
        {
            memoryText = "이 NPC가 기억하는 과거 대화:\n";
            foreach (var m in memories)
            {
                
                memoryText += $"- ({m.importance}) {m.text}\n";
            }
        }

        
        currentSystemPrompt = basePrompt + "\n\n" + memoryText;

        Debug.Log("대화 시작! NPC 프롬프트: " + currentSystemPrompt);
    }

    //외부에서 NPC를 지정해주는 메서드.
    public void SetNPC(string npcId)
    {
        currentNpcId = npcId;
    }

    //ai 끼리의 대화 코루틴 시작
    public void StartAIDialogue()
    {
        StartCoroutine(AIDialogueLoop());
    }

    IEnumerator AIDialogueLoop()
    {
        string lastMessage = "안녕하세요, 오늘은 날씨가 좋네요."; // 대화 시작 문장
        string currentNpc = "npc_girl"; // 첫 발화자

        while (true) 
        {
            yield return StartCoroutine(SendMessageToOllamaAsNPC(currentNpc, lastMessage));

            // 직전 응답을 다음 입력으로 넘기기
            lastMessage = lastAIResponse;

            // 말하는 NPC 교체
            currentNpc = (currentNpc == "npc_girl") ? "npc_blacksmith" : "npc_girl";

            yield return new WaitForSeconds(3f); // 텀 두고 대화
        }
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
                StartCoroutine(EvaluateImportance(userInput, parsed.response));
            }
            else
            {
                CreateMessage("⚠ 오류: " + www.error + "\n내용: " + www.downloadHandler.text);
            }
        }
    }

    // Ollama 서버와 통신하여 특정 NPC가 상대방의 발화(input)에 대한 응답을 생성하고,
    // 그 결과를 UI와 메모리에 기록하는 함수 (NPC ↔ NPC 대화용).
    IEnumerator SendMessageToOllamaAsNPC(string npcId, string input)
    {      
        
      
        string systemPrompt = promptLoader.GetPrompt(npcId);
        string userPrompt = "상대방: " + input + "\n" + "너의 대답:";

        OllamaRequest request = new OllamaRequest()
        {
            model = "llama3",
            prompt = systemPrompt + "\n\n" + userPrompt,
            stream = false
        };

        string json = JsonUtility.ToJson(request);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseJson = www.downloadHandler.text;
                OllamaResponse parsed = JsonUtility.FromJson<OllamaResponse>(responseJson);

                lastAIResponse = parsed.response.Trim();
                CreateMessage($"{npcId}: {lastAIResponse}");

                // 🔹 AI ↔ AI 대화도 중요도 평가 및 저장
                StartCoroutine(EvaluateImportance("상대방: " + input, lastAIResponse, npcId));
            }
            else
            {
                CreateMessage($"{npcId} ⚠ 오류: {www.error}");
            }
        }
    }

    IEnumerator EvaluateImportance(string userInput, string aiResponse , string speakerNpcId = null)
    {
        //AI에게 평가 규칙을 설명하는 프롬프트
        string evalPrompt =
         "다음 대화를 보고 중요도를 1~5로 평가하라.\n" +
         "1 = 전혀 중요하지 않음\n" +
         "5 = 반드시 기억해야 하는 사실\n\n" +
         "대화:\n" + userInput + "\n응답: " + aiResponse + "\n\n" +
         "중요도가 3 이상이면 반드시 아래 형식으로 출력:\n" +
         "LEVEL: <숫자>\nSAVE: <요약된 사실>\n\n" +
         "중요도가 3 미만이면 'IGNORE'라고 출력해.";

        OllamaRequest evalRequest = new OllamaRequest()
        {
            model = "llama3",
            prompt = evalPrompt,
            stream = false
        };

        string json = JsonUtility.ToJson(evalRequest);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            //AI가 스스로 대화 내용을 보고 "이건 중요한 기억이야" 라고 판단 → 그걸 메모리DB에 저장하는 구조
            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseJson = www.downloadHandler.text;
                OllamaResponse parsed = JsonUtility.FromJson<OllamaResponse>(responseJson);

                string result = parsed.response.Trim();
                Debug.Log("중요도 평가 결과: " + result);

                if (result.StartsWith("LEVEL:"))
                {
                    
                    string[] lines = result.Split('\n');
                    int level = int.Parse(lines[0].Replace("LEVEL:", "").Trim());
                    string fact = lines[1].Replace("SAVE:", "").Trim();

                    string targetNpc = string.IsNullOrEmpty(speakerNpcId) ? currentNpcId : speakerNpcId;

                    FindFirstObjectByType<MemoryManager>().AddMemory(targetNpc, fact, level);
                    Debug.Log($"메모리에 저장됨 (중요도 {level}): {fact}");
                }
            }
            else
            {
                Debug.LogWarning("중요도 평가 실패: " + www.error);
            }
        }
    }



}


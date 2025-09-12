using UnityEngine;

//JSON에 들어있는 NPC 데이터를 담는 클래스
[System.Serializable]
public class NPCPrompt
{
    public string id;
    public string name;
    public string prompt;
}

[System.Serializable]
public class NPCPromptList
{
    public NPCPrompt[] prompts;
}

public class PromptLoader : MonoBehaviour
{
    public TextAsset jsonFile; 

    private NPCPromptList npcPromptList;

    void Start()
    {
        npcPromptList = JsonUtility.FromJson<NPCPromptList>(jsonFile.text);

        foreach (var npc in npcPromptList.prompts)
        {
            Debug.Log($"NPC: {npc.name}, Prompt: {npc.prompt}");
        }
    }

    //입력받은 id와 일치하는 NPC를 찾는 매서드
    public string GetPrompt(string id)
    {
        foreach (var npc in npcPromptList.prompts)
        {
            if (npc.id == id) return npc.prompt;
        }
        return null;
    }
}

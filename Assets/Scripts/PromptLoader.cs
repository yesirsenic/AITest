using UnityEngine;

//JSON�� ����ִ� NPC �����͸� ��� Ŭ����
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

    //�Է¹��� id�� ��ġ�ϴ� NPC�� ã�� �ż���
    public string GetPrompt(string id)
    {
        foreach (var npc in npcPromptList.prompts)
        {
            if (npc.id == id) return npc.prompt;
        }
        return null;
    }
}

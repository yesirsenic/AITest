using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public AIManager aiManager;
    public string npcId = "npc_village_beauty"; //임시

    void Start()
    {
        //임시
        aiManager.SetNPC(npcId);         
        aiManager.StartConversation();
    }

    //현재 NPC 오브젝트의 npcId를 AIManager에 전달
    public void TalkToNPC()
    {
        aiManager.SetNPC(npcId);
        aiManager.StartConversation();
    }
}

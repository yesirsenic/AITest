using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public AIManager aiManager;
    public string npcId = "npc_village_beauty"; //�ӽ�

    void Start()
    {
        //�ӽ�
        aiManager.SetNPC(npcId);         
        aiManager.StartConversation();
    }

    //���� NPC ������Ʈ�� npcId�� AIManager�� ����
    public void TalkToNPC()
    {
        aiManager.SetNPC(npcId);
        aiManager.StartConversation();
    }
}

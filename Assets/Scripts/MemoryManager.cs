using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

//하나의 기억 단위
[System.Serializable]
public class MemoryItem
{
    public string text;
    public int importance; 
}

//특정 NPC 한 명의 기억 저장소
[System.Serializable]
public class NPCMemoryEntry
{
    public string npcId;
    public List<MemoryItem> memories = new List<MemoryItem>();
}

//게임 전체 NPC들의 기억 데이터베이스
[System.Serializable]
public class NPCMemoryData
{
    public List<NPCMemoryEntry> entries = new List<NPCMemoryEntry>();
}

public class MemoryManager : MonoBehaviour
{
    private string filePath;
    private NPCMemoryData memoryData;

    void Awake()
    {
        filePath = Application.persistentDataPath + "/npc_memory.json";
        LoadMemory();
    }

        //즉, 기존 메모리 파일 불러오기 → 없으면 새로 시작하기
    void LoadMemory()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            memoryData = JsonUtility.FromJson<NPCMemoryData>(json);
        }
        else
        {
            memoryData = new NPCMemoryData();
        }
    }

       //현재 메모리 데이터를 JSON 파일로 저장
    public void SaveMemory()
    {
        string json = JsonUtility.ToJson(memoryData, true);
        File.WriteAllText(filePath, json);
    }

    public void AddMemory(string npcId, string fact, int importance)
    {
        var entry = memoryData.entries.Find(e => e.npcId == npcId);
        if (entry == null) //새로운 NPC라면 자동으로 “기억 저장소” 생성
        {
            entry = new NPCMemoryEntry { npcId = npcId };
            memoryData.entries.Add(entry);
        }

        entry.memories.Add(new MemoryItem { text = fact, importance = importance }); //NPC에게 새로운 기억을 저장

               // 최대 50개 유지
        if (entry.memories.Count > 50) //중요도 낮고 오래된 것부터 잊어버림
        {
            entry.memories = entry.memories
                .OrderByDescending(m => m.importance) // 중요도 높은 순
                .ThenByDescending(m => entry.memories.IndexOf(m)) // 최근 것 우선
                .Take(50)
                .ToList();
        }

        SaveMemory();
    }

        //특정 NPC의 기억 목록을 읽어오는 메서드
    public List<MemoryItem> GetMemories(string npcId)
    {
        var entry = memoryData.entries.Find(e => e.npcId == npcId);
        return entry != null ? entry.memories : new List<MemoryItem>();
    }
}

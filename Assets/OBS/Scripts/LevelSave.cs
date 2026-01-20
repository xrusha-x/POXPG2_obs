using UnityEngine;

public static class LevelSave
{
    public static void Set(string levelId, bool completed, bool allItems, bool noDamage)
    {
        PlayerPrefs.SetInt(Key(levelId, "completed"), completed ? 1 : 0);
        PlayerPrefs.SetInt(Key(levelId, "allItems"), allItems ? 1 : 0);
        PlayerPrefs.SetInt(Key(levelId, "noDamage"), noDamage ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool GetCompleted(string levelId)
    {
        return PlayerPrefs.GetInt(Key(levelId, "completed"), 0) == 1;
    }

    public static bool GetAllItems(string levelId)
    {
        return PlayerPrefs.GetInt(Key(levelId, "allItems"), 0) == 1;
    }

    public static bool GetNoDamage(string levelId)
    {
        return PlayerPrefs.GetInt(Key(levelId, "noDamage"), 0) == 1;
    }

    static string Key(string levelId, string suffix)
    {
        return $"level_{levelId}_{suffix}";
    }
}

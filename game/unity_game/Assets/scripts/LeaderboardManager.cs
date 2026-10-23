using UnityEngine;

public static class LeaderboardManager
{
    public static void SaveResult(string player, string level, float time, float score)
    {
        PlayerPrefs.SetInt($"Completed_{player}_{level}", 1);
        PlayerPrefs.SetFloat($"Time_{player}_{level}", time);
        PlayerPrefs.SetFloat($"Score_{player}_{level}", score);

        PlayerPrefs.Save();
    }

    public static bool IsLevelCompleted(string player, string level)
    {
        return PlayerPrefs.GetInt($"Completed_{player}_{level}", 0) == 1;
    }

    public static int GetScore(string player, string level)
    {
        return PlayerPrefs.GetInt($"Score_{player}_{level}", 0);
    }

    public static float GetTime(string player, string level)
    {
        return PlayerPrefs.GetFloat($"Time_{player}_{level}", 0f);
    }

    public static bool PlayerExists(string player)
    {
        return PlayerPrefs.HasKey($"Player_{player}");
    }

    public static void RegisterPlayer(string player)
    {
        PlayerPrefs.SetInt($"Player_{player}", 1);
        PlayerPrefs.Save();
    }
}
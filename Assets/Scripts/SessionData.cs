using UnityEngine;

public static class SessionData  {
    public static string playerName;
    public static float startTime = Time.time;
    public static Vector2 lastCheckpoint = new Vector2(0, Player.SEA_LEVEL_Y);

    public static void InitGame() {
        lastCheckpoint = new Vector2(0, Player.SEA_LEVEL_Y);
        startTime = Time.time;
    }
}

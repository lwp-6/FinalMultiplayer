public class GlobleVar
{
    public static bool isPause = false;
    public static int TotalGameLevel = 2;
    public static int GameLevelNameLen = 5;

    // 连接状态
    public static bool isOnline = false;
    public static bool isJoinRoom = false;
    public static bool isJoinLobby = false;

    // 分辨率
    public static int[,] ScreenResolution = new int[3, 2]{
        { 640, 360 },
        { 1280, 720},
        { 1920, 1080},
    };

    // NPC状态
    public enum EnemyState
    {
        Idle = 0,
        Patrol,
        Chase,
        Attack,
        Died,
    }

    // 全屏 / 窗口
    public enum ScreenMode
    {
        WindowScreen = 0,
        FulleScreen,
    }



}

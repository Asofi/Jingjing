public static class EventManager
{

    public delegate void GameEvent();
    public delegate void ScoreEvent(int amount);
    public delegate void SkinEvent(Skin skin);

    public static event GameEvent OnGameStart, OnGameOver,
                                    OnMute, OnSecondChance,
                                    OnAddCoin, OnLevelUp, OnCandycallipce;
    public static event ScoreEvent OnAddScore;
    public static event SkinEvent OnChangeSkin;

    public static void Candycallipce() {
        if (OnCandycallipce != null) OnCandycallipce();
    }

    public static void ChangeSkin(Skin skin)
    {
        if (OnChangeSkin != null) OnChangeSkin(skin);
    }

    public static void GameOver()
    {
        if (OnGameOver != null) OnGameOver();
    }

    public static void SecondChance()
    {
        if (OnSecondChance != null) OnSecondChance();
    }

    public static void GameStart()
    {
        if (OnGameStart != null) OnGameStart();
    }

    public static void AddScore(int amount)
    {
        if (OnAddScore != null) OnAddScore(amount);
    }

    public static void AddCoin()
    {
        if (OnAddCoin != null) OnAddCoin();
    }

    public static void Mute()
    {
        if (OnMute != null) OnMute();
    }

    public static void LevelUp()
    {
        if (OnLevelUp != null) OnLevelUp();
    }
}

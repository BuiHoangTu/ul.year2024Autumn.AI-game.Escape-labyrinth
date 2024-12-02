public static class Rewards
{
    ////// Common rewards
    public const float WIN = 100f;
    public const float LOSE = -100f;
    public const float PROLONG_MATCH = 10f;
    public const float HIT_WALL = -1f;
    public const float EXPLORING = 1;

    ////// Finder rewards
    public const float SEEING_ESCAPER = 0.1f;
    public const float CHASING_ESCAPER = 0.01f;

    ////// Escaper rewards
    public const float SEEING_EXIT = 0.1f;
    public const float DISTANCE_TO_EXIT = 0.01f;

}

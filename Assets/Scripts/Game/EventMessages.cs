public static class EventMessages
{
    public static string ON_ZOMBIE_TRY_MOVE = "ON_ZOMBIE_TRY_MOVE"; // <BaseZombie, Tile>, <bool>
    public static string ON_ZOMBIE_MOVE = "ON_ZOMBIE_MOVE"; // <BaseZombie, Tile>
    public static string ON_ZOMBIE_TAKE_DAMAGE = "ON_ZOMBIE_TAKE_DAMAGE"; // <BaseZombie, int, DamageType>
    public static string ON_ZOMBIE_DIE = "ON_ZOMBIE_DIE"; // <BaseZombie>
    public static string ON_PLAYER_TAKE_DAMAGE = "ON_PLAYER_TAKE_DAMAGE"; // <int>
    public static string ON_DAYTIME_CHANGE = "ON_DAYTIME_CHANGE"; // <DayTime>
    public static string ON_OBSTACLE_TAKE_DAMAGE = "ON_OBSTACLE_TAKE_DAMAGE"; // <BaseObstacle, int, DamageType>
    public static string ON_OBSTACLE_DIE = "ON_OBSTACLE_DIE"; // <BaseObstacle>
    public static string ON_ZOMBIE_DEAL_DAMAGE_TO_OBSTACLE = "ON_ZOMBIE_DEAL_DAMAGE_TO_OBSTACLE"; // <BaseZombie, int, BaseObstacle>
    public static string ON_PLAYER_END_TURN = "ON_PLAYER_END_TURN";
    public static string ON_PLAYER_MAKE_REST = "ON_PLAYER_MAKE_REST";
    public static string ON_CARD_PLAYED = "ON_CARD_PLAYED"; // <Card>
}
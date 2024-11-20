public enum TypeOfTile
{
    None = -99,
    Normal = 0,

}

public enum TypeOfRestrain
{
    None = -99,
    Padlock = 0,
    Ice = 1,
    FallingPadlock = 2,
    Cage = 3

}


public enum Bonus
{
    None = 0,
    DestroyOne = 1,
    SwitchGemTeleport = 2,
    Destroy3x3 = 3,
    DestroyHorizontal = 4,
    DestroyVertical = 5,
    DestroyHorizontalAndVertical = 6,
    DestroyAllGemsWithThisColor = 7,
    GiveMoreTime = 8,
    GiveMoreMoves = 9,
    HealMe = 10,
    DamageOpponent = 11
}

[System.Serializable]
public class BoardElementFile { //for save and load

    public TileElement tile = new TileElement();
    public Content content = new Content();
    public RestrainElement restrain = new RestrainElement();

}


[System.Serializable]
public class BoardElement : BoardElementFile //for actual play
{
    public MoveInfo moveInfo;

}


[System.Serializable]
public class TileElement
{
    public TypeOfTile type = TypeOfTile.None;
    public int hp;

}
[System.Serializable]
public class RestrainElement
{
    public TypeOfRestrain type = TypeOfRestrain.None;
    public int hp;

}


public class MoveInfo
{
    

}

#region Content
public enum TypeOfContent
{
    None = -99,
    Gem = 0,
    Block = 1,
    Token = 2,
    Junk = 3,
    Bonus = 4,
    FallingBlock = 5,
    GenerativeBlock = 6,
}

public enum ContentColor
{
    None = -99,
    Random = -1,
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    E = 4,
    F = 5,
    G = 6,

}

[System.Serializable]
public class Content 
{
    public TypeOfContent type = TypeOfContent.None;
    public ContentColor color = ContentColor.None;
    public int hp;
    public Bonus bonus = Bonus.None;

}
#endregion
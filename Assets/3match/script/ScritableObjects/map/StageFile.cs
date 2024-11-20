using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "3Match/Stage")]
public class StageFile : ScriptableObject
{
    public RulesetTemplate rules;
    public ThemeTemplate theme;
    public CameraTemplate camera;
    public Sprite background;


    public BoardElementFile[] map;
    public int width;
    public int height;

    public void SetTile(int x, int y, BoardElementFile myTile)
    {
        map[y * width + x] = myTile;
    }

    public BoardElementFile GetTile(int x, int y)
    {
        return map[y * width + x];
    }

    public int GetTileIndexFromCoordinates(int x, int y)
    {
        return (y * width + x);
    }

    public Vector2Int GetTileCoordinates(int index)
    {
        int x = index & width;
        int y = index / width;

        return new Vector2Int(x, y);
    }

    public RestrainElement IntantiateRestrain(TypeOfRestrain type, int hp)
    {
        RestrainElement restrain = new RestrainElement();
        restrain.type = type;
        restrain.hp = hp;

        return restrain;
    }

    public TileElement InstantiateTileElement(TypeOfTile type, int hp)
    {
        TileElement tile = new TileElement();
        tile.type = type;
        tile.hp = hp;

        return tile;
    }

    public Content EmptyContent()
    {
        Content content = new Content();
        content.type = TypeOfContent.None;
        content.color = ContentColor.None;
        content.hp = -99;
        content.bonus = Bonus.None;

        return content;
    }

    public Content IntantiateGem(ContentColor color)
    {
        Content content = new Content();
        content.type = TypeOfContent.Gem;
        content.color = color;
        content.hp = 1;
        content.bonus = Bonus.None;

        return content;
    }

    public Content IntantiateJunk()
    {
        Content content = new Content();
        content.type = TypeOfContent.Junk;
        content.color = ContentColor.None;
        content.hp = 1;
        content.bonus = Bonus.None;

        return content;
    }

    public Content IntantiateToken()
    {
        Content content = new Content();
        content.type = TypeOfContent.Token;
        content.color = ContentColor.None;
        content.hp = 1;
        content.bonus = Bonus.None;

        return content;
    }

    public Content IntantiateBlock(int hp, TypeOfContent blockType)
    {
        Content content = new Content();
        content.type = blockType;
        content.color = ContentColor.None;
        content.hp = hp;
        content.bonus = Bonus.None;

        return content;
    }



    public Content IntantiateBonus(Bonus bonus)
    {
        Content content = new Content();
        content.type = TypeOfContent.Bonus;
        content.color = ContentColor.None;
        content.hp = 1;
        content.bonus = bonus;

        return content;
    }
}

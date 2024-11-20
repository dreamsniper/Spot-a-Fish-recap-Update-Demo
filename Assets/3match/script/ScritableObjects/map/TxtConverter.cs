using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System; //need for parsing

public class TxtConverter : MonoBehaviour {

    public string saveFolderPath = "Assets/3match/Resources/Maps/TxtConverted/";
    public TextAsset[] txtMaps;

	// Use this for initialization
	void Start () {
		
	}
	

    public void ConvertAll()
    {
        print("ConvertAll()");

        if (txtMaps == null)
            return;

        if (txtMaps.Length < 0)
            return;

        for (int i = 0; i < txtMaps.Length; i++)
            ConvertTxt(txtMaps[i]);

        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif

        print("Conversion done");
    }

    void ConvertTxt(TextAsset txt)
    {
        //load
        string fileContents = txt.text;//read here
        string[] parts = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

        int _X_tiles = Int16.Parse(parts[0]);
        int _Y_tiles = Int16.Parse(parts[1]);
        int[,,] board_array_master = new int[_X_tiles, _Y_tiles, 15];

        for (int y = 0; y < _Y_tiles + 2; y++)
        {
            if (y > 1)
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    string[] tile = parts[y].Split(new string[] { "|" }, StringSplitOptions.None);

                    for (int z = 0; z < 5; z++)
                    {
                        string[] tile_characteristic = tile[x].Split(new string[] { "," }, StringSplitOptions.None);
                        board_array_master[x, y - 2, z] = Int16.Parse(tile_characteristic[z]);
                    }
                }

            }
        }



        //convert and write
        StageFile asset = ScriptableObject.CreateInstance("StageFile") as StageFile;
        asset.height = _Y_tiles;
        asset.width = _X_tiles;
        asset.map = new BoardElementFile[_X_tiles * _Y_tiles];

        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                BoardElementFile temp = new BoardElementFile();

                if (board_array_master[x, y, 0] > -1)//if there is a tile
                {
                    temp.tile = asset.InstantiateTileElement(TypeOfTile.Normal, board_array_master[x, y, 0]);

  
                    if (board_array_master[x, y, 1] >= 0 && board_array_master[x, y, 1] <= 10)//if there is a random gem here
                    {
                        if (board_array_master[x, y, 1] == 10)//if there is a random gem here
                            temp.content = asset.IntantiateGem(ContentColor.Random);
                        else if (board_array_master[x, y, 1] < 9)
                            temp.content = asset.IntantiateGem((ContentColor)board_array_master[x, y, 1]);
                        else if (board_array_master[x, y, 1] == 9)//special
                        {
                            if (board_array_master[x, y, 4] == -200)//token
                                temp.content = asset.IntantiateToken();
                            else if (board_array_master[x, y, 4] == -100)//junk
                                temp.content = asset.IntantiateJunk();
                            else if (board_array_master[x, y, 4] > 0)//bonus
                                temp.content = asset.IntantiateBonus((Bonus)board_array_master[x, y, 4]);
                        }

                        if (board_array_master[x, y, 3] > 0)//padlock
                            temp.restrain = asset.IntantiateRestrain(TypeOfRestrain.Padlock, board_array_master[x, y, 3]);

                    }
                    else
                        temp.restrain = asset.IntantiateRestrain(TypeOfRestrain.None, 0);

                    if ((board_array_master[x, y, 1] > 40) && (board_array_master[x, y, 1] < 50))//block
                        temp.content = asset.IntantiateBlock(board_array_master[x, y, 1] - 40, TypeOfContent.Block);
                }
                else//no tile 
                {
                    
                    temp.content = asset.EmptyContent();
                    temp.tile = asset.InstantiateTileElement(TypeOfTile.None, -99);
                    temp.restrain = asset.IntantiateRestrain(TypeOfRestrain.None, 0); 
                }

                asset.SetTile(x, y, temp);
            }
        }

        #if UNITY_EDITOR
        //save
        AssetDatabase.CreateAsset(asset, saveFolderPath + txt.name + ".asset");
        #endif
    }
}

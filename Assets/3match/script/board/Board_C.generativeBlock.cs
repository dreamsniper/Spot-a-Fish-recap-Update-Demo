using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public partial class Board_C : MonoBehaviour {

    [System.Serializable]
    public class GenerativeBlockInfo
    {
        public Vector2Int myPosition;
        public Vector2Int myGenerativeTargertPosition;
        public bool generatorIsOn;
    }
    [HideInInspector] public List<GenerativeBlockInfo> generativeBlockInfos;

    public GenerativeBlockInfo GetGenerativeBlockInfo(Vector2Int blockPosition)
    {
        for (int i = 0; i < generativeBlockInfos.Count; i++)
        {
            if (generativeBlockInfos[i].myPosition == blockPosition)
                return generativeBlockInfos[i];
        }

        return null;
    }

    public bool ThisTileIsUnderAnActiveGenerativeBlock(Vector2Int tilePosition)
    {
        for (int i = 0; i < generativeBlockInfos.Count; i++)
        {
            if (generativeBlockInfos[i].myGenerativeTargertPosition == tilePosition && generativeBlockInfos[i].generatorIsOn)
                return true;
        }

        return false;
    }

}

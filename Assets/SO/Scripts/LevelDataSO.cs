using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName ="LevelData", menuName ="LevelData")]
public class LevelDataSO : ScriptableObject
{
    public Level[] levels;

    [Serializable]
    public class Level
    {
        public SceneReference scene;
        public LevelRanks[] levelTimes;
    }

    [Serializable]
    public class LevelRanks
    {
        public string rankName;

        [Tooltip("Reach this time or lower to get this rank")]
        public int rankTimesInSeconds;
        public Sprite rankImage;
        [TextArea]
        public string foodFlavorText;
    }
}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public SerializableDictonary<int, bool> keysCollected;
    public bool gameLost;

    public GameData()
    {
        this.playerPosition = Vector3.zero;
        this.keysCollected = new SerializableDictonary<int, bool>();
        this.gameLost = false;
    }
}
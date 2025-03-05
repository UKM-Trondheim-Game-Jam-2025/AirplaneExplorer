using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public SerializableDictonary<int, bool> keysCollected;

    public GameData()
    {
        playerPosition = Vector3.zero;
        this.keysCollected = new SerializableDictonary<int, bool>();
    }
}
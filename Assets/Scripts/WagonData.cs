using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WagonData
{
    public string wagonName;
    public WagonMaterial wagonMaterial;
    public int wagonMeshIndex;

    public Station startingStation;
    public List<PathPoint> paths = new();

    [Serializable]
    public class PathPoint
    {
        public Vector3 position;
        public Quaternion rotation;
        public float elapsedTime;
    }
}
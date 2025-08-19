using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using UnityEngine;
using Color = System.Drawing.Color;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct TileInfo
    {
        public int Id { get; set; }
        public TileKey Key { get; set; }
    }
}

using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.BoardEffects
{
    public interface IBoardEffect
    {
        BoardEffectConfig BoardEffectConfig { get; }
        IReadOnlyCollection<Vector2Int> Evaluate(ITileMatchInfo match, IBoardState boardState);
    }
}
using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface IBoardEffectService
    {
        public HashSet<Vector2Int> EvaluateEffects(IBoardState boardState, FindMatchResult findMatchResult);
    }
}
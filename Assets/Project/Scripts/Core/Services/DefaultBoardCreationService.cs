using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultBoardCreationService : IBoardCreationService
    {
        private readonly int width;
        private readonly int height;
        private readonly TileKey[] availableTileKeys;
        private readonly ITileGenerationService tileGenerationService;

        public DefaultBoardCreationService(GameplayInfo gameplayInfo, ITileGenerationService tileGenerationService)
        {
            this.width = gameplayInfo.BoardWidth;
            this.height = gameplayInfo.BoardHeight;
            this.availableTileKeys = gameplayInfo.AvailableTileKeys;
            this.tileGenerationService = tileGenerationService;
        }

        public IBoardState CreateBoard()
        {
            var board = new List<List<TileInfo>>(height);
            var boardState = new DefaultBoardState(board, 0);;

            for (var y = 0; y < height; y++)
            {
                board.Add(new List<TileInfo>(width));
                for (var x = 0; x < width; x++)
                {
                    board[y].Add(new TileInfo { Id = -1, Key = (TileKey) (-1) });
                }
            }

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var noMatchTypes = new List<TileKey>(availableTileKeys.Length);
                    for (var i = 0; i < availableTileKeys.Length; i++)
                    {
                        noMatchTypes.Add(availableTileKeys[i]);
                    }

                    if (x > 1 && board[y][x - 1].Key == board[y][x - 2].Key)
                    {
                        noMatchTypes.Remove(board[y][x - 1].Key);
                    }
                    
                    if (y > 1 && board[y - 1][x].Key == board[y - 2][x].Key)
                    {
                        noMatchTypes.Remove(board[y - 1][x].Key);
                    }
                    
                    tileGenerationService.GenerateNextTile(board[y][x], noMatchTypes, boardState);
                }
            }

            return boardState;
        }
    }
}
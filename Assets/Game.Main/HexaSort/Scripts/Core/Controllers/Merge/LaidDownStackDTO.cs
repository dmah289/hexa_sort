using HexaSort.Core.Entities.Grid;
using manhnd_sdk.Scripts.SystemDesign.EventBus;

namespace HexaSort.Controllers.DifficultyAlgorithm
{
    public struct LaidDownStackDTO : IEventDTO
    {
        public HexCell cell;
        
        public LaidDownStackDTO(HexCell cell)
        {
            this.cell = cell;
        }
    }
}
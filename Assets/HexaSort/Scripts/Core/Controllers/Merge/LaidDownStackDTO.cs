using HexaSort.Scripts.Core.Entities;
using manhnd_sdk.Scripts.SystemDesign.EventBus;

namespace HexaSort.Scripts.Core.Controllers
{
    public struct LaidDownStackDTO : IEventDTO
    {
        public HexCellController cell;
        
        public LaidDownStackDTO(HexCellController cell)
        {
            this.cell = cell;
        }
    }
}
using HexaSort.Scripts.Core.Entities;
using manhnd_sdk.Scripts.SystemDesign.EventBus;

namespace HexaSort.Scripts.Core.Controllers
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
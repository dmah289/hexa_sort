using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;

namespace HexaSort.Scripts.Core.Controllers
{
    public class MergeController : MonoBehaviour, IEventBusListener
    {
        [SerializeField] private PathFinder pathFinder;

        #region Event Bus Listeners

        public void RegisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Register(onEventWithArgs: OnStackLaidDown);
        }

        public void DeregisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Deregister(onEventWithArgs: OnStackLaidDown);
        }

        #endregion

        private void OnStackLaidDown(LaidDownStackDTO dto)
        {
            
        }
    }
}
using UnityEngine;

namespace HexaSort.Scripts.Core.Entities
{
    public class GridController : MonoBehaviour
    {
        [Header("Self Components")]
        [SerializeField] private GridSpawner gridSpawner;
        
        [Header("References")]
        [SerializeField] private TrayController trayController;

        private void Awake()
        {
            gridSpawner = GetComponent<GridSpawner>();
            
            SetupLevel();
        }

        public void SetupLevel()
        {
            gridSpawner.SetupBoardLayout(trayController).Forget();
        }
    }
}
using Cysharp.Threading.Tasks;
using Game.Main.HexaSort.Scripts.Managers;
using HexaSort.UI.Loading.InGame;
using HexaSort.UI.Loading.MainMenu.Home;
using LevelEditor.LevelData;
using TMPro;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.UI.Popup
{
    public class UnlockMechanicPopup : APopup
    {
        [SerializeField] private eMechanicsType _mechanicType;
        [SerializeField] private TextMeshProUGUI title;
        
        [SerializeField] private GameObject woodGroup;
        [SerializeField] private GameObject packedGroup;

        protected override void Awake()
        {
            base.Awake();

            _mechanicType = eMechanicsType.None;
        }

        public void SetupMechanicType(eMechanicsType mechanicType)
        {
            title.text = $"Unlock {mechanicType.ToString()} cell";
            _mechanicType = mechanicType;
            woodGroup.SetActive(false);
            packedGroup.SetActive(false);
            
            if (_mechanicType == eMechanicsType.Wood)
            {
                woodGroup.SetActive(true);
                LocalDataManager.HasShownWoodMechanicTutorial = true;
            }
            else if (_mechanicType == eMechanicsType.Packed)
            {
                packedGroup.SetActive(true);
                LocalDataManager.HasShownPackedMechanicTutorial = true;
            }
        }
    }
}
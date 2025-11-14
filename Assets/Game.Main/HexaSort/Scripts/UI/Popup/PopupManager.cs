using System.Collections.Generic;
using Framework.UI;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace Game.Main.HexaSort.Scripts.UI.Popup
{
    public class PopupManager : MonoSingleton<PopupManager>
    {
        [SerializeField] private List<BackgroundClickHandler> popups;

        protected override void Awake()
        {
            base.Awake();
            
            popups = new();
            for (int i = 0; i < transform.childCount; i++)
            {
                BackgroundClickHandler panel;
                if (transform.GetChild(i).TryGetComponent(out panel))
                    popups.Add(panel);
            }
        }
        
        public void DisableAllPopups()
        {
            for (int i = 0; i < popups.Count; i++)
            {
                popups[i].gameObject.SetActive(false);
            }
        }
    }
}
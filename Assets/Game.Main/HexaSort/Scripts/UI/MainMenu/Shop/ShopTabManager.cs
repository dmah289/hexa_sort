using System;
using Coffee_Rush.UI.BaseSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Coffee_Rush.UI.Shop
{
    public class ShopTabManager : MonoBehaviour
    {
        [SerializeField] private RectTransform noAds_bg;
        

        private void Update()
        {
            AutoRotateNoadsBg();
        }
        
        private void AutoRotateNoadsBg()
        {
            Vector3 currentRotation = noAds_bg.rotation.eulerAngles;
            currentRotation.z += 20 * Time.deltaTime;
            noAds_bg.rotation = Quaternion.Euler(currentRotation);
        }
    }
}
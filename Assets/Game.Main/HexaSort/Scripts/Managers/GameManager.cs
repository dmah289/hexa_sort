using System;
using UnityEngine;

namespace HexaSort.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Runtime.UI
{
	public class PanelWinEffect : MonoBehaviour
	{
		[SerializeField] private ParticleSystem[] _confettis;

		protected void OnEnable()
		{
			WaitForAnimationAsync().Forget();
		}

		private async UniTask WaitForAnimationAsync()
		{
			await UniTask.Delay(1000);

			foreach (var confettis in _confettis)
			{
				confettis.Play();
			}

			await UniTask.Delay(3000);
		}
	}
}
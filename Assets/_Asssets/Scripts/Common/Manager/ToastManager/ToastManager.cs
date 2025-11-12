using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign;
using UnityEngine;

namespace Runtime.Manager.Toast
{
	public class ToastManager : MonoSingleton<ToastManager>
	{
		[SerializeField] private Transform _textPositionTransform;

		public async UniTaskVoid Show(string toastContent)
		{
			var toast = await ObjectPooler.GetFromPool<Toast>(PoolingType.Toast, destroyCancellationToken, _textPositionTransform);
			var positionTransform = _textPositionTransform;
			toast.Init(toastContent, positionTransform);
		}
	}
}
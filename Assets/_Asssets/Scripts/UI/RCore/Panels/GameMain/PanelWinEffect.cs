using Analytics;
using Cysharp.Threading.Tasks;
using LiveOps.Data;
using RCore.UI;
using Runtime.Audio;
using Runtime.Definition;
using Spine.Unity;
using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Runtime.UI
{
	public class PanelWinEffect : PanelController
	{
		public event Action OnAnimationFinish;

		[Header("UI References")]
		[SerializeField] private ParticleSystem[] _confettis;
		// [SerializeField] private Transform _logoTransform;
		// [SerializeField] private SkeletonGraphic _skeletonGraphic;

		protected override void OnDisable()
		{
			base.OnDisable();

			// _skeletonGraphic.AnimationState.ClearTracks();
			// _skeletonGraphic.AnimationState.SetEmptyAnimation(0, 0);
		}

		protected void OnEnable()
		{
			// _skeletonGraphic.AnimationState.ClearTracks();
			// _skeletonGraphic.AnimationState.SetAnimation(0, "Appear_2", false);
			// AnimateLogo().Forget();

			AudioController.Instance.PlaySoundEffect(Constant.AUDIO_WIN_GAME);

			WaitForAnimationAsync().Forget();
		}

		protected override void AfterShowing()
		{
			base.AfterShowing();

			Tracker.LogEventScreenTracking(new Feature_SCREEN_TRACK
			{
				screen_name = $"Win Level {DataManager.Instance.PlayerData.level - 1}",
			});
		}

		private async UniTask WaitForAnimationAsync()
		{
			await UniTask.Delay(1700);

			foreach (var confettis in _confettis)
			{
				confettis.Play();
			}

			await UniTask.Delay(3000);

			BtnBack_Pressed();

			OnAnimationFinish?.Invoke();

			OnAnimationFinish = null;
		}
	}
}
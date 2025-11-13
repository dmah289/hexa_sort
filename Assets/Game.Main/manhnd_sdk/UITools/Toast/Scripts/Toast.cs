using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using TMPro;
using UnityEngine;

namespace manhnd_sdk.UITools.Toast
{
	public class Toast : MonoBehaviour
	{
		[SerializeField]
		protected TextMeshProUGUI _displayText;

		public void Init(string toastContent, Transform positionTransform)
		{
			var canvasGroup = GetComponent<CanvasGroup>();
			if (canvasGroup)
				canvasGroup.alpha = 1;
			transform.SetParent(positionTransform);
			transform.position = positionTransform.position;
			transform.localScale = Vector3.one;
			gameObject.SetActive(true);
			transform.SetAsLastSibling();
			_displayText.text = toastContent;
		}

		public void Finishing()
		{
			ObjectPooler.ReturnToPool(PoolingType.Toast, this, destroyCancellationToken);
		}
	}
}

using UnityEngine;
using TMPro;

public class FrameRateText : MonoBehaviour
{

	//定数////////////////////////////////////////////////////////////////////



	//Pubic Field////////////////////////////////////////////////////////////////////

	[SerializeField]
	TextMeshProUGUI display ;

	[SerializeField, Range(0.1f, 2f)]
	float sampleDuration = 1f;

	public enum DisplayMode {FPS, MS}

	[SerializeField]
	DisplayMode displayMode = DisplayMode.FPS; // こうすることでFPS or MSをエディタから変更できる

	

	//Private Field////////////////////////////////////////////////////////////////////

	int frames;
	float duration, bestDuration = float.MaxValue, worstDuration;


	//仮想メソッド定義////////////////////////////////////////////////////////////////////



	//Unityメソッド定義////////////////////////////////////////////////////////////////////

	private void Update()
	{
		float frameDuration = Time.unscaledDeltaTime;
		frames += 1;
		duration += frameDuration;
		if (frameDuration < bestDuration)
		{
			bestDuration = frameDuration;
		}

		if(frameDuration > worstDuration)
		{
			worstDuration = frameDuration;
		}
		
		if (duration >= sampleDuration)
		{
			if (displayMode == DisplayMode.FPS){
				display.SetText("FPS\n{0:0}\n{1:0}\n{2:0}", 1.0f / frameDuration
				, 1f/bestDuration, 1f/worstDuration);
			}
			else {
				display.SetText("MS\n{0:0}\n{1:0}\n{2:0}", bestDuration * 1000f
				, duration/frames * 1000f, worstDuration * 1000f);
			}

			frames = 0;
			duration = 0f;
			bestDuration = float.MaxValue;
			worstDuration = 0f;
		}

	}

	//publicメソッド定義////////////////////////////////////////////////////////////////////



	//privateメソッド定義////////////////////////////////////////////////////////////////////


}

/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Playables
{
	using UnityEngine.Playables;
	
	public class RhythmMixerBehaviour : PlayableBehaviour
	{
		//This will iterate through the clips and forcebly call their ProcessFrame,
		//providing more data (like the current global Timeline time) so that the clip can handle bullets before and past its actual duration
		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			double timelineCurrentTime = (playable.GetGraph().GetResolver() as PlayableDirector).time;
			int inputCount = playable.GetInputCount();

			for (int i = 0; i < inputCount; i++)
			{
				ScriptPlayable<RhythmBehaviour> inputPlayable = (ScriptPlayable<RhythmBehaviour>)playable.GetInput(i);
				RhythmBehaviour input = inputPlayable.GetBehaviour();
				input.MixerProcessFrame(inputPlayable, info, playerData, timelineCurrentTime);
			}
		}
	}
}

using LevelEditor.LevelData;

namespace HexaSort.UI.Gameplay.Goals
{
    public class OtherTrackerPanel : GoalTrackerPanel
    {
        public override void SetUp(LevelGoalData goalData)
        {
            counter.text = $"{goalData.targetAmount}";
        }
    }
}
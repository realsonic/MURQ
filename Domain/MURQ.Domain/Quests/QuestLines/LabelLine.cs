using MURQ.Domain.Quests.Locations;

namespace MURQ.Domain.Quests.QuestLines;

public record LabelLine(Location Location) : QuestLine(Location);

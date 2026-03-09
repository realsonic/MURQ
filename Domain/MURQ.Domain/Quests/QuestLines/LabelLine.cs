using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.Quests.QuestLines;

public record LabelLine(string Label, Location Location) : QuestLine(Location);

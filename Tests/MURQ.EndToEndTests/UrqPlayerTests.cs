using FluentAssertions;

using Moq;

using MURQ.Application.Interfaces;
using MURQ.Application.Services;
using MURQ.Infrastructure.QuestLoaders;

namespace MURQ.EndToEndTests;
public class UrqPlayerTests
{
    [Fact(DisplayName = "Qst-файл с двумя p проигрывается")]
    public async Task Two_p_qst_produces_one_line()
    {
        // Arrange
        IQuestLoader questLoader = new FileQuestLoader(@"Quests/Two_P.qst");
        UserInterfaceMock userInterfaceMock = new([-1]);
        string header = UserInterfaceMock.GetHeader("Two_P.qst");
        string footer = UserInterfaceMock.GetFooter();
        IVersionProvider versionProvider = new Mock<IVersionProvider>().Object;
        UrqPlayer urqPlayer = new(questLoader, userInterfaceMock, versionProvider);

        // Act
        await urqPlayer.Run(default);

        // Assert
        userInterfaceMock.Output.ToString().Should().Be(header + "Привет, мир!" + footer);
    }
}

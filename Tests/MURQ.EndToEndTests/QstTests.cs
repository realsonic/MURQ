using FluentAssertions;

using MURQ.Application;
using MURQ.Domain.Games;
using MURQ.Domain.Quests;

namespace MURQ.EndToEndTests;

public class QstTests
{
    [Fact(DisplayName = "Qst-���� �� ���� p ���������� ���� ������ ������")]
    public async Task TwoP_qst_shows_one_line()
    {
        // Arrange
        string questSource = await File.ReadAllTextAsync(@"Quests/TwoP.qst");
        UrqLoader urqLoader = new(questSource);
        Quest quest = urqLoader.LoadQuest();
        Game sut = new(quest); 

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("������, ���!");
    }
}
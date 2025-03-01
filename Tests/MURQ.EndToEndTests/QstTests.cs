using FluentAssertions;

using MURQ.Application;
using MURQ.Domain.Games;
using MURQ.Domain.Quests;

namespace MURQ.EndToEndTests;

public class QstTests
{
    [Fact(DisplayName = "qst-файл из двух p отображает одну строку текста")]
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
        sut.CurrentLocation.Text.Should().Be("Привет, мир!");
    }

    [Fact(DisplayName = "qst-файл из pln отображает одну строку текста с новой строкой на конце")]
    public async Task Pln_shows_one_line_with_new_line_on_end()
    {
        // Arrange
        string questSource = await File.ReadAllTextAsync(@"Quests/Pln.qst");
        UrqLoader urqLoader = new(questSource);
        Quest quest = urqLoader.LoadQuest();
        Game sut = new(quest); 

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Привет, мир!\n");
    }
}
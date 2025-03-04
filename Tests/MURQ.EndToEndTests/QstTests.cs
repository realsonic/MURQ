using FluentAssertions;

using MURQ.Application;
using MURQ.Domain.Games;
using MURQ.Domain.Quests;

namespace MURQ.EndToEndTests;

public class QstTests
{
    [Fact(DisplayName = "Два p отображают одну строку текста")]
    public async Task Two_p_shows_one_line()
    {
        // Arrange
        string questSource = await File.ReadAllTextAsync(@"Quests/Two_P.qst");
        UrqLoader urqLoader = new(questSource);
        Quest quest = urqLoader.LoadQuest();
        Game sut = new(quest); 

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Привет, мир!");
    }

    [Fact(DisplayName = "p без пробела не генерирует текста")]
    public async Task P_with_no_space_gives_no_text()
    {
        // Arrange
        string questSource = await File.ReadAllTextAsync(@"Quests/P_wo_text.qst");
        UrqLoader urqLoader = new(questSource);
        Quest quest = urqLoader.LoadQuest();
        Game sut = new(quest); 

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Привет, мир!");
    }

    [Fact(DisplayName = "pln отображает одну строку текста с новой строкой в конце")]
    public async Task Pln_shows_one_line_with_new_line()
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
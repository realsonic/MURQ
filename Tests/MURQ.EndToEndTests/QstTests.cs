using FluentAssertions;

using MURQ.Application;
using MURQ.Domain.Games;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

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

    [Fact(DisplayName = "Метки загружаются")]
    public async Task Labels_loaded()
    {
        // Arrange
        string questSource = await File.ReadAllTextAsync(@"Quests/Labels.qst");
        UrqLoader urqLoader = new(questSource);
        Quest quest = urqLoader.LoadQuest();
        Game sut = new(quest);

        // Act
        sut.Start();

        // Assert
        List<string> labelList = [.. sut.Quest.Statements.OfType<LabelStatement>().Select(labelStatement => labelStatement.Label)];
        labelList.Should().BeEquivalentTo([
            "Метка1", 
            "Метка2", 
            "Метка3"
        ]);
    }

    [Fact(DisplayName = "Кнопка переходит на эту же локацию")]
    public async Task Button_works()
    {
        // Arrange
        string questSource = await File.ReadAllTextAsync(@"Quests/Location_and_btn.qst");
        UrqLoader urqLoader = new(questSource);
        Quest quest = urqLoader.LoadQuest();
        Game sut = new(quest);

        // Act
        sut.Start();
        sut.CurrentLocation.Buttons[0].Press();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Привет, это тест кнопки!\n");
        sut.CurrentLocation.Buttons[0].Caption.Should().Be("Нажми меня, чтобы попасть снова сюда.");
    }

    [Fact(DisplayName = "Из первой локации можно перейти во вторую и в третью")]
    public async Task Can_go_from_first_location_to_second_and_third()
    {
        // Arrange
        string questSource = await File.ReadAllTextAsync(@"Quests/Three_locations.qst");
        UrqLoader urqLoader = new(questSource);
        Quest quest = urqLoader.LoadQuest();
        Game sut = new(quest);

        // Act
        sut.Start();
        sut.CurrentLocation.Buttons[0].Press();
        sut.CurrentLocation.Buttons[0].Press();

        // Assert
        sut.CurrentLocation.Name.Should().Be("Далеко");
        sut.CurrentLocation.Text.Should().Be("Вы попали совсем далеко!\n");
        sut.CurrentLocation.Buttons[0].Caption.Should().Be("Назад!");
    }

    [Fact(DisplayName = "Комментарии игнорируются")]
    public async Task Comments_ignored()
    {
        // Arrange
        string questSource = await File.ReadAllTextAsync(@"Quests/Pln_and_single_comments_on_separate_lines.qst");
        UrqLoader urqLoader = new(questSource);
        Quest quest = urqLoader.LoadQuest();
        Game sut = new(quest);

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Привет, мир!\n");
    }
}
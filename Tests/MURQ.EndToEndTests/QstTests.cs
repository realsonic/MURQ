using FluentAssertions;

using MURQ.Application.UrqLoaders;
using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

namespace MURQ.EndToEndTests;

public class QstTests
{
    [Fact(DisplayName = "Два p отображают одну строку текста")]
    public async Task Two_p_shows_one_line()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Two_P.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Привет, мир!");
    }

    [Fact(DisplayName = "p без пробела не генерирует текста")]
    public async Task P_with_no_space_gives_no_text()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/P_wo_text.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Привет, мир!");
    }

    [Fact(DisplayName = "pln отображает одну строку текста с новой строкой в конце")]
    public async Task Pln_shows_one_line_with_new_line()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Pln.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Привет, мир!\n");
    }

    [Fact(DisplayName = "Метки загружаются")]
    public async Task Labels_loaded()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Labels.qst");

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
        Game sut = await LoadQuestIntoGame(@"Quests/Location_and_btn.qst");

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
        Game sut = await LoadQuestIntoGame(@"Quests/Three_locations.qst");

        // Act
        sut.Start();
        sut.CurrentLocation.Buttons[0].Press();
        sut.CurrentLocation.Buttons[0].Press();

        // Assert
        sut.CurrentLocation.Name.Should().Be("Далеко");
        sut.CurrentLocation.Text.Should().Be("Вы попали совсем далеко!\n");
        sut.CurrentLocation.Buttons[0].Caption.Should().Be("Назад!");
    }

    [Fact(DisplayName = "Однострочные комментарии в отдельной строке игнорируются")]
    public async Task Comments_ignored()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Pln_and_single_comments_on_separate_lines.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Привет, мир!\n");
    }

    [Fact(DisplayName = "Однострочные комментарии игнорируются")]
    public async Task Comments_everywhere_ignored()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Pln_and_single_comments_everywhere.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Name.Should().Be("1");
        sut.CurrentLocation.Text.Should().Be("Привет, мир! \n");
        sut.CurrentLocation.Buttons[0].Caption.Should().Be("Повторить!");
        (sut.Quest.Statements[2] as ButtonStatement)!.LabelStatement!.Label.Should().Be("1");
    }

    [Fact(DisplayName = "cls очищает текст и кнопки локации и вызывает событие OnScreenCleared")]
    public async Task Cls_clears_text_and_buttons_and_fires_OnScreenCleared()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Loc_Cls.qst");
        bool eventWasFired = false;
        sut.OnScreenCleared += () => eventWasFired = true;

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().BeEmpty();
        sut.CurrentLocation.Buttons.Should().BeEmpty();
        eventWasFired.Should().BeTrue();
    }

    [Fact(DisplayName = "Переменным присваиваются числа")]
    public async Task Number_set_to_variable()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Numeric_var.qst");

        // Act
        sut.Start();

        // Assert
        var variable1 = sut.GetVariable("bT");
        variable1!.Value.As<NumberValue>().Value.Should().Be(4);
        var variable2 = sut.GetVariable("_under");
        variable2!.Value.As<NumberValue>().Value.Should().Be(5);
        var variable3 = sut.GetVariable("und_er");
        variable3!.Value.As<NumberValue>().Value.Should().Be(10);
    }

    [Fact(DisplayName = "if проверяет значение числа")]
    public async Task If_checks_var()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/If_a_4_then_pln.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Всего хорошего!\n");
    }

    [Fact(DisplayName = "Из меток-дублей выбирается первая")]
    public async Task First_label_double_wins()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Duplicate_labels.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Buttons[0].Press();
        sut.CurrentLocation.Buttons[0].Press();
        sut.CurrentLocation.Text.Should().Be("Метка1\n");
    }

    [Fact(DisplayName = "Однострочные комментарии вырезаются из кнопок")]
    public async Task Singleline_comments_cut_from_btn()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Comments_in_btn.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Тест комментариев \n");
        IEnumerable<string> buttonCaptions = sut.CurrentLocation.Buttons.Select(button => button.Caption);
        buttonCaptions.Should().BeEquivalentTo([
            "тест кнопки без комментария",
            ""
        ]);
    }

    [Fact(DisplayName = "Многострочные комментарии вырезаются")]
    public async Task Multiline_comments_cut()
    {

        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Multiline_comments.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("1&pln 2");
    }

    [Fact(DisplayName = "Строковая переменная сравнивается со строковым литералом")]
    public async Task String_variable_related_to_string_literal()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/String_variables.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("Привет!");
    }

    [Fact(DisplayName = "Системные переменные current_loc и previous_loc работают")]
    public async Task Current_loc_and_previous_loc_vars_works()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Prev_and_curr_locs.qst");

        // Act
        sut.Start();

        // Act Ⅰ
        sut.CurrentLocation.Buttons.Single(button => button.Caption == "В лес").Press();
        sut.CurrentLocation.Buttons.Single(button => button.Caption == "В пещеру").Press();
        // Assert Ⅰ
        sut.CurrentLocation.Text.Should().Be("Вы пришли из лесу в пещеру.");

        // Act Ⅱ        
        sut.CurrentLocation.Buttons.Single(button => button.Caption == "На озеро").Press();
        sut.CurrentLocation.Buttons.Single(button => button.Caption == "На поляну").Press();
        // Assert Ⅱ
        sut.CurrentLocation.Text.Should().Be("Вы пришли с озера на поляну.");
    }

    [Fact(DisplayName = "Слэш без звезды не вырезается как многострочный комментарий")]
    public async Task Slash_wo_asterisk_not_cut()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/slash_in_pln.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("  |\n/ |\n");
    }

    [Fact(DisplayName = "При проходе дублирующей метки не происходит переброс на оригинальную метку")]
    public async Task Running_double_label_doesnt_jump_to_original()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Double_sniff.qst");

        // Act
        sut.Start();
        sut.CurrentLocation.Buttons.Single().Press();

        // Assert
        sut.CurrentLocation.Text.Should().Be("\nя опустил морду - четкий запах манит мен¤ вперед.\n");
    }

    [Fact(DisplayName = "Подстановка с переменной вставляет значение")]
    public async Task Interpolated_var_puts_value()
    {
        // Arrange
        Game sut = await LoadQuestIntoGame(@"Quests/Interpolated_var.qst");

        // Act
        sut.Start();

        // Assert
        sut.CurrentLocation.Text.Should().Be("У меня 4 рубля!\n");
    }

    private static async Task<Game> LoadQuestIntoGame(string filePath)
    {
        string questSource = await File.ReadAllTextAsync(filePath);
        UrqLoader urqLoader = new();
        Quest quest = urqLoader.LoadQuest(questSource);
        Game game = new(quest);
        return game;
    }
}
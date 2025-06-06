﻿using MURQ.Domain.Quests;
using MURQ.Domain.Quests.Statements;

namespace MURQ.Demo._2;

internal class PoiskeQuestBuilder
{
    public static Quest BuildPoiskeQuest() => new PoiskeQuestBuilder().Build();

    private Quest Build()
    {
        CreateLabels();

        return new Quest([
            Label(1),
            Pln("Однажды вечером"),
            Pln("Сидел на стуле"),
            Pln("Сакэ-васаби..."),
            Btn(2, "Далее"),
            End(),

            Label(2),
            Pln("Поиске."),
            Pln("Квест на хокку и дуэль."),
            Pln("2 из 1, 7 из 2 и 1 из 3"),
            Btn(3, "Начать игру"),
            End(),

            Label(3),
            Pln("Геннадий Йцукенг выпил кофе."),
            Pln("Все началось в субботу."),
            Pln("Геннадий спал."),
            Pln("Потом проснулся, и решил, что спать не надо."),
            Pln("Геннадий не любит спать, когда спать не надо."),
            Pln("Потому он и не спал."),
            Btn(4, "Далее"),
            End(),

            Label(4),
            Pln("Геннадий Йцукенг выпил кофе."),
            Pln("После чего зазвонил телефон."),
            Pln("Геннадий подумал взять трубку"),
            Pln("Или не брать трубку?"),
            Btn(5, "Первое"),
            Btn(6, "Не брать"),
            End(),

            Label(5),
            Pln("Геннадий Йцукенг выпил кофе."),
            Pln("В трубке был голос"),
            Pln("Геннадий понял, что голос говорит."),
            Pln("Геннадий послушал голос и положил трубку."),
            Pln("Геннадий решил почистить зубы."),
            Pln("Или нет?"),
            Btn(7, "Да"),
            Btn(8, "Не чистить"),
            End(),

            Label(6),
            Pln("Геннадий Йцукенг выпил кофе."),
            Pln("Геннадий не взял трубку."),
            Pln("Геннадий решил почистить зубы."),
            Pln("Или не чистить?"),
            Btn(7, "Попытать счастья"),
            Btn(8, "Нет"),
            End(),

            Label(7),
            Pln("Геннадий Йцукенг выпил кофе."),
            Pln("Геннадий пошел в ванную, но заперто"),
            Pln("Ключ от ванной в кармане"),
            Pln("Геннадий хлоп! По карману"),
            Pln("Ключ в руках"),
            Btn(9, "Открыть дверь"),
            End(),

            Label(8),
            Pln("Геннадий Йцукенг выпил кофе."),
            Pln("У Геннадия на кухне таракан"),
            Pln("Геннадий топ! на таракана."),
            Pln("Таракан - шмыг!"),
            Pln("Геннадий рад."),
            Btn(10, "Далее"),
            End(),

            Label(9),
            Pln("Геннадий Йцукенг выпил кофе."),
            Pln("В ванной жена Геннадия"),
            Pln("Жена зла"),
            Pln("Геннадий пошел на кухню"),
            Btn(8, "И"),
            End(),

            Label(10),
            Pln("Геннадий Йцукенг выпил кофе."),
            Pln("Опять телефонный звонок"),
            Pln("Геннадий взял."),
            Btn(11, "И что же дальше?"),
            End(),

            Label(11),
            Pln("Геннадий Йцукенг выпил кофе."),
            Pln("И тут-то начинается самое интересное. Несмотря на то, что, сидя на диване, невозможно летать по воздуху на космическом истребителе, Геннадий внезапно решил посмотреть, что же идет по телевизору в данный момент. По телевизору шла интереснейшая передача наподобие той, что идет каждый день по телевизору. Выступал мужчина с шикарной прической и двумя глазами. Мужчина рассказывал что-то о таком, чего, в принципе, никогда нельзя рассказывать. Но он рассказывал, потому что мужчине было абсолютно все равно, что о нем подумает человечество. Геннадий смотрел на мужчину, и казалось ему, будто сердце его замирает. Будто сам Геннадий и есть тот телевизор, та сущность, что превалирует над остальными человеческими чувствами."),
            Pln("Жена Геннадия сидела неподалеку и смотрела, смотрела чисто и ясно на лазуревую синь небесного светила, широко растворяющуюся в бриллиантовой дали всего сущего. Ей тоже было все равно, что о ней подумают.\r\n"),
            Pln("Была зима, три часа дня..."),
            Btn(12, "Конец"),
            End(),

            Label(12),
            Pln("Игру делал ZombX"),
            Pln("Всем спасибо."),
            Pln("Кто скажет, что игра плоха, тот дурак."),
            Btn(1, "В саааааамое начало"),
            End()
        ]);
    }

    private void CreateLabels()
    {
        for (var i = 1; i <= 12; i++)
        {
            LabelDictionary[i] = new LabelStatement { Label = i.ToString() };
        }
    }

    private LabelStatement Label(int label) => LabelDictionary[label];

    private static PrintStatement Pln(string text) => new() { Text = text + "\n" };

    private ButtonStatement Btn(int label, string caption) =>
        new() { Caption = caption, LabelStatement = LabelDictionary[label] };

    private static EndStatement End() => new();

    private Dictionary<int, LabelStatement> LabelDictionary { get; } = [];
}
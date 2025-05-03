Архитектура MURQ.Console.

# Диаграмма вызовов
```plantuml
title Основной игровой цикл плеера

[-> IUrqPlayer ++ : Играть
IUrqPlayer -> IQuestLoader ++: Загрузить квест
return Квест

IUrqPlayer -> Game **: Создать игру
IUrqPlayer -> Game: Запустить игру

loop Пока игрок не выбрал выход из игры
    IUrqPlayer -> Game ++ : Получить текущий вид
    return Текущий вид (текст, кнопки)

    IUrqPlayer -> IUserInterface: Отрисовать текущий вид
    IUrqPlayer -> IUserInterface ++: Запросить выбор игрока
    return Выбор игрока

    alt Игрок выбрал кнопку локации
        IUrqPlayer -> Game: Нажать выбранную кнопку
    else Игрок выбрал выход из плеера
        IUrqPlayer ->x[: Игра закончена
    end
end

box Application
    participant IUrqPlayer
    participant IQuestLoader
    participant IUserInterface
end box
box Domain
    participant Game
end box
```

```mermaid
sequenceDiagram
    title: Основной игровой цикл плеера

    MURQ.Console ->>+ IUrqPlayer: Играть

    IUrqPlayer ->> IQuestLoader: Загрузить квест
    IQuestLoader ->> IUrqPlayer: Квест

    create participant Game

    IUrqPlayer ->> Game: Создать игру
    IUrqPlayer ->> Game: Запустить игру

    loop Пока игрок не выбрал выход из игры
        IUrqPlayer ->>+ Game: Получить текущий вид
        Game ->>- IUrqPlayer: Текущий вид (текст, кнопки)
        IUrqPlayer ->> IUserInterface: Отрисовать текущий вид
        IUrqPlayer ->>+ IUserInterface: Запросить выбор игрока
        IUserInterface ->>- IUrqPlayer : Выбор игрока
        alt Игрок выбрал кнопку локации
            IUrqPlayer ->> Game: Нажать выбранную кнопку
        else Игрок выбрал выход из плеера
            IUrqPlayer ->>+ MURQ.Console: Игра закончена
        end
    end
    
    box Domain
        participant Game
    end
    box Application
        participant IUrqPlayer
        participant IQuestLoader
        participant IUserInterface
    end
    %% box Presentation
    %%     participant MURQ.Console
    %% end
```

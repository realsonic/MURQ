﻿# История изменений MURQ.Console

## _Версия в разработке (0.3.0)_
Назначение целочисленных переменных, их проверки и многострочные комментарии.
### Добавлено
- Поддержка присвоения переменной целого числа (`a=4`).
- Поддержка инструкции `if`-`then` с проверкой числа на значение (`if a=4 then pln Всего хорошего!`).
- Многострочные комментарии вырезаются до загрузки квеста.
### Исправлено
- Загрузка квеста не падает, если в квесте есть дублирующие метки.
- Однострочные комментарии вырезаются до загрузки квеста.

## 0.2.0
Перенаправление ввода-вывода и перезагрузка квеста.
### Добавлено
- Добавлена поддержка запуска MURQ.Console в режиме перенаправления ввода-ввывода (конвейер).
- Возможность перезагрузки квеста комбинацией `Ctrl`+`R`.
### Исправлено
- Исправлена ошибка, из-за которой приложение вылетало, когда в локации не было текста.

## 0.1.1
### Исправлено
- Исправлена ошибка, из-за которой не очищался экран по команде `cls`.

## 0.1.0
Пилотная версия, позволяет запускать простейшие URQ-игры.
### Добавлено
- Базовая реализация URQL-команд:
	- метки
	- `p`/`pln`
	- `btn`
	- `end`
	- `cls`
	- однострочные комментарии
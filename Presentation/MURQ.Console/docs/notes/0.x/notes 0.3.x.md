﻿# Заметки к выпускам MURQ.Console 0.3.x

**MURQ.Console** - консольное приложение для проигрывания URQ-файлов (квестов), написанных на URQL.  

## Цель выпуска
### Основные изменения
- Поддержка присвоения переменной целого числа (`a=4`).
- Поддержка инструкции `if`-`then` с проверкой числа на значение (`if a=4 then pln Всего хорошего!`).
### Дополнительные
- Поддержка многострочных комментариев. Все комментарии (однострочные и многострочные) вырезаются до загрузки квеста.
- Загрузка квеста не падает, если в квесте есть дублирующие метки.
- Нажатая кнопка выводится на экран в виде: `> [1] В лес`.
- При нормальном выходе (по `Ctrl`+`Q`) курсор снова становится видимым.

## Параметры запуска
Приложение принимает один обязательный параметр - путь к qst-файлу в кодировке `Windows-1251`. Если параметр не задан, приложение выдаёт ошибку и останавливает работу.

## Возможности (фичи)
- Запуск простейших URQ-квестов.
- Выбор варианта действий (кнопки) через ввод цифры (возможен выбор только вариантов `1`-`9`).
- Перезагрузка квеста комбинацией `Ctrl`+`R` (или `r`/`R` в режиме перенаправленного ввода).
- Выход из приложения комбинацией `Ctrl`+`Q` (или `q`/`Q` или конец ввода в режиме перенаправленного ввода).

### Поддерживаемый URQL
| Команда URQL                       | Пример             | Заметка
| ---------------------------------- | ------------------ | -----------
| **Метка**                          | `:метка`           | Названием метки считается текст без начальных и конечных пробелов и табуляции.
| **Вывод текста**                   | `pln Привет, мир!` | Допускаются `p` и `pln`.
| **Кнопка**                         | `btn 1,В начало`   | Между `btn` и меткой допускается пробел или табуляция. Метка так же берётся без начальных и конечных пробелов и табуляции.
| **Конец локации**                  | `end`
| **Присвоение значения переменной** | `a=4` | _Пока поддерживаются только целые числа._
| **Условие**                        | `if a=4 then pln Всего хорошего!` | _Пока поддерживаются только выражения проверки на равенство переменных и числовых значений, а также только одна команда для выполнения._
| **Очистка экрана**                 | `cls`              | **Не работает в режиме перенаправленного вывода.** _Пока не учитывает никаких настроек через переменные._
| **Однострочный комментарий**       | `; Комментарий`      | Допускается в любой строке. Вырезается до загрузки квеста.
| **Многострочный комментарий**      | `/* Комментарий */` | Допускается в любом месте. Вырезается до загрузки квеста.

### Ограничения
- Подстановки `#$` пока не поддерживаются.
- `Common`-локации пока не поддерживаются.
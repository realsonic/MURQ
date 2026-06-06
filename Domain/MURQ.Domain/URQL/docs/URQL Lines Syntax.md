# Синтаксическая диаграмма
```plantuml
@startebnf URQL Line
!theme crt-amber
' skinparam monochrome reverse

statementLine = [ joinedStatements ];

joinedStatements = 
      joinedStatements, ?&?, statement
    | statement;

statement =
    assignVariableStatement
  | ifStatement
  | ? Print ?
  | ? Button ?
  | ? End ?
  | ? ClearScreen ?
  | ? Goto ?
  | ? Perkill ?
  | ? Pause ?;

assignVariableStatement = ? Variable ?,  ?=?, expression;

ifStatement = ? If ?, relationExpression, ? Then ?, joinedStatements, [ ? Else ?, joinedStatements ];

relationExpression = expression, ?=?, expression;

expression =
    expression, (?+? | ?-?), multiplication
  | multiplication;

multiplication =
    multiplication, (?*? | ?/?), factor
  | factor;

factor = ?(?, expression, ?)? | ?Variable? | ?Number? | ?StringLiteral?;
@endebnf
```

# Адаптация левых рекурсий
$$
A \rightarrow A\alpha \mid A\beta \mid \gamma
$$
преобразуется в:
$$
A \rightarrow \gamma R \newline
R \rightarrow \alpha R \mid \beta R \mid \epsilon
$$

## Адаптация `joinedStatements`
### Исходный нетерминал
```plantuml
@startebnf joinedStatements
!theme crt-amber

joinedStatements = 
      joinedStatements, ? & ?, statement
    | statement;
@endebnf
```

### Разбивка на элементы
| Элемент | Значение  |
|:-------:| --------- |
| $A$ | $joinedStatements$  |
| $\alpha$  | ${"\&"}, statement$ | 
| $\gamma$  | $statement$ |

### Преобразование
| Формула | Результат |
| ------- | --------- |
| $A \rightarrow \gamma R$  | $joinedStatements \rightarrow statement, joinedStatementsRest$  |
| $R \rightarrow {\alpha R} \mid {\beta R} \mid \epsilon$ | $joinedStatementsRest \rightarrow {{"\&"}, statement, joinedStatementsRest} \mid \epsilon$  |

```plantuml
@startebnf Адаптация joinedStatements
!theme crt-amber

joinedStatements = statement, joinedStatementsRest;
joinedStatementsRest = [? & ?, statement, joinedStatementsRest];
@endebnf
```

## Адаптация `expression`
### Исходный нетерминал
```plantuml
@startebnf joinedStatements
!theme crt-amber

expression =
    expression, (? + ? | ? - ?), multiplication
  | multiplication;
@endebnf
```

### Разбивка на элементы
| Элемент | Значение  |
|:-------:| --------- |
| $A$ | $expression$  |
| $\alpha$  | ${"+"}, multiplication$ |
| $\beta$ | ${"-"}, multiplication$ |
| $\gamma$  | $multiplication$ |

### Преобразование
| Формула | Результат |
| ------- | --------- |
| $A \rightarrow \gamma R$  | $expression \rightarrow multiplication, expressionRest$  |
| $R \rightarrow {\alpha R} \mid {\beta R} \mid \epsilon$ | $expressionRest \rightarrow {{"+"}, multiplication, expressionRest} \mid {{"-"}, multiplication, expressionRest} \mid \epsilon$  |

```plantuml
@startebnf joinedStatements
!theme crt-amber
expression = multiplication, expressionRest;
expressionRest = [ (? + ? | ? - ?), multiplication, expressionRest] ;
@endebnf
```

## Адаптация `multiplication`
### Исходный нетерминал
```plantuml
@startebnf joinedStatements
!theme crt-amber

multiplication =
    multiplication, (? * ? | ? / ?), factor
  | factor;
@endebnf
```

### Разбивка на элементы
| Элемент | Значение  |
|:-------:| --------- |
| $A$ | $multiplication$  |
| $\alpha$  | ${"*"}, factor$ |
| $\beta$ | ${"*"}, factor$ |
| $\gamma$  | $factor$ |

### Преобразование
| Формула | Результат |
| ------- | --------- |
| $A \rightarrow \gamma R$  | $multiplication \rightarrow factor, multiplicationRest$  |
| $R \rightarrow {\alpha R} \mid {\beta R} \mid \epsilon$ | $multiplicationRest \rightarrow {{"*"}, factor, multiplicationRest} \mid {{"/"}, factor, multiplicationRest} \mid \epsilon$  |

```plantuml
@startebnf joinedStatements
!theme crt-amber
multiplication = factor, multiplicationRest;
multiplicationRest = [ (? * ? | ? / ?) ], factor, multiplicationRest;
@endebnf
```
# Синтаксическая диаграмма
```plantuml
@startebnf URQL Line
!theme crt-amber
' skinparam monochrome reverse

title Строка кода URQL

statementLine = [ joinedStatements ];

joinedStatements = 
      joinedStatements, '&', statement
    | statement;

statement =
    assignVariableStatement
  | ifStatement
  | ?Print?
  | ?Button?
  | 'end'
  | 'cls'
  | ?Goto?
  | 'perkill'
  | ?Pause?;

assignVariableStatement = ?Variable?,  '=', expression;

ifStatement = 'if', expression, 'then', joinedStatements, [ 'else', joinedStatements ];

expression = orExpression;

orExpression =
    orExpression, 'or', andExpression
  | andExpression;

andExpression =
    andExpression, 'and', relationExpression
  | relationExpression;

relationExpression = additionExpression, [ ( '=' | '<' | '<=' | '>' | '>=' | '<>' ), additionExpression ];

additionExpression =
    additionExpression, ( '+' | '-' ), multiplicationExpression
  | multiplicationExpression;

multiplicationExpression =
    multiplicationExpression, ( '*' | "/" ), signExpression
  | signExpression;

signExpression =
    ( '+' | '-' ), signExpression
  | factorExpression;

factorExpression =
    ( '(', expression, ')' )
  | ?Variable?
  | ?Number?
  | ?StringLiteral?;
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
```plantuml
@startebnf Исходный joinedStatements
!theme crt-amber

title Исходный нетерминал

joinedStatements = 
      joinedStatements, '&', statement
    | statement;
@endebnf
```

### Разбивка на элементы
| Элемент | Значение  |
|:-------:| --------- |
| $A$ | $joinedStatements$  |
| $\alpha$  | ${'\&'}, statement$ | 
| $\gamma$  | $statement$ |

### Преобразование
| Формула | Результат |
| ------- | --------- |
| $A \rightarrow \gamma R$  | $joinedStatements \rightarrow statement, joinedStatementsRest$  |
| $R \rightarrow {\alpha R} \mid {\beta R} \mid \epsilon$ | $joinedStatementsRest \rightarrow {{'\&'}, statement, joinedStatementsRest} \mid \epsilon$  |

```plantuml
@startebnf Адаптация joinedStatements через рекурсию
!theme crt-amber

title Через рекурсию

joinedStatements = statement, joinedStatementsRest;
joinedStatementsRest = [ '&', statement, joinedStatementsRest ];
@endebnf
```

```plantuml
@startebnf Адаптация joinedStatements через цикл
!theme crt-amber

title Через цикл

joinedStatements = statement, { '&', statement };
@endebnf
```

## Адаптация `additionExpression`
```plantuml
@startebnf Исходный additionExpression
!theme crt-amber

title Исходный нетерминал

additionExpression =
    additionExpression, ('+' | '-'), multiplicationExpression
  | multiplicationExpression;
@endebnf
```

### Разбивка на элементы
| Элемент | Значение  |
|:-------:| --------- |
| $A$ | $additionExpression$  |
| $\alpha$  | ${'+'}, multiplicationExpression$ |
| $\beta$ | ${'-'}, multiplicationExpression$ |
| $\gamma$  | $multiplicationExpression$ |

### Преобразование
| Формула | Результат |
| ------- | --------- |
| $A \rightarrow \gamma R$  | $additionExpression \rightarrow multiplicationExpression, additionExpressionRest$  |
| $R \rightarrow {\alpha R} \mid {\beta R} \mid \epsilon$ | $additionExpressionRest \rightarrow {{'+'}, multiplicationExpression, additionExpressionRest} \mid {{'-'}, multiplicationExpression, additionExpressionRest} \mid \epsilon$  |

```plantuml
@startebnf Адаптация additionExpression через рекурсию
!theme crt-amber

title Через рекурсию

additionExpression = multiplicationExpression, additionExpressionRest;
additionExpressionRest = [ ('+' | '-'), multiplicationExpression, additionExpressionRest] ;
@endebnf
```

```plantuml
@startebnf Адаптация additionExpression через цикл
!theme crt-amber
title Через цикл
additionExpression = multiplicationExpression, { ('+' | '-'), multiplicationExpression };
@endebnf
```

## Адаптация `multiplicationExpression`
```plantuml
@startebnf Исходный multiplicationExpression
!theme crt-amber
title Исходный нетерминал
multiplicationExpression =
    multiplicationExpression, ( '*' | "/" ), signExpression
  | signExpression;
@endebnf
```

### Разбивка на элементы
| Элемент | Значение  |
|:-------:| --------- |
| $A$ | $multiplicationExpression$  |
| $\alpha$  | ${'*'}, signExpression$ |
| $\beta$ | ${'*'}, signExpression$ |
| $\gamma$  | $signExpression$ |

### Преобразование
| Формула | Результат |
| ------- | --------- |
| $A \rightarrow \gamma R$  | $multiplicationExpression \rightarrow signExpression, multiplicationRest$  |
| $R \rightarrow {\alpha R} \mid {\beta R} \mid \epsilon$ | $multiplicationRest \rightarrow {{'*'}, signExpression, multiplicationRest} \mid {{'/'}, signExpression, multiplicationRest} \mid \epsilon$  |

```plantuml
@startebnf Адаптация multiplicationExpression через рекурсию
!theme crt-amber
title Через рекурсию
multiplicationExpression = signExpression, multiplicationRest;
multiplicationRest = [ ( '*' | '/' ), signExpression, multiplicationRest ];
@endebnf
```

```plantuml
@startebnf Адаптация multiplicationExpression через цикл
!theme crt-amber
title Через цикл
multiplicationExpression = signExpression, { ( '*' | '/' ), signExpression };
@endebnf
```

## Адаптация `orExpression`
```plantuml
@startebnf Исходный orExpression
!theme crt-amber
title Исходный нетерминал
orExpression =
    orExpression, 'or', andExpression
  | andExpression;
@endebnf
```

### Разбивка на элементы
| Элемент | Значение  |
|:-------:| --------- |
| $A$ | $orExpression$  |
| $\alpha$  | ${'or'}, andExpression$ |
| $\gamma$  | $andExpression$ |

### Преобразование
| Формула | Результат |
| ------- | --------- |
| $A \rightarrow \gamma R$  | $andExpression \rightarrow andExpression, orExpressionRest$  |
| $R \rightarrow {\alpha R} \mid \epsilon$ | $orExpressionRest \rightarrow {{'or'}, andExpression, orExpressionRest} \mid \epsilon$  |

```plantuml
@startebnf Адаптация orExpression через рекурсию
!theme crt-amber
title Через рекурсию
orExpression = andExpression, orExpressionRest;
orExpressionRest = [ 'or', andExpression, orExpressionRest ];
@endebnf
```

```plantuml
@startebnf Адаптация orExpression через цикл
!theme crt-amber
title Через цикл
orExpression = andExpression, { 'or', andExpression };
@endebnf
```
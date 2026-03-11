# Синтаксическая диаграмма
```plantuml
@startebnf URQL Line
!theme crt-amber

statementLine = [ joinedStatements ];

joinedStatements = 
      joinedStatements, ? & ?, statement
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

assignVariableStatement = ? Variable ?,  ? Equality ? (*""=""*), valueExpression;

ifStatement = ? If ?, relationExpression, ? Then ?, joinedStatements, [ ? Else ?, joinedStatements ];

valueExpression = ? Variable ? | ? Number ? | ? StringLiteral ?;

relationExpression = valueExpression, ? Equality ? (*""=""*), valueExpression;
@endebnf
```

## Адаптация левых рекурсий
### Общая формула
$A \rightarrow A\alpha \mid A\beta \mid \gamma$
преобразуется в:
- $A \rightarrow \gamma R$
- $R \rightarrow \alpha R \mid \beta R \mid \epsilon$

### Адаптация `joinedStatements`
Исходный нетерминал:
```plantuml
@startebnf joinedStatements
!theme crt-amber

joinedStatements = 
      joinedStatements, ? & ?, statement
    | statement;
@endebnf
```

Разбивка на элементы:
- $A = joinedStatements$
- $\alpha = `\&`, statement$
- $\gamma = statement$

Преобразование:
- $A \rightarrow statement, R$
- $R \rightarrow `\&`, statement, R \mid \epsilon$

```plantuml
@startebnf Адаптация joinedStatements
!theme crt-amber

joinedStatementsAdapted = statement, joinedStatementsRest;
joinedStatementsRest = [? & ?, statement, joinedStatementsRest];
@endebnf
```
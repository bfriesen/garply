# garply

_Because, why not?_

The garply runtime is currently implemented in c#, but should be portable with relative ease to other languages and platforms.

### REPL

To start the garply REPL, run the garply executable with no parameters. You should be presented with a console that looks like this:

```
garply> 
```

#### REPL commands

Command | Description
:--- | :---
`:q` | Exit the REPL
`:c` | Clear the console
`:d` | Dump the current state of the REPL

#### Literal values

Type a literal value, and the terminal will print it out. Current supported types are 64 bit int or float, boolean, string, tuple, list, and type.

```
garply> 123
123
garply> 456.789
456.789
garply> true
true
garply> "abc"
"abc"
```

Tuples are surrounded by curly braces.

```
garply> {"abc", 123}
{"abc",123}
```

Lists are surrounded by square braces.

```
garply> [1, 2, 3]
[1,2,3]
```

Types are surrounded by angle brackets.

```
garply> <int>
<int>
garply> <float>
<float>
garply> <bool>
<bool>
garply> <string>
<string>
garply> <tuple>
<tuple>
garply> <list>
<list>
garply> <type>
<type>
```

#### Variables

Variables can contain any type of value, and can be declared as immutable or mutable. Assignment operations return the value assigned.

Mutable variables are prefixed with a `$` character, otherwise variables are declare with the same set of rules:
- The first character must be a letter.
- Any additional characters must be a letter, number, underscore (`_`), or hyphen (`-`).

```
garply> foo = 123
123
garply> foo
123
garply> foo = 456
[{"Cannot rebind to immutable variable."}]
garply> 
```

```
garply> $bar = "abc"
"abc"
garply> $bar
"abc"
garply> $bar = 123
123
garply> $bar
123
garply> 
```
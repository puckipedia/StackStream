# StackStream

StackStream is an esolang-y stack-based language with streams thrown in, because why not?

# A tiny history
After reading the exam requirements for the (simple) calculator, I found out there's nothing stopping me from making my own and having it use, e.g. rpn. I started making a small RPN language, and slowly grew it (even making a horrible implementation on my TI-84). Then, I started thinking 'what if I added a feature for functions?' and so I did. And because of the 'why not' factor, I decided to implement brainfuck, but hit a snag: I can't store the tape. So I added the streams and buffers. This is now StackStream.

# Syntax
The syntax of the language is very simple, it contains only six data types 
 (referred to as 'tokens'):

 - `number`: An arbitrary large signed integer, e.g. `42`
              Note: \`A is shorthand for `65`.
 - `symbol`: An alphanumeric string starting with ' e.g. `'asdf`
 - `method`: An alphanumeric string not starting with '. `dive`
 - `stream`: A datatype capable of reading and writing, like a FILE *, and
              possibly seeking and arbitrary reading/writing.
 - `codeblock`: A list of tokens, surrounded by "{" and "}" `{ 5 dive }`
 - `packedblock`: A token containing a list of data tokens, as a sort of list.
              "asdf" pushes a packed block containing (\`f \`d \`s \`a), etc.
number, symbol, method, and codeblock can be represented in code. It is not
 possible to represent a stream as a code token.

# The stacks
StackStream contains two stacks: the data stack, and the code stack. Both are
 capable of storing tokens, as described above.
When the interpreter is started, the code will be read in by an implementation-
 defined method, and pushed onto the code stack, in such a way that the first
 token in the file is first to be popped.
When executing one cycle, the first token is popped from the code stack, and
 based on the type, something happens. If the token is a method, it is
 executed. All other tokens are pushed onto the data stack.

The data stack also has a 'dive', whiich allows code to run at an 'offset' into
 the stack, instead of at the top. When using a 'dive', negative offsets into 
 the stack are allowed. Example:

    Starting stack: 1 2 3 4 5
    { 1 + } 3 dive # Dive 3 deep into the stack, then execute { 1 + }
    Ending stack: 1 3 3 4 5 

The representation of stacks in this document are as follows:
 `a b c` is a stack on which first a is pushed, then b, then c.

When executing a code block, its contents are pushed onto the code stack. 
 This means that code stack overflows are possible.

    { dup 1 dive { while } if drop } 'while def # Unsafe! Every time 'while' is called, another 'drop' lingers
    { dup 1 dive { drop } { while } elseif } 'while def # Safe!

# Built-in methods
StackStreams contains a list of built-in methods, as those can't be represented
 using other methods.

note: offsets are calculated after popping arguments.

## Numbers

- `(+ / - * % | &)`: a:number b:number → c:number - Calculates a (+ / - \* % | &) b.
- `(< > <= >=)`: a: number b:number → c:number - Calculates a (> < >= <=) b.
- `=`: a:number b:number → c:number - If a and b are equal, c is 1, else it is zero.

## Stack

- `dup`: a → a a - duplicates the top item on the stack.
- `drop`: a → - drops the top item.
- `dive`: a:codeblock N:number → - dives N items into the stack, and executes a. After a is done executing, dives back N items.
- `swap`: a b → b a - swaps the top two items on the stack.
- `dig`: b ... N:number → ... b - moves an item N items into the stack onto the top.
- `bury`: ... a N:number → a ... - moves an item on top of the stack N items into the stack.
- `stack-size`: → a:number - Counts the amount of items on the stack.

## Conditionals

- `if`: cc:number b:codeblock → - if cc is non-zero, executes b. Else, just pops it from the stack.
- `elseif`: cc:number b:codeblock a:codeblock → - if cc is non-zero, executes a. Else, executes b.

## Runtime

- `exec`: a:codeblock → - Executes a.
- `def`: b:codeblock a:symbol → - Defines a to be a method, which will execute b.
- `redef`: a:symbol b:symbol → Defines b to point towards the same method as a.
- `assert`: a:number → - If a is zero, errors

## Metaprogramming

- `parse`: a:packedblock → b - Parses the string contained in a into a token.
- `codeblock-make`: a:packedblock → b:codeblock - Turns a into a codeblock, with the top of the stack being the first executed token.
- `codeblock-pack`: a:codeblock → b:packedblock - Turns a into a packedblock, with the top of the stack being the first executed token.

note: `codeblock-make` and `codeblock-pack` can also take a symbol refering to a previously `def`-ed method.

## Packed

- `packed-make`: ... N:number → a:packedblock - Packs the top N items in the stack into a packed block.
- `packed-unmake`: a:packedblock → ... N:number - Unpacks the packed block, and pushes the size of it on top.
- `packed-size`: a:packedblock → N:number - pushes the amount of items inside the packed block.
- `packed-reverse`: a:packedblock → b:packedblock - reverses the packedblock on top of the stack.
- `packed-concat`: a:packedblock b:packedblock → c:packedblock - concatenates b at the end (bottom) of a
- `packed-set`: a:packedblock b:number c → d:packedblock - sets the item at b to be c
- `packed-get`: a:packedblock b:number → c - gets the specific value from the packed.
- `packed-new`: N:number → b:packedblock - makes a new packed block, filled with N placeholders
- `packed-resize`: a:packedblock N:number → b:packedblock - Resizes the packed block to specified size. 

## Streams

- `stream-stdio`: → a:stream - pushes a stream referencing stdin / stdout to the stack.
- `stream-read`: a:stream → b:number - reads of the top stream, and returns the value.
- `stream-write`: a:stream b:number → - writes b onto stream a.
- `stream-tell`: a:stream → b:number - gets the location of the cursor in the stream, if not possible errors.
- `stream-seek`: a:stream b:number → - sets the location of the cursor in the stream, if not possible errors.
- `stream-eof?`: a:stream → b:number - if stream a hit eof, pushes 1, else 0

- `buffer-new`: → a:stream - Creates a new empty buffer, with pointer to location 0.
- `buffer-write`: a:stream b:number c:number → - Writes c into location b of the stream.
- `buffer-read`: a:stream b:number → c:number - Reads the data at location b of the stream.

Depending on the interpreter, `stream-tcp`, `file-read` and `file-write` might be available:

- `stream-tcp`: a:packedblock b:number → c:stream - connects to the host represented by a, and port b.
- `file-read`: a:packedblock → b:stream - opens the file represented by a, read-only.
- `file-write`: a:packedblock → b:stream - opens the file represented by a, read-write.

# Convenience methods
Some convenience methods are presented, implemented in StackStream itself:

`while`: a:codeblock → - Executes code block a, and checks the top value of the stack afterwards. If non-zero, loop. (do { } while())

    { dup 1 dive swap { drop } { while } elseif } 'while def

`compare`: a:number b:number → a c:number - Compares a to b, and pushes the result value, while keeping value a intact.

    { swap dup 2 dig = } 'compare def

`dig'`: a ... N:number → a ... a - Digs up an item, but keeps a duplicate of it on its original position

    { dup 1 + { dup } swap dive dig } 'dig' def

`repeat`: a:codeblock N:number → ... - Executes a N times.

    { dup { drop drop } { 1 - swap dup 2 dive swap repeat } elseif } 'repeat def

`stack-check`: a:codeblock → - Executes a, but makes sure the stack doesn't change size.

    { stack-size swap 1 dive stack-size = assert } 'stack-check def

`dropn`: ... N:number → - removes N items from the top of the stack.

    { { drop } swap repeat } 'dropn def

# Example code
`cat`: (Only gets stdinout once)

    stdinout { dup dup dup stream-read stream-wriet stream-eof? not } while

Note: any code might be incorrect.
(that's a feature, not a bug)

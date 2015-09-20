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
              "asdf" pushes a packed block containing (`f `d `s `a 5), etc.
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

- `(+ / - * % | &)`: a:number b:number → c:number - Calculates a (+ / - \* % | &) b.
- `(< > <= >=)`: a: number b:number → c:number - Calculates a (> < >= <=) b.
- `=`: a:number b:number → c:number - If a and b are equal, c is 1, else it is zero.

- `dup`: a → a a - duplicates the top item on the stack.
- `drop`: a → - drops the top item.
- `dive`: a:codeblock N:number → - dives N items into the stack, and executes a. After a is done executing, dives back N items.
- `dig`: b ... N:number → ... b - moves an item N items into the stack onto the top.
- `bury`: ... a N:number → a ... - moves an item on top of the stack N items into the stack.
- `count-stack`: → a:number - Counts the amount of items on the stack.
- `swap`: a b → b a - swaps the top two items on the stack.

- `if`: cc:number b:codeblock → - if cc is non-zero, executes b. Else, just pops it from the stack.
- `elseif`: cc:number b:codeblock a:codeblock → - if cc is non-zero, executes a. Else, executes b.

- `exec`: a:codeblock → - Executes a.
- `def`: b:codeblock a:symbol → - Defines a to be a method, which will execute b.
- `assert`: a:number → - If a is zero, errors
- `to-codeblock`: a:packedblock → b:codeblock - Turns a into a codeblock, with the top of the stack being the first executed token.
- `from-codeblock`: a:codeblock → b:packedblock - Turns a into a packedblock, with the top of the stack being the first executed token.
- `parse`: a:packedblock → b - Parses the string contained in a into a token.

- `pack`: ... N:number → a:packedblock - Packs the top N items in the stack into a packed block.
- `unpack`: a:packedblock → ... N:number - Unpacks the packed block, and pushes the size of it on top.
- `count-packed`: a:packedblock → N:number - pushes the amount of items inside the packed block.

- `stdinout`: → a:stream - pushes a stream referencing stdin / stdout to the stack.
- `read-stream`: a:stream → b:number - reads of the top stream, and returns the value.
- `write-stream`: a:stream b:number → - writes b onto stream a.
- `tell-stream`: a:stream → b:number - gets the location of the cursor in the stream, if not possible errors.
- `seek-stream`: a:stream b:number → - sets the location of the cursor in the stream, if not possible errors.
- `eof-stream`: a:stream → b:number - if stream a hit eof, pushes 1, else 0

- `new-buffer`: → a:stream - Creates a new empty buffer, with pointer to location 0.
- `write-buffer`: a:stream b:number c:number → - Writes c into location b of the stream.
- `read-buffer`: a:stream b:number → c:number - Reads the data at location b of the stream.

Depending on the interpreter, `new-tcpstream`, `read-file` and `write-file` might be available:

- `new-tcpstream`: a:packedblock b:number → c:stream - connects to the host represented by a, and port b.
- `read-file`: a:packedblock → b:stream - opens the file represented by a, read-only.
- `write-file`: a:packedblock → b:stream - opens the file represented by a, read-write.

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
    { count-stack swap 1 dive count-stack = assert } 'stack-check def

`dropn`: ... N:number → - removes N items from the top of the stack.
    { { drop } swap repeat } 'dropn def
    
Some other convenience methods for working with packed blocks:

`concat-packed`: a:packedblock b:packedblock → c:packedblock - concatenates b at the end (bottom) of a
    { swap { unpack } 1 dive swap { unpack } 1 dive + pack } 'concat-packed def

`replace-packed`: a:packedblock b:number c → d:packedblock - sets the item at b to be c
    { { unpack } 2 dive swap dup 3 + { drop } swap dive 1 + bury pack } 'replace-packed def

`dig'-packed`: a:packedblock b:number → c - runs dig' inside the packed block, but keeps the original entry.
    { { unpack } 1 dive 1 + dig { 1 - dropn } 1 dive } 'dig'-packed def

`new-packed`: N:number → b:packedblock - makes a new packed block, filled with N placeholders
    { dup { { 'uninitialised-element } swap repeat } 1 dive pack } 'new-packed def

`shrink-packed`: a:packedblock N:number → b:packedblock - Removes the bottom N items from the packed block. 
    { { unpack } 1 dive dup { - pack } 1 dive swap { dropn } 1 dive } 'shrink-packed def

# Example code
`cat`: (Only gets stdinout once)

    stdinout { dup dup dup read-stream write-stream eof-stream not } while

Note: any code might be incorrect.
(that's a feature, not a bug)

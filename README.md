# StackStream

StackStream is an esolang-y stack-based language with streams thrown in, because why not?

# Syntax
The syntax of the language is very simple, it contains only five data types 
 (referred to as 'tokens'):

 - `number`: A 32-bit signed integer (might change to a bignum later) e.g. `42`
              Note: \`A is shorthand for `65`.
 - `symbol`: An alphanumeric string starting with ' e.g. `'asdf`
 - `method`: An alphanumeric string not starting with '. `dive`
 - `stream`: A datatype capable of reading and writing, like a FILE *, and
              possibly seeking and arbitrary reading/writing.
 - `codeblock`: A list of tokens, surrounded by "{" and "}" `{ 5 dive }`

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

- (+ / - \*): a:number b:number → c:number - Calculates a +/-* b.
- =: a:number b:number → c:number - If a and b are equal, c is 1, else it is zero.

- dup: a → a a - duplicates the top item on the stack.
- drop: a → - drops the top item.
- dive: a:codeblock N:number → - dives N items into the stack, and executes a. After a is done executing, dives back N items.
- dig: b ... N:number → ... b - moves an item N items into the stack onto the top.
- bury: ... a N:number → a ... - moves an item on top of the stack N items into the stack.
- stack-count: → a:number - Counts the amount of items on the stack.
- swap: a b → b a - swaps the top two items on the stack.

- if: cc:number b:codeblock → - if cc is non-zero, executes b. Else, just pops it from the stack.
- elseif: cc:number b:codeblock a:codeblock → - if cc is non-zero, executes a. Else, executes b.

- exec: a:codeblock → - Executes a.
- def: b:codeblock a:symbol → - Defines a to be a method, which will execute b.
- assert: a:number → - If a is zero, errors

- stdinout: → a:stream - pushes a stream referencing stdin / stdout to the stack.
- read-stream: a:stream → b:number - reads of the top stream, and returns the value.
- write-stream: a:stream b:number → - writes b onto stream a.
- tell-stream: a:stream → b:number - gets the location of the cursor in the stream, if not possible errors.
- seek-stream: a:stream b:number → - sets the location of the cursor in the stream, if not possible errors.
- eof-stream: a:stream → b:number - if stream a hit eof, pushes 1, else 0

- new-buffer: → a:stream - Creates a new empty buffer, with pointer to location 0.
- write-buffer: a:stream b:number c:number → - Writes c into location b of the stream.
- read-buffer: a:stream b:number → c:number - Reads the data at location b of the stream.


# Convenience methods
Some convenience methods are presented, implemented in StackStream itself:

while: a:codeblock → - Executes code block a, and checks the top value of the stack afterwards. If non-zero, loop. (do { } while())

    { dup 1 dive swap { drop } { while } elseif } 'while def

compare: a:number b:number → a c:number - Compares a to b, and pushes the result value, while keeping value a intact.

    { swap dup 2 dig = } 'compare def

dig': a ... N:number → a ... a - Digs up an item, but keeps a duplicate of it on its original position

    { dup dup -1 * swap 1 + bury { dup 2 dig bury } dive } 'dig' def

stack-check: a:codeblock → - Executes a, but makes sure the stack doesn't change size.

    { stack-count swap 1 dive stack-count = assert } 'stack-check def

# Example code
cat: (Only gets stdinout once)

    stdinout { dup dup dup read-stream write-stream eof-stream } while

Note: any code might be incorrect.
(that's a feature, not a bug)
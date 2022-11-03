# functionfizzbuzz

## Bare 

Naive solution

``` fsharp
for i in 1..100 do
    if i % 3 = 0 && i % 5 = 0 
    then 
        printfn "FizzBuzz"
    else 
        if i % 3 = 0 
        then 
            printfn "Fizz"
        else 
            if i % 5 = 0 
            then
                printfn "Buzz"
            else
                printfn "%i" i
```

## ElIf

Collapse the nesting

``` fsharp
for i in 1..100 do
    if i % 3 = 0 && i % 5 = 0 then
        printfn "FizzBuzz"
    elif i % 3 = 0 then 
        printfn "Fizz"
    elif i % 5 = 0 then
        printfn "Buzz"
    else
        printfn "%i" i
```

## Move printf up a level

``` fsharp
for i in 1..100 do
    printfn "%s" (
        if i % 3 = 0 && i % 5 = 0 then
            "FizzBuzz"
        elif i % 3 = 0 then 
            "Fizz"
        elif i % 5 = 0 then
            "Buzz"
        else
            sprintf "%i" i
    )
```

## Extract a function

``` fsharp
let fizzBuzz i = 
    if i % 3 = 0 && i % 5 = 0 then
        "FizzBuzz"
    elif i % 3 = 0 then 
        "Fizz"
    elif i % 5 = 0 then
        "Buzz"
    else
        sprintf "%i" i

for i in 1..100 do
    printfn "%s" (fizzBuzz i)
```

## Pipe

``` fsharp
let fizzBuzz i = 
    if i % 3 = 0 && i % 5 = 0 then
        "FizzBuzz"
    elif i % 3 = 0 then 
        "Fizz"
    elif i % 5 = 0 then
        "Buzz"
    else
        sprintf "%i" i

for i in 1..100 do
    fizzBuzz i
    |> printfn "%s"
```

## Remove the for loop

``` fsharp
let fizzBuzz i = 
    if i % 3 = 0 && i % 5 = 0 then
        "FizzBuzz"
    elif i % 3 = 0 then 
        "Fizz"
    elif i % 5 = 0 then
        "Buzz"
    else
        sprintf "%i" i

[1..100]
|> Seq.map fizzBuzz
|> Seq.iter (printfn "%s")
```

## Use pattern matching

``` fsharp
let fizzBuzz i = 
    match i with
    | x when (x % 5 = 0) && (x % 3 = 0) -> 
        "FizzBuzz"
    | x when (x % 3 = 0) -> 
        "Fizz"
    | x when (x % 5 = 0) -> 
        "Buzz"
    | x -> 
        sprintf "%i" x

[1..100]
|> Seq.map fizzBuzz
|> Seq.iter (printfn "%s")
```

## Extract fizz and buzz

``` fsharp
let fizzBuzz i = 
    let fizz x = x % 3 = 0
    let buzz x = x % 5 = 0

    match i with
    | x when fizz x && buzz x -> 
        "FizzBuzz"
    | x when fizz x -> 
        "Fizz"
    | x when buzz x -> 
        "Buzz"
    | x -> 
        sprintf "%i" x

[1..100]
|> Seq.map fizzBuzz
|> Seq.iter (printfn "%s")
```

## Remove when

``` fsharp
let fizzBuzz i = 
    let fizz x = x % 3 = 0
    let buzz x = x % 5 = 0

    match ((fizz i), (buzz i)) with
    | (true, true) -> 
        "FizzBuzz"
    | (true, false) -> 
        "Fizz"
    | (false, true) -> 
        "Buzz"
    | _ -> 
        sprintf "%i" i

[1..100]
|> Seq.map fizzBuzz
|> Seq.iter (printfn "%s")
```

## Remove bool

``` fsharp
let fizzBuzz i = 
    let fizz x = x % 3
    let buzz x = x % 5

    match ((fizz i), (buzz i)) with
    | (0, 0) -> 
        "FizzBuzz"
    | (0, _) -> 
        "Fizz"
    | (_, 0) -> 
        "Buzz"
    | _ -> 
        sprintf "%i" i

[1..100]
|> Seq.map fizzBuzz
|> Seq.iter (printfn "%s")
```

## Active Pattern Match

``` fsharp
let (|Value|Fizz|Buzz|FizzBuzz|) i =
    match i % 3, i % 5 with
    | 0, 0 -> FizzBuzz
    | 0, _ -> Fizz
    | _, 0 -> Buzz
    | _, _ -> Value i

let fizzBuzz i = 
    match (i) with
    | FizzBuzz -> 
        "FizzBuzz"
    | Fizz -> 
        "Fizz"
    | Buzz -> 
        "Buzz"
    | _ -> 
        sprintf "%i" i

[1..100]
|> Seq.map fizzBuzz
|> Seq.iter (printfn "%s")
```

## Discriminated Unions

``` fsharp
open System

// The Object Ortiented Way

type IShape =
    abstract member Area: float
    abstract member Circumference: float

type CircleShape (radius: float) =
    interface IShape with
        member this.Area = Math.PI * radius * radius
        member this.Circumference = 2.0 * Math.PI * radius
    member this.Radius = radius

type RectangleShape (width: float, height: float) =
    interface IShape with
        member this.Area = width * height
        member this.Circumference = 2.0 * (width + height)
    member this.Width = width
    member this.Height = height

type TriangleShape (side1: float, side2: float, side3: float) =
    interface IShape with
        member this.Area =
            let s = (side1 + side2 + side3) / 2.0
            Math.Sqrt(s * (s - side1) * (s - side2) * (s - side3))
        member this.Circumference = side1 + side2 + side3
    member this.Side1 = side1
    member this.Side2 = side2
    member this.Side3 = side3

[
    CircleShape 1.0 :> IShape
    RectangleShape (2.0, 3.0) :> IShape
    TriangleShape (3.0, 4.0, 5.0) :> IShape
]
|> List.map (fun x -> (x.Area, x.Circumference))
|> List.iter (printfn "%A")


// The Functional Way

type Shape =
    | Circle of float
    | Rectangle of float * float
    | Triangle of float * float * float

let area shape =
    match shape with
    | Circle r -> 
        Math.PI * r * r
    | Rectangle (w, h) -> 
        w * h
    | Triangle (a, b, c) -> 
        let s = (a + b + c) / 2.0
        Math.Sqrt (s * (s - a) * (s - b) * (s - c))

let circumference shape =
    match shape with
    | Circle r -> 
        2.0 * Math.PI * r
    | Rectangle (w, h) -> 
        2.0 * (w + h)
    | Triangle (a, b, c) -> 
        a + b + c

[
    Circle 1.0
    Rectangle (2.0, 3.0)
    Triangle (3.0, 4.0, 5.0)
]
|> List.map (fun x -> (area x, circumference x))
|> List.iter (printfn "%A")
```

## FSharp Option

Functional patterns don't cheat. 
They just use the functional language.

``` fsharp
// nulls don't exist. How to handle them?

let hasValue = Some 1
let noValue = None

match hasValue with
| Some x -> printfn "has value: %i" x
| None -> printfn "no value"

match noValue with
| Some x -> printfn "has value: %i" x
| None -> printfn "no value"

// How does Option work?
// It's just a built-in discriminated union

type MyOption<'a> =
    | Some of 'a
    | None

let hasValue' = MyOption.Some 1
let noValue' = MyOption.None

match hasValue' with
| MyOption.Some x -> printfn "has value: %i" x
| MyOption.None -> printfn "no value"

match noValue' with
| MyOption.Some x -> printfn "has value: %i" x
| MyOption.None -> printfn "no value"
```
## Partial Active Pattern Match

``` fsharp
let (|Fizz|_|) n =
    if (n % 3 = 0)
    then Some Fizz
    else None

let (|Buzz|_|) n =
    if (n % 5 = 0)
    then Some Buzz
    else None

let fizzBuzz i = 
    match (i) with
    | Fizz & Buzz -> 
        "FizzBuzz"
    | Fizz -> 
        "Fizz"
    | Buzz -> 
        "Buzz"
    | _ -> 
        sprintf "%i" i

[1..100]
|> Seq.map fizzBuzz
|> Seq.iter (printfn "%s")
```

## Rules based approach

``` fsharp
let fizz x =
    if (x % 3 = 0) 
    then Some "Fizz" 
    else None

let buzz x =
    if (x % 5 = 0) 
    then Some "Buzz" 
    else None

let value x =
    sprintf "%i" x

let fizzbuzz x =
    [
        fizz
        buzz
    ]
    |> List.choose (fun rule -> rule x)
    |> String.concat ""
    |> function
        | "" -> 
            value x
        | y -> 
            y

[1..100]
|> Seq.map fizzbuzz
|> Seq.iter (printfn "%s")
```

## Use fold

``` fsharp
let fizz i appliedRules =
    if (i % 3 = 0) 
    then "Fizz"::appliedRules
    else appliedRules

let buzz i appliedRules =
    if (i % 5 = 0) 
    then "Buzz"::appliedRules
    else appliedRules

let value i appliedRules =
    if (appliedRules = []) 
    then [string i]
    else appliedRules

let fizzbuzz i =
    [
        fizz
        buzz
        value
    ]
    |> List.fold 
        (fun appliedRules rule -> 
            rule i appliedRules
        ) []
    |> List.rev
    |> String.concat ""

[1..100]
|> Seq.map fizzbuzz
|> Seq.iter (printfn "%s")
```

## DivisibleBy

``` fsharp
let divisibleBy text factor appliedRules i =
    if (i % factor = 0) 
    then text::appliedRules
    else appliedRules

let value appliedRules i =
    if (appliedRules = []) 
    then [string i]
    else appliedRules

let fizzbuzz i =
    [
        divisibleBy "Fizz" 3
        divisibleBy "Buzz" 5
        value
    ]
    |> List.fold 
        (fun appliedRules rule -> 
            rule appliedRules i
        ) []
    |> List.rev
    |> String.concat ""

[1..100]
|> Seq.map fizzbuzz
|> Seq.iter (printfn "%s")
```

## Types as function definitions, SubModules, Currying

``` fsharp
type Rule = string list -> int32 -> string list

module Rule = 
    let isDivisibleBy text factor : Rule =
        fun appliedRules i ->
            if (i % factor = 0) 
            then text::appliedRules
            else appliedRules

    let valueIfNoRulesApplied : Rule = 
        fun appliedRules i ->
            if (appliedRules = []) 
            then [string i]
            else appliedRules
open Rule

let applyRules j rules  =
    rules
    |> List.fold 
        (fun appliedRules rule -> 
            rule appliedRules j
        ) []
    |> List.rev

let fizzbuzz i =
    [
        isDivisibleBy "Fizz" 3
        isDivisibleBy "Buzz" 5
        valueIfNoRulesApplied
    ]
    |> applyRules i
    |> String.concat ""

[1..100]
|> Seq.map fizzbuzz
|> Seq.iter (printfn "%s")
```

## As an applicative functor

``` fsharp
type Rule = int32 -> string list -> string list

let isDivisibleBy text factor : Rule =
    fun i appliedRules ->
        if (i % factor = 0) 
        then appliedRules @ [text]
        else appliedRules

let valueIfNoRulesApplied : Rule = 
    fun i appliedRules ->
        if (appliedRules = []) 
        then [string i]
        else appliedRules

type RulesFunctor = 
    | RulesFunctor of int * string list

let (<*>) 
    (RulesFunctor (value, appliedRules)) 
    (rule : Rule)  
    =
    let newAppliedRules = rule value appliedRules
    RulesFunctor (value, newAppliedRules)

let (<!>) 
    value 
    rule 
    =
    (RulesFunctor (value, [])) <*> rule

let (<=>) 
    (RulesFunctor (_, appliedRules)) 
    f 
    =
    f appliedRules

type RulesBuilder() =
    member __.Return(x) = Some x

let rules = RulesBuilder()

let fizzbuzz value = 
    value
    <!> isDivisibleBy "Fizz" 3
    <*> isDivisibleBy "Buzz" 5
    <*> isDivisibleBy "Bang" 7    
    <*> isDivisibleBy "Bong" 11
    <*> valueIfNoRulesApplied
    <=> String.concat ""

[1..100]
|> Seq.map fizzbuzz
|> Seq.iter (printfn "%s")
```
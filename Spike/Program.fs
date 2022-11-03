let input = [1..100]

let nestedIfs input = 
    for x in input do
        if ((x % 5 = 0) && (x % 3 = 0))
        then
            printfn "FizzBuzz"
        else 
            if ((x % 3 = 0))
            then 
                printfn "Fizz"
            else 
                if ((x % 5 = 0))
                then 
                    printfn "Buzz"
                else 
                    printfn "%i" x

let pattern_matching input =
    for x in input do
        match x with
        | _ when ((x % 5 = 0) && (x % 3 = 0)) ->
            printfn "FizzBuzz"
        | _ when (x % 3 = 0) ->
            printfn "Fizz"
        | _ when (x % 5 = 0) ->
            printfn "Buzz"
        | _ ->
            printfn "%i" x

let return_seq input =
    input
    |> Seq.map (
        function
        | x when ((x % 5 = 0) && (x % 3 = 0)) ->
            "FizzBuzz"
        | x when (x % 3 = 0) ->
            "Fizz"
        | x when (x % 5 = 0) ->
            "Buzz"
        | x ->
            sprintf "%i" x
    )
    
    

let pattern_matching_cleaned input =
    let fizz x = x % 3 = 0
    let buzz x = x % 5 = 0

    input
    |> Seq.map (
        function
        | x when fizz x && buzz x ->
            "FizzBuzz"
        | x when fizz x ->
            "Fizz"
        | x when buzz x ->
            "Buzz"
        | x ->
            sprintf "%i" x
    )

let pattern_matching_in_match_parameter input =
    let fizz x = x % 3
    let buzz x = x % 5
    
    input
    |> Seq.map (fun x ->
        match (fizz x, buzz x) with
        | (0, 0) ->
            "FizzBuzz"
        | (0, _) ->
            "Fizz"
        | (_, 0) ->
            "Buzz"
        | (_ , _) ->
            sprintf "%i" x
    )

let active_pattern_match input =
    let (|Fizz|Buzz|FizzBuzz|Value|) n =
        match ((n % 3), (n % 5)) with
        | (0, 0) -> FizzBuzz
        | (0, _) -> Fizz
        | (_, 0) -> Buzz
        | (_, _) -> Value n

    input
    |> Seq.map (
        function
        | FizzBuzz -> "FizzBuzz"
        | Fizz -> "Fizz"
        | Buzz -> "Buzz"
        | Value n -> sprintf "%i" n
    )

let partial_active_pattern_match input =
    let (|Fizz|_|) n =
        if (n % 3 = 0)
        then Some Fizz
        else None

    let (|Buzz|_|) n =
        if (n % 5 = 0)
        then Some Buzz
        else None

    input
    |> Seq.map 
        (fun x ->
            match x with
            | Fizz & Buzz -> "FizzBuzz"
            | Fizz -> "Fizz"
            | Buzz -> "Buzz"
            | _-> sprintf "%i" x
        )

let folding inputs  =
    let fizz current value =
        if (value % 3 = 0) 
        then current + "Fizz" 
        else current
    
    
    let buzz current value = 
        if (value % 5 = 0) 
        then current + "Buzz" 
        else current
    

    let value current value = 
        if (current = "")
        then sprintf "%i" value
        else current

    let rules =
        [
            fizz    
            buzz
            value
        ]

    let applyRules input =
        rules
        |> Seq.fold
            (fun combined rule 
                -> rule combined input) 
            ""

    inputs
    |> Seq.map applyRules

let folding_single_func inputs  =
    let divisibleBy current x text  y = 
        if (y % x = 0)
        then current + text
        else text

    let rules =
        [
            (divisibleBy "Fizz" 3)
            
            (divisibleBy "Buzz" 5)

            (fun current value ->
                if (current = "")
                then sprintf "%i" value
                else current
            )
        ]

    inputs
    |> Seq.map (fun value ->
        rules
        |> Seq.fold
            (fun combined rule -> 
                rule combined value)
            ""
    )


input
// nestedIfs
// pattern_matching
//|> return_seq
//|> pattern_matching_cleaned
//|> pattern_matching_in_match_parameter
//|> active_pattern_match
// |> folding
|> folding_single_func
|> Seq.iter (printfn "%s")
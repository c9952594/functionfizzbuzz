let fizz x =
    if (x % 3 = 0) 
    then Some "Fizz" 
    else None

let buzz x =
    if (x % 5 = 0) 
    then Some "Buzz" 
    else None

let fizzbuzz x =
    [
        fizz
        buzz
    ]
    |> List.choose (fun rule -> rule x)
    |> String.concat ""
    |> function
        | "" -> 
            sprintf "%i" x
        | appliedRules -> 
            appliedRules

[1..100]
|> Seq.map fizzbuzz
|> Seq.iter (printfn "%s")
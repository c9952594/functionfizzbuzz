// let log p = printfn "expression is %A" p

// let loggedWorkflow =
//     let x = 42
//     log x
//     let y = 43
//     log y
//     let z = x + y
//     log z
//     //return
//     z

// type LoggingBuilder() =
//     let log p = printfn "expression is %A" p

//     member this.Bind(x, f) =
//         log x
//         f x

//     member this.Return(x) =
//         x

// let logger = new LoggingBuilder()

// let loggedWorkflow' =
//     logger
//         {
//         let! x = 42
//         let! y = 43
//         let! z = x + y
//         return z
//         }

// let divideBy bottom top =
//     if bottom = 0
//     then None
//     else Some(top/bottom)

// let divideByWorkflow init x y z =
//     let a = init |> divideBy x
//     match a with
//     | None -> None  // give up
//     | Some a' ->    // keep going
//         let b = a' |> divideBy y
//         match b with
//         | None -> None  // give up
//         | Some b' ->    // keep going
//             let c = b' |> divideBy z
//             match c with
//             | None -> None  // give up
//             | Some c' ->    // keep going
//                 //return
//                 Some c'

// let good = divideByWorkflow 12 3 2 1
// printfn "good = %A" good

// let bad = divideByWorkflow 12 3 0 1
// printfn "bad = %A" bad

// type MaybeBuilder() =

//     member this.Bind(x, f) =
//         match x with
//         | None -> None
//         | Some a -> f a

//     member this.Return(x) =
//         Some x

// let maybe = new MaybeBuilder()

// let divideByWorkflow' init x y z =
//     maybe
//         {
//         let! a = init |> divideBy x
//         let! b = a |> divideBy y
//         let! c = b |> divideBy z
//         return c
//         }
// printfn "good = %A" (divideByWorkflow' 12 3 2 1)
// printfn "bad = %A" (divideByWorkflow 12 3 0 1)

// let map1 = [ ("1","One"); ("2","Two") ] |> Map.ofList
// let map2 = [ ("A","Alice"); ("B","Bob") ] |> Map.ofList
// let map3 = [ ("CA","California"); ("NY","New York") ] |> Map.ofList


// type OrElseBuilder() =
//     member this.ReturnFrom(x) = x
//     member this.Combine (a,b) =
//         match a with
//         | Some _ -> a  // a succeeds -- use it
//         | None -> b    // a fails -- use b instead
//     member this.Delay(f) = f()

// let orElse = new OrElseBuilder()

// let multiLookup key = 
//     orElse {
//         return! map1.TryFind key
//         return! map2.TryFind key
//         return! map3.TryFind key
//     }

// multiLookup "A" |> printfn "Result for A is %A"
// multiLookup "CA" |> printfn "Result for CA is %A"
// multiLookup "X" |> printfn "Result for X is %A"

// let x = 42 in
//   let y = 43 in
//     let z = x + y in
//        z    // the result

// 42 |> (fun x ->
//   43 |> (fun y ->
//      x + y |> (fun z ->
//        z))) |> ignore

// let pipeInto (someExpression,lambda) =
//     printfn "expression is %A" someExpression
//     someExpression |> lambda

// pipeInto (42, fun x ->
//   pipeInto (43, fun y ->
//     pipeInto (x + y, fun z ->
//        z))) |> ignore

// let pipeInto' (someExpression,lambda) =
//    match someExpression with
//    | None ->
//        None
//    | Some x ->
//        x |> lambda

// let return' c = Some c

// let divideByResult x y w z =
//     pipeInto' (x |> divideBy y, fun a ->
//     pipeInto' (a |> divideBy w, fun b ->
//     pipeInto' (b |> divideBy z, fun c ->
//     Some c //return
//     )))

// let good = divideByWorkflow 12 3 2 1
// printfn "good = %A" good
// let bad = divideByWorkflow 12 3 0 1
// printfn "bad = %A" bad

// let (>>=) m f = pipeInto'(m,f)
// let divideByWorkflow' x y w z =
//     x |> divideBy y >>= divideBy w >>= divideBy z
// printfn "good = %A" (divideByWorkflow' 12 3 2 1)
// printfn "bad = %A" (divideByWorkflow' 12 3 0 1)

let strToInt str =
    try
        Some (int str)
    with
    | _ -> None
    
type YourWorkFlowBuilder() =
    member this.Bind(x, f) =
        match x with
        | None -> None
        | Some a -> f a

    member this.Return(x) =
        Some x

let yourWorkflow = new YourWorkFlowBuilder()

let stringAddWorkflow x y z =
    yourWorkflow
        {
        let! a = strToInt x
        let! b = strToInt y
        let! c = strToInt z
        return a + b + c
        }

// test
// let good = stringAddWorkflow "12" "3" "2"
// printfn "good = %A" good
// let bad = stringAddWorkflow "12" "xyz" "2"
// printfn "bad = %A" bad

let (>>=) m f = 
    match m with
    | None -> None
    | Some a -> f a

let strAdd str i = 
    (strToInt str) >>= (fun a -> Some (a + i))

let good = strToInt "1" >>= strAdd "2" >>= strAdd "3"
printfn "good = %A" good

let bad = strToInt "1" >>= strAdd "xyz" >>= strAdd "3"
printfn "bad = %A" bad

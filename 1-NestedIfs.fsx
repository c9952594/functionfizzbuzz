// https://dev.to/shimmer/the-state-monad-in-f-3ik0
// https://fsharpforfunandprofit.com/posts/computation-expressions-builder-part3/



type Stateful<'state, 'result> =
    | Stateful of ('state -> 'result * 'state)

module Stateful =
    let run state (Stateful stateful) =
        stateful state

    let ret result =
        Stateful (fun state -> 
            (result, state))

    let bind binder stateful =
        Stateful (fun state ->
            let result, state' = stateful |> run state
            binder result |> run state')

type StatefulBuilder() =
    member __.Return (result) = 
        Stateful.ret result

    member __.ReturnFrom(stateful) = 
        stateful

    member __.Bind (stateful, binder) = 
        Stateful.bind binder stateful
    
    member __.Zero = 
        Stateful.ret ()
        
    member __.Combine (statefulA, statefulB) = 
        Stateful.bind (fun _ -> statefulB) statefulA

    member __.Delay(f) = 
        f ()

let state = StatefulBuilder()






type Stack<'t> = private Stack of List<'t>

module Stack =
    let ofList list = Stack list

    let push item (Stack items) =
        Stack (item :: items)

    let pop (Stack stack) = 
        match stack with
        | head :: tail -> 
            (head, Stack tail)
        | [] -> 
            failwith "Empty stack"

module StatefulStack =
    let popC = 
        Stateful (fun stack ->
            let poppedItem, stackAfterPop = Stack.pop stack
            (poppedItem, stackAfterPop))

    let pushC item =
        Stateful (fun stack ->
            let stackAfterPush = Stack.push item stack
            let result = ()
            (result, stackAfterPush))

let computation =
    state {
        let! a = StatefulStack.popC

        if a = 5 then
            do! StatefulStack.pushC 7
        else
            do! StatefulStack.pushC 3
            do! StatefulStack.pushC 8

        return a
    }

computation
|> (fun (Stateful stateful) -> 
    stateful (Stack.ofList [5; 6; 7])
)
|> printfn "%A"

let stack''' = [9; 0; 2; 1; 0] |> Stack.ofList
printfn "%A" (Stateful.run stack''' computation)

// Output: (5, Stack [7; 1])
let stack'''' = [5; 1] |> Stack.ofList
printfn "%A" (Stateful.run stack'''' computation)




type AppliedRules = 
    | AppliedRules of string list

module AppliedRules = 
    let empty = AppliedRules []

    let add rule (AppliedRules rules) = 
        AppliedRules (rule :: rules)

    let ret rules =
        rules
        |> List.map (fun (AppliedRules rules) -> rules 
        |> List.rev 
        |> String.concat ", ")

    let bind binder (AppliedRules rules) =
        AppliedRules (
            rules 
            |> List.map binder 
            |> List.concat)

type AppliedRulesBuilder() =
    member __.Return (result) = 
        AppliedRules.ret result

    member __.ReturnFrom(appliedRules) = 
        appliedRules

    member __.Bind (appliedRules, binder) = 
        AppliedRules.bind binder appliedRules
    
    member __.Zero = 
        Stateful.ret ()

p    // do!
    member __.Combine (statefulA, statefulB) = 
        Stateful.bind (fun _ -> statefulB) statefulA

    member __.Delay(f) = 
        f ()

let rules = AppliedRulesBuilder()

let fizz i state =
    if i % 3 = 0 then
        Some (i, "Fizz"::state)
    else
        None

let buzz i state =
    if i % 5 = 0 then
        Some (i, "Buzz"::state)
    else
        None

let value i state =
    if fizz i state |> Option.isNone && buzz i state |> Option.isNone then
        Some (i, string i::state)
    else
        None

let computation i =
    rules {
        do! fizz i
        do! buzz i
        let! result = value i
        return result
    }
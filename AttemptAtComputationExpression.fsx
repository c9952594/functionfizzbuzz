type Stateful<'state, 'result> =
    | Stateful of ('state -> 'result * 'state)

module Stateful =
    let run 
        (state : 'state) 
        (Stateful stateful) 
        =
        stateful state

    let ret 
        (result : 'result) 
        =
        fun (state : 'state) -> 
            (result, state)
        |> Stateful

    let bind 
        (binder : ('result -> Stateful<'state, 'result>)) 
        (stateful : Stateful<'state, 'result>)
        =
        fun state ->
            let result, state' = 
                stateful 
                |> run state
            
            binder result 
            |> run state'
        |> Stateful 

type StatefulBuilder() =
    member this.Return (result) =
        Stateful.ret result

    member this.ReturnFrom(stateful) = 
        stateful

    member this.Bind (stateful, binder) = 
        Stateful.bind binder stateful
    
    member this.Zero = 
        ()
        |> Stateful.ret

    member this.Combine (statefulA, statefulB) = 
        Stateful.bind (fun _ -> statefulB) statefulA

    member this.Delay(f) = 
        f()

let state = StatefulBuilder()


let fizzbuzz i = 
    let add = (+)
    let mult = (*)
    
    let run (Stateful expression) initialState = 
        let (result, state) = 
            expression initialState
        state
 
    let computation =
        let computationWrapper wrappedFunc value = 
            fun state -> 
                let result = wrappedFunc value state
                (result, result)
            |> Stateful
    
        state {
            let! a = computationWrapper add 2
            printfn "a: %i" a
            let! b = computationWrapper mult 3
            printfn "b: %i" b
            return b
        }

    run computation i

[1..100]
|> Seq.map fizzbuzz
|> Seq.iter (printfn "%A")
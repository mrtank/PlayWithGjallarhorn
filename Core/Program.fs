namespace SimpleForm

open System

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Bindable.Framework

module Program =

    type User =
        { FirstName : string;
          SecondName : string;
          ID : Int64}

    type SomeInfo =
        {
            User : User
            SomeData : List<Int64>
            AnotherData : List<Int64>
        }

    type SomeNav = 
        | SomePage
        | OtherPage

    type Model = 
        | Guest 
        | Member of User

    let (|Auth|_|) model =
        match model with
        | Member user -> 
            Some user
        | _ ->
            None

    let update (nav : Dispatcher<SomeNav>) (message : Model) = 
        match message with
        | Auth user ->
            SomePage |> nav.Dispatch 
            user |> Member 
        | _ -> Guest

    type Queries =
        static member readSome id =
            seq [ id ; 4L ]

        static member readAnother id =
            seq [ id ; 5L ]

    let getSomeInfo = 
        function
        | Guest -> failwith "..."
        | Member user ->
            let someData = Queries.readSome user.ID |> Seq.toList
            let anotherData = Queries.readAnother user.ID |> Seq.toList
            {
                User = user
                SomeData = someData
                AnotherData = anotherData
            }

    let bindToSource _nav source (model : ISignal<Model>) : IObservable<Dispatcher<SomeNav>> list = 
        let nothin _ =
            new Dispatcher<SomeNav>()

        let variable =
            model
            |> Signal.map nothin

        let getId model =
            match model with
            | Guest -> 0L
            | Member user -> user.ID

        model
        |> Signal.map getId
        |> Bind.Explicit.oneWay source "ID"

        [ 
            Signal.map (fun x -> x) variable
        ]

    let applicationCore = Framework.application Model.Guest update (Component.fromExplicit bindToSource) Nav.empty

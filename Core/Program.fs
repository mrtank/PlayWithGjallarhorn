namespace SimpleForm

open System

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Bindable.Framework

module Program =

    type User =
        { 
            FirstName : string;
            SecondName : string;
            ID : Int64 
        }

    type SomeNav = 
        | SomePage of User
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

    let update (nav : Dispatcher<SomeNav>) (message : Model) _ = 
        match message with
        | Auth user ->
            SomePage user |> nav.Dispatch 
            user |> Member 
        | _ -> Guest

    type Queries =
        static member readSome id =
            seq [ id ; 4L ]

        static member readAnother id =
            seq [ id ; 5L ]

    type SomeInfo =
        {
            User : User
            SomeData : List<Int64>
            AnotherData : List<Int64>
        }

    let getSomeInfo(user: User) = 
        let someData = Queries.readSome user.ID |> Seq.toList
        let anotherData = Queries.readAnother user.ID |> Seq.toList
        {
            User = user
            SomeData = someData
            AnotherData = anotherData
        }

    let def = { User = { ID = -1L; FirstName = ""; SecondName = ""} ; SomeData = []; AnotherData = [] }

    let showInfoComponent  = 
        Component.create<SomeInfo, SomeNav, User> [
            <@ def.User @> |> Bind.oneWay (fun m -> m.User)
            <@ def.SomeData @> |> Bind.oneWay (fun m -> m.SomeData)
            <@ def.AnotherData @> |> Bind.oneWay (fun m -> m.AnotherData)
        ]

    type AuthMessage = 
        | TryAuthenticate of string * string
        | Approved of User

    let credentialBindings _nav source (model : ISignal<Model>) : IObservable<AuthMessage> list =  
        let fname = Mutable.create ""
        let sname = Mutable.create ""

        Bind.Explicit.twoWayMutable source "FirstName"  fname     
        Bind.Explicit.twoWayMutable source "SecondName" sname 

        let submit = Bind.Explicit.createCommand "AuthCommand" source
        [
            submit |> Observable.map (fun _ -> TryAuthenticate (fname.Value, sname.Value))
        ]

    let handleAuthenticationAttempt (msg : AuthMessage) _currentModel =
        match msg with
        | TryAuthenticate (f,s) ->
            async {
                printfn "Try...%A %A" f s
                if f = "aa" then
                    return {ID = 0L; FirstName = f; SecondName = s} |> Approved |> Some
                else
                    return None
            } 
        | _ -> async { return None }

    let upMapper =
        function
        | Approved user -> Member user
        | _ -> failwith "..."

    let credentialComponent : IComponent<Model,SomeNav,Model>  = 
        Component.fromExplicit<Model,SomeNav,AuthMessage> credentialBindings 
        |> Component.withSubscription handleAuthenticationAttempt
        |> Component.withMappedMessages upMapper

    let applicationCore nav = 
        let model = Guest
        let disp = new Dispatcher<SomeNav>()
        Framework.application model (update disp) credentialComponent nav
        |> Framework.withNavigation disp
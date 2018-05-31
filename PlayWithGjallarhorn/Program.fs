open System

open SimpleForm

open FsXaml
open Gjallarhorn.Wpf
open Gjallarhorn.Bindable
open Gjallarhorn.Bindable.Framework
open Gjallarhorn

// The WPF Platform specific bits of this application need to do 2 things:
// 1) They create the view (the actual Window)
// 2) The start the WPF specific version of the framework with the view

// ----------------------------------     View      ---------------------------------- 
// Our platform specific view type
type MainWin = XAML<"MainWindow.xaml">
type SomeInfo = XAML<"SomeInfo.xaml">
type App = XAML<"App.xaml">

// ----------------------------------  Application  ---------------------------------- 
[<STAThread>]
[<EntryPoint>]
let main _ =         
    let somePageComponent nav source model : IObservable<Program.SomeInfo> list =
        [ model ]

    let updateNavigation (app : ApplicationCore<_,_,_>) request =         
        match request with
        | Program.SomePage ->
            Navigation.Page.fromComponent 
                SomeInfo Program.getSomeInfo (Component.fromExplicit somePageComponent) id
        | _ -> failwith "..."

    let navigator = Navigation.singlePage App MainWin (Dispatcher<Program.SomeNav>()) updateNavigation
    let app = Program.applicationCore
    Framework.RunApplication (navigator, app)
    // Run using the WPF wrappers around the basic application framework       
    1
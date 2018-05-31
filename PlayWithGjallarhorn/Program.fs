open System

open SimpleForm

open FsXaml
open Gjallarhorn.Wpf
open Gjallarhorn.Bindable
open Gjallarhorn.Bindable.Framework

// The WPF Platform specific bits of this application need to do 2 things:
// 1) They create the view (the actual Window)
// 2) The start the WPF specific version of the framework with the view

// ----------------------------------     View      ---------------------------------- 
// Our platform specific view type
type MainWin = XAML<"MainWindow.xaml">
type SomeInfo = XAML<"SomeInfo.xaml">
type App = XAML<"App.xaml">
type Auth = XAML<"Auth.xaml">

// ----------------------------------  Application  ---------------------------------- 
[<STAThread>]
[<EntryPoint>]
let main _ =         
    let updateNavigation (app : ApplicationCore<_,_,_>) request =         
        match request with
        | Program.OtherPage ->
            Navigation.Page.fromComponent 
                Auth id Program.credentialComponent id            
        | Program.SomePage ->
            Navigation.Page.fromComponent 
                SomeInfo Program.getSomeInfo Program.showInfoComponent id

    let navigator = Navigation.singlePage App MainWin Program.OtherPage updateNavigation
    let app = Program.applicationCore navigator.Navigate
    Framework.RunApplication (navigator, app)   
    1
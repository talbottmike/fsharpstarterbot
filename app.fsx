#r "packages/Suave/lib/net40/Suave.dll"
#r "packages/Microsoft.Bot.Builder/lib/net46/Microsoft.Bot.Connector.dll"
#r "packages/Microsoft.Bot.Builder/lib/net46/Microsoft.Bot.Builder.dll"
#r "packages/Microsoft.Rest.ClientRuntime/lib/net45/Microsoft.Rest.ClientRuntime.dll"
#r "packages/Microsoft.WindowsAzure.ConfigurationManager/lib/net40/Microsoft.WindowsAzure.Configuration.dll"
#r "packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "packages/Autofac/lib/net40/Autofac.dll"
#r "packages/System.Net.Http/lib/net46/System.Net.Http.dll"

open Suave
open Suave.Successful
open Suave.Web
open Suave.Operators
open Suave.Filters
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Microsoft.Bot.Connector
open Microsoft.Bot.Builder.Dialogs
open System
open System.Threading.Tasks
open System.Net.Http

let envAsOption (envVarName : string) =
    let envVarValue = Environment.GetEnvironmentVariable(envVarName)
    if ((isNull envVarValue) || (envVarValue.Trim().Length = 0)) then None else Some envVarValue

let appId = defaultArg (envAsOption "MicrosoftAppId") "John"
let appSecret = defaultArg (envAsOption "MicrosoftAppPassword") "Secret"

[<AutoOpen>]
module Helpers = 
    let toJson v =
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, jsonSerializerSettings) |> OK
        >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

    let getResourceFromReq<'a> (req : HttpRequest) =
        let getString rawForm =
            System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

[<Serializable>]
type MyBot () =
    // A count that shows the number of the current message
    let mutable count = 0

    // Called from PromptDialog.confirm
    member this.confirmReset (ctx : IDialogContext) (a : IAwaitable<bool>) =
        Task.Factory.StartNew(fun () ->
            let confirm = a.GetAwaiter().GetResult()

            if (confirm) then
                count <- 0
                "Count was reset" |> ctx.PostAsync |> ignore
            else
                "Count was not reset" |> ctx.PostAsync |> ignore

            ctx.Wait <| ResumeAfter(this.messageReceived)
        )
    
    // Handle received message 
    member this.messageReceived (ctx : IDialogContext) (a : IAwaitable<Activity>) = 
        Task.Factory.StartNew(fun () ->
            let message = a.GetAwaiter().GetResult()

            if (message.Text = "reset") then
                PromptDialog.Confirm(ctx, ResumeAfter(this.confirmReset), "Are you sure you want to reset the count?", "Didn't get that!", 2, PromptStyle.None)
            else
                count <- count + 1
                message.CreateReply(sprintf "%d : You said: %s" count message.Text) |> ctx.PostAsync |> Async.AwaitTask |> Async.RunSynchronously
                ctx.Wait <| ResumeAfter(this.messageReceived)
        )

    interface IDialog with
        member this.StartAsync ctx = 
            Task.Factory.StartNew(fun () ->
                ctx.Wait <| ResumeAfter(this.messageReceived)
            )

/// Handle messages
let botHandler (activity : Activity) =
    printfn "Received messsage of type %s: %s" activity.Type activity.Text

    async {
        match activity.Type with
        | ActivityTypes.Message ->
            try
                Conversation.SendAsync(activity, (fun _ -> MyBot () :> IDialog<obj>), Threading.CancellationToken()).RunSynchronously |> ignore
                return ""
            with
            | exn -> let reply = (sprintf "Sorry, I am having a hard time understanding you because of %s." exn.Message)
                     return reply
        | ActivityTypes.Ping ->
            let reply = "Ping"
            return reply
        | _ -> return "Unhandled activity type."
    } |> Async.RunSynchronously 

/// Suave application
let app = 
    choose [ path "/" >=> OK "Hello World!" 
             Authentication.authenticateBasic ((=) (appId, appSecret)) <|
                 choose [ path "/api/messages" >=> request (getResourceFromReq >> botHandler >> toJson ) ] ]

// This is handled in the host specific code within build.fsx, such as Azure/Heroku
//startWebServer defaultConfig app
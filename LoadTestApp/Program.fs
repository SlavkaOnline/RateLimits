open System
open System.Net.Http
open NBomber.Contracts
open NBomber.FSharp

let userId = Guid.NewGuid().ToString("N")
let httpFactory =
        ClientFactory.create (
            name = "http_factory",
            clientCount = 1,
            initClient =
                fun (number, context) ->
                    task {
                        let apiClient = new HttpClient()
                        apiClient.BaseAddress <- Uri("http://localhost:5243")
                        return apiClient
                    }
        )

let createStep (userId: string) = 
    Step.create("Get", clientFactory = httpFactory, execute = fun ctx ->
            task {
                let request = new HttpRequestMessage(HttpMethod.Get, "WeatherForecast")
                request.Headers.Add("userId", userId)
                let! response = ctx.Client.SendAsync request
                if response.IsSuccessStatusCode then 
                    return Response.ok()
                else
                    return Response.fail(error = response.ReasonPhrase, statusCode = (int response.StatusCode))
        })
let scenario1 = Scenario.create "InjectPerSec" [createStep <| Guid.NewGuid().ToString("N")]
                   |> Scenario.withoutWarmUp
                   |> Scenario.withLoadSimulations [InjectPerSec(10, during = TimeSpan.FromMinutes 1.0)]
                   
let scenario2 = Scenario.create "InjectPerSecRandom" [createStep <| Guid.NewGuid().ToString("N")]
                   |> Scenario.withoutWarmUp
                   |> Scenario.withLoadSimulations [InjectPerSecRandom(minRate = 1, maxRate = 20, during = TimeSpan.FromMinutes 1.0)]                   
                   
let scenario3 = Scenario.create "KeepConstant" [createStep <| Guid.NewGuid().ToString("N")]
                   |> Scenario.withoutWarmUp
                   |> Scenario.withLoadSimulations [KeepConstant(5, during = TimeSpan.FromMinutes 1.0)]                          
                   
NBomberRunner.registerScenarios [scenario1; scenario2; scenario3]
|> NBomberRunner.run
|> ignore

printfn "Finish"
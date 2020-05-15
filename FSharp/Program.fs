namespace TestWebAppFsharp

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open TerseIgnore

module Program =

    let CreateHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webBuilder ->
                !webBuilder.UseStartup<Startup>()
            )

    [<EntryPoint>]
    let main args =
        CreateHostBuilder(args)
            .Build().Run()
        0

# Cross-platform Microsoft BotFramework starter written in FSharp using Suave.io with deploy on Azure and Heroku

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)
[![Deploy to Heroku](https://www.herokucdn.com/deploy/button.png)](https://heroku.com/deploy)

This site is based on a combination of [BotFramework-Suave](https://github.com/Krzysztof-Cieslak/BotFramework-Suave) and also
[suave-fshome](https://github.com/tpetricek/suave-fshome) by [@tpetricek](https://github.com/tpetricek) which
in turn is based on [suavebootstrapper](https://github.com/shanselman/suavebootstrapper) by [@shanselman](http://github.com/shanselman). 
It is a small sample that shows how to create a [Microsoft BotFramework](https://dev.botframework.com) bot using [FSharp](http://fsharp.org) 
and [Suave.io](https://suave.io), that can be hosted on both Azure and Heroku.

The FSharp code has been tested on both Windows 10 using the [BotEmulator](https://aka.ms/bf-bc-emulator) and on OSX El Capitan using
[Console BotEmulator](http://aka.ms/bfemulator).

## Getting started Windows

* Get the required components to run [FSharp on Windows](http://fsharp.org/use/windows/) if you don't already have them.
* Clone the repo: `git clone https://github.com/jgoalby/fsharpstarterbot.git`.
* Run build.cmd to install the paket dependencies: `build.cmd`.
* Test the bot with the [BotEmulator](https://aka.ms/bf-bc-emulator).

## Getting started OSX/Linux

* Get required components to run [FSharp on Mac](http://fsharp.org/use/mac) or [FSharp on Linux](http://fsharp.org/use/linux) if you don't already have them.
* Clone the repo: `git clone https://github.com/jgoalby/fsharpstarterbot.git`.
* Run build.sh to install the paket dependencies: `./build.sh`.
* Test the bot with the [Console BotEmulator](http://aka.ms/bfemulator).

## Configuring the AppId and AppSecret

* Set MicrosoftAppId and MicrosoftAppPassword environment variables for the application id and application secret
* In Windows you use the SET command such as SET MicrosoftAppId=MyID or there is the [SETX](http://ss64.com/nt/setx.html) command in Windows 7 and above
* In OSX you use the EXPORT command such as EXPORT MicrosoftAppId="MyID" which you will want in your profile if you want it more permanent
* In Azure you set environment variables under Settings -> Application Settings -> App settings
* In Heroku you set [config vars](https://devcenter.heroku.com/articles/config-vars)

### What's included

You'll find the following directories and files:

```
fsharpstarterbot/
├── .paket/
│   └── paket.bootstrapper.exe    bootstraps paket.exe
│
├── .deployment                   [kudu](https://github.com/projectkudu/kudu) continuous deployment file for azure
├── Procfile                      deployment file for heroku
│
├── app.azure.fsx                 fsharp script for azure which loads app.fsx
├── app.fsx                       fsharp script containing the botframework code
├── app.heroku.fsx                fsharp script for heroku which loads app.fsx
│
├── app.json                      information about this app
│
├── build.cmd                     install and run on windows
├── build.fsx                     common build fsharp script
├── build.sh                      install and run on mono platforms
│
├── paket.dependencies            project dependencies
├── paket.lock                    project dependencies
│
└── web.config                    configuration for azure
```

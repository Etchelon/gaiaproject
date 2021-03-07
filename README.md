# Gaia Project

Full implementation of the Gaia Project board game as a web app.

The app is optimized for desktop and also works nicely on mobile but it's not optimal: the game has lots of information to show but there's not much space on a mobile screen for all of those :)
The app requires authentication and currently users can only be invitated as it seems it's not legal to provide a free implementation of this game because of digital copyrights acquired by Digidiced. For more details on what another implementer went through see [this thread](https://forum.boardgamers.space/topic/53/site-is-now-publicly-available) [and this other thread](https://forum.boardgamers.space/topic/94/big-news-on-bgs-and-digidiced) :)

## Gameplay

Currently you can play by inviting users to your game, no public queues have been implemented.
It is possible to play with 2 to 4 people, the automa is not available.
Faction selection can be free, random or you can choose to have an auction, in which case the factions can either be chosen by players or determined randomly.
With 3 players, the map is always X shaped with 8 sectors, to make it a bit stricter and maintain the tension high.

![Game setup](https://i.imgur.com/MH4YXkq.png "Game setup")

The game page is heavily inspired by Boardgame Arena, with the board taking most of the space and player information and logs on the right, with the difference that the board elements are grouped in tabs instead of being stacked vertically, so there is no need to scroll to access different boards. Actions and the state of the active player is placed in a status bar above the board.

![Game page](https://i.imgur.com/LFe5oCN.png "Game Page")

## Architecture

The app is structured as a static React frontend that talks to a .NET Core WebApi.

Authentication is delegated to Auth0, SignalR is used to establish real time communication with the backend for real time updates during a game, and MongoDB is the database chosen for data persistence.

## Installation

This software is provided under the MIT license, and you can freely download and use it if you want, keeping in mind the aforementioned limitations regarding public distribution of the app. I'm in no way responsible of how you will use this software.

### Prerequisites

In order to run the app you need the following:

-   An Auth0 app to manage authentication
-   A SendGrid account
-   A recent version of Node JS
-   .NET Core 5+
-   A MongoDB instance

### Run the app

To start the React frontend, in the `Frontend/StarGate` folder

-   define the following environment variables: `REACT_APP_AUTH0_DOMAIN` (your Auth0 domain), `REACT_APP_AUTH0_CLIENT_ID` (the Id of this SPA app), `REACT_APP_AUTH0_AUDIENCE` (the Identifier of the API), `REACT_APP_AUTH0_SCOPE` (the scopes needed by the client). These variables are required by the Auth0 library which will redirect to your Auth0 app for authentication
-   run `npm i` to install the dependencies
-   run `npm start` to launch the Typescript compiler and the development server, which will listen on port 3000

To start the .NET Core backend, go to the `Backend/Endpoint` folder and:

-   define the following user secrets (using `dotnet user-secrets set`): `SendGrid:ApiKey` (the API key of the SendGrid account), `Auth0:Domain` (your Auth0 domain), `Auth0:Client:Id` (the Id of this Machine to Machine app that validates the Bearer token that comes with HTTP requests), `Auth0:Audience` (the Identifier of the API)
-   make sure the mongod instance is active and listening on port 27017 (you can change the connection string by editing the appsettings.Development.json file)
-   `dotnet run`

And that's it :)

## Free Software and assets used in this project

The dependencies used in this project are obviously listed in the various .csproj and package.json files, but I'd like to explicitly list other valuable assets that I've used to build this app and thank their creators:

-   In the backend code, I'm using a modified (simplified for my use case) version of [MongoDB Generic Repository](https://github.com/alexandre-spieser/mongodb-generic-repository) by Alexandre Spieser
-   In the React frontend, I'm using wherever possible the real assets of the game.
    For most of the game components, I've used the assets found in [this amazing project](https://github.com/stones314/GaiaMapGenerator/) by BGG user [Steinar Nerhus](https://boardgamegeek.com/user/Stones)
-   The faction boards were not among the assets provided, so I used the amazing work by [Aaron Cotton (Olagarro)](https://boardgamegeek.com/thread/2197795/gaia-project-graphic-overhaul-work-progress)
-   For the Scoring Board and Research Board, I've scanned the components of my copy of the game and, since I have 0 skills in image editors, had them refined by the very kind BGG user [bokuteki](https://boardgamegeek.com/user/bokuteki)

That should be all, enjoy!

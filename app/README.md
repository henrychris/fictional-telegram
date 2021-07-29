# EpumpBot

On App Start, the webhook is launched and requests are received in the Webhook Controller.
HandleUpdateService is responsible for, well, handling updates.

# How to run
Using ngrok, type this in the terminal
`ngrok http https://localhost:80`
The https address produced by ngrok will serve as the Webhook Url in appsettings.json

using Discord;
using Discord.Interactions;

namespace Zordon_Bot.ExampleModule;

public class ExampleModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    public async Task GreetUserAsync()
    => await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);

    [SlashCommand("isnumber", "Check if the input text is a number.")]
    public Task IsNumber(double number)
        => RespondAsync($"The text {number} is a number!", ephemeral: true);

    [SlashCommand("multiplii", "Get the product of two numbers.")]
    public async Task Multiply(int a, int b)
    {
        long product = Math.BigMul(a, b);
        await RespondAsync($"The product of `{a} * {b}` is `{product}`.", ephemeral: true);
    }

    [SlashCommand("square", "Get the square root of a number.")]
    public async Task Square(double number)
    {
        double sqrt = Math.Sqrt(number);
        await RespondAsync($"The square root of `{number}` is `{sqrt}`.", ephemeral: true);
    }



    [SlashCommand("components", "Button demo command")]
    public async Task ButtonInput()
    {
        var components = new ComponentBuilder();

        // A SelectMenuBuilder is created
        var select = new SelectMenuBuilder()
        {
            CustomId = "menu1",
            Placeholder = "Select something",
            MinValues = 1,
            MaxValues = 3,
        };
        // Options are added to the select menu. The option values can be generated on execution of the command. You can then use the value in the Handler for the select menu
        // to determine what to do next. An example would be including the ID of the user who made the selection in the value.
        select.AddOption("abc", "abc_value");
        select.AddOption("def", "def_value");
        select.AddOption("ghi", "ghi_value");

        components.WithSelectMenu(select);

        var button = new ButtonBuilder()
        {
            Label = "Button",
            CustomId = "button1",
            Style = ButtonStyle.Primary
        };

        // Messages take component lists. Either buttons or select menus. The button can not be directly added to the message. It must be added to the ComponentBuilder.
        // The ComponentBuilder is then given to the message components property.
        components.WithButton(button);

        var button2 = new ButtonBuilder()
        {
            Label = "Button",
            CustomId = "button2",
            Style = ButtonStyle.Success
        };

        // Messages take component lists. Either buttons or select menus. The button can not be directly added to the message. It must be added to the ComponentBuilder.
        // The ComponentBuilder is then given to the message components property.
        components.WithButton(button2);



        var embed = new EmbedBuilder
        {
            // Embed property can be set within object initializer
            Title = "Hello world!",
            Description = "I am a description set by initializer.",
            //ImageUrl = "https://upload.wikimedia.org/wikipedia/en/a/a2/Watchmen%2C_issue_1.jpg"
        };
        // Or with methods
        embed.AddField("Field title",
            "Field value. I also support [hyperlink markdown](https://example.com)!")
            .WithAuthor(Context.Client.CurrentUser)
            .WithFooter(footer => footer.Text = "I am a footer.")
            .WithColor(Color.Blue)
            .WithTitle("I overwrote \"Hello world!\"")
            .WithDescription("I am a description.")
            .WithUrl("https://example.com")
            .WithImageUrl("https://upload.wikimedia.org/wikipedia/en/a/a2/Watchmen%2C_issue_1.jpg")
            .WithCurrentTimestamp();

        var test = components.Build();


        await RespondAsync("This message has a button!", components: components.Build(), embed: embed.Build());
    }
}

using Discord.Interactions;
using Discord;
using Zordon_Bot.Handlers;

namespace Zordon_Bot.Modules;

public class UpworkModule(UpworkSearchHandler searchHandler) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly UpworkSearchHandler _searchHandler = searchHandler;

    [SlashCommand("upwork", "Upwork Search")]
    public async Task Upwork([Summary(name: "allWords", description: "All words to include")] string allWords, [Summary(name: "noneWords", description: "Do not include these words")] string noneWwords)
    {
        await FollowupAsync("Tester");
    }

    [SlashCommand("upworkmodel", "Get Upwork Advanced Search Form")]
    public async Task UpworkModel()
    {
        await Context.Interaction.RespondWithModalAsync<AdvancedSearchModal>("upwork_search_modal");
    }

    [ModalInteraction("upwork_search_modal")]
    public async Task ModalResponse(AdvancedSearchModal modal)
    {
        _searchHandler.BuildSearchString(modal.AllWords, modal.AnyWords, modal.NoneWords, modal.ExactPhrase, modal.TitleWords);

        var components = new ComponentBuilder();

        var selectSort = new SelectMenuBuilder()
        {
            CustomId = "sort_menu",
            Placeholder = "Sort Results By:"
        };

        selectSort.AddOption("Newest", "recency");
        selectSort.AddOption("Relevance", "relevance");
        selectSort.AddOption("Client Spend", "client_total_charge%2Bdesc");
        selectSort.AddOption("Client Rating", "client_rating%2Bdesc");

        components.WithSelectMenu(selectSort);

        var test = components.Build();


        await RespondAsync(components: components.Build());
    }

    [ComponentInteraction("sort_menu")]
    public async Task SortMenuHandler(string[] selections)
    {
        _searchHandler.SortFilter = selections.First();

        var components = new ComponentBuilder();

        var selectLocation = new SelectMenuBuilder()
        {
            CustomId = "location_menu",
            Placeholder = "US only?:"
        };

        selectLocation.AddOption("Yes", "y");
        selectLocation.AddOption("No", "n");

        components.WithSelectMenu(selectLocation);

        await RespondAsync(components: components.Build());
    }


    [ComponentInteraction("location_menu")]
    public async Task LocationMenuHandler(string[] selections)
    {
        _searchHandler.UsOnly = selections.First() == "y" ? "&user_location_match=1" : "";

        //**** Final Response ****
        // Specify the AllowedMentions so we don't actually ping everyone.
        AllowedMentions mentions = new();
        // Filter for the presense of role or everyone pings
        mentions.AllowedTypes = AllowedMentionTypes.Users;

        // Respond to the modal.
        await RespondAsync($"Search String: {_searchHandler.SearchString} \n \n {_searchHandler.ReturnResultTitles()}", allowedMentions: mentions, ephemeral: true);
    }


    public class AdvancedSearchModal : IModal
    {
        public string Title => "Advanced Search";

        [RequiredInput(false)]
        [InputLabel("All of these words")]
        [ModalTextInput("all_words_input", TextInputStyle.Short)]
        public string AllWords { get; set; } = string.Empty;

        [RequiredInput(false)]
        [InputLabel("Any of these words")]
        [ModalTextInput("any_words_input", TextInputStyle.Short)]
        public string AnyWords { get; set; } = string.Empty;

        [RequiredInput(false)]
        [InputLabel("None of these words")]
        [ModalTextInput("none_words_input", TextInputStyle.Short)]
        public string NoneWords { get; set; } = string.Empty;

        [RequiredInput(false)]
        [InputLabel("The exact phrase")]
        [ModalTextInput("exact_phrase_input", TextInputStyle.Short, placeholder: "Without quotation marks")]
        public string ExactPhrase { get; set; } = string.Empty;

        [RequiredInput(false)]
        [InputLabel("Title Search")]
        [ModalTextInput("title_words_input", TextInputStyle.Short)]
        public string TitleWords { get; set; } = string.Empty;
    }
}

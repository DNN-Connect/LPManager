// Sets the time to wait before attempting a request to the server (using ClientAPI)
var delayRequestTime = 300;

// Sets the expected maximum of records (must be the same as the server setting)
var limitResults = 10;

var usernameSuggestControl;
var requestTimeoutId;
var isRequesting = false;

function pageLoad() {
    usernameSuggestControl = new AutoSuggestControl(document.getElementById(dnn.getVar('UsernameInput')), new UsernameSuggestions());
}

function ClientApiGetUsernamesSuccess(result, ctx, req) {
    // Set request state to false
    isRequesting = false;
    
    var aSuggestions = [];
    var aSuggestionsValues = [];
    
    var sTextboxValue = usernameSuggestControl.textbox.value;

    // Clear list and return if result is empty
    if (result == null || result.length == 0) {
        usernameSuggestControl.autosuggest(aSuggestions, aSuggestionsValues, false);
        return;
    }
    
    // Split result to get all username + email pairs
    values = result.split("/");
    for (var i = 0; i < values.length; i++) {
        var username = values[i].split(",")[0];
        var usernameHighlighted = values[i].split(",")[1];
        var emailHighlighted = values[i].split(",")[2];

        aSuggestions.push(usernameHighlighted + " (" + emailHighlighted + ")");
        aSuggestionsValues.push(username);
    }

    // Provide suggestions to the control
    usernameSuggestControl.autosuggest(aSuggestions, aSuggestionsValues, false);
}

function ClientApiGetUsernamesFail(result, ctx, req) {
    // Ignore ClientAPI errors
}

// Request suggestions for the given autosuggest control.
function UsernameSuggestions() { }
UsernameSuggestions.prototype.requestSuggestions = function(oAutoSuggestControl /*:AutoSuggestControl*/,
                                                          bTypeAhead /*:boolean*/) {

    var sTextboxValue = oAutoSuggestControl.textbox.value;
    if (sTextboxValue.length > 1) { // Start searching after 2 characters have been inserted

        // Get the possible values from the server

        // Stop current request if active
        if (isRequesting) {
            clearTimeout(requestTimeoutId);
        }
        
        isRequesting = true;
        requestTimeoutId = setTimeout("GetUsernames('" + sTextboxValue + "')", delayRequestTime);
        
    }
};

function GetUsernames(filter) {
    dnn.xmlhttp.callControlMethod('DNNEurope.Modules.LocalizationEditor.Users', 'GetUsernamesByFilter', { "filter": filter }, ClientApiGetUsernamesSuccess, ClientApiGetUsernamesFail);
}
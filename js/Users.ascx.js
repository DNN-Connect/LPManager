// 
// Copyright (c) 2004-2011 DNN-Europe, http://www.dnn-europe.net
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation files (the "Software"), to deal in the Software 
// without restriction, including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

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
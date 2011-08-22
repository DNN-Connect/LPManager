<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ResourceEditor.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.ResourceEditor" %>
<p>
  <asp:PlaceHolder ID="PlaceHolder1" runat="server" />
</p>
<p style="margin-top: 20px;">
  <asp:LinkButton runat="server" ID="cmdUpdate" resourcekey="cmdUpdate" Text="Update" CssClass="CommandButton" />&nbsp;&nbsp;
  <asp:LinkButton runat="server" ID="cmdCancel" resourcekey="cmdCancel" Text="Cancel" CssClass="CommandButton" />&nbsp;&nbsp;
  <asp:LinkButton runat="server" ID="cmdSave" resourcekey="cmdSave" Text="Save" CssClass="CommandButton" />
</p>

<script type="text/javascript" src="http://www.google.com/jsapi">
//Google AJAX API loader.
</script>

<script type="text/javascript" language="javascript">

//Load Google language API (version 1).
google.load("language", "1");

function setTextValue(baseId, panelID, textId, textValue) {
    //Sets the text of a single localization input textbox to the specified value.
    
    var textBox;
    textBox = document.getElementById(baseId + '_' + 'panel' + panelID + 'edit' + textId + '_txtValue');
    var rteBox;
    rteBox = document.getElementById(baseId + '_' + 'panel' + panelID + 'edit' + textId + '_teValue_teValue');
    
    if (textBox) {
        textBox.value=textValue;
    }
    else {
        //alert('Not with Rich Text Editor.');
    }
}

function transferAllTextValues(panelID, typeoflocale) {
    //Transfers all texts from either the original locale or the source locale to the corresponding localization input textboxes.
    //This occurs per panel only, except when panelID has a value of 0, meaning doing this for the whole page.
    
    var theImages;
    theImages = document.getElementsByTagName('img');

    var curElement; 
    for (var cnt = 0; cnt < theImages.length; cnt++) {
        curElement = theImages[cnt];
        if ((curElement.getAttribute("panelID") == panelID || (curElement.getAttribute("panelID") > 0 && panelID == 0)) 
            && curElement.getAttribute("typeoflocale") == typeoflocale) {
            curElement.onclick();
        }
    }
}

function translateTextValue(baseId, panelID, textId, fromValue, fromLanguage, toLanguage, defaultLanguage) {
    //Translates a single text from either the original locale or the source locale to the corresponding localization input textbox.
    //The translation is done via Google Translate.
    
    if (defaultLanguage == '') {
        defaultLanguage = google.language.Languages.ENGLISH;    //The mother of all defaults.
    }
    if (fromLanguage == '') {
        fromLanguage = defaultLanguage;
    }
    if (toLanguage == '') {
        toLanguage = defaultLanguage;
    }
    
    //The language enumeration in the code-behind (Visual Basic) consists of integers.
    //The language enumeration on the client side (JavaScript) consists of strings (of 2 characters).
    //I don't have access to the 2-character language codes in the code-behind.
    //But evaluating the following strings works well!
    eval('fromLanguage = google.language.Languages.' + fromLanguage.toUpperCase());
    eval('toLanguage = google.language.Languages.' + toLanguage.toUpperCase());
    
    //String it.
    fromValue = fromValue.toString();
    
    //Prevent '{' and '}' becoming '(' and ')' by translation: replace '{' and '}' by '(((' and ')))'.
    fromValue = fromValue.replace(/{/g, '(((').replace(/}/g, ')))');
    
    //Handle HTML special characters. (Unescape/Encode)
    fromValue = fromValue.replace(/&/g, '&amp;').replace(/\'/g, '&#39;').replace(/"/g, '&quot;'); //.replace(/</g, '&lt;').replace(/>/g, '&gt;');
    
    //Because of length limitations on the text to translate via the Google Translate AJAX API,
    //the text to translate is split into chunks of a bounded number of characters.
    //Each chunk is translated via Google Translate separately
    //and all the partial results are concatenated back into a single translated text.
    var maxLength = 1200;
    //var minLength = 200;
    //var chopChars = '?!.' + ':;,' + ' '; //Separators for sentences, sentence parts, and words.
    var toValue = '';
    var fromChunk = fromValue;
    var chunkStart = 0;
    var chunkEnd = 0;
    var chopUp = 0;
    
    while (chunkStart < fromValue.length) {
        //Find a suitable chunk of text to translate.
        chunkEnd = Math.min(chunkStart + maxLength, fromValue.length);
        fromChunk = fromValue.substring(chunkStart, chunkEnd);
        
        //If we have not reached the end, look for a place to chop off the next part.
        if (chunkEnd < fromValue.length) {
            //Chop it off at a separator character: first try sentence separators.
            chopUp = Math.max(fromChunk.lastIndexOf('?'), fromChunk.lastIndexOf('!'), fromChunk.lastIndexOf('.'));
            //Chop it off at a separator character: then try sentence part separators.
            if (chopUp == -1) {
                chopUp = Math.max(fromChunk.lastIndexOf(':'), fromChunk.lastIndexOf(';'), fromChunk.lastIndexOf(','));
            }
            //Chop it off at a separator character: finally try word separators.
            if (chopUp == -1) {
                chopUp = fromChunk.lastIndexOf(' ');
            }
            //Chop!
            if (chopUp != -1) {
                chunkEnd = chunkStart + chopUp;
                fromChunk = fromValue.substring(chunkStart, chunkEnd);
            }
            //If we are in the middle of a tag, chop off the whole tag.
            if (fromChunk.lastIndexOf('<') > Math.max(0, fromChunk.lastIndexOf('>'))) {
                chopUp = fromChunk.lastIndexOf('<') - 1;
                chunkEnd = chunkStart + chopUp;
                fromChunk = fromValue.substring(chunkStart, chunkEnd);
            }
        }
        
        //alert('Before: ' + fromChunk);
        
        //Google Translate.
        google.language.translate(fromChunk, fromLanguage, toLanguage, function(result) {
            if (!result.error) {
                //Stitch it together.
                toValue = toValue + result.translation;

                //alert('After: ' + toValue);

                //Finalize translation when we have reached the end.
                if (chunkEnd = fromValue.length) {
                    //Handle HTML special characters. (Escape/Decode)
                    toValue = toValue.replace(/&amp;/g, '&').replace(/&#39;/g, '\'').replace(/&quot;/g, '"'); //.replace(/&lt;/g, '<').replace(/&gt;/g, '>'); //.replace(/&nbsp;/g, ' ');

                    //Remove unintended spaces within end tags returned by Google Translate.
                    toValue = toValue.replace(/<\/ /g,'</');

                    //Prevent '{' and '}' becoming '(' and ')' by translation: replace '(((' and ')))' back to '{' and '}'.
                    toValue = toValue.replace(/\(\(\(/g, '{').replace(/\)\)\)/g, '}');

                    //Put the final translation into the textbox.
                    setTextValue(baseId, panelID, textId, toValue);
                }
            }
            else {
                alert('Error = ' + result.error);
            }
        });
        
        chunkStart = chunkEnd;
    }
}

function translateAllTextValues(panelID, typeoflocale) {
    //Translates all texts from either the original locale or the source locale to the corresponding localization input textboxes.
    //The translation is done via Google Translate.
    //This occurs per panel only, except when panelID has a value of 0, meaning doing this for the whole page.
    
    var theImages;
    theImages = document.getElementsByTagName('img');
    
    var curElement;
    for (var cnt = 0; cnt < theImages.length; cnt++) {
        curElement = theImages[cnt];
        if ((curElement.getAttribute("panelID") == panelID || (curElement.getAttribute("panelID") > 0 && panelID == 0))
            && curElement.getAttribute("typeoflocale") == typeoflocale) {
            curElement.onclick();
        }
    }
}

function clearAllTextValues(panelID) {
    //Clears the contents of every localization input textbox.
    //This occurs per panel only, except when panelID has a value of 0, meaning doing this for the whole page.
    
    var theInputs;
    theInputs = document.getElementsByTagName('input');
    var theTextAreas;
    theTextAreas = document.getElementsByTagName('textarea');
    var curElement;
    
    for (var cnt = 0; cnt < theInputs.length; cnt++) {
        curElement = theInputs[cnt];
        if ((curElement.getAttribute("panelID") == panelID || (curElement.getAttribute("panelID") > 0 && panelID == 0))) {
            curElement.value = '';
        }
    }
    
    for (var cnt = 0; cnt < theTextAreas.length; cnt++) {
        curElement = theTextAreas[cnt];
        if ((curElement.getAttribute("panelID") == panelID || (curElement.getAttribute("panelID") > 0 && panelID == 0))) {
            curElement.value = '';
        }
    }
}
</script>


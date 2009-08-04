<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ResourceEditor.ascx.vb" Inherits="DNNEurope.Modules.LocalizationEditor.ResourceEditor" %>

<p><asp:placeholder id="PlaceHolder1" runat="server" /></p>

<p style="margin-top:20px;">
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
    
    //Prevent '{' and '}' becoming '(' and ')' by translation: replace '{' and '}' by '(((' and ')))'.
    fromValue = fromValue.toString().replace(/{/g, '(((').replace(/}/g, ')))');
    
    //Handle HTML special characters. (Unescape/Encode)
    fromValue = fromValue.toString().replace(/&/g, '&amp;').replace(/\'/g, '&#39;').replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    
    //Google Translate.
    google.language.translate(fromValue, fromLanguage, toLanguage, function(result) {
        if (!result.error) {
            var toValue;
            toValue = result.translation;
               
            //Handle HTML special characters. (Escape/Decode)
            toValue = toValue.toString().replace(/&amp;/g, '&').replace(/&#39;/g, '\'').replace(/&quot;/g, '"').replace(/&lt;/g, '<').replace(/&gt;/g, '>') //.replace(/&nbsp;/g, ' ');
            
            //Prevent '{' and '}' becoming '(' and ')' by translation: replace '(((' and ')))' back to '{' and '}'.
            toValue = toValue.toString().replace(/\(\(\(/g, '{').replace(/\)\)\)/g, '}');
            
            setTextValue(baseId, panelID, textId, toValue);
        }
        else {
            alert('Error = ' + result.error);
        }
    });
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

<asp:UpdatePanel ID="KeepAlivePanel" runat="server">
    <ContentTemplate>
        <asp:Timer ID="KeepAliveTimer" runat="server" OnTick="AjaxTimerTick" Interval="60000" />
        <asp:Label runat="server" ID="lblTimeCheck" />
    </ContentTemplate>
</asp:UpdatePanel>

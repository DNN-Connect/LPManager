# LPManager
The Language Pack Manager is a DNN module that allows you to open up editing of language files (resx files) to other users. The goal is to help you coordinate/manage/delegate translation of DNN stuff. In the settings screen you specify which locales are editable and which resources. 

## When do you want to use Language Pack Manager ?

The module was developed to help 'country/locale leaders' delegate the translation effort. After discussions with various European colleagues it emerged that the pace of DNN releases + their own workload was preventing them from keeping up with language pack releases. It would be a lot easier if these people could create a central DNN site and give access to a group of translators that work on this. It was designed to provide quick overviews of all texts side-by-side so you see what still needs to be done.

## What are the goals of the Localization Editor?

* Allow delegation of translation to individual users based on user X can translate module Y/core into locale Z
* Packs are created on the fly based on the latest translations
* Proper version management of texts and translations. We must be able to edit an old pack and download old packs
* Support for generic languages. A user can do the translation for ‘fr’ and those translations will download into the fr-FR pack
* Allow the translation of non-installed items. The manager will upload a new version of module X/core into the module and the module will extract the resources
* Use automatic translation to improve productivy

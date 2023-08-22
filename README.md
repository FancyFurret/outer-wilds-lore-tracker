# Lore Tracker

Outer Wilds has a ton of amazing lore, but it's hard to remember what you've already read, and what is new. This mod
keeps track of what you've seen across runs and restarts so that you can easily see what is new.

This mod was designed so that you could quickly tell if there is something new to read at a glance. Basically,
purple = new, white = seen. I have tried to make it integrate as seamlessly as possible with the game, so Nomai tech
changes color (just like wall text does natively), and Hearthian things get a purple exclamation point above them.

## Supported Lore Types

* Wall text
    * Translated text will stay white; untranslated text will still be purple
* Scrolls
    * The scroll itself will turn white once it's been fully translated
* Nomai recorders
    * The recorder itself will turn white once it's been fully translated
* Projection stones
    * Once you have translated everything the stone has to offer, it will turn white
* Projection stone slots
    * Once you have translated everything the slot has to offer, it will turn white
* Characters/dialogue
    * If a character has something new to say, there will be a purple exclamation point above their head
    * Any text that is purple is new; white has already been seen
    * Dialogue options will only turn white once you have seen all of its (currently available) paths
* Hearthian recorders/posted notes/etc
    * If you have not read everything it has to offer, it will have a purple exclamation point above it
    * Any text that is purple is new; white has already been seen

## Usage

Just start up the game and it will start tracking! The file it uses to track things is saved right next to the native
Outer Wilds save file, so it should be included in any cloud save system you might be using. Currently there are no
options to configure. Lastly, Outer Wilds does not already keep track of everything the player has seen/translated, so
for existing saves everything will look new until you read it again.
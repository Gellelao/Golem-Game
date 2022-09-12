# Golem

Trying to make a SAP clone where you build a golem instead of a team of pets.

### TODO list/ideas
* Give each part a price? Or standardize like SAP?
* Resource sysyem where you buy blueprints then need to "craft" the part with clay, iron, etc? Then you could salvage old parts for resources, but might be overkill compared to a straightforward "just buy the parts" shop
* Weight and speed, where heavier parts reduce speed, encouraging not just stuffing the grid full of parts. Higher speed means you attack more often (FFTA2 style), or just first (pokemon style)?
* Add an accuracy stat or is missing unfun?
* Cool to have alternative win conditions like reaching the turn limit. Parts could have text like "Turn limit reduced by 5 turns" or "If a draw would happen, instead decide the winner via X"
* Type matchups like pokemon, and effects and parts that depend on your type.
* Energy tanks that need to be stored on the golem, and maybe during the battle the golem can charge it up (could be parts that give it some charge at the start of battle). Can be spent to activate powerful part effects
* Some way to "program" priorities for your golem? FF12 style.
  * Otherwise, how do they decide which "moves" to use?
  * Are there "moves" at all? Or just "attack"
  * Maybe parts can have a "move" on them (different parts could have the same move, and some parts/combinations of parts could grant you a unique move), which get added to a pool from which you choose 3-4 to use in battle. You can slot them into conditions like "On first turn always use this move" or "When I reach 50% or below HP, use this move".
* How are these golems going to look like golems rather than a bunch of random bits stuck together...
* Summons - when summoned they get tracked in the Resolver and have their own speed etc to decide when they act
* SCAVENGE parts from opponents, but only if you defeat them? is this a win-more mechanic though? They wouldn't necessarily be more powerful parts, in fact they are likely to be less powerful if you managed to defeat them, but it means more options that a golem who loses would get...
* Expand from just Golem to "creatures" in geenral? Will provide mor flexiblity in the shapes
  * Perhaps you can select a "frame" e.g "Golem", "Beast", "Snake", "Angel", etc, and that would overlay a shape on the grid. The more squares inside the shape and the less outside the shape, would confer a bonus based on the chosen shape. E.g keeping all your parts within the golem frame might give +HP since golems are bulky?
* Render parts during combat programmatically, maybe via Cellular Automata?
* What if you could select one of three opponents to fight once you've locked in your golem for the turn?
  * Or alternatively, you are shown your next opponent at the start of the turn, and can change your golem/swap out parts from storage in order to better defeat your next opponent
  * My only concern with that is it would encourage calculating the outcome ahead of time, which should not be the correct way to play the game
  * OR, perhaps that's a format you could spec your golem into, FORECASTING, which would let you see selected parts of your opponent, but not the whole golem!
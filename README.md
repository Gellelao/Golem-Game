# Golem

A personal project to build an asynchronous multiplayer game, inspired by the game Super Auto Pets.

Using MonoGame for the UI.

The idea is you are building golems, or mechs or creatures of some kind, by assembling parts together. These creatures are uploaded to a database, from which they can be pulled later as opponents for other players. So your creatures go up against creatures that other players have made, but there is no need for any real-time multiplayer, or to directly connect any players together.

In the example, I have the basics working, where "golems" can be assembled from parts, uploaded and downloaded from AWS, and fight against each other. The next steps would be to flesh it out more with other parts and mechanics, and to add proper graphics and UI.

Main issues I ran into:
* How to distribute the game to others without revealing the AWS secrets
* How to encourage players to create recognizable creature shapes, instead of just random collections of parts
* Architecture - I tried to separate the game logic from the view, but the "controller" part of MVC is a bit lost. Also found that I couldn't always find elegant solutions and that led the code to feel quite messy and hard to work with over time.

[Example.webm](https://user-images.githubusercontent.com/18452032/213841716-8fc8cbdf-c5a1-438e-8cd1-a213e48f5e53.webm)

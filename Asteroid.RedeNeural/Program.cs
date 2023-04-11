using Asteroid.Gui.Guis;
using Asteroid.RedeNeural.Game;

var func = (dynamic constructor) =>
{
    var nextGame = new Game(constructor as AsteroidGame);
    return nextGame;
};

using var game = new AsteroidGame(func);
game.Run();

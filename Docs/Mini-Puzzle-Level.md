# Mini-Puzzle-Level

Ok, le temps est venu de se livrer à un petit exercice de level design.

## Description

<img src="Images/MiniPuzzleLevel-Board24x24.jpg">

Réaliser un petit niveau sur la base d'un plateau de 24 x 24 cases.

Le niveau, composé de blocks qui dessinent un parcours, doit contenir, outre le
`PlayerCube`, les briques de gameplay suivantes : 
- SpawnPoint (pour ne pas avoir à recommencer le parcours depuis le début)
- Destroyer / KillZone (n'importe quel collider accompagné du script [../Assets/Common/Scripts/Destroyer.cs]), 
  histoire de proposer une difficulté.
- StupidBots, des ennemis stupides qui avancent dans la direction du joueur.

L'évaluation se fera selon les 2 critères suivants :
- Maîtrise technique (pas de bug, relative complexité du level design)
- Level design (le parcours présente-t-il un intérêt ?)

Notes: 
- Parmi les ajustements nécessaires, il faut particulièrement faire attention à 
  la "hauteur" de saut" du script cubemove ("Jump Speed"), une valeur de `9` permet
  de sauter 2 cubes, mais pas 3 (d'autres valeurs sont possible pour par exemple
  pouvoir sauter 1 cube mais pas 2).
- Il se trouve un mini niveau de démonstration dans People/jniac/Scenes/MiniPuzzleLevel.
  Celui peut servir de base à votre version.

Rendus:
- Une scène Unity nommé "MiniPuzzleLevel" présent dans le dossier "Scenes" de votre
  dossier personnel (et "poussé" sur Github).



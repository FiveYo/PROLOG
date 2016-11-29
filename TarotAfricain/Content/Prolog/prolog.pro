:- dynamic player/1. 
:- dynamic pari/2.
:- dynamic jeuPlayer/2.
:- dynamic carte/3.

% le joueur, ce qu'il a joué, le numéro de la manche
:- dynamic carteJouee/3.

%(Player, PointManche)
:- dynamic pointManchePlayer/2.

carte(1, "7 coeur", 0).
carte(2, "7 carreaux", 0).
carte(3, "7 pique", 0).
carte(4, "7 trèfle", 0).
carte(5, "8 coeur", 0).
carte(6, "8 carreaux", 0).
carte(7, "8 pique", 0).
carte(8, "8 trèfle", 0).
carte(9, "9 coeur", 0).
carte(10, "9 carreaux", 0).
carte(11, "9 pique", 0).
carte(12, "9 trèfle", 0).
carte(13, "10 coeur", 0).
carte(14, "10 carreaux", 0).
carte(15, "10 pique", 0).
carte(16, "10 trèfle", 0).
carte(17, "V coeur", 0).
carte(18, "V carreaux", 0).
carte(19, "V pique", 0).
carte(20, "V trèfle", 0).
carte(21, "D coeur", 0).
carte(22, "D carreaux", 0).
carte(23, "D pique", 0).
carte(24, "D trèfle", 0).
carte(25, "R coeur", 0).
carte(26, "R carreaux", 0).
carte(27, "R pique", 0).
carte(28, "R trèfle", 0).
carte(29, "AS coeur", 0).
carte(30, "AS carreaux", 0).
carte(31, "AS pique", 0).
carte(32, "AS trèfle", 0).

jeuCartes(JeuCartes) :- findall(carte(X,Y,Z),carte(X,Y,Z),JeuCartes).

playManche(Players, NbCarte):- writeln("Nouvelle manche"),
   				initPlayers(Players),
    			Players = [FirstPlayer| _],
    			% distribuer retourne false quand il a fini de distribuer
   				ignore(distribuer(NbCarte, player(FirstPlayer))),
   				ignore(parier(player(FirstPlayer), NbCarte)),
   				between(1, NbCarte, Index),
    			write("Tour n°"),writeln(Index),
   				playTour(player(FirstPlayer), Index, Winner),
   				(Index \== NbCarte ->
   				write("Tour terminé, le gagnant est "), writeln(Winner) ;
                
                write("Le vainqueur est "),
                % reset les paris, les cartes jouées à chaque manche, le statut des cartes,
                % normalement le jeu de carte associé à chaque joueur est vide vu qu'il a joué toute ses cartes
                % on reset les points de la manche après avoir identifié le/les perdants
   				resetCarteJouee, resetCartes, resetPari,
   				% compte les points perdus
   				write("Manche terminée")).
			
initPlayers([]):-!.	
initPlayers([H|T]):- assert(player(H)), assert(pointManchePlayer(player(H),0)), initPlayers(T).
%initPlayers(Players):- length(Players, NbJoueur), between(1, NbJoueur, Index), nth1(Index,Players,Player), assert(player(Player)).

resetCartes:- jeuCartes(Y), initCarte(Y).

initCarte([]):-!.
initCarte([H|T]):- H = carte(X,Y,Z), retract(carte(X,Y,Z)), assert(carte(X,Y,0)), initCarte(T).

resetCarteJouee:-findall(carteJouee(X,Y,Z),carteJouee(X,Y,Z),ListeCarteJouee),
    			clearCarteJouee(ListeCarteJouee).

clearCarteJouee([]):-!.
clearCarteJouee([H,T]):- H = carteJouee(X,Y,Z), retract(carteJouee(X,Y,Z)), clearCarteJouee(T).

resetPari:- findall((X,Y),pari(X,Y),ListePari), clearPari(ListePari).

clearPari([]):-!.
clearPari([H,T]):- H = pari(X,Y), retract(pari(X,Y)), clearPari(T).

distribuer(NbCarte, Player):- write(Player), write(" pioche "),
    						piocherCartes(NbCarte, ListeCarte),
    						writeln(ListeCarte),
    						assert(jeuPlayer(Player, ListeCarte)),
    						% nextPlayer renvoie false quand il n'y a plus de next
    						nextPlayer(Player, NextPlayer),
    						distribuer(NbCarte, NextPlayer).
    								
parier(Player, NbCarte):- write(Player), writeln(" réfléchit"),
   			iaPari(Player, NbPli, NbCarte),
   			assert(pari(Player, NbPli)),
   			write(Player), write(" a parié "), write(NbPli), writeln(" pli(s)."),
    		nextPlayer(Player, NextPlayer),
    		parier(NextPlayer, NbCarte).

iaPari(_, NbPli, NbCarte):-random(0, NbCarte, NbPli).



playTour(Player, TourEnCours, Winner):- ignore(jouerCarte(Player, TourEnCours)),
    						findall(carteJouee(X,Y,_),carteJouee(X,Y,TourEnCours),ListeCarteJouee),
    						getBetterPlayer(ListeCarteJouee, Winner, TourEnCours),
    						pointManchePlayer(Winner, NbPointPlayer),
    						retract(pointManchePlayer(Winner, NbPointPlayer)),
    						NbPointPlayerTemp is NbPointPlayer + 1,
    						assert(pointManchePlayer(Winner, NbPointPlayerTemp)).

getBetterPlayer(ListeCarteJouee, Winner, TourEnCours):- createListFromListAtom(2, ListeCarteJouee, _, ListeCartes),
    										createListFromListAtom(1, ListeCartes,_, ListeInt),
    										max_list(ListeInt, Max),
    										carteJouee(Winner,carte(Max,_,1),TourEnCours).
    									
% createListFromListAtom(NbArg, ListAtom, ResultList):-!.
createListFromListAtom(_, [], Acc, ResultList):- ResultList = Acc, !.
createListFromListAtom(NbArg, [H|T], Acc, ResultList):- arg(NbArg, H, R),
    													createListFromListAtom(NbArg, T, [R|Acc], ResultList).

jouerCarte(Player, TourEnCours):- iaChoisit(Player, Carte),
    							write(Player), write(" joue la carte "), writeln(Carte),
    							assert(carteJouee(Player, Carte, TourEnCours)),
    							nextPlayer(Player, NextPlayer),
    							jouerCarte(NextPlayer, TourEnCours).

iaChoisit(Player, Carte):- jeuPlayer(Player, ListeCartes),
    					ListeCartes = [Carte|Tail],
    					retract(jeuPlayer(Player, ListeCartes)),
    					assert(jeuPlayer(Player, Tail)).

piocherCartes(NbCarte, ListeCarte):- jeuCartes(Y),
    								length(Y, NbCarteTotal),
    								ajouterALaMain(NbCarteTotal, NbCarte, [], ListeCarte).

ajouterALaMain(_, 0, Acc, ListeCarte):- ListeCarte = Acc, !.

ajouterALaMain(NbCarteTotal, NbCarte , Acc, ListeCarte):-piocherCarte(NbCarteTotal, Carte),
    											NbCarteTemp is NbCarte - 1,
    											ajouterALaMain(NbCarteTotal, NbCarteTemp, [Carte|Acc], ListeCarte).

piocherCarte(NbCarteTotal, Carte):- repeat,
    								random(1, NbCarteTotal, R),
									carte(R, Nom, 0),
                                    retract(carte(R, Nom, 0)), 
                                    assert(carte(R, Nom, 1)),
    								Carte = carte(R, Nom, 1),!.

nextPlayer(Player, NextPlayer):- findall(player(X),player(X),Liste),
    							nextElement(Player, NextPlayer, Liste).

nextElement(Current, Next, Liste):- indexof(Index, Current, Liste), NextIndex is Index, length(Liste, NbElement),
    							   ( NextIndex =< NbElement -> nth0(NextIndex, Liste, Next) ; !).
    
indexof(Index, Item, List):- nth1(Index, List, Item).
indexof(-1, _, _).
enumPlayer(Player):- player(Player), writeln(Player),
    				nextPlayer(Player, NextPlayer),
    				enumPlayer(NextPlayer).

max_list([H|T], M) :- max_list(T, H, M). 
max_list([], C, C).
max_list([H|T], C, M) :- C2 is max(C, H), max_list(T, C2, M).
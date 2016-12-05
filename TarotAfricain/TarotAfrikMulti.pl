:- dynamic player/1. 
:- dynamic pari/2.
:- dynamic jeuPlayer/2.
:- dynamic carte/3.
:- dynamic rang/1.
:- dynamic firstPlayer/1.
%(player, point)
:- dynamic pointGame/2.

% le joueur, ce qu'il a joué, le numéro de la manche
:- dynamic carteJouee/3.

%(Player, PointManche)
:- dynamic pointManchePlayer/2.

%(Player, 0 ou 1 si IA)
:- dynamic playerIA/2.

% debug/callback rules
:- dynamic callPlayManche/0.
:- dynamic callPlayManche2/0.
:- dynamic callPlayManche3/0.

:- dynamic callJoueurPioche/0.
:- dynamic callJoueurPioche2/0.

:- dynamic callPlayerPari/0.
:- dynamic callPlayerPari2/0.

:- dynamic callPlayTour/1.
:- dynamic callPlayTour2/1.

:- dynamic callPlayerJoue/1.

:- dynamic callPariJoueur/2.
:- dynamic callJouerCarte/2.

callPariJoueur(Player, NbPli):- write("Entrez le nombre de pari que vous pensez faire "),
    						writeln(Player), read(NbPli).

callJouerCarte(Player, Carte):- write("Voici vos cartes "),
    						writeln(Player), jeuPlayer(Player, Jeu), write(" "), write(Jeu), writeln("Quelle carte souhaitez vous jouer (uniquement l'id)"),
    						read(Carte).

callPlayManche:- writeln("Nouvelle manche").
callPlayManche2:- writeln("Fin de la manche").
callPlayManche3:- writeln("Fin du jeu").

callJoueurPioche:-currentPlayer(Player), write(Player), writeln(" pioche.").
callJoueurPioche2:-currentPlayer(Player), write(Player), write(" a pioché "), jeuPlayer(Player, Jeu), writeln(Jeu).

callPlayerPari:- currentPlayer(Player), write(Player), write(" réfléchit ... ").
callPlayerPari2:- currentPlayer(Player), write(Player), write(" a parié "), pari(Player, Pari), writeln(Pari).

callPlayTour(NbTour):-write("Début du tour "), writeln(NbTour).
callPlayTour2(Winner):-write("Fin du tour, le gagnant est "),writeln(Winner).

callPlayerJoue(Carte):-currentPlayer(Player), write(Player), write(" joue la carte "), writeln(Carte).

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

tousJoueurs(ListJoueurs) :- findall(player(X),player(X),ListJoueurs).

tousParis(ListParis) :- findall(pari(X,Y),pari(X,Y),ListParis).

playGame(Players, PlayersIA, NbCarte):- checkConstraints(Players, PlayersIA, NbCarte),
    			initPlayers(Players, PlayersIA), Players = [First| _],
    			FirstPlayer = player(First),
    			setFirstPlayer(FirstPlayer),
    			playManche(NbCarte), !.

checkConstraints(Players, PlayersIA, NbCarte):- length(Players, NbPlayers), length(PlayersIA, NbType),
    											( NbPlayers \= NbType ->   
    											throw("List must have same size"); 
                                                (   checkListInteger(PlayersIA), 
                                                	(   integer(NbCarte)) ->  !; throw("NbCarte must be integer"))
                                                ).

checkListInteger([]):-!.
checkListInteger([H|T]):-( (H=0 ; H=1) ->  checkListInteger(T); throw("IAList must contains only 0 and 1")).
                                                
initPlayers([],[]):- !.
initPlayers([H|T], [H2|T2]):- 
    					assert(player(H)), assert(pointGame(player(H),0)), 
    					assert(playerIA(player(H), H2)), 
                       initPlayers(T, T2).

playManche(0):- callPlayManche3, !.
playManche(NbCarte):- callPlayManche,
   				initPointManchePlayers,
    			% distribuer retourne false quand il a fini de distribuer
   				distribuer(NbCarte),
   				parier(NbCarte),
   				playTour(0, NbCarte),
    			comparePariGain,
   				resetCarteJouee, resetCartes, resetPari, resetPointManchePlayer,
    			resetJeuPlayer,
   				% compte les points perdus
   				NbNextCarte is NbCarte - 1,
   				callPlayManche2, playManche(NbNextCarte).

initPointManchePlayers:- findall(player(X),player(X),Liste), initPointManchePlayers(Liste).

initPointManchePlayers([]):-!.	
initPointManchePlayers([H|T]):-  assert(pointManchePlayer(H,0)), initPointManchePlayers(T).
%initPlayers(Players):- length(Players, NbJoueur), between(1, NbJoueur, Index), nth1(Index,Players,Player), assert(player(Player)).

resetCartes:- jeuCartes(Y), initCarte(Y).

initCarte([]):-!.
initCarte([H|T]):- H = carte(X,Y,Z), retract(carte(X,Y,Z)), assert(carte(X,Y,0)), initCarte(T).

resetCarteJouee:-findall(carteJouee(X,Y,Z),carteJouee(X,Y,Z),ListeCarteJouee),
    			clearCarteJouee(ListeCarteJouee).

clearCarteJouee([]):-!.
clearCarteJouee([H|T]):- H = carteJouee(X,Y,Z), retract(carteJouee(X,Y,Z)), clearCarteJouee(T).

resetPari:- findall(pari(X,Y),pari(X,Y),ListePari), clearPari(ListePari).

clearPari([]):-!.
clearPari([H|T]):- H = pari(X,Y), retract(pari(X,Y)), clearPari(T).

resetPointManchePlayer:-findall(pointManchePlayer(X,Y), pointManchePlayer(X,Y),ListePoint), clearPointManche(ListePoint).

clearPointManche([]):-!.
clearPointManche([H|T]):- H = pointManchePlayer(X,Y), retract(pointManchePlayer(X,Y)), clearPointManche(T).

resetJeuPlayer:-findall(jeuPlayer(X,Y),jeuPlayer(X,Y),ListeJeuPlayer), clearJeuPlayer(ListeJeuPlayer).

clearJeuPlayer([]):-!.
clearJeuPlayer([H|T]):-H = jeuPlayer(X,Y), retract(jeuPlayer(X,Y)), clearJeuPlayer(T).

distribuer(NbCarte):- piocherCartes(NbCarte),
    				(   not(nextPlayer(_)) ->  
                        ! ;
    				distribuer(NbCarte)).


piocherCartes(NbCarte):- callJoueurPioche,
    				  jeuCartes(Y),
                      length(Y, NbCarteTotal),
                      ajouterALaMain(NbCarteTotal, NbCarte, [], ListeCarte),
    				  currentPlayer(Player),
    				  assert(jeuPlayer(Player, ListeCarte)),
    				  callJoueurPioche2.

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


parier(NbCarte):- callPlayerPari,
    		currentPlayer(Player),
    		playerIA(Player, IsIA),
    		% on regarde si c'est une IA ou pas, si ça l'est pas on vérifie que le nombre entrée est correct
    		(   IsIA = 1 ->  pariIABourrin(Player, NbPli);
            joueurPari(Player, NbPli)
            ),
   			assert(pari(Player, NbPli)),
   			callPlayerPari2,
    		(   not(nextPlayer(_)) ->  
                        ! ;
    		parier(NbCarte)).

joueurPari(Player, NbPli):- once((   repeat,
            		callPariJoueur(Player, NbPli),
                	(   (integer(NbPli), NbPli >= 0 ) -> true; false)
            		)).


playTour(TourMax, TourMax):- !.
playTour(TourEnCours, TourMax):- callPlayTour(TourEnCours),
    						jouerCarte(TourEnCours),
    						findall(carteJouee(X,Y,_),carteJouee(X,Y,TourEnCours),ListeCarteJouee),
    						getBetterPlayer(ListeCarteJouee, Winner, TourEnCours),
    						pointManchePlayer(Winner, NbPointPlayer),
    						NbPointPlayerTemp is NbPointPlayer + 1,
    						retract(pointManchePlayer(Winner, NbPointPlayer)),
    						assert(pointManchePlayer(Winner, NbPointPlayerTemp)),
    						TourProchain is TourEnCours + 1,
    						callPlayTour2(Winner),
    						setFirstPlayer(Winner),
    						playTour(TourProchain, TourMax).

getBetterPlayer(ListeCarteJouee, Winner, TourEnCours):- createListFromListAtom(2, ListeCarteJouee, _, ListeCartes),
    										createListFromListAtom(1, ListeCartes,_, ListeInt),
    										max_list(ListeInt, Max),
    										carteJouee(Winner,carte(Max,_,1),TourEnCours).


jouerCarte(TourEnCours):- currentPlayer(Player),
    					playerIA(Player, IsIA),
                        % on regarde si c'est une IA ou pas, si ça l'est pas on vérifie que le nombre entrée est correct
                        (   IsIA = 1 ->  iaChoisitBourrin(Player, Carte);
                        	joueurJoueCarte(Player, Carte)
                        ),
                        assert(carteJouee(Player, Carte, TourEnCours)),
    					callPlayerJoue(Carte),
                        (   not(nextPlayer(_)) ->  
                        ! ;
                        jouerCarte(TourEnCours)).

joueurJoueCarte(Player, Carte):- once((   repeat,
                            callJouerCarte(Player, IdCarte),
                            (   (integer(IdCarte), IdCarte >= 0 ) -> true; false),
                            Carte = carte(IdCarte,_,_),
							jeuPlayer(Player, Jeu),
                            member(Carte, Jeu),
                            supprime(Carte, Jeu, NewMain),
                            retract(jeuPlayer(Player, Jeu)),
                            assert(jeuPlayer(Player, NewMain))
                        )).

supprime(_,[],[]).
supprime(X,[X|B],S) :- supprime(X,B,S), !.
supprime(X,[Y|B],[Y|S]) :- supprime(X,B,S).

iaChoisitBourrin(Player, PlusForte) :- jeuPlayer(Player, ListeCartes),
    								ListeCartes = [Head|Tail],
    							   trouverPlusForte(Head, Tail, PlusForte),
    								retract(jeuPlayer(Player, ListeCartes)),
    								supprime(PlusForte, ListeCartes, NouvelleMain),
    								assert(jeuPlayer(Player, NouvelleMain)).
									

trouverPlusForte(Carte1 ,[], PlusForte) :- PlusForte = Carte1.

trouverPlusForte(Carte1,[Carte2|ResteCarte],PlusForte) :- 
    											Carte1 = carte(R1, _, _),
    											Carte2 = carte(R2, _, _),
    											(   R1 < R2 ->  
                                                	trouverPlusForte(Carte2,ResteCarte,PlusForte)
                                                ;   trouverPlusForte(Carte1,ResteCarte,PlusForte)).





testPariValide(Player, NbPli, NbPliFinal) :- tousParis(ListParis),
                    createListFromListAtom(2,ListParis,_,ListPari),
    				length(ListPari, Lrang),
    				jeuPlayer(Player, ListeCarte),
    				length(ListeCarte, NbCarte),
    				tousJoueurs(ListPlayers),
                    tousParis(ListParis),
    				LrangFinal is Lrang +1,
                    length(ListPlayers,NbJoueurs),
    				(   LrangFinal == NbJoueurs ->  
                   		sumListePli(ListPari,SumPari),
                        NbPliTotal is SumPari + NbPli,
                        (
                        	NbPliTotal == NbCarte ->
                        	(   
                            	NbCarte == 1 ->  
                            	NbPliFinal is 0
                            ;   NbPliFinal is NbPli + 1          
                            )    	
                        ;   
                        	NbPliFinal is NbPli
                        )
                    ;  NbPliFinal is NbPli
                    ).

pariIABourrin(Player, NbPliFinal):- jeuPlayer(Player, ListeCartes), indiceMefiancePari(Player,Indice), indiceConfiance(ListeCartes, ListeCartes, NbPli, Indice)
    ,% Pour le dernier joueur, regarde si le nombre total de Pari est égal au nombre de carte
    		testPariValide(Player, NbPli, NbPliFinal), ! .

% Indice de confiance
chancePli(ListeCartes, Carte, Pli, Indice) :- Carte=carte(Ra, _, _),
    		jeuCartes(JeuCartes),
    		length(ListeCartes, NbCarteMain),
    		comparePropresCartes(Carte, ListeCartes, [], CompareListe),
    		sumListePli(CompareListe, CarteForteMain),
    		length(JeuCartes, NbCarteTotal),
    		NbPlusFortJeu is NbCarteTotal - Ra,
    		NbPlusFort is NbPlusFortJeu - CarteForteMain,
    
    		ChancePerd is ( (NbPlusFort / ( NbCarteTotal - (NbCarteMain)))) * 100,
    		(   ChancePerd =< 20 -> 
            	Pli = 1
            ;
            	ChancePerd2 = ChancePerd / Indice,
                
            	(   ChancePerd2 =< 50 ->  
                	Pli = 1
                ;   Pli =0         
                )
            ).

comparePropresCartes( _, [], Acc, CompareListe) :- CompareListe = Acc, !.
comparePropresCartes(Carte, [Carte2|ResteCartes], Acc, CompareListe ) :- 
    Carte = carte(R, _, _),
    Carte2 = carte(R2, _, _),
	(   R >= R2 ->  
    	PlusGrand = 0
   	 ;   PlusGrand = 1
    ),
    comparePropresCartes(Carte, ResteCartes, [PlusGrand|Acc], CompareListe).

sumListePli([], 0).
sumListePli([H|T] , Sum) :-
    sum_list(T, Rest),
    Sum is H + Rest.


    
indiceMefiancePari(Player, Indice) :- %findall(pari(X,Y),pari(X,Y),ListPariPrecedent), 
    							jeuPlayer(Player, ListeCarte),
    							%tousJoueurs(ListPlayers),
   								tousParis(ListParis),
    							%length(ListPlayers,NbJoueurs),
    							length(ListeCarte, NbCarteManche),
    							createListFromListAtom(2,ListParis,_,ListPari),
    							length(ListPari, NbParisEffectuee),
    							(    ListPari = [] ->  
                                	Indice = 1
                                ;   sumListePli(ListPari,SumPari),
    								length(ListPari, Lrang),
    								Indice is (Lrang / (SumPari + Lrang)) * (1- (SumPari/(NbCarteManche*(NbParisEffectuee+1))) )
                                 ), !.
    							

ajouterPliListe( _, [], Acc, ListePli, _) :- ListePli = Acc, !.
ajouterPliListe(ListeCartes, [Carte|ResteCartes], Acc, ListePli, Indice) :- 
    chancePli(ListeCartes, Carte,Pli, Indice),
    ajouterPliListe(ListeCartes ,ResteCartes, [Pli|Acc], ListePli, Indice).


indiceConfiance(ListeCartes, ListeCarte, NbPli,Indice) :-  
           ajouterPliListe(ListeCartes, ListeCarte, [], ListePli, Indice),
    sumListePli(ListePli, NbPli).


comparePariGain:- findall(player(X),player(X),ListePlayer), comparePariPlayer(ListePlayer).
    
comparePariPlayer([]):-!.
comparePariPlayer([Player|T]):-pari(Player,PointParie), pointManchePlayer(Player,PointFait),  pointGame(Player, PointEnCours),
    			NouveauPoint is PointEnCours + abs(PointParie - PointFait), retract(pointGame(Player, PointEnCours)), 
    			assert(pointGame(Player, NouveauPoint)), comparePariPlayer(T).



nextPlayer(NextPlayer):- findall(player(X),player(X),Liste),
    					firstPlayer(FirstPlayer), 
     					length(Liste, NbElement),
    					rang(Index),
    					retract(rang(Index)),
    					Y is Index+1,
    					NextIndex is Y mod NbElement,
    					assert(rang(NextIndex)),
    					nth0(NextIndex, Liste, NextPlayer),
    					NextPlayer\=FirstPlayer.

currentPlayer(Player):-findall(player(X),player(X),Liste),
    				rang(Index),
    				nth0(Index, Liste, Player), !.


setFirstPlayer(Player):-ignore(retractRang), ignore(retractFirstPlayer),
    					findall(player(X),player(X),Liste),
    					nth0(Index, Liste, Player),
    					assert(firstPlayer(Player)), assert(rang(Index)), !.

retractRang:-rang(X), retract(rang(X)).
retractFirstPlayer:-firstPlayer(P), retract(firstPlayer(P)).


createListFromListAtom(_, [], Acc, ResultList):- ResultList = Acc, !.
createListFromListAtom(NbArg, [H|T], Acc, ResultList):- arg(NbArg, H, R),
    													createListFromListAtom(NbArg, T, [R|Acc], ResultList).



createEmptyList(ListZero):-findall(player(X),player(X),ListePlayer),
    			currentPlayer(Player),
    			firstPlayer(FirstPlayer),
    			nth1(IndexPlayer, ListePlayer, Player),
    			nth1(IndexFirstPlayer, ListePlayer, FirstPlayer),
    			length(ListePlayer, NbPlayer),
    			NbJoueurRestant is (NbPlayer - IndexPlayer + IndexFirstPlayer) mod NbPlayer,
    			% si il y a un tour en plus
    			LengthArbre is NbJoueurRestant + NbPlayer,
    			createListZero(LengthArbre, _, ListZero).
    

createListZero(0, Acc, Liste):- Liste = Acc, !.
createListZero(NbPlayer, Acc, Liste):- NbPlayerNext is NbPlayer -1,
    								createListZero(NbPlayerNext, [0|Acc], Liste).
    

addListZero(L, Acc, Result):- once(length(L,R)), sumListePli(L,S), R \= S, addListZero2(L, Acc, Result).
addListZero2([H|T], Acc, Result):- (   H is 0 ->  (   
                                                 Actual = [1|T],
                                                 (   Acc \= 0 ->  
                                                 append(Acc, Actual, Result);
                                                 Result = Actual ) ) ;
                                  (   Acc \= 0 -> 
                                  append(Acc, [0], AccTmp),
                                  addListZero2(T, AccTmp, Result) ;
                                  addListZero2(T, [0], Result) ) ).





    
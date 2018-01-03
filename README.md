# quizAmoroso
## Introduzione
L’applicazione è stata pensata per eseguire principalmente le seguenti funzioni:
1.	Permettere all’utente di esercitarsi sulle domande ad esso assegnate;
2.	Controllare i propri risultati.

## Struttura
<p>La struttura dell’applicativo è molto semplice, essa chiede all’utente di autenticarsi prima di poter usufruire dei servizi.
Se il login avviene con successo esso avrà la possibilità di vedere le proprie Statistiche e di accedere ai propri concorsi o piani formativi extra.
Un utente potrà usufruire dell’app su un massimo di 3 dispositivi, e non potrà autenticarsi contemporaneamente su due dispositivi diversi.</p>

## Login
<p>
L’utente all’apertura dell’applicativo si troverà in una schermata di Login, dove potrà inserire i suoi dati per accedere al servizio.
I dati di accesso saranno composti da un username (codice Fiscale) e una password personale.
Dopo aver inserito i propri dati di accesso basterà premere il tasto entra e partirà in automatico l’invio dei dati al server per controllare la correttezza degli stessi. L’utente potrà notare che tale processo è in corso grazie ad una barra circolare che indica il caricamento.

<p align="center">
  <img src="https://user-images.githubusercontent.com/28861456/30808727-84683698-a1ff-11e7-8925-dd54e8d2ec14.png">
</p>

In più per facilitare all’utente l’inserimento dei dati l’applicativo salverà l’ultimo username inserito ed in più sarà possibile premendo l’icona dell’occhio vedere la password che si sta inserendo.
Al momento del login l’utente sarà registrato sul data-base come loggato e non potrà effettuare l’accesso su altri dispositivi nello stesso momento.
nSe per qualche errore improvviso l’utente non dovesse effettuare il log-out non potrà rilegare solo se effettua l’accesso dal ultimo dispositivo utilizzato.
</p>

## Home-Page


Una volta effettuato il login con successo l’utente si troverà nella Home-Page, qui saranno presenti tutti i dati inerenti ad “Amoroso Concorsi”:
-	Indirizzo
-	Telefono
-	Mobile
-	E-Mail
-	Pagina Facebook
-	Mappa posizione
<p align="center">
  <img src="https://user-images.githubusercontent.com/28861456/30808983-79d9380c-a200-11e7-9404-aa875f988032.png">
</p>

Si può notare subito in alto la barra di navigazione, dove si potrà navigare fra le pagine, l’utente avrà anche la possibilità di spostarsi fa le stesse tramite un semplice swap da sinistra verso destra o viceversa.


## Piani-Formativi

Andando nella schermata dei piani formativi si potranno vedere tutti piani formativi assegnati, con la seguente formattazione:
- Titolo: Nome del concorso a cui appartiene il piano, se sarà un piano extra questo avrà come titolo “extra”
-	Descrizione: Qui sarà presente il nome del piano alla quale si fa riferimento
Per selezionare il piano all’utente basterà effettuare un tap sul piano scelto(si sposterà nella scheda “set domande disponibili”).

<p align="center">
  <img width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30809103-e48526c0-a200-11e7-9f55-8e09a9056b61.png">
</p>
Il caricamento dei piani viene effettuato al momento del Login, quindi l’utente non potrà visualizzare eventuali aggiornamenti finché non effettuerà un log-out.


## Set domande disponibili


All’apertura di tale finestra si potranno vedere tutti i set di domande disponibili per quel piano didattico.
Si può notare fin da subito una variazione nella grafica poiché scomparirà la barra di navigazione e al suo posto sarà presente un bottone situato in alto a sinistra che consentirà di tornare alle schermate principali.
I vari set di domande si presenteranno in questo modo:
-	Titolo: nome del set
-	Descrizione: sarà presente la descrizione del set di domande
La scelta del set risulta molto semplice poiché basterà cliccare sul set desiderato (verremmo spostati nella finestra “Modalità”).
<p align="center">
  <img width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30809171-305ee068-a201-11e7-9170-a6fcbd3adea2.png">
</p>


## Modalità


Qui potremmo scegliere la modalità, fra quelle disponibili:
-	Dispensa
-	Apprendimento
-	Simulazione assistita
-	Simulazione

<p align="center">
  <img width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30809259-6d1b5310-a201-11e7-95b8-a53dc52efb93.png">
</p>

Al click su di una modalità comparirà una breve descrizione della stessa e il pulsante per avviarla.


## Dispensa

<p align="center">
Qui l’utente potrà visualizzare tutte le domande facenti parte del set, sarà possibile visualizzare la risposta esatta.
Non ci saranno limiti di tempo e potrà scorrere tutte le domande andando avanti e indietro coi bottoni.
La dispensa si chiuderà in automatico non appena si supererà l’ultima domanda o premendo il bottone back situato in alto a sinistra.
<p align="center">
  <img  width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30809456-0ef76282-a202-11e7-9ff0-38eac2e0cb04.png">
  <img  width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30809590-8c63cbd4-a202-11e7-8e77-9f77ca812a98.png">
</p>
</p>
Ora passiamo alla modalita:

## Apprendimento

<p>
Qui l’utente avrà a disposizione tutte le domande del set e potrà cambiare domanda solo quando sceglierà la risposta giusta.
Potrà quindi effettuare tutti gli errori che vuole e non avrà nessun limite di tempo.
<p align="center">
  <img width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30809808-55a4ae96-a203-11e7-9855-5828eb6902a3.png">
  <img width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30809809-55a4ccd2-a203-11e7-9f10-0b72b095fbc6.png">
</p>
</p>

## Simulazione Assistita

<p>
Qui si svolgerà una simulazione dove l’utente avrà un timer e dovrà rispondere a tutte le domande del set. Non avrà la possibilità di correggere la sua risposta ma se la risposta non è giusta il programma segnerà in verde la risposta giusta ed in rosso quella sbagliata data dall’utente.

<p align="center">
  <img  width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30809896-a4e6c8e0-a203-11e7-874f-547085952d6b.png">
  <img  width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30809897-a4e90b5a-a203-11e7-8236-7f2180022e55.png">
</p>
</p>

## Simulazione

<p>
Qui si potrà effettuare una vera e propria simulazione sul set di domande, quindi si avrà un timer e l’utente non potrà vedere se ha sbagliato o meno la sua risposta finché non finirà l’intera simulazione.
Alla fine della simulazione tutti i dati verranno inviati al server e salvati in memoria mentre all’utente comparirà una schermata coi risultati della simulazione.
Se l’utente decide di uscire dalla simulazione prima che essa sia conclusa i dati verranno comunque inviati al server.

<p align="center">
<img  width="250" height="400" src="https://user-images.githubusercontent.com/28861456/30809970-cba8223a-a203-11e7-8fdb-1d6ec89b1f16.png">
<img  width="250" height="400" src="https://user-images.githubusercontent.com/28861456/30809969-cba6cd86-a203-11e7-9a03-043de1fe807a.png">
<img  width="250" height="400" src="https://user-images.githubusercontent.com/28861456/30810074-1bc945be-a204-11e7-8580-b56d83b911d4.png">
</p>
</p>

## Statistiche

<p>
Qui l’utente potrà controllare i suoi risultati.
Si troverà una schermata con due data.picker dove dovrà inserire il range di data sulla quale vuole le statistiche.
Inserito il range di data potrà vedere tutti i suoi risultati con l’ausilio di diagrammi a torta e un diagramma temporale con i picchi di risposte esatte e sbagliate suddivisi per data.

<p align="center">
<img  width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30810235-acdfc492-a204-11e7-8c6a-8fdaa3927f30.png">
<img  width="400" height="640" src="https://user-images.githubusercontent.com/28861456/30810233-acda16e6-a204-11e7-9e82-251545d15105.png">
</p>
</p>

## Log-out

<p>
L’utente potrà slogarsi in qualsiasi momento anche durante una simulazione, se si effettua il log-out sarà possibile accedere da un altro dispositivo.
Se l’utente manda l’app in background essa effettuerà in automatico il log-out.
</p>

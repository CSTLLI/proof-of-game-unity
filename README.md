<div align="center">
  <a href="https://www.wf3.fr/" target="_blank">
    <img src="https://img.gothru.org/283/3821330491768879313/overlay/assets/20201210050148.Efu4dY.png?save=optimize" alt="Logo" width="200">
  </a>
</div>

# Unity Game Project - Proof of Game - Valerio Castelli

## Sommaire
### Concept
- [Description](#description)
- [Liens utiles](#liens-utiles)
- [Concept](#concept)
### Configuration
- [Installation et Configuration](#installation-et-configuration)
### Fonctionnalités
- [Fonctionnalités principales](#fonctionnalités-principales)
- [En cours de développement](#en-cours-de-développement)
### Annexes
- [Captures d'écran](#captures-décran)
- [Crédits](#crédits)
- [Contact](#contact)

## Description
Serious Game en réalité mixte conçu pour former les professionnels du secteur industriel à la gestion des risques. L'application simule un environnement de travail où les joueurs doivent identifier, évaluer et gérer des risques potentiels tout en accomplissant leurs tâches quotidiennes, avec une dimension blockchain pour la traçabilité.

## Liens utiles
- Backend Repository: [Repository du backend](https://github.com/CSTLLI/proof-of-game-unity-backend)
- Serious Game Design Document: [Lire ici](https://doc.clickup.com/9015781403/d/h/8cp3u0v-135/074eab5e73eb885)
- Game Concept: [Lire ici](https://doc.clickup.com/9015781403/d/h/8cp3u0v-75/36c27ec172ac29f)
- Business Mode: [Lire ici](https://doc.clickup.com/9015781403/d/h/8cp3u0v-155/cba5d4767e9c6f3)
- Database (inclus dans le repo backend) : [Database](https://github.com/CSTLLI/proof-of-game-unity-backend/blob/main/database-setup.sql)
- Collection PostMan (inclus dans le repo backend) : [Collection](https://github.com/CSTLLI/proof-of-game-unity-backend/blob/main/UnityApp1.postman_collection.json)

## Concept 

Le joueur incarne un responsable sécurité dans un environnement industriel où il doit effectuer des missions tout en gérant différents niveaux de risque. La plateforme propose deux modes d'expérience:

- Mode Standard: Simulation classique avec vérifications manuelles et risque de défaillance
- Mode Blockchain: Vérification instantanée et traçabilité complète via technologie blockchain

L'objectif est de sensibiliser aux avantages des systèmes de traçabilité avancés dans la prévention et la gestion des risques industriels.

## Installation et Configuration

- Cloner le repository
- Ouvrir le projet avec Unity 2022.2 ou version ultérieure
- S'assurer que le package HDRP est correctement installé
- Lancer la scène principale pour démarrer le jeu

## Fonctionnalités principales

### Système de Gestion des Risques

- Calcul du risque en temps réel basé sur les actions du joueur
- Déclenchement d'événements aléatoires liés au niveau de risque
- Différenciation claire entre le mode blockchain et le mode classique

### Scan QR pour Traçabilité

- Station de scan pour vérifier l'authenticité des pièces et équipements
- Interface détaillée montrant les données de traçabilité
- Comparaison entre vérification standard (erreurs possibles) et blockchain (fiabilité)

### Système de Missions et Tâches

- Missions à accomplir avec objectifs clairs
- Gestion du temps et des priorités
- Système de validation des tâches accomplies

### Interface Utilisateur

- HUD avec indicateurs de progression et timer
- Système d'interaction avec les objets et équipements
- Retours visuels et sonores sur les actions du joueur

### Technologies Utilisées

- Unity 3D (Version 2022.2 LTS)
- HDRP (High Definition Render Pipeline)
- C# pour la logique de jeu
- Architecture MVC avec système de managers

### Architecture du Système
Managers Principaux

- GameManager: Gestion des états de jeu et configuration globale
- RiskManager: Calcul du risque en temps réel et déclenchement d'événements
- TaskManager: Gestion des tâches et validation de leur complétion

### Systèmes Core

- ValidationSystem: Vérifie les actions du joueur selon le mode de jeu
- ScoreSystem: Calcule les performances basées sur le temps et le niveau de risque
- UIManager: Gère l'interface utilisateur dynamique

## En cours de développement

- Backend pour l'authentification et la persistance des données
- API pour enregistrer les statistiques de jeu
- Dashboard administrateur pour suivre les performances des apprenants
  
## Captures d'écran
_À venir avec la version alpha_

## Crédits
Développé dans le cadre du Master M2i au CCI Campus Strasbourg.

## Contact
Pour toute question concernant le projet: valeriocastellipro@gmail.com
